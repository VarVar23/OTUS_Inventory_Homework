using System;

namespace Gameplay.Currency
{
    internal class CurrencyRepository
    {
        public long Balance { get; private set; }
        public event Action<long> OnChanged;

        public void Set(long value)
        {
            Balance = value;
            OnChanged?.Invoke(Balance);
        }

        public void Add(long value)
        {
            Balance += value;
            OnChanged?.Invoke(Balance);
        }

        public bool TrySpend(long value)
        {
            if (Balance < value)
                return false;

            Balance -= value;
            OnChanged?.Invoke(Balance);
            return true;
        }
    }
}