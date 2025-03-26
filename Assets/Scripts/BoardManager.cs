using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject[] whitePiecePrefabs;
    public GameObject[] blackPiecePrefabs;

    public Tile whiteTile;
    public Tile blackTile;
    public Tile selectedWhiteTile;
    public Tile selectedBlackTile;
    public GameObject moveIndicatorPrefab;
    private List<GameObject> moveIndicators = new List<GameObject>();
    private Vector2Int? selectedTile = null;

    private Dictionary<Vector2Int, GameObject> piecePositions = new Dictionary<Vector2Int, GameObject>();

    private bool whiteTurn = true;
    public CapturedPieceManager capturedPieceManager;
    private (Vector2Int from, Vector2Int to, GameObject piece)? lastMove;
    public event Action<string> OnGameOver;
    public bool playing = false;
    public bool rotated = false;
    public bool promoted = false;
    public PieceChanger pieceChanger;
    private GameObject winningPiece;
    public void Init()
    {

        playing = true;
        PlacePieces();
    }

    public void ResetBoard()
    {
        foreach (var piece in piecePositions.Values)
        {
            Destroy(piece);
        }
        piecePositions.Clear();

        ResetTile(selectedTile.Value);
        selectedTile = null;
        lastMove = null;
        whiteTurn = true;
        ClearMoveIndicators();
        playing = false;
        winningPiece = null;
        capturedPieceManager.ClearGraveyard();
        if (rotated)
        {
            FlipBoard();
        }
    }


    void PlacePieces()
    {
        for (int i = 0; i < 8; i++)
        {
            CreatePiece(0, "B", i - 4, 2);
            CreatePiece(0, "W", i - 4, -3);
        }

        int[] pieceOrder = { 1, 2, 3, 4, 5, 3, 2, 1 };
        for (int i = 0; i < 8; i++)
        {
            CreatePiece(pieceOrder[i], "B", i - 4, 3);
            CreatePiece(pieceOrder[i], "W", i - 4, -4);
        }
    }

    void CreatePiece(int pieceIndex, string color, int x, int y)
    {
        Vector2Int gridPosition = new Vector2Int(x, y);
        GameObject pieceObject = Instantiate(
            color == "W" ? whitePiecePrefabs[pieceIndex] : blackPiecePrefabs[pieceIndex],
            tilemap.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0)),
            Quaternion.identity
        );

        pieceObject.transform.SetParent(this.transform, true);

        Piece piece = pieceObject.GetComponent<Piece>();
        if (piece != null)
        {
            piece.Initialize(color, x, y);
        }

        piecePositions[gridPosition] = pieceObject;
    }

    void Update()
    {
        if (winningPiece != null)
        {
            Animator winAnimation = winningPiece.GetComponent<Animator>();
            if (winAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !winAnimation.IsInTransition(0))
            {
                Piece piece = winningPiece.GetComponent<Piece>();
                OnGameOver?.Invoke(piece.color);
                ResetBoard();
            }
        }
        if (playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPosition = tilemap.WorldToCell(worldPoint);
                Vector2Int gridPosition2D = new Vector2Int(gridPosition.x, gridPosition.y);
                if (selectedTile.HasValue)
                {
                    if (piecePositions.TryGetValue(selectedTile.Value, out GameObject selectedPiece))
                    {
                        Piece piece = selectedPiece.GetComponent<Piece>();
                        List<Vector2Int> validMoves = piece.GetValidMoves(piecePositions, lastMove);

                        if (validMoves.Contains(gridPosition2D))
                        {
                            MovePiece(selectedTile.Value, gridPosition2D, selectedPiece);
                            if (piece is King && piece.castledLeft)
                            {
                                Vector2Int rookFrom = new Vector2Int(selectedTile.Value.x - 4, selectedTile.Value.y);
                                Vector2Int rookTo = new Vector2Int(selectedTile.Value.x - 1, selectedTile.Value.y);

                                if (piecePositions.TryGetValue(rookFrom, out GameObject leftRook))
                                {
                                    MovePiece(rookFrom, rookTo, leftRook);
                                }

                                piece.castledLeft = false;
                            }
                            else if (piece is King && piece.castledRight)
                            {
                                Vector2Int rookFrom = new Vector2Int(selectedTile.Value.x + 3, selectedTile.Value.y);
                                Vector2Int rookTo = new Vector2Int(selectedTile.Value.x + 1, selectedTile.Value.y);

                                if (piecePositions.TryGetValue(rookFrom, out GameObject rightRook))
                                {
                                    MovePiece(rookFrom, rookTo, rightRook);
                                }

                                piece.castledRight = false;
                            }
                            if (playing)
                            {
                                if (!promoted && winningPiece == null)
                                {
                                    FlipBoard();
                                }
                                else
                                {
                                    promoted = false;

                                }
                                whiteTurn = !whiteTurn;
                            }
                        }
                    }
                    if (playing)
                    {
                        ResetTile(selectedTile.Value);
                        ClearMoveIndicators();
                        selectedTile = null;
                    }
                }
                else if (piecePositions.TryGetValue(gridPosition2D, out GameObject pieceObject))
                {
                    Piece piece = pieceObject.GetComponent<Piece>();
                    if ((whiteTurn && piece.color == "W") || (!whiteTurn && piece.color == "B"))
                    {
                        ShowValidMoves(piece.GetValidMoves(piecePositions, lastMove));
                        bool isBlackTile = (gridPosition2D.x + gridPosition2D.y) % 2 == 0;
                        tilemap.SetTile(new Vector3Int(gridPosition2D.x, gridPosition2D.y, 0), isBlackTile ? selectedBlackTile : selectedWhiteTile);

                        selectedTile = gridPosition2D;
                    }
                }
            }
        }
    }

    void MovePiece(Vector2Int fromPosition, Vector2Int toPosition, GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();

        if (piecePositions.TryGetValue(toPosition, out GameObject capturedPiece))
        {
            capturedPieceManager.CapturePiece(capturedPiece);
            piecePositions.Remove(toPosition);
        }
        else if (piece is Pawn pawn)
        {
            int direction = pawn.color == "W" ? 1 : -1;
            if (Mathf.Abs(fromPosition.x - toPosition.x) == 1 && fromPosition.y + direction == toPosition.y)
            {
                Vector2Int enPassantCapturePosition = new Vector2Int(toPosition.x, fromPosition.y);
                if (piecePositions.TryGetValue(enPassantCapturePosition, out GameObject enPassantPiece))
                {
                    Piece capturedPawn = enPassantPiece.GetComponent<Piece>();
                    if (capturedPawn is Pawn && capturedPawn.color != pawn.color)
                    {
                        capturedPieceManager.CapturePiece(enPassantPiece);
                        piecePositions.Remove(enPassantCapturePosition);
                    }
                }
            }
        }

        piecePositions.Remove(fromPosition);

        pieceObject.transform.position = tilemap.GetCellCenterWorld(new Vector3Int(toPosition.x, toPosition.y, 0));
        piece.Move(toPosition);
        piecePositions[toPosition] = pieceObject;

        if ((piece is Pawn && piece.color == "W" && toPosition.y == 3) || (piece is Pawn && piece.color == "B" && toPosition.y == -4))
        {
            promoted = true;
            StartCoroutine(PromotePawn(pieceObject, piece, toPosition, fromPosition));
        }
        bool isBlackTile = (toPosition.x + toPosition.y) % 2 == 0;
        tilemap.SetTile(new Vector3Int(toPosition.x, toPosition.y, 0), isBlackTile ? blackTile : whiteTile);

        lastMove = (fromPosition, toPosition, pieceObject);

        IsGameOver(whiteTurn ? "W" : "B");
    }

    IEnumerator PromotePawn(GameObject pieceObject, Piece piece, Vector2Int toPosition, Vector2Int fromPosition)
    {
        pieceChanger.ShowPromotionModal(piece.color);
        yield return pieceChanger.WaitForPieceSelection();
        PromotionPiece selectedPromotion = pieceChanger.GetSelectedPromotion();

        int pieceIndex = selectedPromotion switch
        {
            PromotionPiece.Queen => 4,
            PromotionPiece.Rook => 1,
            PromotionPiece.Bishop => 3,
            PromotionPiece.Knight => 2,
            _ => 4
        };

        string pieceColor = piece.color;

        Destroy(pieceObject);
        piecePositions.Remove(fromPosition);

        GameObject newPiece = Instantiate(
            pieceColor == "W" ? whitePiecePrefabs[pieceIndex] : blackPiecePrefabs[pieceIndex],
            tilemap.GetCellCenterWorld(new Vector3Int(toPosition.x, toPosition.y, 0)),
            Quaternion.identity
        );

        newPiece.transform.SetParent(this.transform, true);
        Piece newPieceComponent = newPiece.GetComponent<Piece>();
        newPieceComponent.Initialize(pieceColor, toPosition.x, toPosition.y);

        piecePositions[toPosition] = newPiece;
        FlipBoard();
    }



    void ShowValidMoves(List<Vector2Int> validMoves)
    {
        foreach (var move in validMoves)
        {
            Vector3Int gridMove = new Vector3Int(move.x, move.y, 0);
            Vector3 worldPosition = tilemap.GetCellCenterWorld(gridMove);
            GameObject indicator = Instantiate(moveIndicatorPrefab, worldPosition, Quaternion.identity);
            moveIndicators.Add(indicator);
        }
    }

    void ClearMoveIndicators()
    {
        for (int i = moveIndicators.Count - 1; i >= 0; i--)
        {
            if (moveIndicators[i] != null)
            {
                Destroy(moveIndicators[i]);
            }
        }
        moveIndicators.Clear();
    }

    void ResetTile(Vector2Int position)
    {
        bool isBlackTile = (position.x + position.y) % 2 == 0;
        tilemap.SetTile(new Vector3Int(position.x, position.y, 0), isBlackTile ? blackTile : whiteTile);
    }

    public void IsGameOver(String potentialWinner)
    {
        foreach (var piece in piecePositions.Values)
        {
            Piece currentPiece = piece.GetComponent<Piece>();
            if (currentPiece != null && currentPiece.color == potentialWinner)
            {
                if (currentPiece.TargetingKing(piecePositions))
                {
                    foreach (var piece_ in piecePositions.Values)
                    {
                        Piece opposingPiece = piece_.GetComponent<Piece>();
                        if (opposingPiece != null && opposingPiece.color != potentialWinner)
                        {
                            List<Vector2Int> moves = opposingPiece.GetValidMoves(piecePositions, lastMove);
                            if (moves.Count > 0)
                            {
                                return;
                            }
                        }
                    }
                    foreach (var piece_ in piecePositions.Values)
                    {
                        Piece kingPiece = piece_.GetComponent<Piece>();
                        if (kingPiece is King && kingPiece.color == potentialWinner)
                        {
                            winningPiece = piece_;
                            break;
                        }
                    }
                    if (winningPiece != null)
                    {
                        Animator winAnimation = winningPiece.GetComponent<Animator>();
                        winAnimation.SetTrigger("win");
                    }
                    return;
                }
            }
        }
    }

    public void FlipBoard()
    {
        foreach (var entry in piecePositions)
        {
            GameObject pieceObject = entry.Value;

            pieceObject.transform.Rotate(0, 0, 180);
        }
        capturedPieceManager.FlipPieces();
        if (capturedPieceManager.boardAnimator != null)
        {
            capturedPieceManager.boardAnimator.SetTrigger("flipBoard");
        }
        rotated = !rotated;
    }

}
