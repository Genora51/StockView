﻿using StockView.Model;
using System;
using System.Collections.Generic;

namespace StockView.UI.Wrapper
{
    public class StockWrapper : ModelWrapper<Stock>
    {

        public StockWrapper(Stock model) : base(model)
        {
        }
        public int Id { get { return Model.Id; } }

        public string Symbol
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Industry
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Symbol):
                    if (string.Equals(Symbol, "INVL", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Invalid Symbol";
                    }
                    break;
            }
        }
    }
}