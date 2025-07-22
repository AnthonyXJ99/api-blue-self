using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlueSelfCheckout.WebApi.Models.Products
{
    /// <summary>
    /// Receta de materiales
    /// </summary>
    public class ProductMaterial
    {
        [Required]
        [StringLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [StringLength(150)]
        public string ItemName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0.")]
        public decimal Quantity { get; set; }

        public string? ImageUrl { get; set; }

        /// <summary>
        /// es Y cuando el ingrediente esta si o si en la receta por defecto
        /// </summary>
        [StringLength(1)]
        public string IsPrimary { get; set; } = "N";

        // Llave foránea para la relación con Product
        [Required]
        [StringLength(50)]
        public string ProductItemCode { get; set; } = string.Empty;

        // Relación con Product
        [JsonIgnore]
        public Product? Product { get; set; }
    }

}// fin del namespace
