using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private LevelPieceManager _pieceList;
    
    // a list to contain all possible level pieces for each cell
    private List<GameObject>[,,] _availablePieces;

    
    


    
}
