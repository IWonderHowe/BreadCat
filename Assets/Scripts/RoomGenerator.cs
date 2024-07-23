using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _roomPieces = new List<GameObject>();
    [SerializeField] private Vector3 _roomSize;

    public void GenerateRoom()
    {
        GameObject[,,] roomLayout = new GameObject[(int)_roomSize.x,(int)_roomSize.y,(int)_roomSize.z];
        Vector3 startCell = FindStartCell("North");
        Debug.Log("start cell is " + startCell);


    }

    private Vector3 FindStartCell(string startWall)
    {
        Vector3 startCell = Vector3.one;

        switch (startWall)
        {
            case "North":
                startCell.x = Random.Range(1, (int)_roomSize.x + 1);
                startCell.z = 5;
                break;
            case "South":
                startCell.x = Random.Range(1, (int)_roomSize.x + 1);
                startCell.z = 1;
                break;
            case "East":
                startCell.x = 5;
                startCell.z = Random.Range(1, (int)_roomSize.z + 1);
                break;
            case "West":
                startCell.x = 1;
                startCell.z = Random.Range(1, (int)_roomSize.z + 1);
                break;
            default:
                Debug.Log("Whoops");
                break;
        }
        return startCell;
    }
}
