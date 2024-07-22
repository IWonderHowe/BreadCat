using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float _maxPlayerHealth = 150f;
    [SerializeField][Range(0, 1)] private float _playerArmorDamageReduction = 0.5f;
    

    private float _currentPlayerHealth;
    [SerializeField] private float _currentPlayerArmor;


    // Start is called before the first frame update
    void Start()
    {
        _currentPlayerHealth = _maxPlayerHealth;
        _currentPlayerArmor = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageToTake)
    {
        float effectiveDamage = damageToTake;

        if (_currentPlayerArmor > 0)
        {
            if (damageToTake >= 2 * _currentPlayerArmor)
            {
                effectiveDamage = damageToTake - (_currentPlayerArmor * _playerArmorDamageReduction);
            }
            else
            {
                effectiveDamage -= _currentPlayerArmor * _playerArmorDamageReduction;
            }
        }
    }

    public void AddArmor(float armorToAdd)
    {
        _currentPlayerArmor += armorToAdd;
    }
}
