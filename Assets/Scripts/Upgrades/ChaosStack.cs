using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChaosStack
{
    private static int _stacks;
    private static int _maxStacks = 150;
    private static float _stacksMultiplier = 0.5f;

    public static int MaxStacks => _maxStacks;
    public static int Stacks => _stacks;
    public static float CurrentChaosMultiplier => _stacksMultiplier * _stacks;

    private static bool _perfectAccuracy = false;

    public static bool PerfectAccuracy => _perfectAccuracy;

    public static void ResetStacks()
    {
        _stacks = 0;
    }

    public static void AddStack()
    {
        Debug.Log("Chaos incremented");
        _stacks++;
    }

    public static void SetHasPerfectAccuracy(bool perfectAccuracy)
    {
        _perfectAccuracy = perfectAccuracy;
    }

}
