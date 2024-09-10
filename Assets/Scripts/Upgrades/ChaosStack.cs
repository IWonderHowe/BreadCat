using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class ChaosStack
{
    
    private static int _stacks;
    private static int _baseMaxStacks = 150;
    private static float _stacksMultiplier = 0.25f;
    private static int _maxStacks = _baseMaxStacks;
    private static float _perStackAccuracyPenalty = 0.05f;
    public static float AccuracyPenalty => _perStackAccuracyPenalty * _stacks;

    public static int MaxStacks => _maxStacks;
    public static int Stacks => _stacks;
    public static float CurrentChaosBoost => _stacksMultiplier * _stacks;
    public static float PerStackMultiplier => _stacksMultiplier;

    private static bool _affectsRoF = true;
    public static bool AffectsRoF => _affectsRoF;
    private static bool _affectsDamage = false;
    public static bool AffectsDamage => _affectsDamage;
   
    public static bool PerfectAccuracy => _perfectAccuracy;
    private static bool _perfectAccuracy = false;
    

    public static void ResetStacks()
    {
        Debug.Log("Chaos reset");
        _stacks = 0;
    }

    public static void ModifyStackAccuracyPenalty(float penaltyMultiplier)
    {
        _perStackAccuracyPenalty *= penaltyMultiplier;
    }

    public static void AddStacks(int stacksToAdd)
    {
        _stacks += stacksToAdd;
        Debug.Log("Chaos incremented by" + stacksToAdd);
        Debug.Log("Current Chaos" + ChaosStack.Stacks);
    }

    public static void AddMaxStacks(int limitToAdd)
    {
        Debug.Log("Max stacks added");
        _maxStacks += limitToAdd;
    }

    public static void ResetMaxStacks()
    {
        _maxStacks = _baseMaxStacks;
        Debug.Log(_baseMaxStacks);
    }
    
    public static void ModifyStackMultiplier(float stackMultiplier)
    {
        _stacksMultiplier *= stackMultiplier;
    }
    
    public static void SetStackMultiplier(float stackMultiplier)
    {
        _stacksMultiplier = stackMultiplier;
    }

    public static void SetHasPerfectAccuracy(bool perfectAccuracy)
    {
        _perfectAccuracy = perfectAccuracy;
    }
    public static void SetAffectsDamage(bool affectsDamage)
    {
        _affectsDamage = affectsDamage;
    }
    public static void SetAffectsRoF(bool affectsRoF)
    {
        _affectsRoF = affectsRoF;
    }

}
