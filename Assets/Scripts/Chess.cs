using UnityEngine;

public enum Turn
{
    Black, White
}

public enum State
{
    None, Black, White
}

public class ChessTile
{
    public Vector2 pos; // 位置
    public State state; // 状态
}

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
    private static readonly ChessTile[][] ChessGrid = new ChessTile[15][]; // 可以放置棋子的位置

    private Camera _mainCamera;
    private Vector2 _mousePos;

    private Turn _currTurn = Turn.Black;

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
        
        for (int i = 0; i < ChessGrid.Length; i++)
        {
            ChessGrid[i] = new ChessTile[15];
            for (int j = 0; j < ChessGrid[i].Length; j++)
            {
                ChessGrid[i][j] = new ChessTile
                {
                    pos = new Vector2(_posTL.x + _tileWidth * j, _posTL.y - _tileHeight * i),
                    state = State.None
                };
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
        
        foreach (var row in ChessGrid)
        {
            foreach (var cell in row)
            {
                Vector2 chessPosition = cell.pos;
                if (Mathf.Abs(chessPosition.y - _mousePos.y) < _tileHeight / 2 &&
                    Mathf.Abs(chessPosition.x - _mousePos.x) < _tileWidth / 2)
                {
                    // 如果这个位置已经存在棋子，则不能再放置
                    if (cell.state == State.None)
                    {
                        (GameObject chessPrefab, Turn nextTurn, State chessState) = 
                            _currTurn == Turn.Black ? (blackChess, Turn.White, State.Black) : (whiteChess, Turn.Black, State.White);
                        _currTurn = nextTurn;
                        cell.state = chessState;
                        GameObject chessObj = Instantiate(chessPrefab, transform, true);
                        chessObj.transform.position = chessPosition;   
                    }
                    goto outer;
                }
            }
        }
        outer:;
        
    }
}
