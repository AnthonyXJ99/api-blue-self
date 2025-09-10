using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueSelfCheckout.WebApi.Models.Products
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CategoryAccompaniment
    {
        [Key]
        [Column(Order = 0)]
        public string CategoryItemCode { get; set; }

        [Key]
        [Column(Order = 1)]
        public int LineNumber { get; set; }

        public string AccompanimentItemCode { get; set; } // Sin [ForeignKey] aquí
        public decimal? Discount { get; set; }

        public string? EnlargementItemCode { get; set; } // Sin [ForeignKey] aquí
        public decimal? EnlargementDiscount { get; set; }

        // Navigation properties correctamente configuradas
        public virtual ProductCategory Category { get; set; }
        public virtual Product AccompanimentProduct { get; set; }
        public virtual Product? EnlargementProduct { get; set; }
    }

}
