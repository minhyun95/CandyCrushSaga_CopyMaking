using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Functions
{
    public class Swap_Board
    {
        C_GameManager gm = C_GameManager.Instance;
        C_ImBlock First = new C_ImBlock(), Second = new C_ImBlock();
        XY xy = new XY();
        XY s_xy = new XY();

        // 특수 블록 카운트
        int ColorBoomCount = 0, HorBoomCount = 0, VerBoomCount = 0, BagBoomCount = 0, SpecialBoomCount = 0;

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

                First = gm.Get_ImBlock(xLine, yLine);
                Second = gm.Get_ImBlock(s_xLine, s_yLine);
                Special_BoomCount_Check();

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

                if(SpecialBoomCount == 2 || ColorBoomCount == 1)
                {
                    EventManager.Emit("MatchComplete");
                    Special_Swap();
                    gm.Game_Logic_State = 1;
                    gm.ChangeLogicTimer(0.3f);
                }
                // Sum > 0 바꾸기 가능
                else if (Sum > 0)
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

        void Special_BoomCount_Check()
        {
            // 스왑하는것중에 특수블록이 있는지 체크
            ColorBoomCount = First.ColorBoom + Second.ColorBoom;
            HorBoomCount = First.HorizontalBoom + Second.HorizontalBoom;
            VerBoomCount = First.VerticalBoom + Second.VerticalBoom;
            BagBoomCount = First.BagBoom + Second.BagBoom;
            SpecialBoomCount = ColorBoomCount + HorBoomCount + VerBoomCount + BagBoomCount;
        }

        void Special_Swap()
        {

            // 2개 모두 컬러밤일때 다른 모든 블록을 파괴.
            if (ColorBoomCount == 2)
            {
                Debug.Log("컬러밤 2개");
                Double_ColorBoom_Explode();
            }
            else if(ColorBoomCount == 1)
            {
                if (HorBoomCount == 1)
                {
                    Debug.Log("컬러밤 1개 Hor1개 ");
                    if (First.ColorBoom == 1)
                    {
                        gm.candy_Init.OneColor_Fruit_Change_All_Special(Second.Fruit_Type, e_Boom_Number_Type.HorizontalBoom);
                        gm.breaking.Color_Explode(Second.Fruit_Type);
                    }
                    else if (Second.ColorBoom == 1)
                    {
                        gm.candy_Init.OneColor_Fruit_Change_All_Special(First.Fruit_Type, e_Boom_Number_Type.HorizontalBoom);
                        gm.breaking.Color_Explode(First.Fruit_Type);
                    }
                }
                else if(VerBoomCount == 1)
                {
                    Debug.Log("컬러밤 1개 Ver1개 ");
                    if (First.ColorBoom == 1)
                    {
                        gm.candy_Init.OneColor_Fruit_Change_All_Special(Second.Fruit_Type, e_Boom_Number_Type.VerticalBoom);
                        gm.breaking.Color_Explode(Second.Fruit_Type);
                    }
                    else if (Second.ColorBoom == 1)
                    {
                        gm.candy_Init.OneColor_Fruit_Change_All_Special(First.Fruit_Type, e_Boom_Number_Type.VerticalBoom);
                        gm.breaking.Color_Explode(First.Fruit_Type);
                    }
                }
                else if(BagBoomCount == 1)
                {
                    Debug.Log("컬러밤 1개 Bag 1개");
                    if (First.ColorBoom == 1)
                    {
                        gm.candy_Init.OneColor_Fruit_Change_All_Special(Second.Fruit_Type, e_Boom_Number_Type.BagBoom);
                        gm.breaking.Color_Explode(Second.Fruit_Type);
                    }
                    else if (Second.ColorBoom == 1)
                    {
                        gm.candy_Init.OneColor_Fruit_Change_All_Special(First.Fruit_Type, e_Boom_Number_Type.BagBoom);
                        gm.breaking.Color_Explode(First.Fruit_Type);
                    }
                }
                else
                {
                    Debug.Log("그냥 컬러밤 1개 ");
                    if (First.ColorBoom == 1)
                    {
                        gm.breaking.Break(xy.X, xy.Y);
                        gm.breaking.Color_Explode(Second.Fruit_Type);
                    }
                    else if (Second.ColorBoom == 1)
                    {
                        gm.breaking.Break(s_xy.X, s_xy.Y);
                        gm.breaking.Color_Explode(First.Fruit_Type);
                    }
                }
            }
            else if((VerBoomCount == 1 && HorBoomCount == 1) || VerBoomCount == 2 || HorBoomCount == 2)
            {
                Debug.Log("Ver Hor 2개 ");
                gm.breaking.Line_Explode(s_xy.X, s_xy.Y, (int)e_Boom_Number_Type.VerticalBoom);
                gm.breaking.Line_Explode(s_xy.X, s_xy.Y, (int)e_Boom_Number_Type.HorizontalBoom);
            }
            else if(BagBoomCount == 2)
            {
                Debug.Log("Bag밤 2개 ");
                gm.breaking.Bag_Explode(s_xy.X, s_xy.Y, 3);
            }
        }

        // 모든블록 파괴
        void Double_ColorBoom_Explode()
        {
            First.ColorBoom = 0;
            Second.ColorBoom = 0;

            // 스테이지 보석 종류 모두 파괴
            for (int i = 0; i < 5; i++)
            {
                gm.breaking.Color_Explode(i);
            }
        }
    }
}

