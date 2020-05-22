using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.WebApi.Shared
{
    public class CustomerParameters : QueryStringParameters
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
    }
}
