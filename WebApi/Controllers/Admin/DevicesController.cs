using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueSelfCheckout.WebApi.Models.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlueSelfCheckout.Data;
using BlueSelfCheckout.WebApi.Models.Customers;
using BlueSelfCheckout.WebApi.Models;

namespace BlueSelfCheckout.WebApi.Controllers.Admin
{
    /// <summary>
    /// Este controlador gestiona los dispositivos de autoservicio.
    /// Proporciona métodos para obtener, crear, actualizar y eliminar dispositivos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del controlador <see cref="DevicesController"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        public DevicesController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene un listado con todos los dispositivos de autoservicio.
        /// </summary>
        /// <returns>Una lista de dispositivos.</returns>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Device>>> GetAllDevice()
        {
            return await _context.Device.ToListAsync();
        }



        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<PagedResponse<Device>>> GetDevice(
            int pageNumber = 1,
            int pageSize = 10,
            string search = null) // Parámetro de búsqueda (search)
        {
            try
            {
                // Comenzamos la consulta con todos los dispositivos
                IQueryable<Device> query = _context.Device;

                // Si se proporciona un término de búsqueda, filtramos los resultados
                if (!string.IsNullOrEmpty(search))
                {
                    // Realizamos una búsqueda que sea insensible a mayúsculas y minúsculas en el nombre del grupo de cliente
                    query = query.Where(d => d.DeviceCode.Contains(search) || d.DeviceName.Contains(search));
                }

                // Calcular el total de dispositivos después de aplicar el filtro
                var totalCount = await query.CountAsync();

                // Obtener la página de datos solicitada
                var devices = await query
                    .Skip((pageNumber - 1) * pageSize)  // Saltar los primeros (pageNumber - 1) * pageSize registros
                    .Take(pageSize)                     // Tomar solo 'pageSize' registros
                    .ToListAsync();

                // Crear la respuesta paginada
                var response = new PagedResponse<Device>(totalCount, pageNumber, pageSize, devices);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Loguear la excepción (puedes usar tu propia forma de registrar los errores)
                Console.Error.WriteLine($"Error al obtener los dispositivos: {ex.Message}");

                // Devolver un mensaje de error más detallado
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un dispositivo específico utilizando el código del dispositivo como parámetro de búsqueda.
        /// </summary>
        /// <param name="deviceCode">El código único del dispositivo.</param>
        /// <returns>El dispositivo solicitado o un error si no se encuentra.</returns>
        [HttpGet("{deviceCode}")]
        public async Task<ActionResult<Device>> GetDevice(string deviceCode)
        {
            var device = await _context.Device.FindAsync(deviceCode);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        /// <summary>
        /// Actualiza un dispositivo existente.
        /// </summary>
        /// <param name="deviceCode">El código del dispositivo a actualizar.</param>
        /// <param name="device">Los nuevos datos del dispositivo.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpPut("{deviceCode}")]
        public async Task<IActionResult> PutDevice(string deviceCode, Device device)
        {
            if (deviceCode != device.DeviceCode)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(deviceCode))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Crea un nuevo dispositivo.
        /// </summary>
        /// <param name="device">Los datos del dispositivo a crear.</param>
        /// <returns>El dispositivo creado y la ubicación del recurso.</returns>
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(Device device)
        {
            _context.Device.Add(device);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeviceExists(device.DeviceCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction(nameof(GetDevice), new { deviceCode = device.DeviceCode }, device);
        }

        /// <summary>
        /// Elimina un dispositivo existente.
        /// </summary>
        /// <param name="deviceCode">El código del dispositivo a eliminar.</param>
        /// <returns>Un resultado indicando el éxito o el error de la operación.</returns>
        [HttpDelete("{deviceCode}")]
        public async Task<IActionResult> DeleteDevice(string deviceCode)
        {
            var device = await _context.Device.FindAsync(deviceCode);
            if (device == null)
            {
                return NotFound();
            }

            _context.Device.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica si existe un dispositivo con el código especificado.
        /// </summary>
        /// <param name="deviceCode">El código del dispositivo a verificar.</param>
        /// <returns>Verdadero si el dispositivo existe; de lo contrario, falso.</returns>
        private bool DeviceExists(string deviceCode)
        {
            return _context.Device.Any(e => e.DeviceCode == deviceCode);
        }


    }// fin de la clase

}// fin del namespace
