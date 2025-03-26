using System;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {

    public override List<Vector2Int> GetValidMoves(Dictionary<Vector2Int, GameObject> boardState,  (Vector2Int from, Vector2Int to, GameObject piece)? lastMove) {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        int direction = (color == "W") ? 1 : -1;

        Vector2Int oneStepForward = new Vector2Int(curX, curY + direction);
        Vector2Int twoStepsForward = new Vector2Int(curX, curY + 2 * direction);
        Vector2Int leftDiagonal = new Vector2Int(curX - 1, curY + direction);
        Vector2Int rightDiagonal = new Vector2Int(curX + 1, curY + direction);

        if (!boardState.ContainsKey(oneStepForward)) {
            if (isMoveSafe(oneStepForward, boardState)) {
                possibleMoves.Add(oneStepForward);
            }
            if (!moved && !boardState.ContainsKey(twoStepsForward)) {
                if (isMoveSafe(twoStepsForward, boardState)) {
                    possibleMoves.Add(twoStepsForward);
                }
            }
        }

        if (boardState.TryGetValue(leftDiagonal, out GameObject leftPiece)) {
            Piece leftEnemy = leftPiece.GetComponent<Piece>();
            if (leftEnemy != null && leftEnemy.color != this.color) {
                if (isMoveSafe(leftDiagonal, boardState)) {
                    possibleMoves.Add(leftDiagonal);
                }
            }
        }

        if (boardState.TryGetValue(rightDiagonal, out GameObject rightPiece)) {
            Piece rightEnemy = rightPiece.GetComponent<Piece>();
            if (rightEnemy != null && rightEnemy.color != this.color) {
                if (isMoveSafe(rightDiagonal, boardState)) {
                    possibleMoves.Add(rightDiagonal);
                }
            }
        }

        if (lastMove.HasValue) {
            (Vector2Int from, Vector2Int to, GameObject piece) last = lastMove.Value;
                if (last.piece == null || !last.piece) 
                {
                    return possibleMoves;
                }
            Piece movedPiece = last.piece.GetComponent<Piece>();

            if (movedPiece is Pawn && movedPiece.color != this.color) {
                if (Math.Abs(last.from.y - last.to.y) == 2 && last.to.y == curY) {
                    if (last.to.x == curX - 1) {
                        Vector2Int enPassantLeft = new Vector2Int(curX - 1, curY + direction);
                        if (isMoveSafe(enPassantLeft, boardState)) {
                            possibleMoves.Add(enPassantLeft);
                        }
                    }
                    if (last.to.x == curX + 1) {
                        Vector2Int enPassantRight = new Vector2Int(curX + 1, curY + direction);
                        if (isMoveSafe(enPassantRight, boardState)) {
                            possibleMoves.Add(enPassantRight);
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    public override bool TargetingKing(Dictionary<Vector2Int, GameObject> boardState) {
        int direction = (color == "W") ? 1 : -1;
        Vector2Int leftDiagonal = new Vector2Int(curX - 1, curY + direction);
        Vector2Int rightDiagonal = new Vector2Int(curX + 1, curY + direction);

        if (boardState.TryGetValue(leftDiagonal, out GameObject leftPiece)) {
            Piece leftEnemy = leftPiece.GetComponent<Piece>();
            if (leftEnemy != null && leftEnemy is King && leftEnemy.color != this.color) {
                return true;
            }
        }

        if (boardState.TryGetValue(rightDiagonal, out GameObject rightPiece)) {
            Piece rightEnemy = rightPiece.GetComponent<Piece>();
            if (rightEnemy != null && rightEnemy is King && rightEnemy.color != this.color) {
                return true;
            }
        }

        return false;
    }
}
