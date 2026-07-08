using EcoMeal.Entities;
using EcoMeal.Repositories.Interfaces;
using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoMeal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController(IPackageService packageService) : ControllerBase
    {
        public async Task<ActionResult<List<Package>>> GetAll()
        {
            return await packageService.GetAll();
        }
        public async Task<ActionResult<Package>> GetById(Guid id)
        {
            var package = await packageService.GetById(id);
            if (package is null)
                return NotFound();
            return package;
        }
        public async Task<ActionResult> Add(Package package)
        {
            await packageService.Add(package);
            return Created();
        }
    }
}
