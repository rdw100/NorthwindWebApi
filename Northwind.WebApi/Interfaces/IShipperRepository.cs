using Northwind.WebApi.Models;
using Northwind.WebApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.WebApi.Interfaces
{
    public interface IShipperRepository
    {
        Task<Shipper> Add(Shipper shipper);

        IEnumerable<Shipper> GetAll();

        Task<List<Shipper>> GetAllShippers();

        Task<Shipper> Find(int id);

        Task<Shipper> Update(Shipper shipper);

        Task<Shipper> Remove(int id);

        Task<bool> Exist(int id);
    }
}
