using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override List<Vector2Int> GetValidMoves(Dictionary<Vector2Int, GameObject> boardState, (Vector2Int from, Vector2Int to, GameObject piece)? lastMove)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int i = 0; i < 8; i++)
        {
            int newX = curX + dx[i];
            int newY = curY + dy[i];
            Vector2Int move = new Vector2Int(newX, newY);

            if (newX >= -4 && newX <= 3 && newY >= -4 && newY <= 3)
            {
                if (boardState.TryGetValue(move, out GameObject blockingPiece))
                {
                    Piece piece = blockingPiece.GetComponent<Piece>();

                    if (piece != null && piece.color != this.color)
                    {
                        if (isMoveSafe(move, boardState))
                        {
                            possibleMoves.Add(move);
                        }
                    }
                }
                else
                {
                    if (isMoveSafe(move, boardState))
                    {
                        possibleMoves.Add(move);
                    }
                }
            }
        }

        if (!moved)
        {
            Vector2Int leftRookPos = new Vector2Int(curX - 4, curY);
            Vector2Int rightRookPos = new Vector2Int(curX + 3, curY);

            if (boardState.TryGetValue(leftRookPos, out GameObject leftRookObj))
            {
                Piece leftRook = leftRookObj.GetComponent<Piece>();
                if (leftRook != null && leftRook is Rook && !leftRook.moved &&
                    isPathClear(new Vector2Int(curX, curY), leftRookPos, boardState) &&
                    isMoveSafe(new Vector2Int(curX - 2, curY), boardState) &&
                    IsCastlingSafe(new Vector2Int(curX, curY), new Vector2Int(curX - 2, curY), boardState))
                {
                    possibleMoves.Add(new Vector2Int(curX - 2, curY)); 
                    castledLeft = true;
                }
            }

            if (boardState.TryGetValue(rightRookPos, out GameObject rightRookObj))
            {
                Piece rightRook = rightRookObj.GetComponent<Piece>();
                if (rightRook != null && rightRook is Rook && !rightRook.moved &&
                    isPathClear(new Vector2Int(curX, curY), rightRookPos, boardState) &&
                    isMoveSafe(new Vector2Int(curX + 2, curY), boardState) &&
                    IsCastlingSafe(new Vector2Int(curX, curY), new Vector2Int(curX + 2, curY), boardState))
                {
                    possibleMoves.Add(new Vector2Int(curX + 2, curY));
                    castledRight = true;
                }
            }
        }

        return possibleMoves;
    }

    private bool isPathClear(Vector2Int start, Vector2Int end, Dictionary<Vector2Int, GameObject> boardState)
    {
        int direction = start.x < end.x ? 1 : -1;
        for (int x = start.x + direction; x != end.x; x += direction)
        {
            if (boardState.ContainsKey(new Vector2Int(x, start.y)))
            {
                return false; 
            }
        }
        return true;
    }

    private bool IsCastlingSafe(Vector2Int kingStart, Vector2Int kingEnd, Dictionary<Vector2Int, GameObject> boardState)
    {
        List<Vector2Int> pathSquares = new List<Vector2Int>();
        int direction = kingStart.x < kingEnd.x ? 1 : -1;

        for (int x = kingStart.x + direction; x != kingEnd.x + direction; x += direction)
        {
            pathSquares.Add(new Vector2Int(x, kingStart.y));
        }

        foreach (var pieceEntry in boardState)
        {
            GameObject pieceObj = pieceEntry.Value;
            Piece piece = pieceObj.GetComponent<Piece>();

            if (piece is not King && piece != null && piece.color != this.color)
            {
                Debug.Log("test");
                List<Vector2Int> opponentMoves = piece.GetValidMoves(boardState, null);
                foreach (Vector2Int move in opponentMoves)
                {
                    if (pathSquares.Contains(move))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }


    public override bool TargetingKing(Dictionary<Vector2Int, GameObject> boardState)
    {
        return false;
    }
}
