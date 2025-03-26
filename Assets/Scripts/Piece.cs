using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {
    public string color;
    protected int curX;
    protected int curY;
    public bool moved = false;
    public bool castledLeft = false;
    public bool castledRight = false;

    public void Initialize(string pieceColor, int startX, int startY) {
        color = pieceColor;
        curX = startX;
        curY = startY;
    }

    public abstract List<Vector2Int> GetValidMoves(Dictionary<Vector2Int, GameObject> boardStat,  (Vector2Int from, Vector2Int to, GameObject piece)? lastMove);

    public abstract bool TargetingKing(Dictionary<Vector2Int, GameObject> boardState);

    public virtual void Move(Vector2Int newPosition) {
        curX = newPosition.x;
        curY = newPosition.y;
        moved = true;
    }

    protected bool isMoveSafe(Vector2Int newPos, Dictionary<Vector2Int, GameObject> boardState) {
        var newBoardState = new Dictionary<Vector2Int, GameObject>(boardState);
        
        Vector2Int originalPosition = new Vector2Int(curX, curY);
        newBoardState.Remove(originalPosition); 
        newBoardState[newPos] = this.gameObject; 

        bool isSafe = !IsKingInCheck(newBoardState);

        return isSafe;
    }

    private bool IsKingInCheck(Dictionary<Vector2Int, GameObject> boardState) {
        foreach (var piece in boardState.Values) {
            Piece currentPiece = piece.GetComponent<Piece>();
            if (currentPiece != null && currentPiece.color != this.color) {
                if (currentPiece.TargetingKing(boardState)) {
                    return true;
                } else {
                    Debug.Log(currentPiece);
                }
            }
        }
        return false;
    }
}
