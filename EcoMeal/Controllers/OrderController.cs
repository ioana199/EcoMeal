using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcoMeal.Controllers
{
    [ApiController]
    [Route("/")]
    public class OrderController(IOrderService orderService):ControllerBase
    {
    }
}
