using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Admin
{
    public class SalesTaxCodes
    {
        [Key]
        [Required]
        [StringLength(10)] // Establece el tamaño máximo a 50 caracteres
        public string TaxCode { get; set; }

        [Required]
        [StringLength(100)] // Establece el tamaño máximo a 150 caracteres
        public string TaxName { get; set; }
        public decimal Rate { get; set; }

        public string DataSource { get; set; }

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public string Enabled { get; set; }



    }// fin de la clase


}// fin del namespace
