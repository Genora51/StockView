using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System.Threading.Tasks;


namespace StockView.UI.ViewModel
{
    public class PageDetailViewModel : DetailViewModelBase, IPageDetailViewModel
    {
        private IPageRepository _pageRepository;
        private PageWrapper _page;
        private IMessageDialogService _messageDialogService;

        public PageDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IPageRepository pageRepository) : base(eventAggregator)
        {
            _pageRepository = pageRepository;
            _messageDialogService = messageDialogService;
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

        public async override Task LoadAsync(int? pageId)
        {
            var page = pageId.HasValue
                ? await _pageRepository.GetByIdAsync(pageId.Value)
                : CreateNewPage();

            InitialisePage(page);
        }

        protected override void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to delete the page {Page.Title}?", "Question");
            if (result == MessageDialogResult.OK)
            {
                _pageRepository.Remove(Page.Model);
                _pageRepository.SaveAsync();
                RaiseDetailDeletedEvent(Page.Id);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Page != null && !Page.HasErrors && HasChanges;
        }

        protected async override void OnSaveExecute()
        {
            await _pageRepository.SaveAsync();
            HasChanges = _pageRepository.HasChanges();
            RaiseDetailSavedEvent(Page.Id, Page.Title);
        }

        private Page CreateNewPage()
        {
            var page = new Page();
            _pageRepository.Add(page);
            return page;
        }

        private void InitialisePage(Page page)
        {
            Page = new PageWrapper(page);
            Page.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _pageRepository.HasChanges();
                }

                if (e.PropertyName == nameof(Page.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
}
