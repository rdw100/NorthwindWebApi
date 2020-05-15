using Microsoft.EntityFrameworkCore;
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

        public CustomerRepository(NorthwindContext context)
        {
            _context = context;
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
            return await _context.Customers.Include(customer => customer.Orders).SingleOrDefaultAsync(a => a.CustomerId == id);
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
