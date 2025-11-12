using MediatR;
using Microsoft.AspNetCore.Mvc;
using Miotto.BankMore.Conta.App.Commands;
using Miotto.BankMore.Conta.App.Controllers;
using Miotto.BankMore.Conta.App.Entities;
using Miotto.BankMore.Conta.App.Validations;
using Miotto.BankMore.Conta.Domain.Entities;
using Miotto.BankMore.Conta.Domain.Interfaces;
using Moq;
using System.Security.Claims;

namespace Miotto.BankMore.Conta.App.Tests
{
    public class CreateAccountCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesAccount_ReturnsNumero()
        {
            // Arrange
            var repoMock = new Mock<IContaCorrenteRepository>();
            repoMock.Setup(r => r.CreateAsync(It.IsAny<ContaCorrente>()))
                .ReturnsAsync((ContaCorrente c) =>
                {
                    c.Numero = 555; // simulate DB-generated value
                    return c;
                });

            var handler = new CreateAccountCommandHandler(repoMock.Object);
            var command = new CreateAccountCommand("Nome Teste", "12345678901", "senha");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(555, result);
            repoMock.Verify(r => r.CreateAsync(It.IsAny<ContaCorrente>()), Times.Once);
        }
    }

    public class DeleteAccountCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryDelete()
        {
            // Arrange
            var repoMock = new Mock<IContaCorrenteRepository>();
            repoMock.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

            var handler = new DeleteAccountCommandHandler(repoMock.Object);
            var id = Guid.NewGuid();
            var command = new DeleteAccountCommand(id);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }

    public class NewTransactionCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WhenDestinationAccountNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var contaRepo = new Mock<IContaCorrenteRepository>();
            var movimentoRepo = new Mock<IMovimentoRepository>();

            // simulate not found by numero
            contaRepo.Setup(r => r.GetByNumeroAsync(It.IsAny<int>())).ReturnsAsync((ContaCorrente?)null);

            var handler = new NewTransactionCommandHandler(contaRepo.Object, movimentoRepo.Object);
            var command = new NewTransactionCommand(Guid.NewGuid(), 9999, "Debito", 10m);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("INVALID_ACCOUNT", ex.Message);
        }

        [Fact]
        public async Task Handle_WhenInvalidType_ThrowsInvalidOperationException()
        {
            // Arrange
            var contaRepo = new Mock<IContaCorrenteRepository>();
            var movimentoRepo = new Mock<IMovimentoRepository>();

            var contaDestino = new ContaCorrente { Numero = 1111, Saldo = 100m };
            contaRepo.Setup(r => r.GetByNumeroAsync(It.IsAny<int>())).ReturnsAsync(contaDestino);

            var handler = new NewTransactionCommandHandler(contaRepo.Object, movimentoRepo.Object);
            // Debito but command.NumeroContaDestino doesn't match contaDestino.Numero -> INVALID_TYPE
            var command = new NewTransactionCommand(Guid.NewGuid(), 2222, "Debito", 10m);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Equal("INVALID_TYPE", ex.Message);
        }

        [Fact]
        public async Task Handle_Success_CreatesMovimentoAndUpdatesSaldo()
        {
            // Arrange
            var contaRepo = new Mock<IContaCorrenteRepository>();
            var movimentoRepo = new Mock<IMovimentoRepository>();

            var contaDestino = new ContaCorrente { Numero = 2222, Saldo = 100m, IsActive = true };
            contaRepo.Setup(r => r.GetByNumeroAsync(contaDestino.Numero)).ReturnsAsync(contaDestino);
            movimentoRepo.Setup(m => m.CreateAsync(It.IsAny<Movimento>())).ReturnsAsync((Movimento mv) => mv);
            contaRepo.Setup(r => r.AtualizarSaldoAsync(contaDestino, It.IsAny<decimal>(), It.IsAny<Domain.Enums.TipoMovimento>()))
                     .Returns(Task.CompletedTask)
                     .Verifiable();

            var handler = new NewTransactionCommandHandler(contaRepo.Object, movimentoRepo.Object);
            var command = new NewTransactionCommand(Guid.NewGuid(), contaDestino.Numero, "Credito", 50m);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            movimentoRepo.Verify(m => m.CreateAsync(It.Is<Movimento>(mv => mv.Valor == 50m && mv.Tipo.ToString() == "Credito")), Times.Once);
            contaRepo.Verify(r => r.AtualizarSaldoAsync(contaDestino, 50m, It.IsAny<Domain.Enums.TipoMovimento>()), Times.Once);
        }
    }

    public class ControllersTests
    {
        [Fact]
        public async Task AuthController_LoginAsync_ReturnsUnauthorized_WhenTokenEmpty()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(string.Empty);

            var controller = new AuthController(mediator.Object);

            // Act
            var login = new LoginCommand() { Identification = "03264290006", Password = "dev123DEVdfgf" };
            var result = await controller.LoginAsync(login);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorized.Value);
        }

        [Fact]
        public async Task AuthController_LoginAsync_ReturnsOk_WhenTokenPresent()
        {
            // Arrange
            var token = "jwt-token";
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var controller = new AuthController(mediator.Object);

            // Act
            var login = new LoginCommand() { Identification = "03264290006", Password = "dev123DEVdfgf" };
            var result = await controller.LoginAsync(login);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task ContaCorrenteController_CreateAccountAsync_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var controller = new ContaCorrenteController(mediator.Object);
            controller.ModelState.AddModelError("x", "error");

            // Act
            var result = await controller.CreateAccountAsync(new CreateAccountCommand("n", "cpf", "s"));

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ContaCorrenteController_CreateAccountAsync_ReturnsCreated_WhenValid()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreateAccountCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(42);
            var controller = new ContaCorrenteController(mediator.Object);

            // Act
            var result = await controller.CreateAccountAsync(new CreateAccountCommand("n", "cpf", "s"));

            // Assert
            var created = Assert.IsType<CreatedResult>(result);
            Assert.NotNull(created.Value);
        }

        [Fact]
        public async Task ContaCorrenteController_DeleteAsync_SendsDeleteCommandAndReturnsNoContent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<DeleteAccountCommand>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var controller = new ContaCorrenteController(mediator.Object);
            var id = Guid.NewGuid();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) }, "Test"));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = principal };

            // Act
            var result = await controller.DeleteAsync();

            // Assert
            Assert.IsType<NoContentResult>(result);
            mediator.Verify(m => m.Send(It.Is<DeleteAccountCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        //[Fact]
        //public async Task ContaCorrenteController_ObterSaldo_ReturnsOkWithValue()
        //{
        //    // Arrange
        //    var mediator = new Mock<IMediator>();

        //    var expected = new GetSaldoResult()
        //    {
        //        NumeroConta = conta.Numero,
        //        NomeTitular = conta.Nome,
        //        DataConsulta = DateTime.Now,
        //        SaldoAtual = conta.Saldo
        //    };

        //    mediator.Setup(m => m.Send(It.IsAny<GetSaldoCommand>(), It.IsAny<CancellationToken>())).Returns(123.45m);

        //    var controller = new ContaCorrenteController(mediator.Object);
        //    var id = Guid.NewGuid();
        //    var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) }, "Test"));
        //    controller.ControllerContext = new ControllerContext();
        //    controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = principal };

        //    // Act
        //    var result = await controller.ObterSaldo();

        //    // Assert
        //    var ok = Assert.IsType<OkObjectResult>(result);
        //    Assert.Equal(123.45m, ok.Value);
        //}

        [Fact]
        public async Task MovimentoContaController_NewTransactionAsync_SendsCommandAndReturnsNoContent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<NewTransactionCommand>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var controller = new MovimentoContaController(mediator.Object);
            var id = Guid.NewGuid();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) }, "Test"));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = principal };

            var request = new NewTransactionRequest { NumeroContaDestino = 1, Tipo = "Credito", Valor = 10m };

            // Act
            var result = await controller.NewTransactionAsync(request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            mediator.Verify(m => m.Send(It.IsAny<NewTransactionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public class NewTransactionValidationTests
    {
        [Fact]
        public async Task Validator_Fails_WhenValorNotGreaterThanZero()
        {
            // Arrange
            var repoMock = new Mock<IContaCorrenteRepository>();
            var validator = new NewTransactionValidationn(repoMock.Object);
            var command = new NewTransactionCommand(Guid.NewGuid(), 0, "Credito", 0m);

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(NewTransactionCommand.Valor));
        }

        [Fact]
        public async Task Validator_ChecksNumeroContaDestino_WhenGreaterThanZero()
        {
            // Arrange
            var repoMock = new Mock<IContaCorrenteRepository>();
            repoMock.Setup(r => r.GetByNumeroAsync(It.IsAny<int>())).ReturnsAsync((ContaCorrente?)null);
            var validator = new NewTransactionValidationn(repoMock.Object);

            var command = new NewTransactionCommand(Guid.NewGuid(), 9999, "Credito", 1m);

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(NewTransactionCommand.NumeroContaDestino));
        }
    }
}