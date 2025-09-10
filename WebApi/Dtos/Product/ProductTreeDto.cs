namespace BlueSelfCheckout.WebApi.Dtos.Product
{
    // ProductTreeDto.cs
    public class ProductTreeDto
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Enabled { get; set; } = "Y";
        public string? DataSource { get; set; }
        public List<ProductTreeItem1Dto>? Items1 { get; set; }
    }

    // ProductTreeItem1Dto.cs
    public class ProductTreeItem1Dto
    {
        public string ItemCode { get; set; } = string.Empty;
        public string? ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public string? IsCustomizable { get; set; } = "Y";
    }


}
