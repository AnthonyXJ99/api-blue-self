using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Admin
{
    public class ShippingTypes
    {
        [Key]
        [Required]
        [StringLength(6)] // Establece el tamaño máximo a 50 caracteres
        public string ShippingCode { get; set; }

        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public string ShippingName { get; set; }

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 50 caracteres
        public string DataSource { get; set; }


        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 50 caracteres
        public string Enabled { get; set; }


    }// fin de la clase

}// fin del namespace
