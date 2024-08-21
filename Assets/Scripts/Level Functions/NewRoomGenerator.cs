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

    private void Start()
    {
        _pieceList = GetComponentInChildren<LevelPieceManager>();


        //_availablePieces = new List<GameObject>[_roomSize.x, _roomSize.y, _roomSize.z];
        _cellProperties = new CellProperties[_roomSize.x, _roomSize.y, _roomSize.z];


        //FillRoomAvailablePieces();
        FillRoomProperties();

        foreach (CellProperties property in _cellProperties)
        {
            Debug.Log(property.Coordinate + " " + property.WallType);
        }
    }

    // generate a new room
    public void GenerateRoom()
    {

    }

    private void FillRoomProperties()
    {
        CallForAllCells(SetBaseCellProperties);
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
        bool onNorthWall = OnNorthWall(coordinate.z);
        bool onSouthWall = OnSouthWall(coordinate.z);
        bool onEastWall = OnEastWall(coordinate.x);
        bool onWestWall = OnWestWall(coordinate.x);

        // count the touching room walls
        int wallCount = 0;
        if (onNorthWall) wallCount++;
        if (onSouthWall) wallCount++;
        if (onEastWall) wallCount++;
        if (onWestWall) wallCount++;

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
                if (onNorthWall && onSouthWall || onEastWall && onWestWall)
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

        // set the coordinates properties to be these properties
        _cellProperties[coordinate.x, coordinate.y, coordinate.z] = cellProperties;
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
