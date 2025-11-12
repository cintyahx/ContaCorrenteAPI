using FluentValidation;
using Miotto.BankMore.Conta.App.Commands;
using Miotto.BankMore.Conta.Domain.Enums;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.App.Validations
{
    public class NewTransactionValidationn : AbstractValidator<NewTransactionCommand>
    {
        public NewTransactionValidationn(IContaCorrenteRepository contaCorrenteRepository)
        {
            RuleFor(x => x.Tipo)
                .NotEmpty().WithMessage(string.Format(ValidationResource.RequiredField, FieldResource.TipoMovimento))
                .MaximumLength(1).WithMessage(string.Format(ValidationResource.MaxLength, FieldResource.TipoMovimento, 1))
                .Must(tipo =>
                {
                    return EnumExtension.TryGetEnumByDescription(tipo, out TipoMovimento type);
                });

            RuleFor(x => x.NumeroContaDestino)
                .GreaterThanOrEqualTo(0)
                    .WithMessage(string.Format(ValidationResource.MustBeValid, FieldResource.NumeroConta))
                .When(x => x.NumeroContaDestino > 0)
                    .DependentRules(() => RuleFor(x => x.NumeroContaDestino)
                                            .MustAsync(async (x, _) => await contaCorrenteRepository.GetByNumeroAsync(x) is not null)
                                                .WithMessage(string.Format(ValidationResource.NotFound, FieldResource.NumeroConta)));

            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage(string.Format(ValidationResource.MustBeValid, FieldResource.Valor));
        }
    }
}
