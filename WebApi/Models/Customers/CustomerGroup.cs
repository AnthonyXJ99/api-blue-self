using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueSelfCheckout.WebApi.Models.Customers
{
    public class CustomerGroup
    {
        [Key]
        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string CustomerGroupCode { get; set; }


        [Required]
        [StringLength(100)] // Establece el tamaño máximo a 100 caracteres
        public required string CustomerGroupName { get; set; }


        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public required string Enabled { get; set; } = "Y";  // Valor por defecto "Y"

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public required string? Datasource { get; set; }

    }// fin de la clase

}// fin del namespace
