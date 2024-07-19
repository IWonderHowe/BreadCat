using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // Variables that define the enemy
    [SerializeField] private float _maxHealth;
    
    // space to store varaibles regarding enemy status
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _isDead;

    // DoT variables
    private List<DoTStack> _dotStacks = new List<DoTStack>();
    public List<DoTStack> DoTStacks => _dotStacks;

    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _dotSlider;




    // Start is called before the first frame update
    void Start()
    {
        // set the enemy to be alive and at full health
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dotDamageLeft = 0;
        foreach(DoTStack i in _dotStacks)
        {
            dotDamageLeft += i.DoTDamageRemaining;
        }


        UpdateHealthBar();
    }


    public void TakeDamage(float damage)
    {
        // take damage if the enemy isn't dead. Set the enemy to dead if the current health drops to 0
        if (!_isDead) _currentHealth -= damage;
        if (_currentHealth <= 0) _isDead = true;
    }

    private void UpdateHealthBar()
    {
        float remainingDoT = GetRemainingDoT();
        if (remainingDoT == 0) _dotSlider.gameObject.SetActive(false);
        else
        {
            _dotSlider.gameObject.SetActive(true);
            _dotSlider.value = GetRemainingDoT() / _maxHealth;
        }

        _healthSlider.value = _currentHealth / _maxHealth;
        
    }

    public void AddDoTStack(float damage, float tickTime, float totalDoTTime)
    {
        // Add a DoT stack to the current enemy, and begin its damage
        DoTStack DoTApplied = new DoTStack(damage, tickTime, totalDoTTime, this);
        StartCoroutine(DoTApplied.ApplyDamage());
        
        // add the DoT stack to this enemies list of DoT stacks, and add the enemy to the list of enemies affected by DoT
        _dotStacks.Add(DoTApplied);
        DoTStack.AddEnemyToDoTList(this.gameObject);
    }

    public void EndDoTStack(DoTStack stackToEnd)
    {
        // remove a finished stack of DoT from the enemy. remove it from the DoT affected enemies list if it has no more DoT stacks left
        _dotStacks.Remove(stackToEnd);
        if (_dotStacks.Count == 0) DoTStack.RemoveEnemyFromDoTList(this.gameObject);
    }

    public float GetRemainingDoT()
    {
        float dotRemaining = 0f;

        foreach(DoTStack dotStack in _dotStacks)
        {
            dotRemaining += dotStack.DoTDamageRemaining;
        }

        if (dotRemaining > _currentHealth) dotRemaining = _currentHealth;

        return dotRemaining;
    }

}
