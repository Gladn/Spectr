using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace Spectr.Validations
{
    public class CustomerValidationRule : ValidationRule, INotifyPropertyChanged
    {
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public bool NotNullable { get; set; }
        public bool OnlyDigits { get; set; }

        private bool _isAddButtonEnabled = false;

        public bool IsAddButtonEnabled
        {
            get { return _isAddButtonEnabled; }
            set
            {
                if (_isAddButtonEnabled != value)
                {
                    _isAddButtonEnabled = value;
                    OnPropertyChanged(nameof(IsAddButtonEnabled));
                }
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string text)
            {
                if (OnlyDigits)
                {
                    if (!text.All(char.IsDigit))
                    {
                        IsAddButtonEnabled = false;
                        return new ValidationResult(false, "Только числа.");
                    }
                }

                if (NotNullable)
                {
                    if (text.Length < 1)
                    {
                        IsAddButtonEnabled = false;
                        return new ValidationResult(false, $"Длина не менее {MinLength} знака.");
                    }
                }

                if (text.Length > MaxLength)
                {
                    IsAddButtonEnabled = false;
                    return new ValidationResult(false, $"Длина не более {MaxLength} знаков.");
                }

                
            }

            return ValidationResult.ValidResult;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
