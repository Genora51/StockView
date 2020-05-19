using StockView.Model;
using StockView.UI.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace StockView.UI.Wrapper
{
    public class StockWrapper : ViewModelBase, INotifyDataErrorInfo
    {
        public StockWrapper(Stock model)
        {
            Model = model;
        }

        public Stock Model { get; }

        public int Id { get { return Model.Id; } }

        public string Symbol
        {
            get { return Model.Symbol; }
            set
            {
                Model.Symbol = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Symbol));
            }
        }

        private void ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch(propertyName)
            {
                case nameof(Symbol):
                    if (string.Equals(Symbol, "INVL", StringComparison.OrdinalIgnoreCase))
                    {
                        AddError(propertyName, "Invalid Symbol");
                    }
                    break;
            }
        }

        public string CompanyName
        {
            get { return Model.CompanyName; }
            set
            {
                Model.CompanyName = value;
                OnPropertyChanged();
            }
        }

        public string Industry
        {
            get { return Model.Industry; }
            set
            {
                Model.Industry = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, List<string>> _errorsByPropertyName
            = new Dictionary<string, List<string>>();

        public bool HasErrors => _errorsByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName)
                ? _errorsByPropertyName[propertyName]
                : null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName[propertyName] = new List<string>();
            }
            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}
