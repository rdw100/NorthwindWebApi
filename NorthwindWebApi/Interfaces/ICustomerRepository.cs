using NorthwindWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindWebApi.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> Add(Customer customer);

        IEnumerable<Customer> GetAll();

        Task<Customer> Find(string id);

        Task<Customer> Update(Customer customer);

        Task<Customer> Remove(string id);

        Task<bool> Exist(string id);
    }
}
