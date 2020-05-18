using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NorthwindWebApi.Interfaces;
using NorthwindWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindWebApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly NorthwindContext _context;
        private IMemoryCache _cache;

        public CustomerRepository(NorthwindContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Customer> Add(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> Exist(string id)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerId == id);
        }

        public async Task<Customer> Find(string id)
        {
            var cachedCustomer = _cache.Get<Customer>(id);
            if (cachedCustomer != null)
            {
                return cachedCustomer;
            }
            else
            {
                var customer = await _context.Customers
                    .Include(customer => customer.Orders).SingleOrDefaultAsync(a => a.CustomerId == id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60));
                _cache.Set(customer.CustomerId, customer, cacheEntryOptions);
                return customer;
            }
        }

        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers;
        }

        public async Task<Customer> Remove(string id)
        {
            var customer = await _context.Customers.SingleAsync(a => a.CustomerId == id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> Update(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}
