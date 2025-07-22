using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueSelfCheckout.WebApi.Models.Orders
{
    /// <summary>
    /// Representa una entidad de orden de la tabla ORDR en la base de datos BSC20.
    /// Contiene la información principal de los documentos de órdenes del sistema.
    /// </summary>
    [Table("ORDR", Schema = "dbo")]
    public class Order
    {
        /// <summary>
        /// Identificador único de la entrada del documento. Clave primaria de la tabla.
        /// Campo NOT NULL de tipo int.
        /// </summary>
        [Key]
        [Column("DocEntry")]
        [Required]
        public int DocEntry { get; set; }

        /// <summary>
        /// Prefijo del folio del documento. Máximo 5 caracteres.
        /// Campo nullable de tipo nvarchar(5).
        /// </summary>
        [Column("FolioPref")]
        [StringLength(5)]
        public string FolioPref { get; set; }

        /// <summary>
        /// Número del folio del documento. Máximo 50 caracteres.
        /// Campo NOT NULL de tipo nvarchar(50).
        /// </summary>
        [Column("FolioNum")]
        [StringLength(50)]
        [Required]
        public string FolioNum { get; set; }

        /// <summary>
        /// Código único del cliente asociado al documento. Máximo 50 caracteres.
        /// Campo nullable de tipo nvarchar(50).
        /// </summary>
        [Column("CustomerCode")]
        [StringLength(50)]
        public string CustomerCode { get; set; }

        /// <summary>
        /// Nombre completo del cliente. Máximo 100 caracteres.
        /// Campo nullable de tipo nvarchar(100).
        /// </summary>
        [Column("CustomerName")]
        [StringLength(100)]
        public string CustomerName { get; set; }

        /// <summary>
        /// Apodo o nombre corto del cliente. Máximo 50 caracteres.
        /// Campo nullable de tipo nvarchar(50).
        /// </summary>
        [Column("NickName")]
        [StringLength(50)]
        public string NickName { get; set; }

        /// <summary>
        /// Código del dispositivo desde el cual se generó el documento. Máximo 50 caracteres.
        /// Campo nullable de tipo nvarchar(50).
        /// </summary>
        [Column("DeviceCode")]
        [StringLength(50)]
        public string DeviceCode { get; set; }

        /// <summary>
        /// Fecha de creación del documento.
        /// Campo nullable de tipo datetime.
        /// </summary>
        [Column("DocDate")]
        public DateTime? DocDate { get; set; }

        /// <summary>
        /// Fecha de vencimiento del documento.
        /// Campo nullable de tipo datetime.
        /// </summary>
        [Column("DocDueDate")]
        public DateTime? DocDueDate { get; set; }

        /// <summary>
        /// Estado actual del documento. Un solo carácter (ej: 'P'=Pendiente, 'C'=Completado).
        /// Campo nullable de tipo char(1).
        /// </summary>
        [Column("DocStatus")]
        [StringLength(1)]
        public string DocStatus { get; set; }

        /// <summary>
        /// Tipo de documento. Un solo carácter (ej: 'O'=Orden, 'F'=Factura).
        /// Campo nullable de tipo char(1).
        /// </summary>
        [Column("DocType")]
        [StringLength(1)]
        public string DocType { get; set; }

        /// <summary>
        /// Tipo de pago utilizado para el documento. Un solo carácter.
        /// Campo nullable de tipo char(1).
        /// </summary>
        [Column("PaidType")]
        [StringLength(1)]
        public string PaidType { get; set; }

        /// <summary>
        /// Indica si el documento ha sido transferido. Un solo carácter ('Y'/'N').
        /// Campo nullable de tipo char(1).
        /// </summary>
        [Column("Transferred")]
        [StringLength(1)]
        public string Transferred { get; set; }

        /// <summary>
        /// Indica si el documento ha sido impreso. Un solo carácter ('Y'/'N').
        /// Campo nullable de tipo char(1).
        /// </summary>
        [Column("Printed")]
        [StringLength(1)]
        public string Printed { get; set; }

        /// <summary>
        /// Tasa de cambio aplicada al documento (para documentos en moneda extranjera).
        /// Campo nullable de tipo numeric(19,6).
        /// </summary>
        [Column("DocRate", TypeName = "decimal(19,6)")]
        public decimal? DocRate { get; set; }

        /// <summary>
        /// Total del documento en moneda local.
        /// Campo nullable de tipo numeric(19,6).
        /// </summary>
        [Column("DocTotal", TypeName = "decimal(19,6)")]
        public decimal? DocTotal { get; set; }

        /// <summary>
        /// Total del documento en moneda extranjera (Foreign Currency).
        /// Campo nullable de tipo numeric(19,6).
        /// </summary>
        [Column("DocTotalFC", TypeName = "decimal(19,6)")]
        public decimal? DocTotalFC { get; set; }

        /// <summary>
        /// Comentarios adicionales o notas del documento.
        /// Campo nullable (tipo no especificado en el esquema proporcionado).
        /// </summary>
        [Column("Comments")]
        [StringLength(254)]
        public string Comments { get; set; }


        // --- Propiedad de navegación para las líneas de la orden ---
        /// <summary>
        /// Colección de líneas de documentos asociadas a esta orden.
        /// </summary>
        public ICollection<OrderLine> OrderLines { get; set; }
    }
}