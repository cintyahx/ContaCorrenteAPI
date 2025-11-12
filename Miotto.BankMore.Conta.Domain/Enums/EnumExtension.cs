using System.ComponentModel;
using System.Reflection;

namespace Miotto.BankMore.Conta.Domain.Enums
{
    public class EnumExtension
    {
        public static TEnum GetEnumByDescription<TEnum>(string description) where TEnum : Enum
        {
            MemberInfo[] fis = typeof(TEnum).GetFields();

            foreach (var fi in fis)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == description)
                    return (TEnum)Enum.Parse(typeof(TEnum), fi.Name);
            }

            throw new ArgumentException($"Enum item with description \"{description}\" could not be found",
                nameof(description));
        }

        public static bool TryGetEnumByDescription<TEnum>(string description, out TEnum result)
            where TEnum : Enum
        {
            try
            {
                result = GetEnumByDescription<TEnum>(description);
                return true;
            }
            catch (ArgumentException)
            {
                result = default(TEnum);
                return false;
            }
        }
    }
}
