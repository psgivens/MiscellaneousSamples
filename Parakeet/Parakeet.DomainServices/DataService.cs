using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parakeet.DataModel;
using System.Threading.Tasks;

namespace Parakeet.DomainServices
{
    public class DataService<TEntity>
        where TEntity : Entity
    {
        public void Save<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            // 1) update cache so that we can access it quickly.

            // 2) write to data file so that it will be here when 
            //    we resume.

            // 3) queue to publish to the web so that we may have it
            //    whereever we go. 

            throw new NotImplementedException("Save");
        }

        public TEntity Read(int id)            
        {
            throw new NotImplementedException("Read");
        }

        public IEnumerable<TEntity> ReadAll()
        {
            throw new NotImplementedException("Read");
        }

        public Task ExecuteQuery(string query)
        {
            // 1) run query against the data base.

            // 2) update the cache and notify the application

            // 3) write to the data file so that it will be here when 
            //    we resume.            
           
            throw new NotImplementedException("ExecuteQuery");
        }
    }
}
