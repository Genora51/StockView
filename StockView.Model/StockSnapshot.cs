using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockView.Model
{
    public class StockSnapshot
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [Index("IX_StockDate", 2, IsUnique = true)]
        public DateTime Date { get; set; }

        public float Value { get; set; }

        [Index("IX_StockDate", 1, IsUnique = true)]
        public int StockId { get; set; }

        public Stock Stock { get; set; }
    }
}
