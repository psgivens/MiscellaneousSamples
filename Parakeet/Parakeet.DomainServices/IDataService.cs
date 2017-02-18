using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parakeet.DataModel;
using System.Threading.Tasks;

namespace Parakeet.DomainServices
{
    public interface IDataService<TEntity>
        where TEntity : Entity
    {
        void Save(TEntity entity);

        TEntity Read(int id);

        IEnumerable<TEntity> ReadAll();

        Task ExecuteQuery(string query);
    }
}
