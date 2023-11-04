using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

#nullable enable

namespace ReScanVisualizer.Validators
{
    internal class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? val = value is string s ? s : null;
            val = val?.Trim();
            if (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val))
            {
                return new ValidationResult(false, "Value can't be empty or containing only spaces.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
