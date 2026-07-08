using EcoMeal.Entities;

namespace EcoMeal.Services.Interfaces
{
    public interface IBusinessService
    {
        public Task<List<Business>> GetAll();
        public Task<Business?> GetById(Guid id);
        public Task Add(Business business);
    }
}
