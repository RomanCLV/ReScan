using ReScanVisualizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReScanVisualizer.ViewModels.AddScatterGraph.Builder
{
    public class ScatterGraphFileBuilder : ScatterGraphBuilderBase
    {
        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                if (SetValue(ref _path, value))
                {
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public string FileName => System.IO.Path.GetFileName(_path);

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetValue(ref _color, value);
        }

        private bool _containsHeader;
        public bool ContainsHeader
        {
            get => _containsHeader;
            set => SetValue(ref _containsHeader, value);
        }

        public ScatterGraphFileBuilder(string path, Color color, bool containsHeader)
        {
            _path = path;
            _color = color;
            _containsHeader = containsHeader;
        }

        /// <summary>
        /// Build an array of ScatterGraphViewModel
        /// </summary>
        /// <returns>Return an array of one ScatterGraph</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public override ScatterGraphViewModel[] Build()
        {
            if (!File.Exists(_path))
            {
                throw new FileNotFoundException("File not found!", _path);
            }
            return new ScatterGraphViewModel[1] { new ScatterGraphViewModel(ScatterGraph.ReadCSV(_path, _containsHeader), _color) };
        }
    }
}
