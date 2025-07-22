using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlueSelfCheckout.WebApi.Models.Products
{
    /// <summary>
    /// Productos de acompañamiento para que puedan agregar al momento de seleccionar
    /// un pedido ITM2
    /// </summary>
    public class ProductAccompaniment
    {
        [Required]
        [StringLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [StringLength(150)]
        public string ItemName { get; set; } = string.Empty;

        public decimal PriceOld { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        // Llave foránea para la relación con Product
        [Required]
        [StringLength(50)]
        public string ProductItemCode { get; set; } = string.Empty;

        // Relación con Product
        [JsonIgnore]
        public Product? Product { get; set; }
    }
}// fin del namespace
