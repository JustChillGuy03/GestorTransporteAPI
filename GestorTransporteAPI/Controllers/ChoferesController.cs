using GestorTransporteAPI.Context;
using GestorTransporteAPI.DTOs;
using GestorTransporteAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorTransporteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoferesController : ControllerBase
    {
        private readonly TransporteContext _context;

        public ChoferesController(TransporteContext context)
        {
            _context = context;
        }

        // GET: api/Choferes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChoferDTO>>> GetChoferes()
        {
            var choferes = await _context.Choferes
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => new ChoferDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Licencia = c.Licencia,
                    Telefono = c.Telefono,
                    CantidadVehiculos = c.Vehiculos.Count
                })
                .ToListAsync();

            return Ok(choferes);
        }

        // GET: api/Choferes/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChoferDTO>> GetChofer(int id)
        {
            var chofer = await _context.Choferes
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ChoferDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Licencia = c.Licencia,
                    Telefono = c.Telefono,
                    CantidadVehiculos = c.Vehiculos.Count
                })
                .FirstOrDefaultAsync();

            if (chofer == null)
                return NotFound(new { mensaje = $"No existe un chofer con Id {id}." });

            return Ok(chofer);
        }

        // GET: api/Choferes/5/vehiculos (vehículos asignados a un chofer)
        [HttpGet("{id:int}/vehiculos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VehiculoDTO>>> GetVehiculosDelChofer(int id)
        {
            var existe = await _context.Choferes.AnyAsync(c => c.Id == id);
            if (!existe)
                return NotFound(new { mensaje = $"No existe un chofer con Id {id}." });

            var vehiculos = await _context.Vehiculos
                .AsNoTracking()
                .Where(v => v.ChoferId == id)
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

        // POST: api/Choferes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ChoferDTO>> PostChofer(ChoferCreateDTO choferDto)
        {
            var licencia = choferDto.Licencia.Trim().ToUpper();

            if (await _context.Choferes.AnyAsync(c => c.Licencia == licencia))
                return Conflict(new { mensaje = $"Ya existe un chofer con la licencia {licencia}." });

            var chofer = new Chofer
            {
                Nombre = choferDto.Nombre.Trim(),
                Licencia = licencia,
                Telefono = string.IsNullOrWhiteSpace(choferDto.Telefono) ? null : choferDto.Telefono.Trim()
            };

            _context.Choferes.Add(chofer);
            await _context.SaveChangesAsync();

            var resultado = new ChoferDTO
            {
                Id = chofer.Id,
                Nombre = chofer.Nombre,
                Licencia = chofer.Licencia,
                Telefono = chofer.Telefono,
                CantidadVehiculos = 0
            };

            // Devuelve 201 con la URL del nuevo recurso: api/Choferes/{id}
            return CreatedAtAction(nameof(GetChofer), new { id = chofer.Id }, resultado);
        }

        // PUT: api/Choferes/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutChofer(int id, ChoferUpdateDTO choferDto)
        {
            var chofer = await _context.Choferes.FindAsync(id);
            if (chofer == null)
                return NotFound(new { mensaje = $"No existe un chofer con Id {id}." });

            var licencia = choferDto.Licencia.Trim().ToUpper();

            if (await _context.Choferes.AnyAsync(c => c.Licencia == licencia && c.Id != id))
                return Conflict(new { mensaje = $"Ya existe otro chofer con la licencia {licencia}." });

            chofer.Nombre = choferDto.Nombre.Trim();
            chofer.Licencia = licencia;
            chofer.Telefono = string.IsNullOrWhiteSpace(choferDto.Telefono) ? null : choferDto.Telefono.Trim();

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Choferes/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteChofer(int id)
        {
            var chofer = await _context.Choferes.FindAsync(id);
            if (chofer == null)
                return NotFound(new { mensaje = $"No existe un chofer con Id {id}." });

            // No se borra un chofer que todavía tiene vehículos asignados
            var tieneVehiculos = await _context.Vehiculos.AnyAsync(v => v.ChoferId == id);
            if (tieneVehiculos)
                return Conflict(new { mensaje = "No se puede eliminar el chofer porque tiene vehículos asignados. Reasigne o elimine esos vehículos primero." });

            _context.Choferes.Remove(chofer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
