using System;

namespace ProductsShared
{
    public interface IProducts
    {
        Product CreateProduct(Guid guid);
        Product GetProduct(Guid guid);
    }
}