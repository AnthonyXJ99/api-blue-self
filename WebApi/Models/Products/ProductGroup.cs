using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Products
{
    public class ProductGroup

    {
        [Key]
        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string ProductGroupCode { get; set; }

        [Required]
        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public required string ProductGroupName { get; set; }

        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public string? FrgnName { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? ImageUrl { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? Description { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? FrgnDescription { get; set; }


        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracter
        public string? Enabled { get; set; } = "Y";

        [Required]
        public int VisOrder { get; set; }

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracter
        public string? DataSource { get; set; }

        [StringLength(50)] // Establece el tamaño máximo a 50 caracter
        public string? ProductGroupCodeERP { get; set; }

        [StringLength(50)] // Establece el tamaño máximo a 50 caracter
        public string? ProductGroupCodePOS { get; set; }



    }// fin de la clase
}// fin del namespace
