﻿using Prism.Commands;
using Prism.Events;
using StockView.UI.Event;
using StockView.UI.View.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private Func<IStockDetailViewModel> _stockDetailViewModelCreator;
        private IMessageDialogService _messageDialogService;
        private IDetailViewModel _detailViewModel;

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IStockDetailViewModel> stockDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _stockDetailViewModelCreator = stockDetailViewModelCreator;
            _messageDialogService = messageDialogService;

            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<AfterStockDeletedEvent>()
                .Subscribe(AfterStockDeleted);

            CreateNewStockCommand = new DelegateCommand(OnCreateNewStockExecute);

            NavigationViewModel = navigationViewModel;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewStockCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }

        public IDetailViewModel DetailViewModel
        {
            get { return _detailViewModel; }
            private set {
                _detailViewModel = value;
                OnPropertyChanged();
            }
        }

        private void AfterStockDeleted(int stockId)
        {
            DetailViewModel = null;
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You've made changes. Navigate away?", "Question");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            
            switch (args.ViewModelName)
            {
                case nameof(StockDetailViewModel):
                    DetailViewModel = _stockDetailViewModelCreator();
                    break;
            }
            await DetailViewModel.LoadAsync(args.Id);
        }

        private void OnCreateNewStockExecute()
        {
            OnOpenDetailView(null);
        }
    }
}
