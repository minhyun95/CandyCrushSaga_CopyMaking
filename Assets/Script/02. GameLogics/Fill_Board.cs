using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Functions
{
    public class Fill_Board
    {
        C_GameManager gm = C_GameManager.Instance;
        public void Fill()
        {
            /*
            채우기 함수
            오른쪽 아래에서부터 9x9x9회 반복하며 실행
            9x9가 아닌 이유는 한번에 1칸씩 내려가기 때문.
            나중에 최적화 필요.
            */
            int[] MakeCount = new int[9];

            int d = 0;
            while (d < 15)
            {
                // gm.GetBlock(i,j)
                // gm.GetBoardState(i,j)
                d++;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 8; j >= 0; j--)
                    {
                        XY xy = new XY();
                        // 해당 위치가 활성화 Block
                        if (gm.GetBoardState(i,j))
                        {
                            // 현재 칸이 null이 아닐때
                            if (gm.GetBlockObject(i,j) != null)
                            {
                                // 현재 칸의 아래 칸이 비었고, 보드가 true일때
                                if (gm.GetBoardState(i, j + 1) && gm.GetBlockObject(i, j + 1) == null)
                                {
                                    gm.PutBackObject_To_Front(i, j, i, j + 1, xy);
                                    //Debug.Log("다음 칸 : x - " + i + ", y - " + (j+1));
                                    continue;
                                }
                                // 왼쪽 하단 체크
                                if (i >= 1)
                                {
                                    // 현재 칸의 왼쪽 하단 칸이 보드가 true에 비었고 , 현재칸의 왼쪽 칸이 false일때
                                    if (gm.GetBoardState(i - 1, j + 1) && gm.GetBlockObject(i - 1, j + 1) == null && gm.GetBoardState(i - 1, j) == false)
                                    {
                                        gm.PutBackObject_To_Front(i, j, i - 1, j + 1, xy);
                                        continue;
                                    }
                                }
                                // 오른쪽 하단 체크
                                else if (i < 8)
                                {
                                    // 현재 칸의 오른쪽 하단 칸이 비었고, 보드가 true일때, 현재칸의 오른쪽 칸이 false일때
                                    if (gm.GetBoardState(i + 1, j + 1) && gm.GetBlockObject(i + 1, j + 1) == null && gm.GetBoardState(i + 1, j) == false)
                                    {
                                        gm.PutBackObject_To_Front(i, j, i + 1, j + 1, xy);
                                        continue;
                                    }
                                }
                            }
                        }
                        // 최상단일때
                        else if (j == 0)
                        {
                            if (gm.GetBoardState(i, j + 1) && gm.GetBlockObject(i, j + 1) == null)
                            {
                                MakeCount[i] += 1;
                                GameObject obj = gm.SleepBlocks.Dequeue();
                                gm.c_board.V[i].H[j + 1].block.HereBlockObject = obj;
                                obj.SetActive(true);
                                obj.transform.position = gm.c_board.V[i].H[j].block.Block_Transform.position;
                                xy.X = i;
                                xy.Y = j + 1;
                                gm.candy_Init.Init(obj);
                                obj.GetComponent<C_ImBlock>().MoveEnQueue(xy, MakeCount[i] / 10f);
                                continue;
                            }
                        }
                    }
                }
            }
        }

    }
}
