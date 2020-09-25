using Prism.Commands;
using Prism.Events;
using StockView.UI.View.Services;
using System;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class SettingsDetailViewModel : DetailViewModelBase
    {
        // TODO: use a propper wrapper object [enable validation etc]
        private string _apiKey;
        private string originalApiKey;

        public SettingsDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
            : base(eventAggregator, messageDialogService)
        {
            Title = "Preferences";
        }

        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                _apiKey = value;
                HasChanges = originalApiKey != _apiKey;
                OnPropertyChanged();
            }
        }

        public override Task LoadAsync(int id)
        {
            Id = id;
            originalApiKey = Properties.Settings.Default["api_key"] as string;
            ApiKey = originalApiKey;
            return Task.CompletedTask;
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges;
        }

        protected override void OnSaveExecute()
        {
            Properties.Settings.Default["api_key"] = ApiKey;
            originalApiKey = ApiKey;
            HasChanges = false;
            Properties.Settings.Default.Save();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
}
