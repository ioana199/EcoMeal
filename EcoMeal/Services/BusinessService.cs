using EcoMeal.Entities;
using EcoMeal.Repositories;
using EcoMeal.Repositories.Interfaces;
using EcoMeal.Services.Interfaces;

namespace EcoMeal.Services
{
    public class BusinessService(IBusinessRepository businessRepository) : IBusinessService
    {
        public async Task<List<Business>> GetAll()
        {
            return await businessRepository.GetAllAsync();
        }

        public async Task<Business?> GetById(Guid id)
        {
            return await businessRepository.GetByIdAsync(id);
        }

        public async Task Add(Business business)
        {
            await businessRepository.AddAsync(business);
        }

    }
}
