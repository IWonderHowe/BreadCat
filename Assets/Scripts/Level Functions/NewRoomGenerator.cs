using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class NewRoomGenerator : MonoBehaviour
{
    /// <summary>
    ///
    ///     ROOM GENERATOR
    ///     
    ///     To Do:
    ///     fill all available pieces list
    ///     
    ///     Compare pieces in piece list with available pieces
    ///     
    /// 
    /// </summary>

    // Size of room
    [SerializeField] private Vector3Int _roomSize;

    // a game object holding all possible level pieces
    private LevelPieceManager _pieceList;

    // a list of the properties of each cell in the room
    private CellProperties[,,] _cellProperties;


    // a list to contain all possible level pieces for each cell
    //private List<GameObject>[,,] _availablePieces;

    // a count of empty cells left, used to determine how many iterations of generation
    private int _emptyCellCount;

    private void Start()
    {
        _pieceList = GetComponentInChildren<LevelPieceManager>();


        //_availablePieces = new List<GameObject>[_roomSize.x, _roomSize.y, _roomSize.z];
        _cellProperties = new CellProperties[_roomSize.x, _roomSize.y, _roomSize.z];

        // make the empty cell count start with all possible cells
        _emptyCellCount = _cellProperties.Length;


        //FillRoomAvailablePieces();
        FillRoomProperties();

        GenerateRoom();

    }

    // generate a new room
    public void GenerateRoom()
    {
        // do while there are still empty cells in the room
        while(_emptyCellCount > 0)
        {
            // find the best possible cell to try to fill
            Vector3Int cellToFill = GetCellToFill();

            // get the level piece to spawn in this cell
            GameObject pieceToSpawn = _pieceList.GetLevelPieceForCell(_cellProperties[cellToFill.x, cellToFill.y, cellToFill.z]);

            // get the position to spawn the piece
            Vector3 spawnPos = cellToFill * 3;

            // get the proper rotation to set the piece at
            float pieceRotation = _cellProperties[cellToFill.x, cellToFill.y, cellToFill.z].GetPieceRotation(pieceToSpawn);

            //Debug.Log(cellToFill + " " + pieceRotation);

            // instantiate the level piece chosen for that cell
            GameObject spawnedPiece = Instantiate(pieceToSpawn, spawnPos, Quaternion.Euler(0, pieceRotation, 0));


            // set the cells properties to have a piece chosen
            _cellProperties[cellToFill.x, cellToFill.y, cellToFill.z].PieceChosen = true;
            _emptyCellCount--;
        }

    }


    private Vector3Int GetCellToFill()
    {
        Vector3Int cellToFill = Vector3Int.one * -1;

        // get the lowest possible pieces for each cell, aka the full total number of available pieces for generation
        int lowestPotentialPieces = _pieceList.LevelPieces.Count;

        // go through all cells and if the cell has the lowest potential pieces
        foreach (CellProperties property in _cellProperties)
        {
            // if the cell has already been filled, skip it
            if (property.PieceChosen) continue;

            // set the cell to fill to be this one if it has the lowest potential pieces
            if (property.PotentialPieces < lowestPotentialPieces)
            {
                cellToFill = property.Coordinate;
                lowestPotentialPieces = property.PotentialPieces;
            }

            // if there is only one potential piece for this cell, skip to generating cell
            if (property.PotentialPieces == 1)
            {
                break;
            }
        }

        return cellToFill;
    }

    private void FillRoomProperties()
    {
        CallForAllCells(SetBaseCellProperties);
    }


    private void SetBaseCellProperties(Vector3Int coordinate)
    {
        // add a new cell property object to the list
        CellProperties cellProperties = new CellProperties(coordinate);


        // if on base level of room set the cell to need to have a room
        if(coordinate.y == 0)
        {
            cellProperties.BelowOpen = false;
        }

        // get which room walls this cell is touching
        cellProperties.NorthOpen = !OnNorthWall(coordinate.z);
        cellProperties.SouthOpen = !OnSouthWall(coordinate.z);
        cellProperties.EastOpen = !OnEastWall(coordinate.x);
        cellProperties.WestOpen = !OnWestWall(coordinate.x);


        // count the touching room walls
        int wallCount = 0;
        if (!cellProperties.NorthOpen) wallCount++;
        if (!cellProperties.SouthOpen) wallCount++;
        if (!cellProperties.EastOpen) wallCount++;
        if (!cellProperties.WestOpen) wallCount++;
        

        // get the wall type based on wall count
        switch (wallCount)
        {
            case 0:
                cellProperties.WallType = WallLayout.NoWalls;
                break;
            case 1:
                cellProperties.WallType = WallLayout.Wall;
                break;

            // if both walls are opposite each other, make the wall type a hall, if not its a corner
            case 2:
                if (!cellProperties.NorthOpen && !cellProperties.SouthOpen || !cellProperties.EastOpen && !cellProperties.WestOpen)
                {
                    cellProperties.WallType = WallLayout.Hall;
                    break;
                }
                cellProperties.WallType = WallLayout.Corner;
                break;
            case 3:
                cellProperties.WallType = WallLayout.DeadEnd;
                break;

            default:
                Debug.Log("wall layout setting bugged");
                break;
        }

        // update the potential piece count from the piece list
        cellProperties.UpdatePotentialPieces(_pieceList);



        // set the coordinates properties to be these properties
        _cellProperties[coordinate.x, coordinate.y, coordinate.z] = cellProperties;

        
    }

    private bool OnEastWall(int coordsEast)
    {
        return coordsEast == _roomSize.x - 1;
    }

    private bool OnNorthWall(int coordsNorth)
    {
        return coordsNorth == _roomSize.z - 1;
    }

    private bool OnSouthWall(int coordsNorth)
    {
        return coordsNorth == 0;
    }

    private bool OnWestWall(int coordsEast)
    {
        return coordsEast == 0;
    }

    // find all avaialable pieces for a given room generation
    private void FillRoomAvailablePieces()
    {
        CallForAllCells(FillCellAvailablePieces);
    }

    private void FillCellAvailablePieces(Vector3Int cell)
    {
        //_availablePieces[cell.x, cell.y, cell.z] = new List<GameObject>();
        //_availablePieces[cell.x, cell.y, cell.z].Add(_pieceList.LevelPieces[0]);
    }




    // cycle through all cells based on their coordinates
    // uses different inputs based on need
    private void CallForAllCells(System.Action methodToCall)
    {
        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    methodToCall();
                }
            }
        }
    }
    private void CallForAllCells(System.Action<Vector3Int> methodToCall)
    {
        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    Vector3Int cell = new Vector3Int(x, y, z);
                    methodToCall(cell);
                }
            }
        }
    }


}
