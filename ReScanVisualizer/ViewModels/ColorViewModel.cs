using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels
{
    public class ColorViewModel : ViewModelBase
    {
        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (SetValue(ref _color, value))
                {
                    OnPropertyChanged(nameof(R));
                    OnPropertyChanged(nameof(G));
                    OnPropertyChanged(nameof(B));
                    OnPropertyChanged(nameof(A));
                }
            }
        }

        public byte R
        {
            get => _color.R;
            set
            {
                if (SetValue(_color.R, value))
                {
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public byte G
        {
            get => _color.G;
            set
            {
                if (SetValue(_color.G, value))
                {
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public byte B
        {
            get => _color.B;
            set
            {
                if (SetValue(_color.B, value))
                {
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public byte A
        {
            get => _color.A;
            set
            {
                if (SetValue(_color.A, value))
                {
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public ColorViewModel() : this(Colors.White)
        { }

        public ColorViewModel(Color color)
        {
            _color = color;
        }

        public void Set(Color color)
        {
            Color = color;
        }

        public override string ToString()
        {
            return _color.ToString();
        }
    }
}
