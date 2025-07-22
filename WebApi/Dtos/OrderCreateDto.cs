using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Dtos
{
    // En una carpeta como Dtos/Orders
    public class OrderCreateDto
    {

        [Required]
        public string FolioNum { get; set; }

        [StringLength(5)]
        public string FolioPref { get; set; }

        [StringLength(50)]
        public string CustomerCode { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        [StringLength(50)]
        public string NickName { get; set; }

        [StringLength(50)]
        public string DeviceCode { get; set; }

        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }

        [StringLength(1)]
        public string DocStatus { get; set; }

        [StringLength(1)]
        public string DocType { get; set; }

        [StringLength(1)]
        public string PaidType { get; set; }

        [StringLength(1)]
        public string Transferred { get; set; }

        [StringLength(1)]
        public string Printed { get; set; }

        public decimal? DocRate { get; set; }
        public decimal? DocTotal { get; set; }
        public decimal? DocTotalFC { get; set; }

        [StringLength(254)]
        public string Comments { get; set; }

        // Aquí no necesitamos DocEntry, ya que se generará en la BD
        // Y no incluimos las propiedades de navegación completas, solo las líneas como DTOs
        public ICollection<DocumentLineCreateDto> OrderLines { get; set; }
    }

    // DTO para crear una Línea de Documento
    // En una carpeta como Dtos/DocumentLines
    public class DocumentLineCreateDto
    {
        // No necesitamos DocEntry aquí, ya que el controlador lo asignará del padre
        // No necesitamos LineId, ya que puede ser generado en la capa de servicio

        [Required]
        [StringLength(50)]
        public string ItemCode { get; set; }

        [Required]
        [StringLength(150)]
        public string ItemName { get; set; }

        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }

        [StringLength(1)]
        public string LineStatus { get; set; }

        [StringLength(10)]
        public string TaxCode { get; set; }

        public decimal? LineTotal { get; set; }

        // ¡Importante! Aquí no tienes la propiedad "Order"
        // Esa relación se maneja a nivel de entidad de base de datos y DbContext
    }
}
