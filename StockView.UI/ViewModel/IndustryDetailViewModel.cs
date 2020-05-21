using Prism.Commands;
using Prism.Events;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class IndustryDetailViewModel : DetailViewModelBase
    {
        private IIndustryRepository _industryRepository;

        public IndustryDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IIndustryRepository industryRepository)
            : base(eventAggregator, messageDialogService)
        {
            _industryRepository = industryRepository;
            Title = "Industries";
            Industries = new ObservableCollection<IndustryWrapper>();
        }

        public ObservableCollection<IndustryWrapper> Industries { get; }

        public async override Task LoadAsync(int id)
        {
            // TODO: Load data here
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
            await _industryRepository.SaveAsync();
            HasChanges = _industryRepository.HasChanges();
            RaiseCollectionSavedEvent();
        }
    }
}
