using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parakeet.DataModel;

namespace Parakeet.DomainServices
{
    public class EntityChangedEventArgs<TEntity> : EventArgs
        where TEntity : Entity
    {
        public EntityChangedEventArgs(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; private set; }
    }
}
