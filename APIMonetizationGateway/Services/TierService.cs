using APIMonetizationGateway.Data;
using APIMonetizationGateway.Models;

namespace APIMonetizationGateway.Services
{
    public interface ITierService
    {
        Task<List<Tier>> GetAll(string name);
        Task<Tier?> GetByIdAsync(int id);
        Task<Tier?> GetByNameAsync(string name);
        Task<Tier> Update(Tier request);
        Task<Tier?> GetByCustomerIdAsync(int customerId);
    }

    public class TierService : ITierService
    {
        /// <summary>
        /// For API Admin to Retriev all tiers
        /// </summary>
        /// <param name="name"></param>
        /// <returns>List<Tier></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<List<Tier>> GetAll(string name) => Task.FromResult(GlobalDataHelper._tiers.ToList());

        /// <summary>
        /// Get by customerId to be utilized in MW
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>Tier</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<Tier?> GetByCustomerIdAsync(int customerId)
        {
            return Task.FromResult((from c in GlobalDataHelper._customers
                                    join t in GlobalDataHelper._tiers
                                    on c.TierId equals t.Id
                                    where c.Id == customerId
                                    select t).FirstOrDefault());
        }

        /// <summary>
        /// Get By Id to be used in MW
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<Tier?> GetByIdAsync(int id) => Task.FromResult(GlobalDataHelper._tiers.FirstOrDefault(t => t.Id == id));

        /// <summary>
        /// Get by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<Tier?> GetByNameAsync(string name) => Task.FromResult(GlobalDataHelper._tiers.FirstOrDefault(t => t.Name == name));

        /// <summary>
        /// Update existing tier by API admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<Tier> Update(Tier request)
        {
            throw new NotImplementedException();
        }
    }
}
