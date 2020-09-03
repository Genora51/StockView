using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockView.Model
{
    public class Summary
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [MaxLength]
        public string Code { get; set; }

        public bool Enabled { get; set; } = true;

        public int SortIndex { get; set; }
    }
}
