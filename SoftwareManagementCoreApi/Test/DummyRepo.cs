using CommandsShared;
using ProductsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareManagementCoreApi.Test
{
    public class ProductStateRepositoryFake : IProductStateRepository
    {

        public IProductState CreateProductState(Guid guid, string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteProductState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IProductState GetProductState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProductState> GetProductStates()
        {
            throw new NotImplementedException();
        }

        public void PersistChanges()
        {
            //throw new NotImplementedException();
        }

        public Task PersistChangesAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class CommandStateRepositoryFake : ICommandRepository
    {
        public void AddCommandState(ICommandState state)
        {
            throw new NotImplementedException();
        }

        public ICommandState CreateCommandState()
        {
            throw new NotImplementedException();
        }

        public bool Exists(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetAllNew()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetAllProcessed()
        {
            throw new NotImplementedException();
        }

        public IList<ICommandState> GetUpdatesSinceLast(long lastReceivedStamp)
        {
            throw new NotImplementedException();
        }

        public void PersistChanges()
        {
            throw new NotImplementedException();
        }

        public void SetProcessed(ICommandState state)
        {
            throw new NotImplementedException();
        }
    }
}
