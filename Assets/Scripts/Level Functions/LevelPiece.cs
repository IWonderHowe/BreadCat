using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPiece : MonoBehaviour
{

    [SerializeField] public WallLayout WallType;
    [SerializeField] public Vector3 Size;
    [SerializeField] public bool AboveOpen;
    [SerializeField] public bool BelowOpen;
    [SerializeField] public bool IsEntrance;
    [SerializeField] public bool IsExit;
    [SerializeField] public bool CenterOccupied;
    
    [SerializeField] public int Weight = 1;

    //properties for debugging
    [SerializeField] public bool IsCourtyard;
    [SerializeField] public bool IsOpenPiece;

}
