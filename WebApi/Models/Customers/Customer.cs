using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Customers
{
    public class Customer
    {
        [Key]
        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string CustomerCode { get; set; }


        [Required]
        [StringLength(100)] // Establece el tamaño máximo a 100 caracteres
        public required string CustomerName { get; set; }


        [StringLength(20)] // Establece el tamaño máximo a 20 caracteres
        public string? TaxIdentNumber { get; set; }


        //[StringLength(100)] // Establece el tamaño máximo a 100 caracteres
        //public string? Address { get; set; }



        [StringLength(15)] // Establece el tamaño máximo a 15 caracteres
        public string? CellPhoneNumber { get; set; }



        [StringLength(100)] // Establece el tamaño máximo a 100 caracteres
        public string? Email { get; set; }

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public required string Enabled { get; set; } = "Y";  // Valor por defecto "Y"

        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public string? Datasource { get; set; }

        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string CustomerGroupCode { get; set; }



    }// fin de la clase

}// fin del namespace
