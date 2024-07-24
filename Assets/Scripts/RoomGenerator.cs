using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _roomPieces = new List<GameObject>();
    [SerializeField] private Vector3Int _roomSize;
    private GameObject[,,] _roomLayout;
    private List<GameObject>[,,] _availablePieces;

    private void Start()
    {
        // set space for the room layout that is updated with established pieces
        _roomLayout = new GameObject[_roomSize.x, _roomSize.y, _roomSize.z];

        // set space for the available room pieces in vacant spaces
        _availablePieces = new List<GameObject>[_roomSize.x, _roomSize.y, _roomSize.z];

        FillAvailableRoomPieces();

        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    //Debug.Log(_availablePieces[x, y, z].Count + " available for coordinate " + x + y + z);
                    foreach (GameObject piece in _availablePieces[x, y, z])
                    {
                        //Debug.Log(piece.name);
                    }
                }
            }
        }
    }

    public void GenerateRoom()
    {
        Vector3Int startCell = FindStartCell("North");
        Vector3Int nextCell = new Vector3Int();
        //Debug.Log(roomLayout.GetLength(0) + " " + roomLayout.GetLength(1) + " " + roomLayout.GetLength(2));
        SpawnRoomPiece(startCell);
        UpdateAvailableRoomPieces(startCell);

        /* Used to see available piece count for each cell
        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    Debug.Log(_availablePieces[x, y, z].Count + " available for coordinate " + x + y + z); 
                }
            }
        }*/

        nextCell = GetNextCell(startCell);

        for (int x = 0; x < _roomLayout.Length; x++)
        {
            SpawnRoomPiece(nextCell);
            UpdateAvailableRoomPieces(nextCell);
            nextCell = GetNextCell(nextCell);
        }

    }

    private void FillAvailableRoomPieces()
    {
        // fill the available pieces array with all possible pieces
        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    _availablePieces[x, y, z] = new List<GameObject>();
                    // add each room piece to possible pieces
                    foreach (GameObject roomPiece in _roomPieces)
                    {
                        // omit room piece being added if it is on the edge of the room layout and the outside wall doesnt exist
                        if (x == 0 && roomPiece.GetComponent<LevelPiece>().WestOpen) continue;
                        if (x == _roomSize.x - 1 && roomPiece.GetComponent<LevelPiece>().EastOpen) continue;
                        if (z == 0 && roomPiece.GetComponent<LevelPiece>().SouthOpen) continue;
                        if (z == _roomSize.z - 1 && roomPiece.GetComponent<LevelPiece>().NorthOpen) continue;

                        // omit room entrance or exit piece if the current gridspot is not on a room edge
                        if (x != 0 && x != _roomSize.x - 1 && z != 0 && z != _roomSize.z - 1)
                        {
                            if (roomPiece.GetComponent<LevelPiece>().IsEntrance || roomPiece.GetComponent<LevelPiece>().IsExit) continue;
                        }

                        _availablePieces[x, y, z].Add(roomPiece);
                    }
                }
            }
        }
    }

    private List<Vector3Int> GetAdjacentEmptyCells(Vector3Int originCell)
    {
        List<Vector3Int> adjacentCells = new List<Vector3Int>();

        // check all adjacjent cells to the origin cell, if they exist and are empty add them to the list of the available cells
        // check cells to the left and right of starter cell
        if (originCell.x - 1 >= 0 && _roomLayout[originCell.x - 1, originCell.y, originCell.z] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x - 1, originCell.y, originCell.z));
        }
        if (originCell.x + 1 < _roomLayout.GetLength(0) && _roomLayout[originCell.x + 1, originCell.y, originCell.z] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x + 1, originCell.y, originCell.z));
        }
        // check cells ahead of and behind the starter cell
        if (originCell.z - 1 >= 0 && _roomLayout[originCell.x, originCell.y, originCell.z - 1] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x, originCell.y, originCell.z - 1));
        }
        if (originCell.z + 1 < _roomLayout.GetLength(2) && _roomLayout[originCell.x, originCell.y, originCell.z + 1] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x, originCell.y, originCell.z + 1));
        }

        return adjacentCells;
    }

    private void UpdateAvailableRoomPieces(Vector3Int cellPlaced)
    {
        // set space for variables needed up update which pieces are available
        List<Vector3Int> adjacjentCells = GetAdjacentEmptyCells(cellPlaced);
        LevelPiece placedPiece = _roomLayout[cellPlaced.x, cellPlaced.y, cellPlaced.z].GetComponent<LevelPiece>();

        // go through each piece and see if the 
        foreach(Vector3Int cell in adjacjentCells)
        {
            // a negative xDiff means this cell is left of the origin cell, positive means right
            int xDiff = cell.x - cellPlaced.x;
            // negative = below, positive = above
            int yDiff = cell.y - cellPlaced.y;
            // negative = behind, positive = ahead
            int zDiff = cell.z - cellPlaced.z;

            // adjust cell to the left
            if(xDiff < 0)
            {
                // remove all pieces that are closed off to an open west
                if(placedPiece.WestOpen)
                {
                    // make a list of pieces to remove, so piece isnt remove while in a foreach loop
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    foreach (GameObject potentialPiece in _availablePieces[cell.x, cell.y, cell.z])
                    {
                        if (!potentialPiece.GetComponent<LevelPiece>().EastOpen) piecesToRemove.Add(potentialPiece);
                    }

                    // remove the pieces 
                    foreach (GameObject removeable in piecesToRemove)
                    {
                        _availablePieces[cell.x, cell.y, cell.z].Remove(removeable);
                    }
                }
                // remove all peices that are open to a closed west
                if(!placedPiece.WestOpen)
                {
                    // make a list of pieces to remove, so piece isnt remove while in a foreach loop
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    foreach (GameObject potentialPiece in _availablePieces[cell.x, cell.y, cell.z])
                    {
                        if (potentialPiece.GetComponent<LevelPiece>().EastOpen) piecesToRemove.Add(potentialPiece);
                    }

                    // remove the pieces 
                    foreach (GameObject removeable in piecesToRemove)
                    {
                        _availablePieces[cell.x, cell.y, cell.z].Remove(removeable);
                    }
                }
            }

            //adjust cell to the right
            if(xDiff > 0)
            {
                // remove all pieces that are closed off to an open east
                if (placedPiece.EastOpen)
                {
                    // make a list of pieces to remove, so piece isnt remove while in a foreach loop
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    // find all pieces to remove
                    foreach (GameObject potentialPiece in _availablePieces[cell.x, cell.y, cell.z])
                    {
                        if (!potentialPiece.GetComponent<LevelPiece>().WestOpen) piecesToRemove.Add(potentialPiece);
                    }

                    // remove the pieces 
                    foreach (GameObject removeable in piecesToRemove)
                    {
                        _availablePieces[cell.x, cell.y, cell.z].Remove(removeable);
                    }
                }
                // remove all pieces that are open to a closed east
                if (!placedPiece.EastOpen)
                {
                    // make a list of pieces to remove, so piece isnt remove while in a foreach loop
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    foreach (GameObject potentialPiece in _availablePieces[cell.x, cell.y, cell.z])
                    {
                        if(potentialPiece.GetComponent<LevelPiece>().WestOpen) piecesToRemove.Add(potentialPiece);
                    }

                    // remove the pieces 
                    foreach (GameObject removeable in piecesToRemove)
                    {
                        _availablePieces[cell.x, cell.y, cell.z].Remove(removeable);
                    }
                }
            }

            // adjust potential pieces for cell behind
            if(zDiff < 0)
            {
                // remove all pieces that are closed to an open south
                if (placedPiece.SouthOpen)
                {
                    // make a list of pieces to remove, so piece isnt remove while in a foreach loop
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    foreach (GameObject potentialPiece in _availablePieces[cell.x, cell.y, cell.z])
                    {
                        if (!potentialPiece.GetComponent<LevelPiece>().NorthOpen) piecesToRemove.Add(potentialPiece);
                    }

                    // remove the pieces 
                    foreach (GameObject removeable in piecesToRemove)
                    {
                        _availablePieces[cell.x, cell.y, cell.z].Remove(removeable);
                    }
                }

                // remove all pieces that are open to a closed south
                if (!placedPiece.SouthOpen)
                {
                    // make a list of pieces to remove, so piece isnt remove while in a foreach loop
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    foreach (GameObject potentialPiece in _availablePieces[cell.x, cell.y, cell.z])
                    {
                        if (potentialPiece.GetComponent<LevelPiece>().NorthOpen) piecesToRemove.Add(potentialPiece);
                    }

                    // remove the pieces 
                    foreach (GameObject removeable in piecesToRemove)
                    {
                        _availablePieces[cell.x, cell.y, cell.z].Remove(removeable);
                    }
                }
            }

            // adjust potential pieces for cell ahead
            if(zDiff > 0)
            {
                // remove all pieces that are closed to an open north
                if (placedPiece.NorthOpen)
                {
                    // make a list of pieces to remove, so piece isnt remove while in a foreach loop
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    foreach (GameObject potentialPiece in _availablePieces[cell.x, cell.y, cell.z])
                    {
                        if (!potentialPiece.GetComponent<LevelPiece>().SouthOpen) piecesToRemove.Add(potentialPiece);
                    }

                    // remove the pieces 
                    foreach (GameObject removeable in piecesToRemove)
                    {
                        _availablePieces[cell.x, cell.y, cell.z].Remove(removeable);
                    }
                }

                // remove all pieces that are open to a closed north
                if (!placedPiece.NorthOpen)
                {
                    // make a list of pieces to remove, so piece isnt remove while in a foreach loop
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    foreach (GameObject potentialPiece in _availablePieces[cell.x, cell.y, cell.z])
                    {
                        if (potentialPiece.GetComponent<LevelPiece>().SouthOpen) piecesToRemove.Add(potentialPiece);
                    }

                    // remove the pieces 
                    foreach (GameObject removeable in piecesToRemove)
                    {
                        _availablePieces[cell.x, cell.y, cell.z].Remove(removeable);
                    }
                }
            }
        }
        
    }

    private Vector3Int GetNextCell(Vector3Int lastCell)
    {
        // have a place to store the information for the next cell
        Vector3Int nextCell = Vector3Int.zero;

        // make a list of available cells adjecent to the previous cell
        List<Vector3Int> availableCells = new List<Vector3Int>();
        availableCells = GetAdjacentEmptyCells(lastCell);

        // find which adjacent cell has the least amount of possible room pieces
        int lowestPieces = 100;
        foreach(Vector3Int cell in availableCells)
        {
            if (_availablePieces[cell.x, cell.y, cell.z].Count < lowestPieces && 0 < _availablePieces[cell.x,cell.y,cell.z].Count)
            {
                nextCell = cell;
                lowestPieces = _availablePieces[cell.x, cell.y, cell.z].Count;
            }
        }

        return nextCell;
    }

    /// <summary>
    /// THIS NEEDS UPDATING, RUDIMENTARY CURRENTLY
    /// </summary>
    /// <param name="startWall"></param>
    /// <returns></returns>
    private Vector3Int FindStartCell(string startWall)
    {
        Vector3Int startCell = new Vector3Int();
        startCell.y = 0;

        switch (startWall)
        {
            case "North":
                startCell.x = Random.Range(0, (int)_roomSize.x);
                startCell.z = _roomSize.z - 1;
                break;
            case "South":
                startCell.x = Random.Range(0, (int)_roomSize.x);
                startCell.z = 0;
                break;
            case "East":
                startCell.x = _roomSize.x - 1;
                startCell.z = Random.Range(0, (int)_roomSize.z);
                break;
            case "West":
                startCell.x = 0;
                startCell.z = Random.Range(0, (int)_roomSize.z);
                break;
            default:
                Debug.Log("Whoops");
                break;
        }

        // remove any available pieces for the cell that arent an entrance
        _availablePieces[startCell.x, startCell.y, startCell.z].RemoveAll(i => !i.GetComponent<LevelPiece>().IsEntrance);

        return startCell;
    }

    private bool FindIsEntrance(GameObject levelPiece)
    {
        return levelPiece.GetComponent<LevelPiece>().IsEntrance;
    }


    private void SpawnRoomPiece(Vector3Int cell)
    {
        // set the spawn location of the cell. all multiplied by 3 to account for 3x3 cell size in our grid
        // we add 1 to our z coordinate because thats the way it goes
        Vector3 spawnLocation = new Vector3(cell.x * 3, cell.y * 3, (cell.z + 1) * 3);

        // get a random piece to place based on available pieces
        int randomIndex = Random.Range(0, _availablePieces[cell.x, cell.y, cell.z].Count - 1);
        Debug.Log(randomIndex);
        Debug.Log(cell);
        Debug.Log(_availablePieces[cell.x, cell.y, cell.z].Count);

        GameObject pieceToPlace = _availablePieces[cell.x, cell.y, cell.z][randomIndex];

        // add the cell into the room layout array
        _roomLayout[cell.x, cell.y, cell.z] = pieceToPlace;

        // add the cell to the world
        Instantiate(_roomLayout[cell.x, cell.y, cell.z], spawnLocation, Quaternion.identity);

        // clear all potential pieces for the cell so it can't be generated again
        _availablePieces[cell.x, cell.y, cell.z].Clear();
    }
}
