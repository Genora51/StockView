using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class SummaryDetailViewModel : DetailViewModelBase, IDropTarget
    {
        private readonly ISummaryRepository _summaryRepository;
        private SummaryWrapper _selectedSummary;

        public SummaryDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            ISummaryRepository summaryRepository)
            : base(eventAggregator, messageDialogService)
        {
            _summaryRepository = summaryRepository;
            Title = "Summaries";
            Summaries = new ObservableCollection<SummaryWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        public SummaryWrapper SelectedSummary
        {
            get { return _selectedSummary; }
            set
            {
                _selectedSummary = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<SummaryWrapper> Summaries { get; }

        public override async Task LoadAsync(int id)
        {
            Id = id;
            foreach (var wrapper in Summaries)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            Summaries.Clear();

            var summaries = (await _summaryRepository.GetAllAsync()).OrderBy(s => s.SortIndex);
            foreach (var model in summaries)
            {
                var wrapper = new SummaryWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                Summaries.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _summaryRepository.HasChanges();
            }
            if (e.PropertyName == nameof(SummaryWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected override void OnDeleteExecute()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && Summaries.All(s => !s.HasErrors);
        }

        protected override async void OnSaveExecute()
        {
            try
            {
                await _summaryRepository.SaveAsync();
                HasChanges = _summaryRepository.HasChanges();
                RaiseCollectionSavedEvent();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                await MessageDialogService.ShowInfoDialogAsync("Error while saving the entities, " +
                    "the data will be reloaded. Details: " + ex.Message);
                await LoadAsync(Id);
            }
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedSummary != null;
        }

        private void OnRemoveExecute()
        {
            SelectedSummary.PropertyChanged -= Wrapper_PropertyChanged;
            _summaryRepository.Remove(SelectedSummary.Model);
            Summaries.Remove(SelectedSummary);
            SelectedSummary = null;
            RecalculateSummaryIndices();
            HasChanges = _summaryRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            var wrapper = new SummaryWrapper(new Summary());
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            _summaryRepository.Add(wrapper.Model);
            Summaries.Add(wrapper);
            RecalculateSummaryIndices();

            // Trigger validation
            wrapper.Name = "";
            wrapper.Code = "";
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
            {
                return;
            }

            if (!(dropInfo.Data is SummaryWrapper wrapper))
            {
                return;
            }

            var insertIndex = dropInfo.UnfilteredInsertIndex;

            if (dropInfo.VisualTarget is ItemsControl itemsControl)
            {
                if (itemsControl.Items is IEditableCollectionView editableItems)
                {
                    var newItemPlaceholderPosition = editableItems.NewItemPlaceholderPosition;
                    if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && insertIndex == 0)
                    {
                        ++insertIndex;
                    }
                    else if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd && insertIndex == itemsControl.Items.Count)
                    {
                        --insertIndex;
                    }
                }
            }

            var index = Summaries.IndexOf(wrapper);
            if (index != -1)
            {
                Summaries.RemoveAt(index);
                // so, is the source list the destination list too ?
                if (index < insertIndex)
                {
                    --insertIndex;
                }
            }

            Summaries.Insert(insertIndex, wrapper);

            RecalculateSummaryIndices();
        }

        private void RecalculateSummaryIndices()
        {
            for (int i = 0; i < Summaries.Count; i++)
            {
                Summaries[i].SortIndex = i;
            }
        }
    }
}
