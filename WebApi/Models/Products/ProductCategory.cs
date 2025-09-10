using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Products
{
    public class ProductCategory
    {
        [Key]
        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string CategoryItemCode { get; set; }

        [Required]
        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public required string CategoryItemName { get; set; }

        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public string? FrgnName { get; set; }


        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? ImageUrl { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? Description { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? FrgnDescription { get; set; }


        [Required]
        public required int VisOrder { get; set; }

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracter
        public required string Enabled { get; set; } = "Y";

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracter
        public required string DataSource { get; set; }


        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public string? GroupItemCode { get; set; }


        public virtual ICollection<CategoryAccompaniment> Accompaniments { get; set; } = new List<CategoryAccompaniment>();


    }// fin de la clase

}// fin del namespace
