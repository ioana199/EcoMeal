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
        public async Task<ActionResult<Package>> GetById(Guid? id)
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

        public async Task<ActionResult<Package>> Update(Package package, Guid id)
        {
            var updatedPackage = await packageService.Update(package, id);
            if (updatedPackage is null)
                return NotFound();
            return updatedPackage;
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            await packageService.Delete(id);
            return NoContent();
        }
    }
}
