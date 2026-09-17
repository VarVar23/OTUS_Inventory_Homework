using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField] private Unit targetUnit;
        [SerializeField] private HealthBarUI view;
        [SerializeField] private float lerpSpeed = 5f;

        private float _currentFill = 1f;
        private float _targetFill = 1f;

        void Start()
        {
            if (targetUnit == null)
            {
                targetUnit = GetComponentInParent<Unit>();
                if (targetUnit == null)
                {
                    Debug.LogWarning($"[{nameof(HealthBarController)}] Unit не найден на родителе {transform.parent?.name}!");
                    enabled = false;
                    return;
                }
            }

            if (view == null || targetUnit.HealthSystem == null)
            {
                enabled = false;
                return;
            }

            targetUnit.HealthSystem.Stat.OnHealthChanged += OnHealthChanged;
            targetUnit.HealthSystem.Stat.OnAlive += Show;
            targetUnit.HealthSystem.Stat.OnDeath += Hide;
            InitializeFill();
        }

        void OnEnable()
        {
            InitializeFill();
        }

        void InitializeFill()
        {
            if (targetUnit == null || targetUnit.HealthSystem == null) return;

            view.Show();
            UpdateTargetFill();
            _currentFill = _targetFill;
            view.SetFill(_currentFill);
        }

        private void Show()
        {
            view.Show();
        }

        private void Hide()
        {
            view.Hide();
        }

        void OnHealthChanged()
        {
            UpdateTargetFill();
        }

        void Update()
        {
            if (Mathf.Approximately(_currentFill, _targetFill))
                return;

            _currentFill = Mathf.Lerp(_currentFill, _targetFill, lerpSpeed * Time.deltaTime);
            view.SetFill(_currentFill);
        }

        void UpdateTargetFill()
        {
            _targetFill = (float)targetUnit.HealthSystem.Stat.CurrentHealth /
                        targetUnit.HealthSystem.Stat.MaxHealth;

            if (targetUnit.IsDead)
                HandleDeath();
        }

        void HandleDeath()
        {
            _currentFill = 1f;
            _targetFill = 1f;
            view.SetFill(1f);
        }

        void OnDestroy()
        {
            if (targetUnit?.HealthSystem != null)
                targetUnit.HealthSystem.Stat.OnHealthChanged -= OnHealthChanged;
        }
    }
}