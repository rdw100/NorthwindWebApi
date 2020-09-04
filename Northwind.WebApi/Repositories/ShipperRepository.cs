using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Northwind.WebApi.Interfaces;
using Northwind.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.WebApi.Repositories
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly NorthwindContext _context;
        private IMemoryCache _cache;

        public ShipperRepository(NorthwindContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Shipper> Add(Shipper shipper)
        {
            await _context.Shippers.AddAsync(shipper);
            await _context.SaveChangesAsync();
            return shipper;
        }

        public async Task<bool> Exist(int id)
        {
            return await _context.Shippers.AnyAsync(c => c.ShipperId == id);
        }

        public async Task<Shipper> Find(int id)
        {
            var cachedShipper = _cache.Get<Shipper>(id);
            if (cachedShipper != null)
            {
                return cachedShipper;
            }
            else
            {
                var shipper = await _context.Shippers
                    //.Include(shipper => shipper.Orders)
                    .SingleOrDefaultAsync(a => a.ShipperId == id);
                if (shipper != null) { 
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(60));
                    _cache.Set(shipper.ShipperId, shipper, cacheEntryOptions);
                }
                return shipper;
            }
        }

        public IEnumerable<Shipper> GetAll()
        {
            return _context.Shippers;
        }

        public async Task<List<Shipper>> GetAllShippers()
        {
            IQueryable<Shipper> shippers = _context.Shippers;

            return await shippers.ToListAsync();
        }

        public async Task<Shipper> Remove(int id)
        {
            var shipper = await _context.Shippers.SingleAsync(a => a.ShipperId == id);
            _context.Shippers.Remove(shipper);
            await _context.SaveChangesAsync();
            return shipper;
        }

        public async Task<Shipper> Update(Shipper shipper)
        {
            _context.Entry(shipper).State = EntityState.Modified;
            _context.Shippers.Update(shipper);
            await _context.SaveChangesAsync();
            return shipper;
        }
    }
}
