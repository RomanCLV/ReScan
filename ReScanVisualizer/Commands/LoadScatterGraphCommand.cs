﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReScanVisualizer.Models;
using ReScanVisualizer.ViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels;
using ReScanVisualizer.ViewModels.AddScatterGraphViewModels.Builders;

#nullable enable

namespace ReScanVisualizer.Commands
{
    internal class LoadScatterGraphCommand : AsyncCommandBase
    {
        private readonly AddScatterGraphViewModel _viewModel;
        private readonly bool _closeAfterExecute;

        public LoadScatterGraphCommand(AddScatterGraphViewModel viewModel, bool closeAfterExecute)
        {
            _viewModel = viewModel;
            _closeAfterExecute = closeAfterExecute;

            _viewModel.Items.CollectionChanged += Items_CollectionChanged;
        }

        ~LoadScatterGraphCommand()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                _viewModel.Items.CollectionChanged -= Items_CollectionChanged;
                base.Dispose();
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems)
                    {
                        if (newItem is KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
                        {
                            item.Key.PropertyChanged += Key_PropertyChanged;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var oldItem in e.OldItems)
                    {
                        if (oldItem is KeyValueObservable<ScatterGraphBuilderBase, ScatterGraphBuildResult> item)
                        {
                            item.Key.PropertyChanged -= Key_PropertyChanged;
                        }
                    }
                    break;
            }
            OnCanExecuteChanged();
        }

        private void Key_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScatterGraphBuilderBase.State))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return
                base.CanExecute(parameter) &&
                _viewModel.Items.Count > 0 &&
                _viewModel.Items.Any(x => x.Key.State == ScatterGraphBuilderState.Success);
        }

        public override Task ExecuteAsync(object? parameter)
        {
            return _viewModel.LoadAllAsync(_closeAfterExecute);
        }
    }
}
