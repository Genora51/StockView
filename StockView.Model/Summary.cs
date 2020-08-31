using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    }
}
