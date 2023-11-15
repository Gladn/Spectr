using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace Spectr.Validations
{
    public class CustomerValidationRule : ValidationRule
    {
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public bool NotNullable { get; set; }
        public bool OnlyDigits { get; set; }


        

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string text)
            {
                if (OnlyDigits)
                {
                    if (!text.All(char.IsDigit))
                    {
                        return new ValidationResult(false, "Только числа.");
                    }
                }

                if (NotNullable)
                {
                    if (text.Length < 1)
                    {
                        return new ValidationResult(false, $"Длина не менее {MinLength} знака.");
                    }
                }

                if (text.Length > MaxLength)
                {
                    return new ValidationResult(false, $"Длина не более {MaxLength} знаков.");
                }

                
            }
            return ValidationResult.ValidResult;
        }
    }
}
