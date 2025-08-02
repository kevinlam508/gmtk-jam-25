using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Serializable]
    public class DamageTakenEvent : UnityEvent<int, int> { }
    [Serializable]
    public class DeathEvent : UnityEvent<GameObject, bool> { }

    [SerializeField] private int _maxHealth;

    [SerializeField] private DamageTakenEvent _damageTaken;
    [SerializeField] private DeathEvent _died;

    private int _currentHealth;

    public DamageTakenEvent DamageTaken => _damageTaken;

    private void Awake()
    {
    }

    public DeathEvent GetOnDeathEvent()
    {
        return _died;
    }

    private void OnDisable()
    {
        _died.RemoveAllListeners();
    }

    /// <summary>
    /// Assings an enemy a max health value and sets current to max health
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxHealth(int value)
    {
        _maxHealth = value;
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _damageTaken.Invoke(amount, _currentHealth);

        if (_currentHealth <= 0)
        {
            _died.Invoke(this.gameObject, true);
        }
    }


    /// <summary>
    /// Use this to kill the enemy if the Player did not kill the enemy
    /// </summary>
    public void EnemyFinish()
    {
        _died.Invoke(this.gameObject, false);
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
    }
}
