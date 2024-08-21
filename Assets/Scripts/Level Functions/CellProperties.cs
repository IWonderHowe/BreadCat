using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// a class to store properties of a cell
public class CellProperties
{
    private Vector3Int _coordinate;
    public Vector3Int Coordinate => _coordinate;

    public Vector3Int SizeAvailable;

    public List<CellSide> AllOpenSides;

    public bool NorthOpen;
    public bool SouthOpen;
    public bool EastOpen;
    public bool WestOpen;
    public bool AboveOpen;
    public bool BelowOpen;
    public bool PieceChosen;
    private WallLayout _wallType;
    public WallLayout WallType;
    public CellProperties(Vector3Int coordinate)
    {
        AllOpenSides = new List<CellSide>();
        NorthOpen = true;
        SouthOpen = true;
        EastOpen = true;
        WestOpen = true;
        BelowOpen = true;
        AboveOpen = true;
        PieceChosen = false;
        _coordinate = coordinate;
    }
}

public enum CellSide
{
    North,
    South,
    West,
    East,
    Above,
    Below
}

public enum WallLayout
{
    Wall,
    Corner,
    Hall,
    DeadEnd,
    NoWalls
}