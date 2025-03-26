using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece {
    public override List<Vector2Int> GetValidMoves(Dictionary<Vector2Int, GameObject> boardState,  (Vector2Int from, Vector2Int to, GameObject piece)? lastMove) {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        
        Vector2Int[] directions = { 
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        foreach (var direction in directions) {
            for (int i = 1; i <= 7; i++) {
                Vector2Int gridMove2D = new Vector2Int(curX + i * direction.x, curY + i * direction.y);

                if (gridMove2D.x < -4 || gridMove2D.x > 3 || gridMove2D.y < -4 || gridMove2D.y > 3)
                    break;

                if (boardState.TryGetValue(gridMove2D, out GameObject blockingPiece)) {
                    Piece piece = blockingPiece.GetComponent<Piece>();

                    if (piece != null && piece.color != this.color && isMoveSafe(gridMove2D, boardState)) {
                        possibleMoves.Add(gridMove2D);
                    }
                    break; 
                }

                if (isMoveSafe(gridMove2D, boardState)) {
                    possibleMoves.Add(gridMove2D);
                }
            }
        }

        return possibleMoves;
    }

public override bool TargetingKing(Dictionary<Vector2Int, GameObject> boardState) {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        Vector2Int[] directions = { 
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        foreach (var direction in directions) {
            for (int i = 1; i <= 7; i++) {
                Vector2Int gridMove2D = new Vector2Int(curX + i * direction.x, curY + i * direction.y);

                if (gridMove2D.x < -4 || gridMove2D.x > 3 || gridMove2D.y < -4 || gridMove2D.y > 3)
                    break;

                if (boardState.TryGetValue(gridMove2D, out GameObject blockingPiece)) {
                    Piece piece = blockingPiece.GetComponent<Piece>();

                    if (piece != null && piece.color != this.color) {
                        if (piece is King && piece.color != this.color) {
                            return true;
                        }
                    }

                    break;
                }

            }
        }

        return false;
    }
}
