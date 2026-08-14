using System;
using UnityEngine;

namespace ArenaBreak.Core
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 100;

        // 플레이 중 인스펙터에서 줄어드는 것을 눈으로 확인하려고 직렬화한다. 쓰기는 이 클래스만 한다
        [SerializeField] private int _currentHealth;

        public event Action<int, int> HealthChanged;
        public event Action Died;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        private void Awake()
        {
            ResetHealth();
        }

        // 풀에서 재사용할 때 다시 살려내기 위해 밖에서 부를 수 있게 둔다
        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void TakeDamage(int amount)
        {
            // 죽은 뒤 들어온 데미지로 사망 이벤트가 두 번 발생하는 것을 막는다
            if (IsDead)
            {
                return;
            }

            _currentHealth = Mathf.Max(_currentHealth - amount, 0);
            HealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                // 파괴는 여기서 하지 않는다. 적은 사라지고 플레이어는 남아야 한다
                Died?.Invoke();
            }
        }
    }
}
