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

    public static void ResetStacks()
    {
        _stacks = 0;
    }

    public static void AddStack()
    {
        _stacks++;
    }

}
