using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Northwind.WebApi.Interfaces;
using Northwind.WebApi.Models;
using Northwind.WebApi.Models.Extensions;
using Northwind.WebApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Northwind.WebApi.Repositories
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
                    .Include(customer => customer.Orders)
                    .SingleOrDefaultAsync(a => a.CustomerId == id);
                if (customer != null) { 
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(60));
                    _cache.Set(customer.CustomerId, customer, cacheEntryOptions);
                }
                return customer;
            }
        }

        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers;
        }

        public async Task<List<Customer>> GetAllCustomers([FromQuery] CustomerParameters parameters)
        {
            IQueryable<Customer> customers = _context.Customers;

            if (!string.IsNullOrEmpty(parameters.Country))
            {
                customers = customers.Where(
                    c => c.Country == parameters.Country);
            }

            if (!string.IsNullOrEmpty(parameters.CompanyName))
            {
                customers = customers.Where(
                    c => c.CompanyName.ToLower().Contains(parameters.CompanyName.ToLower()));
            }

            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                if (typeof(Customer).GetProperty(parameters.SortBy) != null)
                {
                    customers = customers.OrderByCustom(parameters.SortBy, parameters.SortOrder);
                }
            }

            // Page should not exceed count divided by page;
            // otherwise, set page to max possible pages
            if ((parameters.Page > 0) && (parameters.Size > 0))
            {
                if (customers.Count() % parameters.Size < parameters.Page)
                {
                    parameters.Page = (customers.Count() % parameters.Size) + 1;
                }

                customers = customers
                .Skip(parameters.Size * (parameters.Page - 1))
                .Take(parameters.Size);
            }

            return await customers.ToListAsync();
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
            _context.Entry(customer).State = EntityState.Modified;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}
