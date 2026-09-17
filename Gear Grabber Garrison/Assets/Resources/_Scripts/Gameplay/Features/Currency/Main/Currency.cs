using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Currency
{
    public class Currency : ICurrency, IInitializable
    {
        public event Action<long> OnChanged;
        public long Balance => _repository.Balance;

        [Inject] private CurrencyRepository _repository;
        [Inject] private CurrencyView _view;
        [Inject] private CurrencyData _config;

        public void Initialize()
        {
            _repository.OnChanged += (balance) => OnChanged?.Invoke(balance);
            OnChanged += _view.UpdateBalance;
        }

        public void Add(long value)
        {
            _repository.Add(value);
        }

        public void Set(long value)
        {
            _repository.Set(value);
        }

        public bool TrySpend(long value)
        {
             bool result = _repository.TrySpend(value);
             return result;
        }

        public Color GetTrySpendColor(long value)
        {
            return (_repository.Balance >= value ? _config.Success : _config.Failure);
        }
    }
}