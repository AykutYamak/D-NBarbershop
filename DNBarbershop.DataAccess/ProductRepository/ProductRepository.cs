using DNBarbershop.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.ProductRepository
{
    public class ProductRepository<T> : IProductRepository<T> where T : class
    {

    }
}
