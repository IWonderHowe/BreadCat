using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _roomPieces = new List<GameObject>();
    [SerializeField] private Vector3 _roomSize;

    public void GenerateRoom()
    {
        GameObject[,,] roomLayout = new GameObject[(int)_roomSize.x,(int)_roomSize.y,(int)_roomSize.z];
        int[] startCell = FindStartCell("North");
        //Debug.Log(roomLayout.GetLength(0) + " " + roomLayout.GetLength(1) + " " + roomLayout.GetLength(2));


        roomLayout[startCell[0], startCell[1], startCell[2]] = _roomPieces.Find(i => i.GetComponent<LevelPiece>().IsEntrance);
        Instantiate(roomLayout[startCell[0], startCell[1], startCell[2]]);


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

    private void SpawnRoomPiece()
    {

    }
}
