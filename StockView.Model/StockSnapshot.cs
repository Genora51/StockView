using System;
using System.ComponentModel.DataAnnotations;

namespace StockView.Model
{
    public class StockSnapshot
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; }

        public int StockId { get; set; }

        public Stock Stock { get; set; }
    }
}
