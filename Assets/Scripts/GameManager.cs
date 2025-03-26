using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public GameObject canvas;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI retryText;
    void Start()
    {
        boardManager.OnGameOver += HandleGameOver;
    }

    void Update()
    {
        
    }

    public void startGame()
    {
        if (boardManager != null) {
            boardManager.Init();
        }
    }

    void HandleGameOver(string winner)
    {
        if (winner == "W") {
            mainText.SetText("Whtie has won!");
        } else {
            mainText.SetText("Black has won!");
        }
        retryText.SetText("Play again!");
        canvas.SetActive(true); 
    }
}
