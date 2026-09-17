using System;
using UnityEngine;

namespace Gameplay.Currency
{
    public interface ICurrency
    {
        public event Action<long> OnChanged;
        public long Balance { get; }
        public void Set(long value);
        public void Add(long value);
        public bool TrySpend(long value);
        public Color GetTrySpendColor(long value);
    }
}