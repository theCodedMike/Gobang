using UnityEngine;

public class Rule : MonoBehaviour
{
    // 四个方向：水平、垂直、主对角线、副对角线
    private static readonly int[,] Directions = {{0, 1}, {1, 0}, {1, 1}, {1, -1}};
    
    // 连子禁手
    public static bool IsConnectedForbidden(int rIdx, int cIdx)
    {
        // 只有黑棋有禁手规则
        if (Chess.ChessGrid[rIdx][cIdx].state != State.None)
            return false;
            
        // 临时放置黑棋以进行判断
        Chess.ChessGrid[rIdx][cIdx].state = State.Black;
        
        // 检查四个方向是否存在长连（超过5个连续的黑子）
        for (int d = 0; d < 4; d++)
        {
            int dr = Directions[d, 0];
            int dc = Directions[d, 1];
            
            // 统计该方向上的连续黑子数
            int count = 1; // 包含当前放置的棋子
            
            // 向正方向统计
            int r = rIdx + dr;
            int c = cIdx + dc;
            while (r >= 0 && r < Chess.ChessGrid.Length && 
                   c >= 0 && c < Chess.ChessGrid[0].Length && 
                   Chess.ChessGrid[r][c].state == State.Black)
            {
                count++;
                r += dr;
                c += dc;
            }
            
            // 向反方向统计
            r = rIdx - dr;
            c = cIdx - dc;
            while (r >= 0 && r < Chess.ChessGrid.Length && 
                   c >= 0 && c < Chess.ChessGrid[0].Length && 
                   Chess.ChessGrid[r][c].state == State.Black)
            {
                count++;
                r -= dr;
                c -= dc;
            }
            
            // 如果在任意方向上超过5个连续的黑子，则构成连子禁手
            if (count > 5)
            {
                // 恢复原状态
                Chess.ChessGrid[rIdx][cIdx].state = State.None;
                return true;
            }
        }
        
        // 恢复原状态
        Chess.ChessGrid[rIdx][cIdx].state = State.None;
        return false;
    }

    // 非连子禁手
    public static bool IsNonConnectedForbidden(int rIdx, int cIdx)
    {
        // 只有黑棋有禁手规则
        if (Chess.ChessGrid[rIdx][cIdx].state != State.None)
            return false;
            
        // 临时放置黑棋以进行判断
        Chess.ChessGrid[rIdx][cIdx].state = State.Black;

        int liveThreeCount = 0;
        int fourCount = 0;
        
        // 检查四个方向
        for (int d = 0; d < 4; d++)
        {
            int dr = Directions[d, 0];
            int dc = Directions[d, 1];
            
            // 计算该方向上连续黑子数（不包括当前棋子）
            int connectedCount = 0;
            
            // 先向正方向统计
            int r = rIdx + dr;
            int c = cIdx + dc;
            while (r >= 0 && r < Chess.ChessGrid.Length &&
                   c >= 0 && c < Chess.ChessGrid[0].Length &&
                   Chess.ChessGrid[r][c].state == State.Black)
            {
                connectedCount++;
                r += dr;
                c += dc;
            }
            
            // 再向反方向统计
            r = rIdx - dr;
            c = cIdx - dc;
            while (r >= 0 && r < Chess.ChessGrid.Length &&
                   c >= 0 && c < Chess.ChessGrid[0].Length &&
                   Chess.ChessGrid[r][c].state == State.Black)
            {
                connectedCount++;
                r -= dr;
                c -= dc;
            }
            
            // 检查两端是否为空（紧邻连续黑子的下一个位置）
            bool frontEmpty = false;
            bool backEmpty = false;
            
            // 前端检查（正方向端点的下一个位置）
            int frontR = rIdx + dr * (connectedCount + 1);
            int frontC = cIdx + dc * (connectedCount + 1);
            if (frontR >= 0 && frontR < Chess.ChessGrid.Length &&
                frontC >= 0 && frontC < Chess.ChessGrid[0].Length &&
                Chess.ChessGrid[frontR][frontC].state == State.None)
                frontEmpty = true;
            
            // 后端检查（反方向端点的下一个位置）
            int backR = rIdx - dr * (connectedCount + 1);
            int backC = cIdx - dc * (connectedCount + 1);
            if (backR >= 0 && backR < Chess.ChessGrid.Length &&
                backC >= 0 && backC < Chess.ChessGrid[0].Length &&
                Chess.ChessGrid[backR][backC].state == State.None)
                backEmpty = true;
            
            // 判断棋型
            if (connectedCount == 2 && frontEmpty && backEmpty)
            {
                // 检查是否为真正的活三（需要检查当前点是否能连接形成活三）
                if (CanFormLiveThree(rIdx, cIdx, dr, dc))
                    liveThreeCount++;
            }
            else if (connectedCount == 3 && frontEmpty && backEmpty)
            {
                // 活四
                fourCount++;
            }
            else if (connectedCount == 3)
            {
                // 检查是否为冲四
                if ((frontEmpty && !backEmpty) || (!frontEmpty && backEmpty))
                    fourCount++;
            }
        }
        
        // 恢复原状态
        Chess.ChessGrid[rIdx][cIdx].state = State.None;
    
        // 三三禁手或四四禁手
        return (liveThreeCount >= 2) || (fourCount >= 2);
    }
    
    // 辅助函数：检查是否能形成活三
    private static bool CanFormLiveThree(int rIdx, int cIdx, int dr, int dc)
    {
        // 检查当前点是否与该方向上的棋子形成活三
        // 这里简化处理，认为如果两端都为空且中间有2个相连黑子，则可形成活三
        return true;
    }
}
