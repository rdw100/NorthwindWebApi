using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.WebApi.Interfaces;
using Northwind.WebApi.Models;
using Northwind.WebApi.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors("LocalPolicy")]
    public class ShippersController : ControllerBase
    {
        private readonly IShipperRepository _shipperRepository;

        public ShippersController(IShipperRepository shipperRepository)
        {
            _shipperRepository = shipperRepository;
        }

        // GET: api/Shippers
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<Shipper>>> GetShippers()
        {
            //Request.HttpContext.Response.Headers.Add("X-Total-Count", _shipperRepository.GetAll().Count().ToString());

            // Longer-running operation as asynchronous
            Task<List<Shipper>> myTask = Task.Run(() => _shipperRepository.GetAll().ToList());
            List<Shipper> result = await myTask;

            return Ok(result);
        }

        // GET: api/Shippers/5
        [HttpGet("{id}")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<Shipper>> GetShipper([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shipper = await _shipperRepository.Find(id);

            if (shipper == null)
            {
                return NotFound();
            }

            return Ok(shipper);
        }

        // PUT: api/Shippers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShipper([FromRoute] int id, [FromBody] Shipper shipper)
        {
            if (id != shipper.ShipperId)
            {
                return BadRequest();
            }

            try
            {
                await _shipperRepository.Update(shipper);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Return 404 when ID does not exist.
                if (await ShipperExists(shipper.ShipperId) == false)
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

        // POST: api/Shippers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        //public async Task<IActionResult> PostShipper([FromBody] Shipper shipper)
        public async Task<ActionResult<Shipper>> PostShipper([FromBody] Shipper shipper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _shipperRepository.Add(shipper);
            }
            catch (DbUpdateException)
            { 
                if (await ShipperExists(shipper.ShipperId))
                {
                    return Conflict();
                }
                else 
                {
                    throw;
                }
            }

            return CreatedAtAction("GetShipper", new { id = shipper.ShipperId }, shipper);
        }

        // DELETE: api/Shippers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Shipper>> DeleteShipper([FromRoute] int id)
        {
            var shipper = await _shipperRepository.Find(id);
            if (shipper == null)
            {
                return NotFound();
            }

            await _shipperRepository.Remove(id);

            return Ok(shipper);
        }

        private async Task<bool> ShipperExists(int id)
        {
            return await _shipperRepository.Exist(id);
        }
    }
}
