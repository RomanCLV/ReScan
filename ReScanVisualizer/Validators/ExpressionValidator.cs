using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MathEvaluatorNetFramework;

namespace ReScanVisualizer.Validators
{
    internal class ExpressionValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool isValid = false;
            if (value is string strValue)
            {
                if (Tools.TryParse(strValue, out double _))
                {
                    isValid = true;
                }
                else
                {
                    Expression expression = new Expression();
                    try
                    {
                        expression.Set(strValue);
                        expression.Evaluate();
                        isValid = true;
                    }
                    catch { }
                }
            }
            return isValid ? ValidationResult.ValidResult : new ValidationResult(false, "Value is not a number.");
        }
    }
}
