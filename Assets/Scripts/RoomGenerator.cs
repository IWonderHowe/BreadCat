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

    private void Awake()
    {
        // set space for the room layout that is updated with established pieces
        _roomLayout = new GameObject[_roomSize.x, _roomSize.y, _roomSize.z];

        // set space for the available room pieces in vacant spaces
        _availablePieces = new List<GameObject>[_roomSize.x, _roomSize.y, _roomSize.z];

        FillAvailableRoomPieces();
        Debug.Log(_availablePieces.GetLength(0));

        for (int x = 0; x < _roomSize.x; x++)
        {
            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int z = 0; z < _roomSize.z; z++)
                {
                    Debug.Log(_availablePieces[x, y, z].Count + ": at coordinate " + x + ", " + y + ", " + z);
                }
            }
        }
    }

    public void GenerateRoom()
    {
        int[] startCell = FindStartCell("North");
        //Debug.Log(roomLayout.GetLength(0) + " " + roomLayout.GetLength(1) + " " + roomLayout.GetLength(2));
        SpawnRoomPiece(startCell);
        GetNextCell(startCell);

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
                        if (x == 0 || x == _roomSize.x - 1 || z == 0 || z == _roomSize.z - 1)
                        {
                            if (roomPiece.GetComponent<LevelPiece>().IsEntrance || roomPiece.GetComponent<LevelPiece>().IsExit) continue;
                        }

                        _availablePieces[x, y, z].Add(roomPiece);
                    }
                }
            }
        }
    }

    private List<Vector3Int> GetAdjacentEmptyCells(int[] originCell)
    {
        List<Vector3Int> adjacentCells = new List<Vector3Int>();

        // check all adjacjent cells to the origin cell, if they exist and are empty add them to the list of the available cells
        // check cells to the left and right of starter cell
        if (originCell[0] - 1 >= 0 && _roomLayout[originCell[0] - 1, originCell[1], originCell[2]] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell[0] - 1, originCell[1], originCell[2]));
        }
        if (originCell[0] + 1 < _roomLayout.GetLength(0) && _roomLayout[originCell[0] + 1, originCell[1], originCell[2]] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell[0] + 1, originCell[1], originCell[2]));
        }
        // check cells ahead of and behind the starter cell
        if (originCell[2] - 1 >= 0 && _roomLayout[originCell[0], originCell[1], originCell[2] - 1] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell[0], originCell[1], originCell[2] - 1));
        }
        if (originCell[2] + 1 < _roomLayout.GetLength(2) && _roomLayout[originCell[0], originCell[1], originCell[2] + 1] == null)
        {
            adjacentCells.Add(new Vector3Int(originCell[0], originCell[1], originCell[2] + 1));
        }

        return adjacentCells;
    }

    private void UpdateAvailableRoomPieces(GameObject previousPlacedPiece)
    {

    }


    private int[] GetNextCell(int[] lastCell)
    {
        // have a place to store the information for the next cell
        int[] nextCell = new int[3];

        // make a list of available cells adjecent to the previous cell
        List<Vector3Int> availableCells = new List<Vector3Int>();


        
        availableCells = GetAdjacentEmptyCells(lastCell);


        foreach(Vector3Int cell in availableCells)
        {
            Debug.Log(cell.x + " " + cell.y + " " + cell.z);
        }


        return nextCell;
    }

    private int[] FindStartCell(string startWall)
    {
        int[] startCell = new int[3];
        startCell[1] = 0;

        switch (startWall)
        {
            case "North":
                startCell[0] = Random.Range(0, (int)_roomSize.x);
                startCell[2] = 4;
                break;
            case "South":
                startCell[0] = Random.Range(0, (int)_roomSize.x);
                startCell[2] = 0;
                break;
            case "East":
                startCell[0] = 4;
                startCell[2] = Random.Range(0, (int)_roomSize.z);
                break;
            case "West":
                startCell[0] = 0;
                startCell[2] = Random.Range(0, (int)_roomSize.z);
                break;
            default:
                Debug.Log("Whoops");
                break;
        }
        return startCell;
    }


    private void SpawnRoomPiece(int[] cell)
    {
        // set the spawn location of the cell. all multiplied by 3 to account for 3x3 cell size in our grid
        // we add 1 to our z coordinate because thats the way it goes
        Vector3 spawnLocation = new Vector3(cell[0] * 3, cell[1] * 3, (cell[2] + 1) * 3);

        // add the cell into the room layout array
        _roomLayout[cell[0], cell[1], cell[2]] = _roomPieces.Find(i => i.GetComponent<LevelPiece>().IsEntrance);

        // add the cell to the world
        Instantiate(_roomLayout[cell[0], cell[1], cell[2]], spawnLocation, Quaternion.identity);
    }
}
