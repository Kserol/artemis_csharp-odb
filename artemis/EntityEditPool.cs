using Artemis.Utils;
using System;

namespace Artemis
{
    public class EntityEditPool
    {
        private Bag<EntityEdit> pool = new Bag<EntityEdit>();
        private EntityManager em;
	
	    private Bag<EntityEdit> edited;
        private Bag<EntityEdit> alternateEdited;
        private BitSet editedIds;

	    private BitSet pendingDeletion;

        internal EntityEditPool(EntityManager entityManager)
        {
            em = entityManager;

            edited = new Bag<EntityEdit>();
            alternateEdited = new Bag<EntityEdit>();
            editedIds = new BitSet();

            pendingDeletion = new BitSet();
        }

        internal void Delete(int entityId)
        {
            pendingDeletion.Set(entityId);

            if (editedIds.Get(entityId))
            {
                ProcessAndRemove(entityId);
            }
        }

        internal bool IsPendingDeletion(int entityId)
        {
            return pendingDeletion.Get(entityId);
        }

        internal bool IsEdited(int entityId)
        {
            return editedIds.Get(entityId);
        }

        internal void ProcessAndRemove(int entityId)
        {
            EntityEdit edit = FindEntityEdit(entityId, true);
            em.UpdateCompositionIdentity(edit);

            pool.Add(edit);

            editedIds.Set(entityId, false);
        }


        /**
         * Get entity editor.
         * @return a fast albeit verbose editor to perform batch changes to entities.
         * @param entityId entity to fetch editor for.
         */
        internal EntityEdit ObtainEditor(int entityId)
        {
            if (editedIds.Get(entityId))
                return FindEntityEdit(entityId, false);

            EntityEdit edit = EntityEdit();
            editedIds.Set(entityId);
            edited.Add(edit);

            edit.EntityId = entityId;

            if (!em.IsActive(entityId))
                throw new InvalidProgramException("Issued edit on deleted " + edit.EntityId);

            // since archetypes add components, we can't assume that an
            // entity has an empty bitset.
            // Note that editing an entity created by an archetype removes the performance
            // benefit of archetyped entity creation.
            BitSet bits = em.ComponentBits(entityId);
            edit.ComponentBits.Or(bits);

            return edit;
        }

        private EntityEdit EntityEdit()
        {
            if (pool.IsEmpty)
            {
                return new EntityEdit(em.World);
            }
            else {
                EntityEdit edit = pool.RemoveLast();
                edit.ComponentBits.Clear();
                return edit;
            }
        }

        private EntityEdit FindEntityEdit(int entityId, bool remove)
        {
            // Since it's quite likely that already edited entities are called
            // repeatedly within the same scope, we start by first checking the last
            // element, before checking the rest.
            int last = edited.Size - 1;
            if (edited[last].EntityId == entityId)
            {
                return remove ? edited.Remove(last) : edited[last];
            }

            int i = 0;
            foreach(var edit in edited)
            { 

                if (edit.EntityId != entityId)
                    continue;

                return (remove) ? edited.Remove(i) : edit;
                i++;
            }

            throw new NotSupportedException();
        }

        internal bool ProcessEntities()
        {
            int size = edited.Size;
            if (size == 0 && pendingDeletion.IsEmpty())
                return false;

            
            Object[] data = edited.data;
            editedIds.Clear();
            // on perd le tableau data suite au clear ?
            edited.Zero();
            SwapEditBags();

            World w = em.World;
            for (int i = 0; size > i; i++)
            {
                EntityEdit edit = (EntityEdit)data[i];
                em.UpdateCompositionIdentity(edit);

                if (!pendingDeletion.Get(edit.EntityId))
                    w.changed.Set(edit.EntityId);

                pool.Add(edit);
            }

            w.deleted.Or(pendingDeletion);
            pendingDeletion.Clear();

            return true;
        }

        private void SwapEditBags()
        {
            Bag<EntityEdit> tmp = edited;
            edited = alternateEdited;
            alternateEdited = tmp;
        }


    }
}