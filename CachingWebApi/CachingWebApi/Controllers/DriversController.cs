using CachingWebApi.Data;
using CachingWebApi.Models;
using CachingWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CachingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;

        public DriversController(ICacheService cacheService,AppDbContext context)
        {
            _cacheService = cacheService;
            _context = context;
        }

        [HttpGet("drivers")]
        public async Task<IActionResult> Get()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = await _context.Drivers.ToListAsync();

            var expirationTime = DateTime.Now.AddSeconds(30);
            _cacheService.SetData("drivers",cacheData,expirationTime);
            return Ok(cacheData);
        }

        [HttpPost("AddDrivers")]
        public async Task<IActionResult> Post(Driver driver)
        {
            var addObj = await  _context.Drivers.AddAsync(driver);
            var expirationTime = DateTime.Now.AddSeconds(30);
            _cacheService.SetData($"driver{driver.Id}",addObj.Entity,expirationTime);

            await _context.SaveChangesAsync();
            return Ok(addObj.Entity);
        }

        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> Delete(int id)
        {
            var exist =await  _context.Drivers.FirstOrDefaultAsync(x => x.Id == id);
            if (exist != null)
            {
                _context.Drivers.Remove(exist);
                _cacheService.RemoveData($"driver{id}");
                await _context.SaveChangesAsync();
                return NoContent();
            }

            return NotFound();
        }
    }
}
