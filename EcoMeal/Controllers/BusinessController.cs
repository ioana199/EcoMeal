using EcoMeal.Constants;
using EcoMeal.Entities;
using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMeal.Controllers
{
    [ApiController]
    [Route("/")]
    public class BusinessController(IBusinessService businessService) : ControllerBase
    {
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<List<Business>>> GetAll()
        {
            return await businessService.GetAll();
        }

        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.BusinessManager)]
        public async Task<ActionResult<Business>> GetById(Guid? id)
        {
            var business = await businessService.GetById(id);
            if (business is null)
                return NotFound();
            return business;
        }

        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult> Add(Business business)
        {
            await businessService.Add(business);
            return Created();
        }

        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.BusinessManager)]
        public async Task<ActionResult<Business>> Update(Business business, Guid id)
        {
            var updatedBusiness = await businessService.Update(business, id);
            if(updatedBusiness is null)
                return NotFound();
            return updatedBusiness;
        }

        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await businessService.Delete(id);
            return NoContent();
        }
    }
}
