using CommandsShared;
using System;

namespace ProductsShared
{
    public interface IProductService: ICommandProcessor
    {
        Product CreateProduct(Guid guid);
        Product GetProduct(Guid guid);
    }
}