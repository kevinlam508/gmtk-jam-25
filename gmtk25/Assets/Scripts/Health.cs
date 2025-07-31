using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Serializable]
    public class DamageTakenEvent : UnityEvent<int, int> { }
    [Serializable]
    public class DeathEvent : UnityEvent { }

    [SerializeField] private int _maxHealth;

    [SerializeField] private DamageTakenEvent _damageTaken;
    [SerializeField] private DeathEvent _died;

    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _damageTaken.Invoke(amount, _currentHealth);

        if (_currentHealth <= 0)
        {
            _died.Invoke();
        }
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
    }
}
