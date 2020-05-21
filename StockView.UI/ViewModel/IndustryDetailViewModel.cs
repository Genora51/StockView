using Prism.Events;
using StockView.UI.View.Services;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class IndustryDetailViewModel : DetailViewModelBase
    {
        public IndustryDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
            : base(eventAggregator, messageDialogService)
        {
            Title = "Industries";
        }

        public override Task LoadAsync(int id)
        {
            // TODO: Load data here
            Id = id;
            return Task.Delay(0);
        }

        protected override void OnDeleteExecute()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}
