using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoulderDashSnilku.Entities
{
    public class EntityManager
    {
        private readonly List<Entity> entities = new List<Entity>();

        public void Add(Entity entity)
        {
            entities.Add(entity);
        }

        public void Remove(Entity entity)
        {
            if (entities.Contains(entity))
            {
                entities.Remove(entity);
            }
            else
            {
                throw new ArgumentException("Entity not found in the manager.");
            }
        }

        public Entity GetEntityAt(int x, int y)
        {
            foreach (Entity entity in entities)
            {
                if (entity.x == x && entity.y == y)
                {
                    return entity;
                }
            }
            return null;
        }

        public bool HasEntityAt(int x, int y)
        {
            return GetEntityAt(x, y) != null;
        }
    }
}
