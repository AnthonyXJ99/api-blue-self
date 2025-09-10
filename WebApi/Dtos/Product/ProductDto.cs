using System.ComponentModel.DataAnnotations;

namespace BlueSelfCheckout.WebApi.Dtos.Product
{
    /// <summary>
    /// Representa un producto con sus propiedades y relaciones.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Código único del artículo proporcionado por el cliente.
        /// </summary>
        [Required(ErrorMessage = "El código del artículo es requerido.")]
        [StringLength(50, ErrorMessage = "El código del artículo no puede exceder los 50 caracteres.")]
        public string ItemCode { get; set; } = string.Empty;

        /// <summary>
        /// Código EAN del artículo.
        /// </summary>
        [StringLength(50, ErrorMessage = "El código EAN no puede exceder los 50 caracteres.")]
        public string? EANCode { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del artículo.
        /// </summary>
        [Required(ErrorMessage = "El nombre del artículo es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre del artículo no puede exceder los 150 caracteres.")]
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del artículo en idioma extranjero.
        /// </summary>
        [StringLength(150, ErrorMessage = "El nombre en idioma extranjero no puede exceder los 150 caracteres.")]
        public string? FrgnName { get; set; }

        /// <summary>
        /// Precio del artículo.
        /// </summary>
        [Required(ErrorMessage = "El precio es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Descuento aplicado al artículo, en porcentaje.
        /// </summary>
        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
        public decimal? Discount { get; set; }

        /// <summary>
        /// URL de la imagen del artículo.
        /// </summary>
        [StringLength(255, ErrorMessage = "La URL de la imagen no puede exceder los 255 caracteres.")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Descripción del artículo.
        /// </summary>
        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres.")]
        public string? Description { get; set; }

        /// <summary>
        /// Descripción del artículo en idioma extranjero.
        /// </summary>
        [StringLength(255, ErrorMessage = "La descripción en idioma extranjero no puede exceder los 255 caracteres.")]
        public string? FrgnDescription { get; set; }

        /// <summary>
        /// Indica si el artículo está disponible para la venta ('Y' o 'N').
        /// </summary>
        [StringLength(1, ErrorMessage = "SellItem debe ser 'Y' o 'N'.")]
        [RegularExpression("^[YN]$", ErrorMessage = "SellItem solo puede ser 'Y' o 'N'.")]
        public string SellItem { get; set; } = "Y";

        /// <summary>
        /// Indica si el artículo está disponible ('Y' o 'N').
        /// </summary>
        [StringLength(1, ErrorMessage = "Available debe ser 'Y' o 'N'.")]
        [RegularExpression("^[YN]$", ErrorMessage = "Available solo puede ser 'Y' o 'N'.")]
        public string Available { get; set; } = "Y";

        /// <summary>
        /// Indica si el artículo está habilitado ('Y' o 'N').
        /// </summary>
        [StringLength(1, ErrorMessage = "Enabled debe ser 'Y' o 'N'.")]
        [RegularExpression("^[YN]$", ErrorMessage = "Enabled solo puede ser 'Y' o 'N'.")]
        public string Enabled { get; set; } = "Y";

        /// <summary>
        /// Código del grupo al que pertenece el artículo.
        /// </summary>
        [StringLength(50, ErrorMessage = "El código de grupo no puede exceder los 50 caracteres.")]
        public string? GroupItemCode { get; set; }

        /// <summary>
        /// Código de la categoría del artículo.
        /// </summary>
        [StringLength(50, ErrorMessage = "El código de categoría no puede exceder los 50 caracteres.")]
        public string? CategoryItemCode { get; set; }

        /// <summary>
        /// Tiempo de espera estimado para el artículo.
        /// </summary>
        [StringLength(50, ErrorMessage = "El tiempo de espera no puede exceder los 50 caracteres.")]
        public string? WaitingTime { get; set; }

        /// <summary>
        /// Calificación promedio del artículo (0.0 a 5.0).
        /// </summary>
        [Range(0, 5.0, ErrorMessage = "La calificación debe estar entre 0.0 y 5.0.")]
        public decimal? Rating { get; set; }

        /// <summary>
        /// Materiales asociados al artículo.
        /// </summary>
        //public List<ProductMaterialCreateDto>? Material { get; set; }
        public List<ProductTreeItem1Dto>? Material { get; set; }

        /// <summary>
        /// Acompañamientos asociados al artículo.
        /// </summary>
       // public List<ProductAccompanimentCreateDto>? Accompaniment { get; set; }
    }
   
    
    public class ProductMaterialCreateDto
    {
        [Required(ErrorMessage = "El código del material es requerido.")]
        [StringLength(50, ErrorMessage = "El código del material no puede exceder los 50 caracteres.")]
        public string ItemCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del material es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre del material no puede exceder los 150 caracteres.")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad del material es requerida.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public decimal Quantity { get; set; }

        [StringLength(255, ErrorMessage = "La URL de la imagen del material no puede exceder los 255 caracteres.")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "IsPrimary es requerido.")]
        [StringLength(1, ErrorMessage = "IsPrimary debe ser 'Y' o 'N'.")]
        [RegularExpression("^[YN]$", ErrorMessage = "IsPrimary solo puede ser 'Y' o 'N'.")]
        public string IsPrimary { get; set; } = "N"; // Valor por defecto sensato

        // ProductItemCode y la navegación a Product no son necesarios aquí,
        // ya que la relación se establecerá al mapear en el controlador.
    }

    public class ProductAccompanimentCreateDto
    {
        [Required(ErrorMessage = "El código del acompañamiento es requerido.")]
        [StringLength(50, ErrorMessage = "El código del acompañamiento no puede exceder los 50 caracteres.")]
        public string ItemCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del acompañamiento es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre del acompañamiento no puede exceder los 150 caracteres.")]
        public string ItemName { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "El precio anterior debe ser mayor o igual a 0.")]
        public decimal PriceOld { get; set; }

        [Required(ErrorMessage = "El precio es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Price { get; set; }

        [StringLength(255, ErrorMessage = "La URL de la imagen del acompañamiento no puede exceder los 255 caracteres.")]
        public string? ImageUrl { get; set; }

        // ProductItemCode y la navegación a Product no son necesarios aquí,
        // ya que la relación se establecerá al mapear en el controlador.
    }


    // Para crear productos (sin Material/Accompaniment)
    public class ProductCreateDto
    {
        public string ItemCode { get; set; } = string.Empty;
        public string? EANCode { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? FrgnName { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? FrgnDescription { get; set; }
        public string SellItem { get; set; } = "Y";
        public string Available { get; set; } = "Y";
        public string Enabled { get; set; } = "Y";
        public string? GroupItemCode { get; set; }
        public string? CategoryItemCode { get; set; }
        public string? WaitingTime { get; set; }
        public decimal? Rating { get; set; }
    }

    // Para actualizar productos (sin Material/Accompaniment)
    public class ProductUpdateDto
    {
        public string ItemCode { get; set; } = string.Empty;
        public string? EANCode { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? FrgnName { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? FrgnDescription { get; set; }
        public string SellItem { get; set; } = "Y";
        public string Available { get; set; } = "Y";
        public string Enabled { get; set; } = "Y";
        public string? GroupItemCode { get; set; }
        public string? CategoryItemCode { get; set; }
        public string? WaitingTime { get; set; }
        public decimal? Rating { get; set; }
    }

}
