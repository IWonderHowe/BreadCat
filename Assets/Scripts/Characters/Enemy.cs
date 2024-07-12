using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    
    [SerializeField] private float _currentHealth;

    [SerializeField] private bool _isDead;


    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (!_isDead) _currentHealth -= damage;

        if (_currentHealth <= 0) _isDead = true;
    }
}
