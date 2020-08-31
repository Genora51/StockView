using StockView.Model.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace StockView.Model
{
    public class Stock
    {
        public Stock()
        {
            Snapshots = new Collection<StockSnapshot>();
            Pages = new Collection<Page>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string Symbol { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }

        public decimal Cost { get; set; }

        [DecimalPrecision(18, 3)]
        public decimal Yield { get; set; }

        public int Shares { get; set; }

        public int? IndustryId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Industry Industry { get; set; }

        public ICollection<StockSnapshot> Snapshots { get; set; }

        public ICollection<Page> Pages { get; set; }
    }
}
