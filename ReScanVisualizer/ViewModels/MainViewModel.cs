﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Collections.ObjectModel;

#nullable enable

namespace ReScanVisualizer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public Model3DGroup OriginModel { get; private set; }

        private ViewModelBase? _selectedViewModel;
        public ViewModelBase? SelectedViewModel
        {
            get => _selectedViewModel;
            set => SetValue(ref _selectedViewModel, value);
        }

        public ObservableCollection<ScatterGraphViewModel> ScatterGraphs { get; private set; }

        public MainViewModel()
		{
            OriginModel = new Model3DGroup();
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(1, 0, 0), 0.1, Brushes.Red));
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(0, 1, 0), 0.1, Brushes.Green));
            OriginModel.Children.Add(Helper3D.Helper3D.BuildArrowModel(new Point3D(), new Point3D(0, 0, 1), 0.1, Brushes.Blue));

            SelectedViewModel = null;

            ScatterGraphs = new ObservableCollection<ScatterGraphViewModel>();
        }
	}
}
