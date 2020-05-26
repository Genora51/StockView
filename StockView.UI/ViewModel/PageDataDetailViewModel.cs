using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class PageDataDetailViewModel : DetailViewModelBase
    {
        private IPageDataRepository _pageDataRepository;
        private PageWrapper _page;
        private StockSnapshotWrapper _selectedSnapshot;

        public PageDataDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IPageDataRepository pageDataRepository)
            : base(eventAggregator, messageDialogService)
        {
            _pageDataRepository = pageDataRepository;
            // TODO: Reload snapshots on detail save/delete
        }

        public PageWrapper Page
        {
            get { return _page; }
            private set
            {
                _page = value;
                OnPropertyChanged();
            }
        }

        public StockSnapshotWrapper SelectedSnapshot
        {
            get { return _selectedSnapshot; }
            set
            {
                _selectedSnapshot = value;
                OnPropertyChanged();
                // TODO: ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<StockWrapper> Stocks { get; }
        public ObservableCollection<StockSnapshotWrapper> Snapshots { get; }

        public async override Task LoadAsync(int pageId)
        {
            var page = await _pageDataRepository.GetByIdAsync(pageId);

            Id = pageId;
            InitialisePage(page);
        }

        private void InitialisePage(Page page)
        {
            Page = new PageWrapper(page);
            Page.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _pageDataRepository.HasChanges();
                }

                if (e.PropertyName == nameof(Page.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            Title = Page.Title;
        }

        protected override void OnDeleteExecute()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return Page != null && !Page.HasErrors && HasChanges;
        }

        protected override void OnSaveExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}
