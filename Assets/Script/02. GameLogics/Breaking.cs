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
                    if (gm.Get_ImBlock(i,j).VerticalBoom)
                    {
                        gm.Get_ImBlock(i, j).VerticalBoom = false;
                        Debug.Log($"VerticalExplode [{i},{j}]");
                        Vertical_Explode(i, j);
                    }
                    else if (gm.Get_ImBlock(i, j).HorizontalBoom)
                    {
                        gm.Get_ImBlock(i, j).HorizontalBoom = false;
                        Debug.Log($"HorizontalExplode [{i},{j}]");
                        Horizontal_Explode(i, j);
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

        // 가로줄 파괴 폭탄
        void Vertical_Explode(int xline, int yline)
        {
            for (int x = 0; x < 9; x++)
            {
                gm.values.iPoint += 100;
                Break(x, yline);
            }
        }

        // 세로줄 파괴 폭탄
        void Horizontal_Explode(int xline, int yline)
        {
            for (int y = 1; y < 10; y++)
            {
                gm.values.iPoint += 100;
                Break(xline, y);
            }
        }
    }
}
