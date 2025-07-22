using AutoMapper;
using BlueSelfCheckout.WebApi.Dtos;
using BlueSelfCheckout.WebApi.Dtos.Order;
using BlueSelfCheckout.WebApi.Dtos.Product;
using BlueSelfCheckout.WebApi.Models.Orders;
using BlueSelfCheckout.WebApi.Models.Products;

namespace BlueSelfCheckout.WebApi.Mapper
{
    public class Mapper: Profile
    {
        public Mapper()
        {

            // Mapeo de Product
            CreateMap<Product, ProductDto>().ReverseMap();

            // Mapeo de ProductMaterial
            CreateMap<ProductMaterial, ProductMaterialCreateDto>()
                .ReverseMap()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.ProductItemCode, opt => opt.Ignore()); // Se establece manualmente

            // Mapeo de ProductAccompaniment
            CreateMap<ProductAccompaniment, ProductAccompanimentCreateDto>()
                .ReverseMap()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.ProductItemCode, opt => opt.Ignore()); 

                // Mapeo de DTO de creación de Orden a la entidad Order
                CreateMap<OrderCreateDto, Order>()
                // Si DocEntry se genera en la base de datos, no necesitas mapearlo desde el DTO
                .ForMember(dest => dest.DocEntry, opt => opt.Ignore())
                // Para la colección de líneas, AutoMapper las mapeará automáticamente
                .ForMember(dest => dest.OrderLines, opt => opt.MapFrom(src => src.OrderLines)); // Renombrado en DTO a OrderLines

            // Mapeo de DTO de creación de Línea de Documento a la entidad DocumentLine
            CreateMap<DocumentLineCreateDto, OrderLine>()
                // DocEntry se establecerá a través de la relación de EF, no directamente desde el DTO
                .ForMember(dest => dest.DocEntry, opt => opt.Ignore())
                // LineId se puede generar automáticamente en el servicio/repositorio o ignorar aquí si lo generas en BD
                .ForMember(dest => dest.LineId, opt => opt.Ignore())
                // La propiedad de navegación 'Order' no debe ser mapeada desde el JSON de entrada
                .ForMember(dest => dest.Order, opt => opt.Ignore());

            CreateMap<OrderCreateDto, Order>()
            .ForMember(dest => dest.DocEntry, opt => opt.Ignore());


            // Nuevo mapeo de Entidad a DTO de Respuesta para Order
            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderLines, opt => opt.MapFrom(src => src.OrderLines)); // Mapear a OrderLines para consistencia

            // Nuevo mapeo de Entidad a DTO de Respuesta para DocumentLine
            CreateMap<OrderLine, DocumentLineResponseDto>()
                .ForMember(dest => dest.DocEntry, opt => opt.MapFrom(src => src.DocEntry)) // Mapea la clave foránea explícitamente si quieres que aparezca
                .ForMember(dest => dest.LineId, opt => opt.MapFrom(src => src.LineId))
                // ... mapea las otras propiedades ...
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCode))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.ItemName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.LineStatus, opt => opt.MapFrom(src => src.LineStatus))
                .ForMember(dest => dest.TaxCode, opt => opt.MapFrom(src => src.TaxCode))
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.LineTotal));
            // No mapees la propiedad de navegación Order aquí

            // Mapeo para la actualización de la Orden:
            // Ignora DocEntry ya que lo manejamos desde la URL
            CreateMap<OrderUpdateDto, Order>()
                .ForMember(dest => dest.DocEntry, opt => opt.Ignore())
                .ForMember(dest => dest.OrderLines, opt => opt.Ignore()); // Las líneas se manejarán manualmente

            // Mapeo para la actualización de DocumentLine:
            CreateMap<DocumentLineUpdateDto, OrderLine>();
        }
    }
}
