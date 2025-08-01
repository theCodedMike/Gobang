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
    public const int BoardSize = 15;
    public static readonly ChessTile[][] ChessGrid = new ChessTile[BoardSize][]; // 可以放置棋子的位置

    private Camera _mainCamera;
    private Vector2 _mousePos;

    private Turn _currTurn = Turn.Black;
    public bool isWin;
    public Turn winner; // 赢家

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
            ChessGrid[i] = new ChessTile[BoardSize];
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
        _mousePos = Vector2.zero;
        
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
        
        for (int r = 0; r < ChessGrid.Length; r++)
        {
            for (int c = 0; c < ChessGrid[r].Length; c++)
            {
                Vector2 chessPosition = ChessGrid[r][c].pos;
                if (Mathf.Abs(chessPosition.y - _mousePos.y) < _tileHeight / 2 &&
                    Mathf.Abs(chessPosition.x - _mousePos.x) < _tileWidth / 2)
                {
                    // 如果这个位置已经存在棋子，则不能再放置
                    if (ChessGrid[r][c].state == State.None)
                    {
                        // 黑棋禁手判断
                        if(_currTurn == Turn.Black && (Rule.IsConnectedForbidden(r, c) || Rule.IsNonConnectedForbidden(r, c)))
                        {
                            isWin = true;
                            winner = Turn.White;
                            goto outer;
                        }
                        
                        (GameObject chessPrefab, Turn nextTurn, State chessState) = 
                            _currTurn == Turn.Black ? (blackChess, Turn.White, State.Black) : (whiteChess, Turn.Black, State.White);
                        GameObject chessObj = Instantiate(chessPrefab, transform, true);
                        chessObj.transform.position = chessPosition;
                        ChessGrid[r][c].state = chessState;
                        
                        if (Win())
                        {
                            isWin = true;
                            winner = _currTurn;
                        }
                        
                        _currTurn = nextTurn;
                    }
                    
                    goto outer;
                }
            }
        }
        
        outer:;
    }

    // 获胜判断
    private bool Win()
    {
        State currState = _currTurn == Turn.Black ? State.Black : State.White;
        
        for (int row = 0; row < ChessGrid.Length; row++)
        {
            for (int col = 0; col < ChessGrid[row].Length; col++)
            {
                if (Horizontal(row, col, currState) || Vertical(row, col, currState) ||
                    RightSlash(row, col, currState) || LeftSlash(row, col, currState))
                    return true;
            }
        }
        
        return false;
    }

    // 横向判断
    private bool Horizontal(int i, int j, State currState)
    {
        if (j + 4 >= ChessGrid[i].Length)
            return false;
        
        if (ChessGrid[i][j].state == currState && ChessGrid[i][j + 1].state == currState &&
            ChessGrid[i][j + 2].state == currState && ChessGrid[i][j + 3].state == currState &&
            ChessGrid[i][j + 4].state == currState)
            return true;
        
        return false;
    }
    
    // 纵向判断
    private bool Vertical(int i, int j, State currState)
    {
        if (i + 4 >= ChessGrid.Length)
            return false;

        if (ChessGrid[i][j].state == currState && ChessGrid[i + 1][j].state == currState &&
            ChessGrid[i + 2][j].state == currState && ChessGrid[i + 3][j].state == currState &&
            ChessGrid[i + 4][j].state == currState)
            return true;
        
        return false;
    }
    
    // 右斜判断
    private bool RightSlash(int i, int j, State currState)
    {
        if (i + 4 >= ChessGrid.Length || j + 4 >= ChessGrid[i + 4].Length)
            return false;

        if (ChessGrid[i][j].state == currState && ChessGrid[i + 1][j + 1].state == currState &&
            ChessGrid[i + 2][j + 2].state == currState && ChessGrid[i + 3][j + 3].state == currState &&
            ChessGrid[i + 4][j + 4].state == currState)
            return true;
        
        return false;
    }
    
    // 左斜判断
    private bool LeftSlash(int i, int j, State currState)
    {
        if (i + 4 >= ChessGrid.Length || j - 4 < 0)
            return false;

        if (ChessGrid[i][j].state == currState && ChessGrid[i + 1][j - 1].state == currState &&
            ChessGrid[i + 2][j - 2].state == currState && ChessGrid[i + 3][j - 3].state == currState &&
            ChessGrid[i + 4][j - 4].state == currState)
            return true;
        
        return false;
    }
}
