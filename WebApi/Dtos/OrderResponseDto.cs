namespace BlueSelfCheckout.WebApi.Dtos
{
    public class OrderResponseDto
    {
        public int DocEntry { get; set; }
        public string FolioPref { get; set; }
        public string FolioNum { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string NickName { get; set; }
        public string DeviceCode { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public string DocStatus { get; set; }
        public string DocType { get; set; }
        public string PaidType { get; set; }
        public string Transferred { get; set; }
        public string Printed { get; set; }
        public decimal? DocRate { get; set; }
        public decimal? DocTotal { get; set; }
        public decimal? DocTotalFC { get; set; }
        public string Comments { get; set; }

        // Aquí incluyes la colección de líneas, pero usando el DTO de respuesta para las líneas
        public ICollection<DocumentLineResponseDto> OrderLines { get; set; }
    }

    public class DocumentLineResponseDto
    {
        public int DocEntry { get; set; } // Incluir la clave foránea si quieres mostrarla
        public int LineId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string LineStatus { get; set; }
        public string TaxCode { get; set; }
        public decimal? LineTotal { get; set; }

        // ¡IMPORTANTE! NO incluyas la propiedad de navegación 'Order' aquí.
        // public OrderResponseDto Order { get; set; } <--- ¡ESTO CAUSARÍA EL CICLO DE NUEVO!
    }

}
