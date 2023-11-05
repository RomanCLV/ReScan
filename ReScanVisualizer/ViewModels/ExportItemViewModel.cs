using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReScanVisualizer.ViewModels
{
    public class ExportItemViewModel<T> : ViewModelBase
        where T : ViewModelBase
    {
        private readonly T _value;
        public T Value => _value;

		private bool _isSelected;
		public bool IsSelected
		{
			get => _isSelected;
            set => SetValue(ref _isSelected, value);
        }

        public string Name => _render(_value);

        private readonly Func<T, string> _render;

        public ExportItemViewModel(T viewModelBase, Func<T, string> render, bool isSelected=false)
        {
            _value = viewModelBase;
            _render = render;
            _isSelected = isSelected;
        }
    }
}
