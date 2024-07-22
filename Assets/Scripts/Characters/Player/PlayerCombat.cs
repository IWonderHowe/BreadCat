using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float _maxPlayerHealth = 150f;
    [SerializeField][Range(0, 1)] private float _playerArmorDamageReduction = 0.5f;
    

    [SerializeField] private float _currentPlayerHealth = 0f;
    [SerializeField] private float _currentPlayerArmor;


    // Start is called before the first frame update
    void Awake()
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
        // set the starting effective damage, and calculate potential damage reduction from armor
        float effectiveDamage = damageToTake;
        float armorDamageReduction = _playerArmorDamageReduction * _currentPlayerArmor;

        Debug.Log("original damage: " + effectiveDamage);

        // clamp the armor damage reduction to be at max reducing all damage
        armorDamageReduction = Mathf.Clamp(armorDamageReduction, 0, effectiveDamage);
        Debug.Log("damage reduced: " + armorDamageReduction);

        // reduce the blocked damage from the player armor
        _currentPlayerArmor -= effectiveDamage;
        _currentPlayerArmor = Mathf.Max(_currentPlayerArmor, 0);

        // take away damage reduction from the effective damage to take
        effectiveDamage -= armorDamageReduction;

        // reduce the player health by the effective damage taken
        _currentPlayerHealth -= effectiveDamage;


        

    }

    public void AddArmor(float armorToAdd)
    {
        _currentPlayerArmor += armorToAdd;
    }
}
