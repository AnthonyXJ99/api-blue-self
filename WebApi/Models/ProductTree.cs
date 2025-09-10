using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueSelfCheckout.Models
{
    public class ProductTree
    {
        [Key]
        [Required]
        [StringLength(50)] // Establece el tamaño máximo a 50 caracteres
        public required string ItemCode { get; set; }
                               
        [Required]
        [StringLength(150)] // Establece el tamaño máximo a 150 caracteres
        public string ItemName { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public string Enabled { get; set; } = "Y";

        public string DataSource { get; set; }


        public List<ProductTreeItem1> Items1 { get; set; } = new List<ProductTreeItem1>();

        public ProductTree()
        {
            Enabled = "Y";
        }

    }// fin de la clase

    /// <summary>
    /// representa la receta del producto
    /// </summary>
    public class ProductTreeItem1
    {
        // Composite Primary Key with LineNumber
        [Key]
        [Column(Order = 0)]
        [Required]
        [StringLength(50)]
        public string ItemCode { get; set; }

        [Key]
        [Column(Order = 1)]
        public int LineNumber { get; set; }

        [StringLength(150)]
        public string ItemName { get; set; }

        /// <summary>
        /// Cantidad del producto en la receta.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// URL de la imagen asociada al producto.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Llave foránea para la relación con ProductTree
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public required string ProductTreeItemCode { get; set; }

        /// <summary>
        /// Indica si el producto es personalizable. "Y" para sí, "N" para no.
        /// </summary>
        [StringLength(1)]
        public string? IsCustomizable { get; set; } = "Y";

    }

    /// <summary>
    /// representa los productos asociados o acompañamientos con el producto
    /// </summary>
    public class ProductTreeItem2
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string ItemCode { get; set; }

        [StringLength(150)]
        public string ItemName { get; set; }

        public string? ImageUrl { get; set; }

        // Llave foránea para la relación con ProductTree
        public string ProductTreeItemCode { get; set; }


    }// FIN DE LA CLASE




}// fin del namespace
