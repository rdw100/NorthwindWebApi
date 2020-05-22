﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.WebApi.Interfaces;
using Northwind.WebApi.Models;
using Northwind.WebApi.Shared;

namespace Northwind.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [EnableCors("LocalPolicy")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // GET: api/Customers
        [HttpGet]        
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            Request.HttpContext.Response.Headers.Add("X-Total-Count", _customerRepository.GetAll().Count().ToString());
            // Longer-running operation as asynchronous
            Task<List<Customer>> myTask = Task.Run(() => _customerRepository.GetAll().ToList());
            List<Customer> result = await myTask;

            return Ok(result);
        }

        // GET: api/Customers/seek?country=UK&page=1&size=7&sortBy=CustomerId&sortOrder=desc
        [HttpGet("seek")]
        public async Task<IActionResult> GetAllCustomers([FromQuery] CustomerParameters parameters)
        {
            var customers = await _customerRepository.GetAllCustomers(parameters);
            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetCustomer([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerRepository.Find(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer([FromRoute]string id, [FromBody]Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            try
            {
                await _customerRepository.Update(customer);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Return 404 when ID does not exist.
                if (await CustomersExists(customer.CustomerId) == false)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                await _customerRepository.Add(customer);
            }
            catch (DbUpdateException)
            {
                if (await CustomersExists(customer.CustomerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomers", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute]string id)
        {
            var customer = await _customerRepository.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            await _customerRepository.Remove(id);

            return Ok(customer);
        }

        private async Task<bool> CustomersExists(string id)
        {
            return await _customerRepository.Exist(id);
        }
    }
}
