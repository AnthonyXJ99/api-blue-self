using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Models.Orders
{
    [Table("RDR1")]
    public class OrderLine
    {
        // DocEntry y LineId forman una clave primaria compuesta
        [Key]
        [Column(Order = 1)] // Define el orden de la columna en la clave compuesta
        public int DocEntry { get; set; }

        [Key]
        [Column(Order = 2)] // Define el orden de la columna en la clave compuesta
        public int LineId { get; set; }

        [Required] // No nulo
        [StringLength(50)] // Longitud máxima
        public string ItemCode { get; set; }

        [Required] // No nulo
        [StringLength(150)] // Longitud máxima
        public string ItemName { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Tipo de dato SQL con precisión y escala
        public decimal? Quantity { get; set; } // El '?' indica que es anulable

        [Column(TypeName = "decimal(18, 2)")] // Tipo de dato SQL con precisión y escala
        public decimal? Price { get; set; } // El '?' indica que es anulable

        [StringLength(1)] // Longitud máxima
        public string LineStatus { get; set; } // Por defecto es anulable si no se usa [Required]

        [StringLength(10)] // Longitud máxima
        public string TaxCode { get; set; } // Por defecto es anulable si no se usa [Required]

        [Column(TypeName = "decimal(18, 2)")] // Tipo de dato SQL con precisión y escala
        public decimal? LineTotal { get; set; } // El '?' indica que es anulable

        // --- Propiedad de navegación y clave foránea ---
        /// <summary>
        /// Propiedad de navegación a la entidad Order.
        /// </summary>
        [ForeignKey("DocEntry")] // Especifica que DocEntry es la clave foránea a la entidad Order
        public Order Order { get; set; }
    }
}
