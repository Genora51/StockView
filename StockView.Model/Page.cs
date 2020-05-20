using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace StockView.Model
{
    public class Page
    {
        public Page()
        {
            Stocks = new Collection<Stock>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public ICollection<Stock> Stocks { get; set; }
    }
}
