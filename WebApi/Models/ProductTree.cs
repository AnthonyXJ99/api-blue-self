using System.ComponentModel.DataAnnotations;

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
        [Key]
        [Required]
        [StringLength(50)]
        public string ItemCode { get; set; }

        [StringLength(150)]
        public string ItemName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0.")]
        public decimal Quantity { get; set; }

        public string? ImageUrl { get; set; }

        // Llave foránea para la relación con ProductTree
        public string ProductTreeItemCode { get; set; }


    }// FIN DE LA CLASE



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
