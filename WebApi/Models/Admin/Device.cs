using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Admin
{
    public class Device
    {

        [Key]
        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string DeviceCode { get; set; }

        [Required]
        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public required string DeviceName { get; set; }

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public string Enabled { get; set; } = "Y";

        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string IpAddress { get; set; }

        [Required]
        [StringLength(1)] // Establece el tamaño máximo a 1 caracteres
        public required string DataSource { get; set; }

        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 20 caracteres
        public required string PosCode { get; set; }

    }// fin de la clase


}// fin del namespace
