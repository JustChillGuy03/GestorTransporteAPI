using GestorTransporteAPI.Context;
using GestorTransporteAPI.DTOs;
using GestorTransporteAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorTransporteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
        private readonly TransporteContext _context;

        public VehiculosController(TransporteContext context)
        {
            _context = context;
        }

        // GET: api/Vehiculos
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VehiculoDTO>>> GetVehiculos()
        {
            var vehiculos = await _context.Vehiculos
                .AsNoTracking()
                .OrderBy(v => v.Id)
                .Select(v => new VehiculoDTO
                {
                    Id = v.Id,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Placa = v.Placa,
                    Capacidad = v.Capacidad,
                    ChoferId = v.ChoferId,
                    NombreChofer = v.Chofer!.Nombre
                })
                .ToListAsync();

            return Ok(vehiculos);
        }

        // GET: api/Vehiculos/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehiculoDTO>> GetVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos
                .AsNoTracking()
                .Where(v => v.Id == id)
                .Select(v => new VehiculoDTO
                {
                    Id = v.Id,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Placa = v.Placa,
                    Capacidad = v.Capacidad,
                    ChoferId = v.ChoferId,
                    NombreChofer = v.Chofer!.Nombre
                })
                .FirstOrDefaultAsync();

            if (vehiculo == null)
                return NotFound(new { mensaje = $"No existe un vehículo con Id {id}." });

            return Ok(vehiculo);
        }

        // POST: api/Vehiculos
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<VehiculoDTO>> PostVehiculo(VehiculoCreateDTO vehiculoDto)
        {
            // El chofer indicado tiene que existir (evita el error 500 por llave foránea)
            var chofer = await _context.Choferes.FindAsync(vehiculoDto.ChoferId);
            if (chofer == null)
            {
                ModelState.AddModelError(nameof(vehiculoDto.ChoferId), $"No existe un chofer con Id {vehiculoDto.ChoferId}.");
                return ValidationProblem(ModelState);
            }

            var placa = vehiculoDto.Placa.Trim().ToUpper();

            if (await _context.Vehiculos.AnyAsync(v => v.Placa == placa))
                return Conflict(new { mensaje = $"Ya existe un vehículo con la placa {placa}." });

            var vehiculo = new Vehiculo
            {
                Marca = vehiculoDto.Marca.Trim(),
                Modelo = vehiculoDto.Modelo.Trim(),
                Placa = placa,
                Capacidad = vehiculoDto.Capacidad,
                ChoferId = vehiculoDto.ChoferId
            };

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            var resultado = new VehiculoDTO
            {
                Id = vehiculo.Id,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Placa = vehiculo.Placa,
                Capacidad = vehiculo.Capacidad,
                ChoferId = vehiculo.ChoferId,
                NombreChofer = chofer.Nombre
            };

            // Devuelve 201 con la URL del nuevo recurso: api/Vehiculos/{id}
            return CreatedAtAction(nameof(GetVehiculo), new { id = vehiculo.Id }, resultado);
        }

        // PUT: api/Vehiculos/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutVehiculo(int id, VehiculoUpdateDTO vehiculoDto)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
                return NotFound(new { mensaje = $"No existe un vehículo con Id {id}." });

            var choferExiste = await _context.Choferes.AnyAsync(c => c.Id == vehiculoDto.ChoferId);
            if (!choferExiste)
            {
                ModelState.AddModelError(nameof(vehiculoDto.ChoferId), $"No existe un chofer con Id {vehiculoDto.ChoferId}.");
                return ValidationProblem(ModelState);
            }

            var placa = vehiculoDto.Placa.Trim().ToUpper();

            if (await _context.Vehiculos.AnyAsync(v => v.Placa == placa && v.Id != id))
                return Conflict(new { mensaje = $"Ya existe otro vehículo con la placa {placa}." });

            vehiculo.Marca = vehiculoDto.Marca.Trim();
            vehiculo.Modelo = vehiculoDto.Modelo.Trim();
            vehiculo.Placa = placa;
            vehiculo.Capacidad = vehiculoDto.Capacidad;
            vehiculo.ChoferId = vehiculoDto.ChoferId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Vehiculos/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
                return NotFound(new { mensaje = $"No existe un vehículo con Id {id}." });

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
