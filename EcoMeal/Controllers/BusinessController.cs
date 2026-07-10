using EcoMeal.Entities;
using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcoMeal.Controllers
{
    [ApiController]
    [Route("/")]
    public class BusinessController(IBusinessService businessService) : ControllerBase
    {
        public async Task<ActionResult<List<Business>>> GetAll()
        {
            return await businessService.GetAll();
        }

        public async Task<ActionResult<Business>> GetById(Guid? id)
        {
            var business = await businessService.GetById(id);
            if (business is null)
                return NotFound();
            return business;
        }

        public async Task<ActionResult> Add(Business business)
        {
            await businessService.Add(business);
            return Created();
        }

        public async Task<ActionResult<Business>> Update(Business business, Guid id)
        {
            var updatedBusiness = await businessService.Update(business, id);
            if(updatedBusiness is null)
                return NotFound();
            return updatedBusiness;
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            await businessService.Delete(id);
            return NoContent();
        }
    }
}
