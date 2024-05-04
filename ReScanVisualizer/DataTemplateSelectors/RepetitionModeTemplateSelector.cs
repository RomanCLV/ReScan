using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ReScanVisualizer.Models;

#nullable enable

namespace ReScanVisualizer.DataTemplateSelectors
{
    internal class RepetitionModeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? NoneTemplate { get; set; }
        public DataTemplate? TranslationTemplate { get; set; }
        public DataTemplate? RotationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is RepetitionMode repetitionMode)
            {
                switch (repetitionMode)
                {
                    case RepetitionMode.None:
                        return NoneTemplate!;

                    case RepetitionMode.Translation:
                        return TranslationTemplate!;

                    case RepetitionMode.Rotation:
                        return RotationTemplate!;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
