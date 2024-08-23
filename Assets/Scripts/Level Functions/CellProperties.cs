using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// a class to store properties of a cell
public class CellProperties
{
    private Vector3Int _coordinate;
    public Vector3Int Coordinate => _coordinate;

    public int[,] PossibleSizes;

    public List<CellSide> AllOpenSides;

    public bool NorthOpen;
    public bool SouthOpen;
    public bool EastOpen;
    public bool WestOpen;
    public bool AboveOpen;
    public bool BelowOpen;

    // has this piece has been chosen yet
    public bool PieceChosen;

    // the number of potential pieces for this cell
    private int _potentialPieces;
    public int PotentialPieces => _potentialPieces;


    // space to store the type of wall needed for this cell
    private WallLayout _wallType;
    public WallLayout WallType;

    // contstructor
    public CellProperties(Vector3Int coordinate)
    {
        NorthOpen = true;
        SouthOpen = true;
        EastOpen = true;
        WestOpen = true;
        BelowOpen = true;
        AboveOpen = true;
        PieceChosen = false;
        _coordinate = coordinate;
        
    }

    // find all potential pieces withing the level piece manager
    public void UpdatePotentialPieces(LevelPieceManager pieceManager)
    {
        // start a counter for potential pieces
        int potentialCount = 0;

        // go through all level pieces and if it matches this cells needs, add to the potential piece count
        foreach(GameObject piece in pieceManager.LevelPieces)
        {
            if (piece.GetComponent<LevelPiece>().WallType == WallType && DoesPieceFit(piece.GetComponent<LevelPiece>().Size)) 
            {
                potentialCount++;
            }
        }

        _potentialPieces = potentialCount;
    }

    private bool DoesPieceFit(Vector3Int pieceSize)
    {

        // Check to see if a piece can fit into the possible sizes for this cell
        if (pieceSize.x <= PossibleSizes.GetLength(0) && pieceSize.y <= PossibleSizes.GetLength(1) && pieceSize.z <= PossibleSizes[pieceSize.x - 1, pieceSize.y - 1]) return true;
        return false;
    }

    // get the proper rotation for a north based level piece to apply to this cell
    public float GetPieceRotation(GameObject levelPiece)
    {
        // get the level piece properties
        LevelPiece pieceProperties = levelPiece.GetComponent<LevelPiece>();

        // if the piece doesnt have any walls, return no rotation
        if (pieceProperties.WallType == WallLayout.NoWalls) return 0;

        // if the piece is a single wall, return the rotation relating to which wall it needs to be
        if(pieceProperties.WallType == WallLayout.Wall)
        {
            if (!NorthOpen) return 0f;
            if (!EastOpen) return 90f;
            if (!SouthOpen) return 180f;
            if (!WestOpen) return 270f;
        }

        if(pieceProperties.WallType == WallLayout.Corner)
        {
            if (!NorthOpen)
            {
                if (!EastOpen) return 0;
                else return 270;
            }

            else if (!SouthOpen)
            {
                if (!EastOpen) return 90;
                else return 180;
            }
        }

        Debug.Log("Get rotation bugged");
        return 0;
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