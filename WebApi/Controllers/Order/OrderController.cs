using Microsoft.AspNetCore.Mvc;
using BlueSelfCheckout.Data;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.WebApi.Dtos;
using AutoMapper;
using BlueSelfCheckout.WebApi.Dtos.Order;
using BlueSelfCheckout.WebApi.Models.Orders;

namespace BlueSelfCheckout.WebApi.Controllers.Order
{

    /// <summary>
    /// Controlador para la gestión de órdenes del sistema BlueSelfCheckout.
    /// Proporciona endpoints para operaciones CRUD sobre la entidad Order.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// Contexto de base de datos para acceder a las órdenes.
        /// </summary>
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador OrderController.
        /// </summary>
        /// <param name="context">El contexto de base de datos a utilizar.</param>
        /// <param name="mapper"> El mapeador para convertir entre entidades y DTOs.</param>
        /// <exception cref="ArgumentNullException">Se lanza cuando el contexto es null.</exception>
        public OrderController(ApplicationDBContext context,IMapper mapper)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Obtiene todas las órdenes del sistema con paginación y filtro dinámico.
        /// </summary>
        /// <param name="page">Número de página (por defecto: 1).</param>
        /// <param name="pageSize">Tamaño de página (por defecto: 10, máximo: 100).</param>
        /// <param name="filter">Filtro dinámico que busca en múltiples campos de la orden.</param>
        /// <returns>Una lista paginada de órdenes con metadatos de paginación.</returns>
        /// <response code="200">Retorna la lista paginada de órdenes exitosamente.</response>
        /// <response code="400">Parámetros de paginación inválidos.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        // !!! CAMBIO AQUÍ: Ahora devuelve PaginatedResult<OrderResponseDto>
        [ProducesResponseType(typeof(PaginatedResult<OrderResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedResult<OrderResponseDto>>> GetOrders( // !!! CAMBIO AQUÍ: Tipo de retorno del método
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string filter = null)
        {
            try
            {
                // Validar parámetros de paginación
                if (page < 1)
                {
                    return BadRequest("El número de página debe ser mayor que 0.");
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    return BadRequest("El tamaño de página debe estar entre 1 y 100.");
                }

                // Construir query base
                var query = _context.Orders
                    .Include(x => x.OrderLines) // Mantener el Include para que las líneas estén disponibles para el mapeo
                    .AsQueryable();

                // Aplicar filtro dinámico si se proporciona
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    var filterLower = filter.ToLower().Trim();

                    query = query.Where(o =>
                        (o.FolioPref != null && o.FolioPref.ToLower().Contains(filterLower)) ||
                        (o.FolioNum != null && o.FolioNum.ToLower().Contains(filterLower)) ||
                        (o.CustomerCode != null && o.CustomerCode.ToLower().Contains(filterLower)) ||
                        (o.CustomerName != null && o.CustomerName.ToLower().Contains(filterLower)) ||
                        (o.NickName != null && o.NickName.ToLower().Contains(filterLower)) ||
                        (o.DeviceCode != null && o.DeviceCode.ToLower().Contains(filterLower)) ||
                        (o.DocStatus != null && o.DocStatus.ToLower().Contains(filterLower)) ||
                        (o.DocType != null && o.DocType.ToLower().Contains(filterLower)) ||
                        (o.PaidType != null && o.PaidType.ToLower().Contains(filterLower)) ||
                        (o.Comments != null && o.Comments.ToLower().Contains(filterLower)) ||
                        o.DocEntry.ToString().Contains(filterLower)
                    );
                }

                // Obtener el total de registros (después del filtro)
                var totalRecords = await query.CountAsync();

                // Calcular metadatos de paginación
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                var hasNextPage = page < totalPages;
                var hasPreviousPage = page > 1;

                // Aplicar paginación
                var orders = await query
                    .OrderBy(o => o.DocEntry)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // !!! CAMBIO CLAVE AQUÍ: Mapear la lista de entidades a una lista de DTOs de respuesta
                var orderResponseDtos = _mapper.Map<List<OrderResponseDto>>(orders);

                // Crear resultado paginado con DTOs
                var result = new PaginatedResult<OrderResponseDto> // !!! CAMBIO AQUÍ: Usar OrderResponseDto
                {
                    Data = orderResponseDtos, // Asignar la lista de DTOs
                    Page = page,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    HasNextPage = hasNextPage,
                    HasPreviousPage = hasPreviousPage,
                    Filter = filter
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
        /// <summary>
        /// Obtiene una orden específica por su identificador.
        /// </summary>
        /// <param name="id">El identificador único de la orden (DocEntry).</param>
        /// <returns>La orden solicitada si existe.</returns>
        /// <response code="200">Retorna la orden encontrada.</response>
        /// <response code="404">La orden no fue encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{id}",Name = "GetOrder")]
        [ProducesResponseType(typeof(OrderResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o=>o.OrderLines)
                    .FirstOrDefaultAsync(o => o.DocEntry == id);

                if (order == null)
                {
                    return NotFound($"No se encontró una orden con el ID: {id}");
                }

                var orderResponseDto = _mapper.Map<OrderResponseDto>(order);
                return Ok(orderResponseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene órdenes filtradas por código de cliente.
        /// </summary>
        /// <param name="customerCode">El código del cliente para filtrar las órdenes.</param>
        /// <returns>Lista de órdenes del cliente especificado.</returns>
        /// <response code="200">Retorna las órdenes del cliente.</response>
        /// <response code="400">El código de cliente es inválido.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("customer/{customerCode}")]
        [ProducesResponseType(typeof(IEnumerable<Models.Orders.Order>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Models.Orders.Order>>> GetOrdersByCustomer(string customerCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerCode))
                {
                    return BadRequest("El código de cliente no puede estar vacío.");
                }

                var orders = await _context.Set<Models.Orders.Order>()
                    .Where(o => o.CustomerCode == customerCode)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene órdenes filtradas por estado del documento.
        /// </summary>
        /// <param name="status">El estado del documento para filtrar (un carácter).</param>
        /// <returns>Lista de órdenes con el estado especificado.</returns>
        /// <response code="200">Retorna las órdenes con el estado solicitado.</response>
        /// <response code="400">El estado proporcionado es inválido.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<Models.Orders.Order>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Models.Orders.Order>>> GetOrdersByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status) || status.Length != 1)
                {
                    return BadRequest("El estado debe ser un carácter válido.");
                }

                var orders = await _context.Set<Models.Orders.Order>()
                    .Where(o => o.DocStatus == status)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene órdenes dentro de un rango de fechas.
        /// </summary>
        /// <param name="startDate">Fecha de inicio del rango (formato: yyyy-MM-dd).</param>
        /// <param name="endDate">Fecha de fin del rango (formato: yyyy-MM-dd).</param>
        /// <returns>Lista de órdenes dentro del rango de fechas especificado.</returns>
        /// <response code="200">Retorna las órdenes en el rango de fechas.</response>
        /// <response code="400">Las fechas proporcionadas son inválidas.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("date-range")]
        [ProducesResponseType(typeof(IEnumerable<Models.Orders.Order>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Models.Orders.Order>>> GetOrdersByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin.");
                }

                var orders = await _context.Set<Models.Orders.Order>()
                    .Where(o => o.DocDate >= startDate && o.DocDate <= endDate)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea una nueva orden en el sistema.
        /// </summary>
        /// <param name="order">La orden a crear.</param>
        /// <returns>La orden creada con su identificador asignado.</returns>
        /// <response code="201">La orden fue creada exitosamente.</response>
        /// <response code="400">Los datos de la orden son inválidos.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Models.Orders.Order), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Models.Orders.Order>> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var order= _mapper.Map<Models.Orders.Order>(orderDto);

                int lineCounter = 1;
                foreach (var line in order.OrderLines)
                {
                    line.LineId = lineCounter++; // Asigna un LineId secuencial
                }


                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                var orderResponseDto = _mapper.Map<OrderResponseDto>(order);

                return CreatedAtAction(nameof(GetOrder), new { id = order.DocEntry }, orderResponseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza una orden existente.
        /// </summary>
        /// <param name="id">El identificador de la orden a actualizar.</param>
        /// <param name="orderDto">Los nuevos datos de la orden.</param>
        /// <returns>La orden actualizada.</returns>
        /// <response code="200">La orden fue actualizada exitosamente.</response>
        /// <response code="400">Los datos proporcionados son inválidos.</response>
        /// <response code="404">La orden no fue encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OrderResponseDto), 200)] // Devuelve el DTO de respuesta
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDto orderDto) // <--- Cambio a OrderUpdateDto
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 1. Cargar la orden existente y sus líneas asociadas
                var existingOrder = await _context.Orders
                    .Include(o => o.OrderLines) // Incluir las líneas para poder gestionarlas
                    .FirstOrDefaultAsync(o => o.DocEntry == id);

                if (existingOrder == null)
                {
                    return NotFound($"No se encontró una orden con el ID: {id}");
                }

                // 2. Mapear las propiedades escalares (no las colecciones) desde el DTO a la entidad existente
                _mapper.Map(orderDto, existingOrder);

                // 3. Manejar la actualización de las líneas de la orden
                if (orderDto.OrderLines != null)
                {
                    // Un HashSet para rastrear las líneas que deben permanecer/actualizarse
                    var updatedLineIds = new HashSet<int>();

                    foreach (var lineDto in orderDto.OrderLines)
                    {
                        if (lineDto.LineId == 0) // Nueva línea (o un valor específico que indique "nuevo")
                        {
                            var newLine = _mapper.Map<OrderLine>(lineDto);
                            newLine.DocEntry = existingOrder.DocEntry; // Asignar el DocEntry de la orden padre
                            newLine.LineId = existingOrder.OrderLines.Any() ? existingOrder.OrderLines.Max(l => l.LineId) + 1 : 1; // Generar nuevo LineId
                            existingOrder.OrderLines.Add(newLine);
                        }
                        else // Línea existente (actualizar)
                        {
                            var existingLine = existingOrder.OrderLines.FirstOrDefault(l => l.LineId == lineDto.LineId);
                            if (existingLine != null)
                            {
                                _mapper.Map(lineDto, existingLine); // Mapea las propiedades de la línea
                                updatedLineIds.Add(lineDto.LineId); // Marca esta línea como "actualizada/conservada"
                            }
                            // Si existingLine es null, significa que el cliente envió un LineId que no existe en esta orden,
                            // puedes decidir si ignorarlo, devolver un 400, o manejarlo como error.
                            // Por ahora, simplemente lo ignoraremos.
                        }
                    }

                    // 4. Eliminar líneas que ya no están presentes en el DTO (si la colección no está vacía)
                    // Esto maneja el escenario donde el cliente quita líneas de la orden
                    var linesToRemove = existingOrder.OrderLines
                        .Where(l => !updatedLineIds.Contains(l.LineId))
                        .ToList();

                    foreach (var line in linesToRemove)
                    {
                        _context.Entry(line).State = EntityState.Deleted; // Marca para eliminación
                    }
                }
                // Si orderDto.OrderLines es null, puedes decidir si borras todas las líneas o no haces nada.
                // Por ahora, si es null, no se hace nada con las líneas existentes.

                await _context.SaveChangesAsync();

                // 5. Devolver la orden actualizada mapeada a un DTO de respuesta
                // Recargar la orden si necesitas asegurar que todas las propiedades y relaciones estén actualizadas
                // (Especialmente si se crearon o eliminaron líneas, ya que el objeto 'existingOrder' podría no reflejar esto inmediatamente)
                var updatedOrder = await _context.Orders
                    .Include(o => o.OrderLines)
                    .FirstOrDefaultAsync(o => o.DocEntry == id);

                var orderResponseDto = _mapper.Map<OrderResponseDto>(updatedOrder);
                return Ok(orderResponseDto); // Devolver la orden actualizada
            }
            catch (Exception ex)
            {
                // Logear la excepción para depuración
                // _logger.LogError(ex, "Error al actualizar la orden con ID {Id}", id);
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina una orden del sistema.
        /// </summary>
        /// <param name="id">El identificador de la orden a eliminar.</param>
        /// <returns>Confirmación de la eliminación.</returns>
        /// <response code="200">La orden fue eliminada exitosamente.</response>
        /// <response code="404">La orden no fue encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Set<Models.Orders.Order>()
                    .FirstOrDefaultAsync(o => o.DocEntry == id);

                if (order == null)
                {
                    return NotFound($"No se encontró una orden con el ID: {id}");
                }

                _context.Set<Models.Orders.Order>().Remove(order);
                await _context.SaveChangesAsync();

                return Ok("Orden eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }

}