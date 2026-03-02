using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_Api.Data;
using RoyalVilla_Api.Models;
using RoyalVilla_Api.Models.DTO;

namespace RoyalVilla_Api.Controllers
{
    [Route("/api/villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            return Ok(await _db.Villa.ToListAsync());
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Villa>> GetVillabyId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Villa Id must be greater than zero");
                }
                var villa = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound($"Villa with {id} was not found");
                }
                return Ok(villa);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error message occured with id: {id}: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Villa>> CreateVilla(VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest("Villa data is required");
                }
                //new simplified syntax without mentioning Villa before new
                //Villa villa = new() { 
                //    Name = villaDTO.Name,
                //    Details = villaDTO.Details,
                //    ImageUrl = villaDTO.ImageUrl,
                //    Occupancy = villaDTO.Occupancy,
                //    Sqft = villaDTO.Sqft,
                //    Rate = villaDTO.Rate,
                //    CreatedDate = DateTime.Now
                //};

                Villa villa = _mapper.Map<Villa>(villaDTO);
                await _db.Villa.AddAsync(villa);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateVilla), new { id = villa.Id });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error message occured while creating a villa : {ex.Message}");
            }
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<Villa>> UpdateVilla(int id,VillaUpdateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest("Villa data is required");
                }

                if (id != villaDTO.Id)
                {
                    return BadRequest("Villa ID in url does not match with Villa ID in request body");
                }

                var existingVilla = await  _db.Villa.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVilla == null) {
                    return NotFound($"Villa with ID {id} is not found");
                }
                _mapper.Map(villaDTO, existingVilla);
                existingVilla.UpdatedDate = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(villaDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error message occured while updating a villa : {ex.Message}");
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Villa>> DeleteVilla(int id)
        {
            try
            {
                var existingVilla = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVilla == null)
                {
                    return NotFound($"Villa with ID {id} is not found");
                }

                _db.Villa.Remove(existingVilla);
                await _db.SaveChangesAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error message occured while deleting a villa : {ex.Message}");
            }
        }

    }
}
