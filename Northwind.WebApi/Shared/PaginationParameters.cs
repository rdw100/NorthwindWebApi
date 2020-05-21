using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.WebApi.Shared
{
    public class PaginationParameters
    {
        const int _maxSize = 100;
        private int _size = 50;

        public int Page { get; set; }
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = Math.Min(_maxSize, value);
            }
        }
    }
}
