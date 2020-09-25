using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class IndustryDetailViewModel : DetailViewModelBase
    {
        private readonly IIndustryRepository _industryRepository;
        private IndustryWrapper _selectedIndustry;

        public IndustryDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IIndustryRepository industryRepository)
            : base(eventAggregator, messageDialogService)
        {
            _industryRepository = industryRepository;
            Title = "Industries";
            Industries = new ObservableCollection<IndustryWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        public IndustryWrapper SelectedIndustry
        {
            get { return _selectedIndustry; }
            set
            {
                _selectedIndustry = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<IndustryWrapper> Industries { get; }

        public async override Task LoadAsync(int id)
        {
            Id = id;
            foreach (var wrapper in Industries)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            Industries.Clear();

            var industries = await _industryRepository.GetAllAsync();
            foreach (var model in industries)
            {
                var wrapper = new IndustryWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                Industries.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _industryRepository.HasChanges();
            }
            if (e.PropertyName == nameof(IndustryWrapper.HasErrors))
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
            return HasChanges && Industries.All(i => !i.HasErrors);
        }

        protected async override void OnSaveExecute()
        {
            try
            {
                await _industryRepository.SaveAsync();
                HasChanges = _industryRepository.HasChanges();
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
            return SelectedIndustry != null;
        }

        private async void OnRemoveExecute()
        {
            var isReferenced =
                await _industryRepository.IsReferencedByStockAsync(
                    SelectedIndustry.Id);
            if (isReferenced)
            {
                await MessageDialogService.ShowInfoDialogAsync($"The industry {SelectedIndustry.Name}" +
                    $" can't be removed, as it is referenced by at least one stock");
                return;
            }
            SelectedIndustry.PropertyChanged -= Wrapper_PropertyChanged;
            _industryRepository.Remove(SelectedIndustry.Model);
            Industries.Remove(SelectedIndustry);
            SelectedIndustry = null;
            HasChanges = _industryRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            var wrapper = new IndustryWrapper(new Industry());
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            _industryRepository.Add(wrapper.Model);
            Industries.Add(wrapper);

            // Trigger validation
            wrapper.Name = "";
        }
    }
}
