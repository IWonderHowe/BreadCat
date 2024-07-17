using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Variables that define the enemy
    [SerializeField] private float _maxHealth;
    
    // space to store varaibles regarding enemy status
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _isDead;


    private List<DoTStack> _dotStacks = new List<DoTStack>();


    // Start is called before the first frame update
    void Start()
    {
        // set the enemy to be alive and at full health
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(DoTStack.EnemiesAffected.Count);
    }


    public void TakeDamage(float damage)
    {
        // take damage if the enemy isn't dead. Set the enemy to dead if the current health drops to 0
        if (!_isDead) _currentHealth -= damage;
        if (_currentHealth <= 0) _isDead = true;
    }

    public void AddDoTStack(float damage, float tickTime, float totalDoTTime)
    {
        DoTStack DoTApplied = new DoTStack(damage, tickTime, totalDoTTime, this);
        DoTStack.AddEnemyToDoTList(this.gameObject);
        Debug.Log("stack added");
        StartCoroutine(DoTApplied.ApplyDamage());
        _dotStacks.Add(DoTApplied);
    }

    public void EndDoTStack(DoTStack stackToEnd)
    {
        _dotStacks.Remove(stackToEnd);
        if (_dotStacks.Count == 0) DoTStack.RemoveEnemyFromDoTList(this.gameObject);
    }

}
