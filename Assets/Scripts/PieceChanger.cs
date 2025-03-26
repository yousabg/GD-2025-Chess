using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum PromotionPiece
{
    Queen,
    Rook,
    Bishop,
    Knight
}

public class PieceChanger : MonoBehaviour
{
    private Button[] buttons;
    private PromotionPiece selectedPiece;
    private bool pieceSelected = false;
    public GameObject modal;
    public Sprite[] whiteSprites; // Order: Queen, Rook, Bishop, Knight
    public Sprite[] blackSprites; // Order: Queen, Rook, Bishop, Knight

    void Start()
    {
        buttons = GetComponentsInChildren<Button>();

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => OnButtonClick(btn));
        }

        modal.SetActive(false);
    }

    public void ShowPromotionModal(string color)
    {
        Sprite[] selectedSprites = (color == "W") ? whiteSprites : blackSprites;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < selectedSprites.Length)
            {
                buttons[i].image.sprite = selectedSprites[i];
            }
        }

        pieceSelected = false;
        modal.SetActive(true);
    }

    void OnButtonClick(Button btn)
    {
        if (btn.name == "Queen_Button")
        {
            selectedPiece = PromotionPiece.Queen;
        }
        else if (btn.name == "Rook_Button")
        {
            selectedPiece = PromotionPiece.Rook;
        }
        else if (btn.name == "Bishop_Button")
        {
            selectedPiece = PromotionPiece.Bishop;
        }
        else if (btn.name == "Knight_Button")
        {
            selectedPiece = PromotionPiece.Knight;
        }

        Debug.Log(btn.name + " was clicked. " + selectedPiece.ToString() + " selected.");
        pieceSelected = true;
        modal.SetActive(false);
    }

    public IEnumerator WaitForPieceSelection()
    {
        while (!pieceSelected)
        {
            yield return null;
        }
    }

    public PromotionPiece GetSelectedPromotion()
    {
        return selectedPiece;
    }
}
