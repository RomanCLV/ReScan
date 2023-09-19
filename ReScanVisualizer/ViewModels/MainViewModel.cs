using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
        public Model3DGroup OriginModel { get; private set; }

        public MainViewModel()
		{
            OriginModel = new Model3DGroup();
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(1, 0, 0), 0.1, Brushes.Red));
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(0, 1, 0), 0.1, Brushes.Green));
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(0, 0, 1), 0.1, Brushes.Blue));
        }
	}
}
