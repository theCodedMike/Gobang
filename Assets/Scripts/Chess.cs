using UnityEngine;

public class Chess : MonoBehaviour
{
    public Transform topLeft;
    public Transform topRight;
    public Transform bottomLeft;
    public Transform bottomRight;

    public GameObject blackChess;
    public GameObject whiteChess;
    
    private Vector2 _posTL; // 左上角坐标
    private Vector2 _posTR; // 右上角坐标
    private Vector2 _posBL; // 左下角坐标
    private Vector2 _posBR; // 右下角坐标
    private float _tileWidth;  // 瓦片宽度
    private float _tileHeight; // 瓦片高度
    private float _borderTop;
    private float _borderRight;
    private float _borderBottom;
    private float _borderLeft;
    private readonly Vector2[,] _chessPos = new Vector2[15, 15]; // 可以放置棋子的位置

    private Camera _mainCamera;
    private Vector2 _mousePos;

    private void Start()
    {
        _posTL = topLeft.position;
        _posTR = topRight.position;
        _posBL = bottomLeft.position;
        _posBR = bottomRight.position;
        _tileWidth = (_posTR.x - _posTL.x) / 14;
        _tileHeight = (_posTR.y - _posBR.y) / 14;
        _borderTop = _posTR.y + _tileHeight / 2;
        _borderRight = _posTR.x + _tileWidth / 2;
        _borderBottom = _posBL.y  - _tileHeight / 2;
        _borderLeft = _posBL.x - _tileWidth / 2;
        
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                _chessPos[i, j] = new Vector2(_posTL.x + _tileWidth * j, _posTL.y - _tileHeight * i);
            }
        }
        
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

            CreateChess();
        }
    }


    // 生成棋子
    private void CreateChess()
    {
        if (_mousePos.y >= _borderTop || _mousePos.y <= _borderBottom || _mousePos.x >= _borderRight ||
            _mousePos.x <= _borderLeft)
        {
            print("超出边界了...");
            return;
        }
        
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                Vector2 chessPosition = _chessPos[i, j];
                if (Mathf.Abs(chessPosition.y - _mousePos.y) < _tileHeight / 2 &&
                    Mathf.Abs(chessPosition.x - _mousePos.x) < _tileWidth / 2)
                {
                    Instantiate(blackChess, chessPosition, Quaternion.identity);
                    goto outer;
                }
            }
        }
        outer:;
        
    }
}
