using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Functions
{
    public class Swap_Board
    {
        C_GameManager gm = C_GameManager.Instance;
        // 스왑기능
        public void Swap(int xLine, int yLine, int s_xLine, int s_yLine)
        {
            // 게임 로직이 2일때
            if (gm.Game_Logic_State == 2)
            {
                GameObject tempobj;
                tempobj = gm.GetBlockObject(xLine, yLine);
                gm.c_board.V[xLine].H[yLine].block.HereBlockObject = gm.GetBlockObject(s_xLine, s_yLine);
                gm.c_board.V[s_xLine].H[s_yLine].block.HereBlockObject = tempobj;

                XY xy = new XY();
                XY s_xy = new XY();
                xy.X = xLine;
                xy.Y = yLine;
                s_xy.X = s_xLine;
                s_xy.Y = s_yLine;
                int Sum = 0;

                // 마우스 이동방향 오브젝트와 Swap 후
                // 3Match가능한지 검사
                // Sum값이 1 이상이면 바꾸기 가능이므로 애니메이션 실행 
                // 0 이하일 경우 불가능 하므로 갔다오는 애니메이션을 실행한 후 다시 Swap (원상태)
                Sum = gm.match_Finder.GetThreeMatch();


                // Sum > 0 바꾸기 가능
                if (Sum > 0)
                {
                    EventManager.Emit("MatchComplete");
                    gm.Get_ImBlock(xLine, yLine).MoveEnQueue(xy);
                    gm.Get_ImBlock(s_xLine, s_yLine).MoveEnQueue(s_xy);
                    gm.ChangeLogicTimer(0.1f);
                    gm.Game_Logic_State = 0;

                }

                // Sum == 0 바꾸기 불가능
                // 다시 되돌리기.
                else
                {
                    gm.Get_ImBlock(xLine, yLine).MoveEnQueue(xy);
                    gm.Get_ImBlock(s_xLine, s_yLine).MoveEnQueue(s_xy);
                    tempobj = gm.GetBlockObject(xLine, yLine);
                    gm.c_board.V[xLine].H[yLine].block.HereBlockObject = gm.GetBlockObject(s_xLine, s_yLine);
                    gm.c_board.V[s_xLine].H[s_yLine].block.HereBlockObject = tempobj;
                    gm.Get_ImBlock(xLine, yLine).MoveEnQueue(xy, 0.1f);
                    gm.Get_ImBlock(s_xLine, s_yLine).MoveEnQueue(s_xy, 0.1f);
                    gm.ChangeLogicTimer(0.2f);
                    gm.Game_Logic_State = 2;
                }

                // 스왑이 끝나면 다음 액션을 할 수 있도록 Logic Ready 값을 바꿔줌.
                // GameManager.CS 의 Game_Routine 코루틴 참조
                gm.Next_Logic_Ready = false;
            }

        }
    }
}

