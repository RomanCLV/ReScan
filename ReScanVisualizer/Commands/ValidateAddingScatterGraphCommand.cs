using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraph;
using ReScanVisualizer.ViewModels.AddScatterGraph.Builder;
using ReScanVisualizer.Views.AddScatterGraphViews;

#nullable enable

namespace ReScanVisualizer.Commands
{
    public class ValidateAddingScatterGraphCommand : CommandBase
    {
        private readonly AddScatterGraphView _addScatterGraphView;
        private readonly AddScatterGraphViewModel _addScatterGraphModelView;
        private ScatterGraphBuilderBase? _builder;
        private readonly MainViewModel _mainViewModel;

        public ValidateAddingScatterGraphCommand(AddScatterGraphView addScatterGraphView, AddScatterGraphViewModel addScatterGraphModelView, MainViewModel mainViewModel)
        {
            _addScatterGraphView = addScatterGraphView;
            _addScatterGraphModelView = addScatterGraphModelView;
            _mainViewModel = mainViewModel;

            _addScatterGraphModelView.PropertyChanged += AddScatterGraphModelView_PropertyChanged;
            _builder = _addScatterGraphModelView.Builder;

            if (_builder != null)
            {
                _builder.PropertyChanged += Builder_PropertyChanged;
            }
        }

        ~ValidateAddingScatterGraphCommand()
        {
            _addScatterGraphModelView.PropertyChanged -= AddScatterGraphModelView_PropertyChanged;
            if (_builder != null)
            {
                _builder.PropertyChanged -= Builder_PropertyChanged;
            }
        }

        private void AddScatterGraphModelView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AddScatterGraphViewModel.Builder))
            {
                if (_builder != null)
                {
                    _builder.PropertyChanged -= Builder_PropertyChanged;
                }
                _builder = _addScatterGraphModelView.Builder;
                if (_builder != null)
                {
                    _builder.PropertyChanged += Builder_PropertyChanged;
                }
            }
            OnCanExecuteChanged();
        }

        private void Builder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScatterGraphBuilderBase.CanBuild))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return (_addScatterGraphModelView.Builder is null ? true : _addScatterGraphModelView.Builder.CanBuild) && (
                _addScatterGraphModelView.IsEmptySelected ||
                _addScatterGraphModelView.IsToPopulateSelected ||
                _addScatterGraphModelView.IsToOpenSelected
                );
        }

        public override void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                LoadScatterGraphView loadView = new LoadScatterGraphView()
                {
                    Owner = Application.Current.MainWindow
                };

                loadView.DataContext = new LoadScatterGraphViewModel(loadView, _mainViewModel, _addScatterGraphView, _addScatterGraphModelView.Builder);
                loadView.ShowDialog();
            }
        }
    }
}
