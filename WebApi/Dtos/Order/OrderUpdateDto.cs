namespace BlueSelfCheckout.WebApi.Dtos.Order
{
    // DTOs/OrderUpdateDto.cs
    public class OrderUpdateDto
    {
        // No incluir DocEntry aquí, ya que viene de la URL (id)

        public string? FolioPref { get; set; }
        public string FolioNum { get; set; } = string.Empty; // Asegurar no nulo si es requerido
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? NickName { get; set; }
        public string? DeviceCode { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public string? DocStatus { get; set; } // Considera usar un enum si los estados son fijos
        public string? DocType { get; set; } // Considera usar un enum
        public string? PaidType { get; set; } // Considera usar un enum
        public string? Transferred { get; set; }
        public string? Printed { get; set; }
        public decimal? DocRate { get; set; }
        public decimal? DocTotal { get; set; }
        public decimal? DocTotalFC { get; set; }
        public string? Comments { get; set; }

        // Colección de líneas para actualizar (usando un DTO específico para líneas de actualización)
        public ICollection<DocumentLineUpdateDto>? OrderLines { get; set; }
    }

    // DTOs/DocumentLineUpdateDto.cs
    // Este DTO debe tener el LineId para identificar qué línea se está actualizando
    public class DocumentLineUpdateDto
    {
        // DocEntry no es necesario aquí, ya que se asume del padre (OrderUpdateDto)
        public int LineId { get; set; } // Clave para identificar la línea existente o si es nueva (LineId = 0 o no presente)
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string LineStatus { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public decimal LineTotal { get; set; }
        // Puedes añadir una bandera como 'IsDeleted' si manejas borrados lógicos
    }
}
