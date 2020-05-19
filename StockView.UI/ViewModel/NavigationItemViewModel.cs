using Prism.Commands;
using Prism.Events;
using StockView.UI.Event;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
	public class NavigationItemViewModel : ViewModelBase
    {
		private string _displayMember;
		private IEventAggregator _eventAggregator;

		public NavigationItemViewModel(int id, string displayMember,
			IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
			Id = id;
			DisplayMember = displayMember;
			OpenStockDetailViewCommand = new DelegateCommand(OnOpenStockDetailView);
		}

		private void OnOpenStockDetailView()
		{
			_eventAggregator.GetEvent<OpenStockDetailViewEvent>()
						.Publish(Id);
		}

		public int Id { get; }

		public string DisplayMember
		{
			get { return _displayMember; }
			set {
				_displayMember = value;
				OnPropertyChanged();
			}
		}

		public ICommand OpenStockDetailViewCommand { get; }

	}
}
