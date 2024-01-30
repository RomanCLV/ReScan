using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReScanVisualizer.Validators
{
    public class PositiveNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string strValue)
            {
                double number;
                if (Tools.TryParse(strValue, out number))
                {
                    if (number > 0)
                    {
                        return ValidationResult.ValidResult;
                    }
                }
            }

            return new ValidationResult(false, "Value must be greater than 0.");
        }
    }
}
