using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPieceManager : MonoBehaviour
{
    /// <summary>
    /// 
    ///     A manager to help select which is the right piece for a cell
    /// 
    /// </summary>

    private List<GameObject> _borderPieces;

    [SerializeField] private List<GameObject> _levelPieces;
    public List<GameObject> LevelPieces => _levelPieces;

    [SerializeField] private List<GameObject> _walledPieces;

    [SerializeField] private List<GameObject> _openPieces;

    [SerializeField] private List<GameObject> _flooredPieces;

    [SerializeField] private NewRoomGenerator _generator;

    private void Start()
    {
        _generator = GetComponent<NewRoomGenerator>();
    }

    public GameObject GetLevelPieceForCell(CellProperties cellProperties)
    {
        // make a list of potential pieces for the cell
        List<GameObject> potentialPieces = new List<GameObject>();

        // add potential pieces based on if thier properties match with the cell properties
        foreach (GameObject piece in _levelPieces)
        {
            LevelPiece pieceProperties = piece.GetComponent<LevelPiece>();

            // remove unfloored pieces if needed
            if (pieceProperties.BelowOpen != cellProperties.BelowOpen) continue;

            // if the piece type doesnt fit with the needed type, remove it
            if (pieceProperties.WallType != cellProperties.WallType) continue;

            // check to see if this level piece size fits the cell

            potentialPieces.Add(piece);
        }

        return potentialPieces[0];
    }


}
