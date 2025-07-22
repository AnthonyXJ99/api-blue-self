using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Admin
{
    public class Numeration
    {
        [Key]
        [Required]
        [StringLength(10)] // Establece el tamaño máximo a 10 caracteres
        public string ObjectCode { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0.")]
        public int InitialNum { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int NextNumber { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int LastNum { get; set; }

        [StringLength(10)] // Establece el tamaño máximo a 10 caracteres
        public string? Prefix { get; set; }

        [StringLength(255)] // Establece el tamaño máximo a 255 caracteres
        public string? Comments { get; set; }

        [StringLength(20)] // Establece el tamaño máximo a 20 caracteres
        public string PeriodCode { get; set; }


    }// fin de la clase

}// fin del namespace
