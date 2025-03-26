using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece {
    public override List<Vector2Int> GetValidMoves(Dictionary<Vector2Int, GameObject> boardState,  (Vector2Int from, Vector2Int to, GameObject piece)? lastMove) {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions) {
            for (int i = 1; i <= 7; i++) {
                Vector2Int gridMove = new Vector2Int(curX + i * direction.x, curY + i * direction.y);
                if (gridMove.x < -4 || gridMove.x > 3 || gridMove.y < -4 || gridMove.y > 3)
                    break;

                if (boardState.TryGetValue(gridMove, out GameObject blockingPiece)) {
                    Piece piece = blockingPiece.GetComponent<Piece>();

                    if (piece != null && piece.color != this.color && isMoveSafe(gridMove, boardState)) {
                        possibleMoves.Add(gridMove);
                    }
                    break;
                }
                if (isMoveSafe(gridMove, boardState)) {
                    possibleMoves.Add(gridMove);
                }
            }
        }

        return possibleMoves;
    }

    public override bool TargetingKing(Dictionary<Vector2Int, GameObject> boardState) {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions) {
            for (int i = 1; i <= 7; i++) {
                Vector2Int gridMove = new Vector2Int(curX + i * direction.x, curY + i * direction.y);

                if (gridMove.x < -4 || gridMove.x > 3 || gridMove.y < -4 || gridMove.y > 3)
                    break;

                if (boardState.TryGetValue(gridMove, out GameObject blockingPiece)) {
                    Piece piece = blockingPiece.GetComponent<Piece>();

                    if (piece != null) {
                        if (piece is King && piece.color != this.color) {
                            return true;
                        }
                        break;
                    }
                }
            }
        }

        return false;
    }
}
