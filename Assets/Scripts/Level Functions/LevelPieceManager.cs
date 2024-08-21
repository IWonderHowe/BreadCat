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

    public GameObject GetLevelPieceForCell(CellProperties properties)
    {
        // start with all level pieces
        List<GameObject> potentialPieces = _levelPieces;

        // remove pieces based on if thier properties match with the cell properties
        foreach(GameObject piece in potentialPieces)
        {
            LevelPiece pieceProperties = piece.GetComponent<LevelPiece>();

            // remove unfloored pieces if needed
            if (pieceProperties.BelowOpen != properties.BelowOpen) potentialPieces.Remove(piece);

            // if the piece type doesnt fit with the needed type, remove it
            if (pieceProperties.WallType != properties.WallType) potentialPieces.Remove(piece);
        }

        return potentialPieces[0];
    }


}
