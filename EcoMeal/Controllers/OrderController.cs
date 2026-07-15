using EcoMeal.Entities;
using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcoMeal.Controllers
{
    [ApiController]
    [Route("/")]
    public class OrderController(IOrderService orderService):ControllerBase
    {
        public async Task<ActionResult<List<Order>>> GetAll()
        {
            return await orderService.GetAll();
        }

        public async Task<ActionResult<Order?>> GetById(Guid id)
        {
            return await orderService.GetById(id);
        }
    }
}
