using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChaosStack
{
    private static int _stacks;
    private static int _baseMaxStacks = 150;
    private static float _stacksMultiplier = 0.5f;
    private static int _maxStacks = _baseMaxStacks;

    public static int MaxStacks => _maxStacks;
    public static int Stacks => _stacks;
    public static float CurrentChaosMultiplier => _stacksMultiplier * _stacks;

    private static bool _perfectAccuracy = false;

    public static bool PerfectAccuracy => _perfectAccuracy;

    public static void ResetStacks()
    {
        Debug.Log("Chaos reset");
        _stacks = 0;
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
    

    public static void SetHasPerfectAccuracy(bool perfectAccuracy)
    {
        _perfectAccuracy = perfectAccuracy;
    }
}
