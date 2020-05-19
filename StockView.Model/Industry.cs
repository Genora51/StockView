using System.ComponentModel.DataAnnotations;

namespace StockView.Model
{
    public class Industry
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
