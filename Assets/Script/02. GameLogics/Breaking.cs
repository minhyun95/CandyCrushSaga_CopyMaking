using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Functions
{
    public class Breaking
    {
        C_GameManager gm = C_GameManager.Instance;

        /*
            캔디가 파괴할때 실행되는 함수
            캔디크러쉬사가에서는 
            4개가 가로, 세로 직선으로있을때
            가로일때 - 세로줄 파괴 캔디 1개 생성
            세로일때 - 가로줄 파괴 캔디 1개 생성
            5개가 직선으로 있을때
            ColorBoom이 생성되고 ColorBoom을 움직이면
            ColorBoom과 바꾼 Fruit을 보드에서 모두 파괴한다. (가로 세로일 경우 효과 발동됨)
         */
        public void Break(int i, int j)
        {
            if (gm.GetBoardState(i, j))
            {
                if (gm.GetBlockObject(i, j) != null)
                {
                    if (gm.Get_ImBlock(i,j).VerticalBoom == 1)
                    {
                        gm.Get_ImBlock(i, j).VerticalBoom = 0;
                        Debug.Log($"VerticalExplode [{i},{j}]");
                        Line_Explode(i, j, (int)e_Boom_Number_Type.VerticalBoom);
                    }
                    else if (gm.Get_ImBlock(i, j).HorizontalBoom == 1)
                    {
                        gm.Get_ImBlock(i, j).HorizontalBoom = 0;
                        Debug.Log($"HorizontalExplode [{i},{j}]");
                        Line_Explode(i, j, (int)e_Boom_Number_Type.HorizontalBoom);
                    }
                    else if(gm.Get_ImBlock(i,j).BagBoom == 1)
                    {
                        gm.Get_ImBlock(i, j).BagBoom = 0;
                        Bag_Explode(i, j, 1);
                    }
                    else
                    {
                        gm.SleepBlocks.Enqueue(gm.GetBlockObject(i, j));
                        gm.GetBlockObject(i, j).SetActive(false);
                        gm.c_board.V[i].H[j].block.HereBlockObject = null;
                        gm.values.iPoint += 10;
                        gm.values.SetPointText();
                    }
                }
            }
        }

        // 라인 파괴 폭탄
        public void Line_Explode(int xline, int yline, int WhatLine)
        {
            if(WhatLine == (int)e_Boom_Number_Type.VerticalBoom)
            {
                for (int x = 0; x < 9; x++)
                {
                    gm.values.iPoint += 100;
                    Break(x, yline);
                }
            }
            else if (WhatLine == (int)e_Boom_Number_Type.HorizontalBoom)
            {
                for (int y = 1; y < 10; y++)
                {
                    gm.values.iPoint += 100;
                    Break(xline, y);
                }
            }
        }

        // 8방향 파괴폭탄 (side_explode_range를 조정하면 범위를 늘릴 수 있다.)
        public void Bag_Explode(int xline, int yline, int side_explode_range = 1)
        {

            for (int i = 0; i < side_explode_range * 2 + 1; i++)
            {
                for (int j = 0; j < side_explode_range * 2 + 1; j++)
                {
                    Debug.Log("8방향 폭탄");
                    if (xline - side_explode_range + i < 0 || xline - side_explode_range + i > 8 || yline - side_explode_range + j < 1 || yline - side_explode_range + j > 9)
                        continue;
                    Debug.Log($"8방향 폭탄 터짐 [{xline - side_explode_range + i}][{yline - side_explode_range + j}]");
                    Break(xline - side_explode_range + i, yline - side_explode_range + j);
                }
            }
        }

        // 한가지 색 파괴 폭탄.
        public void Color_Explode(int get_fruit_Type)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (gm.GetBlockObject(x, y) != null && gm.Get_ImBlock(x, y).Fruit_Type % 5 == get_fruit_Type % 5)
                    {
                        Break(x, y);
                        gm.values.iPoint += 80;
                    }
                }
            }
        }
        

    }
}
