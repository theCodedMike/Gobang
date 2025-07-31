using UnityEngine;

public class UI : MonoBehaviour
{
    public Texture2D blackWin;
    public Texture2D whiteWin;

    private Chess _chess;

    private void Start()
    {
        _chess = GetComponent<Chess>();
    }

    private void OnGUI()
    {
        if (_chess.isWin)
        {
            Texture2D winnerTexture = _chess.winner == Turn.Black ? blackWin : whiteWin;
            GUI.DrawTexture(
                new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.5f), 
                winnerTexture);
            _chess.enabled = false;
        }
    }
}
