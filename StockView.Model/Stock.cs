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
        }

        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string Symbol { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }

        public int? IndustryId { get; set; }

        public Industry Industry { get; set; }

        public ICollection<StockSnapshot> Snapshots { get; set; }
    }
}
