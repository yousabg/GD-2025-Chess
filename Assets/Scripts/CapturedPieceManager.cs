using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CapturedPieceManager : MonoBehaviour
{
    public Tilemap tilemap;

    private List<GameObject> capturedWhitePieces = new List<GameObject>();
    private List<GameObject> capturedBlackPieces = new List<GameObject>();

    private bool[,] whiteGraveyard = new bool[3, 8];
    private bool[,] blackGraveyard = new bool[3, 8];

    private Vector3Int whiteGraveyardStart = new Vector3Int(-8, 3, 0);
    private Vector3Int blackGraveyardStart = new Vector3Int(5, 3, 0);
    public Animator boardAnimator;

    void Start()
    {
        boardAnimator = GetComponent<Animator>();

    }
    public void CapturePiece(GameObject capturedPiece)
    {
        Piece piece = capturedPiece.GetComponent<Piece>();
        if (piece != null)
        {
            if (piece.color == "W")
            {
                capturedWhitePieces.Add(capturedPiece);
                Vector3Int gridPosition = GetNextAvailableSpot(whiteGraveyard, whiteGraveyardStart);
                capturedPiece.transform.position = tilemap.GetCellCenterWorld(gridPosition);
                Debug.Log($"White piece captured: {capturedPiece.name} at {gridPosition}");
            }
            else
            {
                capturedBlackPieces.Add(capturedPiece);
                Vector3Int gridPosition = GetNextAvailableSpot(blackGraveyard, blackGraveyardStart);
                capturedPiece.transform.position = tilemap.GetCellCenterWorld(gridPosition);
                Debug.Log($"Black piece captured: {capturedPiece.name} at {gridPosition}");
            }
        }
    }

    private Vector3Int GetNextAvailableSpot(bool[,] graveyard, Vector3Int startPosition)
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (!graveyard[x, y])
                {
                    graveyard[x, y] = true;
                    return new Vector3Int(startPosition.x + x, startPosition.y - y, 0);
                }
            }
        }
        Debug.LogError("Graveyard full!");
        return startPosition;
    }

    public void FlipPieces()
    {
        foreach (var pieceObject in capturedWhitePieces)
        {
            pieceObject.transform.Rotate(0, 0, 180);
        }
        foreach (var pieceObject in capturedBlackPieces)
        {
            pieceObject.transform.Rotate(0, 0, 180);
        }
    }

public void ClearGraveyard()
{
    foreach (var pieceObject in capturedWhitePieces)
    {
        Destroy(pieceObject);
    }
    capturedWhitePieces.Clear();

    foreach (var pieceObject in capturedBlackPieces)
    {
        Destroy(pieceObject);
    }
    capturedBlackPieces.Clear();

    ResetGraveyard(whiteGraveyard);
    ResetGraveyard(blackGraveyard);

    Debug.Log("Graveyards cleared.");
}

private void ResetGraveyard(bool[,] graveyard)
{
    for (int x = 0; x < 3; x++)
    {
        for (int y = 0; y < 8; y++)
        {
            graveyard[x, y] = false;
        }
    }
}

}
