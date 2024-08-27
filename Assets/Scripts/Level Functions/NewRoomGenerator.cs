using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using static UnityEngine.UI.Image;

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

        FillCellAvailableSizes();

        FillCellAvailablePieceCount();

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

            // testing for larger pieces
            //if (!_cellProperties[0, 0, 0].PieceChosen) cellToFill = new Vector3Int(0, 0, 0);

            Debug.Log(cellToFill);

            Debug.Log(_cellProperties[cellToFill.x, cellToFill.y, cellToFill.z].PossibleSizes.GetLength(0) + " " +  _cellProperties[cellToFill.x, cellToFill.y, cellToFill.z].PossibleSizes.GetLength(1) + " " + _cellProperties[cellToFill.x, cellToFill.y, cellToFill.z].PossibleSizes[0,0]) ;

            // get the level piece to spawn in this cell
            GameObject pieceToSpawn = _pieceList.GetLevelPieceForCell(_cellProperties[cellToFill.x, cellToFill.y, cellToFill.z]);

            // get the position to spawn the piece
            Vector3 spawnPos = cellToFill * 3;

            // get the proper rotation to set the piece at
            float pieceRotation = _cellProperties[cellToFill.x, cellToFill.y, cellToFill.z].GetPieceRotation(pieceToSpawn);

            //Debug.Log(cellToFill + " " + pieceRotation);

            // instantiate the level piece chosen for that cell
            GameObject spawnedPiece = Instantiate(pieceToSpawn, spawnPos, Quaternion.Euler(0, pieceRotation, 0));


            // set the cells properties to have a piece chosen based on the size of piece placed
            CallForAllCells(SetPieceChosen, spawnedPiece.GetComponent<LevelPiece>().Size, _cellProperties[cellToFill.x, cellToFill.y, cellToFill.z]);

            // update available sizes for each cell
            CallForAllCells(SetCellAvailableSize, _roomSize);

            //_cellProperties[cellToFill.x, cellToFill.y, cellToFill.z].PieceChosen = true;
            _emptyCellCount--;
        }

    }

    private void SetPieceChosen(Vector3Int coordinate)
    {
        _cellProperties[coordinate.x, coordinate.y, coordinate.z].PieceChosen = true;
    }


    private Vector3Int GetCellToFill()
    {
        // start with a vector 3 int of all -1s so errors are easy to spot
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
        CallForAllCells(SetBaseCellProperties, _roomSize);
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
        cellProperties.NorthOpen = !OnNorthRoomWall(coordinate.z);
        cellProperties.SouthOpen = !OnSouthRoomWall(coordinate.z);
        cellProperties.EastOpen = !OnEastRoomWall(coordinate.x);
        cellProperties.WestOpen = !OnWestRoomWall(coordinate.x);


        // count the touching room walls
        int wallCount = 0;
        if (!cellProperties.NorthOpen) wallCount++;
        if (!cellProperties.SouthOpen) wallCount++;
        if (!cellProperties.EastOpen) wallCount++;
        if (!cellProperties.WestOpen) wallCount++;
        

        // get the necessary wall type based on wall count
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

        //Debug.Log(cellProperties.Coordinate + " " + cellProperties.WallType + " " + cellProperties.BelowOpen);

        // set the coordinates properties to be these properties
        _cellProperties[coordinate.x, coordinate.y, coordinate.z] = cellProperties;
    }

    private void FillCellAvailablePieceCount()
    {
        CallForAllCells(UpdateCellAvailablePieces, _roomSize);
    }

    private void UpdateCellAvailablePieces(CellProperties properties)
    {
        properties.UpdatePotentialPieces(_pieceList);
    }

    /*public bool DoesPieceFit(LevelPiece piece, Vector3Int origin)
    {

        // see if it fits on the x origin
        // piece doesnt fit if size of piece goes over size of room from point
        if (origin.x + piece.Size.x >= _roomSize.x) return false;

        // piece doesnt fit if any of the spaces in the x row are 
        for (int i = 1; i < piece.Size.x; i++)
        {
            if (_cellProperties[origin.x + i, origin.y, origin.z].PieceChosen) return false;
        }


    }*/

    // methods to get available size data for cells
    private void FillCellAvailableSizes()
    {
        CallForAllCells(SetCellAvailableSize, _roomSize);
    }

    private void SetCellAvailableSize(CellProperties properties)
    {
        // Method 2: find the max length of every coordinate going from its origin

        Vector3Int originCoordinate = properties.Coordinate;

        Vector3Int originEmptyCellsLength = GetEmptyCellsLengthFromPoint(originCoordinate);

        // get x empty cells legnth along all z coordinates. Do this for each y level possible within origin empty cells length
        // set space for the cell size available data
        properties.PossibleSizes = new int[originEmptyCellsLength.x, originEmptyCellsLength.y];

        // for every y level
        for (int k = 0; k < originEmptyCellsLength.y; k++)
        {
            // loop through all x cells
            for(int i = 0; i < originEmptyCellsLength.x; i++)
            {
                int zSizeAtX = 0;
                // for each x cell, check the length of empty cells from that grid point, up to the z origin empty cells length
                for(int j = 0; j < originEmptyCellsLength.z; j++)
                {
                    // if a piece isnt chosen, iterate on the z size
                    if (_cellProperties[originCoordinate.x + i, originCoordinate.y + k, originCoordinate.z + j].PieceChosen) break;
                    zSizeAtX++;
                }

                // set the z size at the xy coordinate
                properties.PossibleSizes[i, k] = zSizeAtX;
            }
        }


        // Method 1: iterate a box
        /*
        // create the initial available dimensions of the cell
        int cellXSize = 1;
        int cellYSize = 1;
        int cellZSize = 1;

        // store whether more iteration is possible in each direction
        bool xCanIterate = true;
        bool yCanIterate = true;
        bool zCanIterate = true;

        // create a list to store the new cells added to check if available
        List<Vector3Int> newCellsToCheck = new List<Vector3Int>();

        // while iteration in any dimension is still possible
        while(xCanIterate || yCanIterate || zCanIterate)
        {
            if (xCanIterate)
            {
                // if the x coordinate of the cell is greater than or equal to the x legnth dont allow the cell to iterate its x size
                if (properties.Coordinate.x + cellXSize - 1 >= _roomSize.x)
                {
                    xCanIterate = false;
                }
                // if not, iterate on the x size
                else
                {
                    cellXSize++;
                }
                
                // check the cells added to the max size cube
                // if the cells are all 




            }
            
            

        }*/





    }

    private Vector3Int GetEmptyCellsLengthFromPoint(Vector3Int origin)
    {
        // get the x available length
        int xFromOriginLength = 1;
        for (int i = 1; i < _roomSize.x - origin.x; i++)
        {
            if (_cellProperties[origin.x + i, origin.y, origin.z].PieceChosen) break;
            xFromOriginLength++;
        }

        // get the y available length
        int yFromOriginLength = 1;
        for (int i = 1; i < _roomSize.y - origin.y; i++)
        {
            if (_cellProperties[origin.x, origin.y + 1, origin.z].PieceChosen) break;
            yFromOriginLength++;
        }

        // get the z available length
        int zFromOriginLength = 1;
        for (int i = 1; i < _roomSize.z - origin.z; i++)
        {
            if (_cellProperties[origin.x, origin.y, origin.z + i].PieceChosen) break;
            zFromOriginLength++;
        }

        return new Vector3Int(xFromOriginLength, yFromOriginLength, zFromOriginLength);
    }

    // A collection of methods to check which rpo
    private bool OnEastRoomWall(int coordsEast)
    {
        return coordsEast == _roomSize.x - 1;
    }

    private bool OnNorthRoomWall(int coordsNorth)
    {
        return coordsNorth == _roomSize.z - 1;
    }

    private bool OnSouthRoomWall(int coordsNorth)
    {
        return coordsNorth == 0;
    }

    private bool OnWestRoomWall(int coordsEast)
    {
        return coordsEast == 0;
    }


    // cycle through all cells based on their coordinates
    // uses different inputs based on need
    private void CallForAllCells(System.Action methodToCall, Vector3Int gridToIterate)
    {
        for (int x = 0; x < gridToIterate.x; x++)
        {
            for (int y = 0; y < gridToIterate.y; y++)
            {
                for (int z = 0; z < gridToIterate.z; z++)
                {
                    methodToCall();
                }
            }
        }
    }

    private void CallForAllCells(System.Action<Vector3Int> methodToCall, Vector3Int gridToIterate)
    {
        for (int x = 0; x < gridToIterate.x; x++)
        {
            for (int y = 0; y < gridToIterate.y; y++)
            {
                for (int z = 0; z < gridToIterate.z; z++)
                {
                    Vector3Int cell = new Vector3Int(x, y, z);
                    methodToCall(cell);
                }
            }
        }
    }

    private void CallForAllCells(System.Action<CellProperties> methodToCall, Vector3Int gridToIterate)
    {
        for (int x = 0; x < gridToIterate.x; x++)
        {
            for (int y = 0; y < gridToIterate.y; y++)
            {
                for (int z = 0; z < gridToIterate.z; z++)
                {
                    methodToCall(_cellProperties[x,y,z]);
                }
            }
        }

    }
    
    private void CallForAllCells(System.Action<Vector3Int> methodToCall, Vector3Int gridToIterate, CellProperties cell)
    {
        for (int x = 0; x < gridToIterate.x; x++)
        {
            for(int y = 0; y < gridToIterate.y; y++)
            {
                for(int z = 0; z < gridToIterate.z; z++)
                {

                    Vector3Int cellToSetChosen = new Vector3Int();
                    cellToSetChosen.x = x + cell.Coordinate.x;
                    cellToSetChosen.y = y + cell.Coordinate.y;
                    cellToSetChosen.z = z + cell.Coordinate.z;

                    //Debug.Log(cellToSetChosen);
                    SetPieceChosen(cellToSetChosen);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach(CellProperties property in _cellProperties)
        {

            Gizmos.color = UnityEngine.Color.green;
            if (property.PieceChosen) Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawCube((property.Coordinate * 3) + (Vector3.one * 1.5f), Vector3.one);
        }
    }

}

public enum GridDimension { x, y, z }