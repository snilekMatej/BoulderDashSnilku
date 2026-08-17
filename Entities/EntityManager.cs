using System;
using System.Collections.Generic;
using System.Linq;

namespace BoulderDashSnilku.Entities {
    /// <summary>
    /// Storage and management for all entities pressent in a level.
    /// Provides methods for adding, removing, locating and filtering entities.
    /// </summary>
    public class EntityManager {
        private readonly List<Entity> entities = new List<Entity>();
        
        public void Add(Entity entity) {
            entities.Add(entity);
        }
        
        public void Remove(Entity entity) {
            if (entities.Contains(entity)) entities.Remove(entity);
            else throw new ArgumentException("Entity not found in the manager.");
        }

        /// <summary>
        /// Searches for an entity at specified world position
        /// </summary>
        /// <param name="x">Horisontal tile coordinate.</param>
        /// <param name="y">Vertical tile coordinate.</param>
        /// <returns>The entity at that position | if (position is empty) -> null</returns>
        public Entity GetEntityAt(int x, int y) {
            Entity foundEntity = null;
            foreach (Entity entity in entities) {
                if (entity.x == x && entity.y == y) foundEntity = entity;
            }
            return foundEntity;
        }

        public bool HasEntityAt(int x, int y) {
            return GetEntityAt(x, y) != null;
        }
        
        /// <summary>
        /// Get all currently managed entities of requested entity type.
        /// </summary>
        /// <typeparam name="T">Reuested Entity type.</typeparam>
        /// <returns>All entities of requested entity type.</returns>
        public IEnumerable<T> GetEntities<T>() where T : Entity {
            return entities.OfType<T>();
        }
    }
}
