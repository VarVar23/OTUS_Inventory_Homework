using System;
using System.Collections.Generic;
using Gameplay.Itemization.InternalContracts;
using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Services
{
    internal sealed class ItemPool
    {
        private readonly Dictionary<Guid, Item> _allItems = new();
        private readonly Dictionary<Guid, ItemStatus> _statusMap = new();

        private readonly Queue<Guid> _generationQueue = new();

        public int UnclaimedItemCount => _generationQueue.Count;

        public event Action<int> OnUnclaimedQueueUpdate;

        public void RegisterItem(Item item)
        {
            if (_allItems.ContainsKey(item.Id)) return;

            _allItems.Add(item.Id, item);
            _statusMap.Add(item.Id, ItemStatus.Generated);
            _generationQueue.Enqueue(item.Id);
            OnUnclaimedQueueUpdate?.Invoke(_generationQueue.Count);
        }

        public void RegisterItemWithStatus(Item item, ItemStatus status)
        {
            if (_allItems.ContainsKey(item.Id)) return;

            _allItems.Add(item.Id, item);
            _statusMap.Add(item.Id, status);
        }

        public Item PopNextGenerated()
        {
            Guid id;
            do
            {
                if (_generationQueue.Count == 0)
                {
                    throw new InvalidOperationException("[ItemPool] Attempted to pop from an empty Generated queue.");
                }
                id = _generationQueue.Dequeue();
            }
            while (!_allItems.ContainsKey(id));

            OnUnclaimedQueueUpdate?.Invoke(_generationQueue.Count);

            _statusMap[id] = ItemStatus.Received;
            return _allItems[id];
        }

        public Item GetItem(Guid id)
        {
            if (_allItems.TryGetValue(id, out var item)) return item;
            throw new KeyNotFoundException($"[ItemPool] Item {id} not found.");
        }

        public ItemStatus GetStatus(Guid id) => _statusMap[id];

        public void DestroyItem(Guid id)
        {
            if (!_allItems.TryGetValue(id, out Item item)) return;

            item.Dispose();
            _allItems.Remove(id);
            _statusMap.Remove(id);
            
            // Dead guids are left in _generationQueue intentionally (O(n) removal not worth it).
            // PopNextGenerated skips them via a ContainsKey check.
        }
    }
}