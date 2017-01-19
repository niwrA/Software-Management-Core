using CommandsShared;
using System;

namespace ProductsShared
{
    public interface IProducts: ICommandProcessor
    {
        Product CreateProduct(Guid guid);
        Product GetProduct(Guid guid);
    }
}