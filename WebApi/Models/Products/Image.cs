using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueSelfCheckout.WebApi.Models.Products
{
    /// <summary>
    /// Modelo que representa una imagen en el sistema.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Código único de la imagen (Primary Key).
        /// </summary>
        [Key]
        [StringLength(50)]
        public string ImageCode { get; set; }

        /// <summary>
        /// Título descriptivo de la imagen.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ImageTitle { get; set; }

        /// <summary>
        /// Tipo de imagen: Logo, Publicidad, Item.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ImageType { get; set; }

        /// <summary>
        /// Descripción opcional de la imagen.
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Nombre del archivo generado automáticamente.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FileName { get; set; }

        /// <summary>
        /// Ruta física completa del archivo.
        /// </summary>
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        /// <summary>
        /// URL pública para acceder a la imagen.
        /// </summary>
        [Required]
        [StringLength(500)]
        public string PublicUrl { get; set; }

        /// <summary>
        /// Nombre original del archivo subido por el usuario.
        /// </summary>
        [StringLength(200)]
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Tamaño del archivo en bytes.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Tipo MIME del archivo.
        /// </summary>
        [StringLength(100)]
        public string ContentType { get; set; }

        /// <summary>
        /// Fecha y hora de creación del registro.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha y hora de la última actualización.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Fecha y hora de eliminación (soft delete).
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Indica si la imagen está activa (no eliminada).
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Orden de visualización (opcional).
        /// </summary>
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Texto alternativo para accesibilidad.
        /// </summary>
        [StringLength(200)]
        public string? AltText { get; set; }

        /// <summary>
        /// Tags o etiquetas para clasificación adicional.
        /// </summary>
        [StringLength(500)]
        public string? Tags { get; set; }

        /// <summary>
        /// Foreign key para asociar la imagen a un dispositivo. para la publicidad
        /// 
        public string? DeviceCode { get; set; }
    }
}