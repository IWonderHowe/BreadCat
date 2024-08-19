using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class RoomGenerator : MonoBehaviour
{
    // properties to define the generation
    [SerializeField] private List<GameObject> _roomPieces = new List<GameObject>();
    [SerializeField] private Vector3Int _roomSize;
    [SerializeField] private Vector2Int _courtyard;
    [SerializeField] private bool[] _openFloors = new bool[1];

    // a 3D array to store chosen peices for each filled cell
    private GameObject[,,] _roomLayout;

    // a 2D array to show which cells are part of the courtyard
    private bool[,] _courtGrid;

    // a 3d array for lists of potential pieces for each empty cell
    private List<GameObject>[,,] _availablePieces;

    // a list of all cell coordinates that are empty
    private List<Vector3Int> _openCells;

    [SerializeField] private GameObject _levelEntry;
    private GameObject _player;
    private GameObject _playerSpawn;
    
    public List<GameObject> _enemiesInLevel = new List<GameObject>();
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private LevelExit _levelExit;

    [SerializeField] List<ProBuilderMesh> _roomMeshes = new List<ProBuilderMesh>();
    private ProBuilderMesh _totalRoomMesh;
    [SerializeField] private List<ProBuilderMesh> _combinedMeshes;



    private void Start()
    {
        // instantiate the open cells list
        _openCells = new List<Vector3Int>();

        // set space for the room layout that is updated with established pieces
        _roomLayout = new GameObject[_roomSize.x, _roomSize.y, _roomSize.z];

        // set space for the available room pieces in vacant spaces
        _availablePieces = new List<GameObject>[_roomSize.x, _roomSize.y, _roomSize.z];
        
        PopluateCourtyardGrid();

        // populate the lists for open cells and potential room pieces
        FillAvailableRoomPieces();
        FillOpenCellsList();

        

    }

    public void SetPlayerObject(GameObject player)
    {
        _player = player;
    }

    private void FillOpenCellsList()
    {
        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    _openCells.Add(new Vector3Int(x, y, z));
                }
            }
        }
    }

    private void RemoveAllEntrances()
    {

        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    // make a list of pieces to remove
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    // populate that list with all potential entrances in this cell
                    foreach (GameObject piece in _availablePieces[x, y, z])
                    {
                        if (piece.GetComponent<LevelPiece>().IsEntrance) piecesToRemove.Add(piece);
                    }

                    // remove all pieces in that list fromt the potential pieces for this cell
                    foreach (GameObject piece in piecesToRemove)
                    {
                        _availablePieces[x, y, z].Remove(piece);
                    }
                }
            }
        }
    }

    private void RemoveAllExits()
    {

        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    // make a list of pieces to remove
                    List<GameObject> piecesToRemove = new List<GameObject>();

                    // populate that list with all potential exits in this cell
                    foreach (GameObject piece in _availablePieces[x, y, z])
                    {
                        if (piece.GetComponent<LevelPiece>().IsExit) piecesToRemove.Add(piece);
                    }

                    // remove all pieces in that list fromt the potential pieces for this cell
                    foreach (GameObject piece in piecesToRemove)
                    {
                        _availablePieces[x, y, z].Remove(piece);
                    }
                }
            }
        }
    }

    public void GenerateRoom()
    {

        // determine the cell with the room entrance and instantiate a piece for it
        Vector3Int startCell = FindStartCell("North");
        SpawnRoomPiece(startCell);
        UpdateAvailableRoomPieces(startCell);
        RemoveAllEntrances();

        // determine the cell for the roome exit and instantiate a piece for it
        Vector3Int exitCell = FindExitCell("North");
        SpawnRoomPiece(exitCell);
        UpdateAvailableRoomPieces(exitCell);
        RemoveAllExits();

        // create a space to store the next cell to find the piece for, and find the next piece to generate
        Vector3Int nextCell = new Vector3Int();
        nextCell = GetNextCell(startCell);

        // go through the generation for all remaining cells. -2 to get rid of already placed entrance and exit cells
        for (int x = 0; x < _roomLayout.Length - 2; x++)
        {
            SpawnRoomPiece(nextCell);
            UpdateAvailableRoomPieces(nextCell);
            nextCell = GetNextCell(nextCell);
        }

        Instantiate(_playerSpawn, new Vector3(startCell.x * 3, startCell.y * 3, startCell.z * 3), Quaternion.identity);

        _roomMeshes.Add(_totalRoomMesh);
        _combinedMeshes = CombineMeshes.Combine(_roomMeshes, _totalRoomMesh);
        _roomMeshes.Remove(_totalRoomMesh);
        foreach(ProBuilderMesh mesh in _combinedMeshes)
        {
            _roomMeshes.Remove(mesh);
        }
        foreach(ProBuilderMesh mesh in _roomMeshes)
        {
            Destroy(mesh.gameObject);
        }
        
    }

    private void FillAvailableRoomPieces()
    {
        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    bool isPartOfCourtyard = _courtGrid[x, z];

                    // instantiate a list for the given grid coordinates
                    _availablePieces[x, y, z] = new List<GameObject>();



                    // add each room piece to possible pieces for the given grid coordinates
                    foreach (GameObject roomPiece in _roomPieces)
                    {
                        // if the floor is open, dont add not open pieces
                        if(!roomPiece.GetComponent<LevelPiece>().IsOpenPiece && _openFloors[y] == true)
                        {
                            continue;
                        }

                        // dont add level piece to potential pieces if it is part of cthe courtyard and not open on all sides except ground
                        if (isPartOfCourtyard && !CheckIfOpenOnAllSides(roomPiece.GetComponent<LevelPiece>())) continue;

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

                        // add each piece to the list a number of times equal to that pieces weight
                        // MAYBE WILL BITE ME IN THE ASS //////////////////////////////////////////
                        for(int i = 0; i < roomPiece.GetComponent<LevelPiece>().Weight; i++)
                        {
                            _availablePieces[x, y, z].Add(roomPiece);
                        }
                    }
                    //Debug.Log(_availablePieces[x, y, z].Count + "pieces available at cell " + x + y + z + " is it part of the courtyard? " + isPartOfCourtyard);
                }
            }
        }
    }

    // Courtyard methods
    private bool CheckIfOpenOnAllSides(LevelPiece piece)
    {
        if (piece.WestOpen && piece.EastOpen && piece.NorthOpen && piece.SouthOpen && piece.AboveOpen) return true;
        //if (!piece.IsCourtyard) return false;
        return false;
    }

    private void PopluateCourtyardGrid()
    {
        // make a new grid to determine which cells are part of the courtyard
        _courtGrid = new bool[_roomSize.x, _roomSize.z];

        // set the start of the courtyard coordinates: note courtyard will always be centered
        int courtXStart = (_roomSize.x - _courtyard.x) / 2;
        int courtZStart = (_roomSize.z - _courtyard.y) / 2;
        
        // set the end coordinates of the courtyard
        int courtXEnd = courtXStart + _courtyard.x;
        int courtZEnd = courtZStart + _courtyard.y;

        // go through the whole grid
        for(int i = 0; i < _roomSize.x; i++)
        {
            for(int j = 0; j < _roomSize.z; j++)
            {
                // if this coordinate is within the courtyard bounds, set the gridspace to be true
                if (i >= courtXStart && i < courtXEnd && j >= courtZStart && j < courtZEnd)
                {
                    _courtGrid[i, j] = true;
                }
                // if not, set it to be false
                else
                {
                    _courtGrid[i, j] = false;
                }
            }
        }
    }


    private List<Vector3Int> GetAdjacentEmptyCells(Vector3Int originCell)
    {
        // create a new list to store the coordinates of the adjacent cells to return
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

        // check cells above and below starter cell
        if (originCell.y - 1 >= 0 && _roomLayout[originCell.x, originCell.y - 1, originCell.z] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x, originCell.y - 1, originCell.z));
        }
        if (originCell.y + 1 < _roomLayout.GetLength(1) && _roomLayout[originCell.x, originCell.y + 1, originCell.z] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x, originCell.y + 1, originCell.z));
        }

        // check cells below the cells ahead and behind of the starter cell
        if (originCell.y - 1 >= 0 && originCell.z - 1 >= 0 && _roomLayout[originCell.x, originCell.y - 1, originCell.z - 1] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x, originCell.y - 1, originCell.z - 1));
        }
        if (originCell.y - 1 >= 0 && originCell.z + 1 < _roomLayout.GetLength(2) && _roomLayout[originCell.x, originCell.y - 1, originCell.z + 1] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x, originCell.y - 1, originCell.z + 1));
        }

        // check the cells above the cells ahead and behind of the starter cell
        if (originCell.y + 1 < _roomLayout.GetLength(1) && originCell.z - 1 >= 0 && _roomLayout[originCell.x, originCell.y + 1, originCell.z - 1] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x, originCell.y + 1, originCell.z - 1));
        }
        if (originCell.y + 1 < _roomLayout.GetLength(1) && originCell.z + 1 < _roomLayout.GetLength(2) && _roomLayout[originCell.x, originCell.y + 1, originCell.z + 1] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x, originCell.y + 1, originCell.z + 1));
        }

        // check cells below the cells left and right of the starter cell
        if (originCell.y - 1 >= 0 && originCell.x - 1 >= 0 && _roomLayout[originCell.x - 1, originCell.y - 1, originCell.z] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x - 1, originCell.y - 1, originCell.z));
        }
        if (originCell.y - 1 >= 0 && originCell.x + 1 < _roomLayout.GetLength(0) && _roomLayout[originCell.x + 1, originCell.y - 1, originCell.z] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x + 1, originCell.y - 1, originCell.z));
        }

        // check the cells above the cells left and right of the starter cell
        if (originCell.y + 1 < _roomLayout.GetLength(1) && originCell.x - 1 >= 0 && _roomLayout[originCell.x - 1, originCell.y + 1, originCell.z] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x - 1, originCell.y + 1, originCell.z));
        }
        if (originCell.y + 1 < _roomLayout.GetLength(1) && originCell.x + 1 < _roomLayout.GetLength(0) && _roomLayout[originCell.x + 1, originCell.y + 1, originCell.z] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell.x + 1, originCell.y + 1, originCell.z));
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

        // find which adjacent cell has the least amount of possible room pieces
        int lowestPieces = _roomPieces.Count;
        
        // find one of the cells in the grid that has the least amount of potential pieces
        foreach(Vector3Int cell in _openCells)
        {
            // only count unique pieces in the amount of potential pieces to offset weighting
            int uniquePieces = _availablePieces[cell.x, cell.y, cell.z].GroupBy(cellPiece => cellPiece.name).Count();

            // if this cell has the lowest pieces, set it to be the next cell
            if (uniquePieces < lowestPieces && 0 < uniquePieces)
            {
                nextCell = cell;
                lowestPieces = uniquePieces;
            }
        }

        return nextCell;
    }

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

    private Vector3Int FindExitCell(string startWall)
    {
        Vector3Int exitCell = new Vector3Int();
        string exitWall = "None";

        // set the exit wall based on the start wall
        switch (startWall)
        {
            case "North":
                exitWall = "South";
                break;
            case "South":
                exitWall = "North";
                break;
            case "East":
                exitWall = "West";
                break;
            case "West":
                exitWall = "East";
                break;
            default:
                Debug.Log("oops");
                break;
        }

        switch (exitWall)
        {
            case "North":
                exitCell.x = Random.Range(0, (int)_roomSize.x);
                exitCell.z = _roomSize.z - 1;
                break;
            case "South":
                exitCell.x = Random.Range(0, (int)_roomSize.x);
                exitCell.z = 0;
                break;
            case "East":
                exitCell.x = _roomSize.x - 1;
                exitCell.z = Random.Range(0, (int)_roomSize.z);
                break;
            case "West":
                exitCell.x = 0;
                exitCell.z = Random.Range(0, (int)_roomSize.z);
                break;
            default:
                Debug.Log("Whoops");
                break;
        }

        // remove any available pieces for the cell that arent an entrance
        _availablePieces[exitCell.x, exitCell.y, exitCell.z].RemoveAll(i => !i.GetComponent<LevelPiece>().IsExit);

        return exitCell;
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

        GameObject pieceToPlace = _availablePieces[cell.x, cell.y, cell.z][randomIndex];

        // add the cell into the room layout array
        _roomLayout[cell.x, cell.y, cell.z] = pieceToPlace;


        // add the cell to the world
        GameObject placedPiece = Instantiate(_roomLayout[cell.x, cell.y, cell.z], spawnLocation, Quaternion.identity);
        if (placedPiece.GetComponent<LevelPiece>().IsExit) _levelExit = placedPiece.GetComponent<LevelExit>();


        if (Random.Range(0, 10) == 2 || _enemiesInLevel.Count == 0)
        {
            if (!placedPiece.GetComponent<LevelPiece>().CenterOccupied && !placedPiece.GetComponent<LevelPiece>().IsEntrance && !placedPiece.GetComponent<LevelPiece>().IsExit)
            {
                GameObject spawnedEnemy = Instantiate(_enemyPrefab, new Vector3(spawnLocation.x + 1.5f, spawnLocation.y + 0.75f, spawnLocation.z - 1.5f), Quaternion.identity);
                spawnedEnemy.GetComponent<Enemy>().SetRoom(this);
                _enemiesInLevel.Add(spawnedEnemy);
            }

        }

        // clear all potential pieces for the cell so it can't be generated again
        _availablePieces[cell.x, cell.y, cell.z].Clear();


        // clear cell out of open cells list
        _openCells.Remove(cell);

        // add mesh to all meshes
        ProBuilderMesh[] thisPieceMeshes = placedPiece.GetComponentsInChildren<ProBuilderMesh>();
        for(int i = 0; i < thisPieceMeshes.Length; i++)
        {
            if (thisPieceMeshes[i].name != "Floor") continue;
            if (_totalRoomMesh == null) _totalRoomMesh = thisPieceMeshes[i];
            _roomMeshes.Add(thisPieceMeshes[i]);
        }
    }

    public void RemoveFromEnemyList(GameObject enemy)
    {
        _enemiesInLevel.Remove(enemy);
        if (_enemiesInLevel.Count == 0) _levelExit.SetExitActive();
    }

}
