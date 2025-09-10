namespace BlueSelfCheckout.WebApi.Dtos.Product
{
    public class CategoryAccompanimentForCreationDTO
    {
        public string AccompanimentItemCode { get; set; }
        // public decimal? Price { get; set; } // ¡QUITAR ESTO! No existe en la entidad
        public decimal? Discount { get; set; }
        public string? EnlargementItemCode { get; set; }
        public decimal? EnlargementDiscount { get; set; }
    }

    public class CategoryAccompanimentForUpdateDTO
    {
        // public decimal? Price { get; set; } // ¡QUITAR ESTO! No existe en la entidad
        public decimal? Discount { get; set; }
        public string? EnlargementItemCode { get; set; }
        public decimal? EnlargementDiscount { get; set; }
    }
    public class CategoryWithAccompanimentsDTO
    {
        public string CategoryItemCode { get; set; }
        public string CategoryItemName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public List<CategoryAccompanimentDTO> AvailableAccompaniments { get; set; } // Mejor nombre
    }

    public class CategoryAccompanimentDTO
    {
        public int LineNumber { get; set; }
        public string AccompanimentItemCode { get; set; }
        public string AccompanimentItemName { get; set; }
        public string? AccompanimentImageUrl { get; set; }
        public decimal AccompanimentPrice { get; set; } // Precio base del producto
        public decimal? Discount { get; set; } // Descuento normal

        // Campos para el sistema de agrandamiento
        public string? EnlargementItemCode { get; set; } // 'AGRANDA', 'UPGRADE', etc.
        public decimal? EnlargementDiscount { get; set; } // Descuento cuando se activa el enlargement

        // Propiedades calculadas para el frontend
        public bool CanBeEnlarged => !string.IsNullOrEmpty(EnlargementItemCode);
        public decimal FinalPriceWithEnlargement => CanBeEnlarged ?
            AccompanimentPrice - (EnlargementDiscount ?? 0) : AccompanimentPrice;
    }


    public class CategoryAccompanimentUpdateBatchDTO
    {
        public int LineNumber { get; set; }
        public CategoryAccompanimentForUpdateDTO UpdateData { get; set; }
    }

}
