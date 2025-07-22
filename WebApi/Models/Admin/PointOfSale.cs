using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Admin
{
    public class PointOfSale
    {


        [Key]
        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string PosCode { get; set; }

        [Required]
        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public required string PosName { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public required string Enabled { get; set; } = "Y";  // Valor por defecto "Y"

        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public string? Datasource { get; set; }

        [StringLength(20)] // Establece el tamaño máximo a 20 caracteres
        public string? SISCode { get; set; }

        [StringLength(20)] // Establece el tamaño máximo a 20 caracteres
        public string? TaxIdentNumber { get; set; }

    }
}
