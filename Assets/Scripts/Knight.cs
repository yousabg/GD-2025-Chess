using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override List<Vector2Int> GetValidMoves(Dictionary<Vector2Int, GameObject> boardState,  (Vector2Int from, Vector2Int to, GameObject piece)? lastMove)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        int[] dx = { 1, 1, -1, -1, 2, 2, -2, -2 };
        int[] dy = { 2, -2, 2, -2, 1, -1, 1, -1 };

        for (int i = 0; i < 8; i++)
        {
            int newX = curX + dx[i];
            int newY = curY + dy[i];
            Vector2Int gridMove2D = new Vector2Int(newX, newY);

            if (newX >= -4 && newX <= 3 && newY >= -4 && newY <= 3)
            {
                if (boardState.TryGetValue(gridMove2D, out GameObject blockingPiece))
                {
                    Piece piece = blockingPiece.GetComponent<Piece>();

                    if (piece != null && piece.color != this.color)
                    {
                        if (isMoveSafe(gridMove2D, boardState))
                        {
                            possibleMoves.Add(gridMove2D);
                        }
                    }
                }
                else
                {
                    if (isMoveSafe(gridMove2D, boardState))
                    {
                        possibleMoves.Add(gridMove2D);
                    }
                }
            }
        }

        return possibleMoves;
    }

public override bool TargetingKing(Dictionary<Vector2Int, GameObject> boardState)
{
    int[] dx = { 1, 1, -1, -1, 2, 2, -2, -2 };
    int[] dy = { 2, -2, 2, -2, 1, -1, 1, -1 };

    int minX = -4, maxX = 3, minY = -4, maxY = 3;

    for (int i = 0; i < 8; i++)
    {
        int newX = curX + dx[i];
        int newY = curY + dy[i];
        Vector2Int gridMove2D = new Vector2Int(newX, newY);

        if (newX < minX || newX > maxX || newY < minY || newY > maxY)
            continue;

        if (boardState.TryGetValue(gridMove2D, out GameObject blockingPiece))
        {
            Piece piece = blockingPiece.GetComponent<Piece>();

            if (piece is King king && piece.color != this.color)
            {
                return true;
            }
        }
    }

    return false;
}

}
