using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _roomPieces = new List<GameObject>();
    [SerializeField] private Vector3 _roomSize;
    private GameObject[,,] _roomLayout;

    public void GenerateRoom()
    {
        _roomLayout = new GameObject[(int)_roomSize.x,(int)_roomSize.y,(int)_roomSize.z];
        int[] startCell = FindStartCell("North");
        //Debug.Log(roomLayout.GetLength(0) + " " + roomLayout.GetLength(1) + " " + roomLayout.GetLength(2));
        SpawnRoomPiece(startCell);
        GetNextCell(startCell);

    }

    private int[] GetNextCell(int[] lastCell)
    {
        // have a place to store the information for the next cell
        int[] nextCell = new int[3];

        // make a list of available cells adjecent to the previous cell
        List<Vector3Int> availableCells = new List<Vector3Int>();


        // check all adjacjent cells to the starter cell, if they exist and are empty add them to the list of the available cells
        // check cells to the left and right of starter cell
        if (lastCell[0] - 1 >= 0 && _roomLayout[lastCell[0] - 1, lastCell[1], lastCell[2]] == null)
            availableCells.Add(new Vector3Int(lastCell[0] - 1, lastCell[1], lastCell[2]));
        if (lastCell[0] + 1 < _roomLayout.GetLength(0) && _roomLayout[lastCell[0] + 1, lastCell[1], lastCell[2]] == null)
            availableCells.Add(new Vector3Int(lastCell[0] + 1, lastCell[1], lastCell[2]));
        // check cells ahead of and behind the starter cell
        if (lastCell[2] - 1 >= 0 && _roomLayout[lastCell[0], lastCell[1], lastCell[2] - 1] == null)
            availableCells.Add(new Vector3Int(lastCell[0], lastCell[1], lastCell[2] - 1));
        if (lastCell[2] + 1 < _roomLayout.GetLength(2) && _roomLayout[lastCell[0], lastCell[1], lastCell[2] + 1] == null)
            availableCells.Add(new Vector3Int(lastCell[0], lastCell[1], lastCell[2] + 1)); 

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
