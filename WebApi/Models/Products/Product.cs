using BlueSelfCheckout.Models;
using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Products
{
    public class Product
    {
        [Key]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string ItemCode { get; set; }

        [Required]
        [StringLength(50)]
        public string EANCode { get; set; } = string.Empty;


        [Required]
        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public required string ItemName { get; set; }


        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public string? FrgnName { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? ImageUrl { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? Description { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? FrgnDescription { get; set; }


        [StringLength(1)] // Establece el tamaño máximo a 255 caracteres
        public required string SellItem { get; set; } = "Y";  // Valor por defecto "Y"

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracter
        public required string Available { get; set; } = "Y";  // Valor por defecto "Y"

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracter
        public required string Enabled { get; set; } = "Y";  // Valor por defecto "Y"

        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public string? GroupItemCode { get; set; }

        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public string? CategoryItemCode { get; set; }

        [StringLength(50)]
        public string? WaitingTime { get; set; }

        public decimal? Rating { get; set; }



        // Enlaza con los detalles

        /// <summary>
        /// ITM1 La receta y los posibles productos que puedan agregar
        /// </summary>
        public List<ProductMaterial> Material { get; set; } = new List<ProductMaterial>();

        /// <summary>
        /// ITM2 Los productos con los que puede acompañar el producto principal de compra
        /// </summary>
        public List<ProductAccompaniment> Accompaniment { get; set; } = new List<ProductAccompaniment>();

        public ProductTree? ProductTree { get; set; }


    }// fin de la clase

}// fin del namespace
