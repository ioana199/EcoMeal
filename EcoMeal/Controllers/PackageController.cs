using EcoMeal.Entities;
using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcoMeal.Controllers
{
    [ApiController]
    [Route("/")]
    public class PackageController(IPackageService packageService) : ControllerBase
    {
        public async Task<ActionResult<List<Package>>> GetAll()
        {
            return await packageService.GetAll();
        }

        public async Task<ActionResult<List<Package>>> GetForManagement(string callerUserId, bool isAdmin)
        {
            return await packageService.GetForManagement(callerUserId, isAdmin);
        }

        public async Task<ActionResult<Package>> GetById(Guid? id)
        {
            var package = await packageService.GetById(id);
            if (package is null)
                return NotFound();
            return package;
        }

        public async Task<ActionResult> Add(Package package, string callerUserId, bool isAdmin)
        {
            await packageService.Add(package, callerUserId, isAdmin);
            return Created();
        }

        public async Task<ActionResult<Package>> Update(Package package, Guid id, string callerUserId, bool isAdmin)
        {
            var updatedPackage = await packageService.Update(package, id, callerUserId, isAdmin);
            if (updatedPackage is null)
                return NotFound();
            return updatedPackage;
        }

        public async Task<ActionResult> Delete(Guid id, string callerUserId, bool isAdmin)
        {
            await packageService.Delete(id, callerUserId, isAdmin);
            return NoContent();
        }
    }
}