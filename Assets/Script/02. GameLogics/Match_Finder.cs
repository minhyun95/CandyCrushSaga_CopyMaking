using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Functions
{
    public class Match_Finder
    {
        C_GameManager gm = C_GameManager.Instance;
        /*
        3Match를 실행하는 알고리즘
        isMatched[9, 10] 과 BoomType[9, 10] 2중 배열이 2개있는데
        Breaking을 2번 실행하는걸 방지하기 위해 
        3Match가 된다면 해당 isMatched 좌표에 Stack을 넣어준다
        9x10을 돌려 isMatched가 0이상인 좌표에 있는 Fruit를 Breaking한다.
        */
        int[,] isMatched = new int[9, 10];
        int[,] BoomType = new int[9, 10];
        [ContextMenu("매치")]
        public int Match3_Algorithm()
        {
            // isMatched 이중배열 0으로 초기화
            for (int i = 0; i < 9; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    BoomType[i, j] = 0;
                    isMatched[i, j] = 0;
                }
            }
            int Sum = 0;
            Sum = GetThreeMatch();
            // isMatched 이중배열을 검사해서
            // 0 이상이면 파괴한다.
            // 파괴하면서 세로,가로줄 파괴하는 특수 폭탄인지 검사.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    if (isMatched[i, j] > 0)
                    {
                        //Debug.Log($"브레이킹[{i},{j}] 발동 + Matched : {isMatched[i,j]}");
                        if (isMatched[i, j] == 4)
                        {
                            Debug.Log($"4매치 [{i},{j}] 발동");
                            if (BoomType[i, j] == (int)Functions.e_Boom_Number_Type.VerticalBoom)
                            {
                                gm.Get_ImBlock(i, j).VerticalBoom = true;
                                gm.candy_Init.Init(gm.GetBlockObject(i, j), gm.Get_ImBlock(i, j).Fruit_Type + 5);
                            }
                            else if (BoomType[i, j] == (int)Functions.e_Boom_Number_Type.HorizontalBoom)
                            {
                                gm.Get_ImBlock(i, j).HorizontalBoom = true;
                                gm.candy_Init.Init(gm.GetBlockObject(i, j), gm.Get_ImBlock(i, j).Fruit_Type + 10);
                            }
                        }
                        else if (isMatched[i, j] == 5)
                        {

                        }
                        else
                        {
                            gm.breaking.Break(i, j);
                        }
                    }
                }
            }
            return Sum;
        }

        public int GetThreeMatch()
        {
            int Left = 0, Top = 0;
            for (int i = 8; i >= 0; i--)
            {
                for (int j = 9; j >= 1; j--)
                {
                    // 활성화된 보드만.
                    if (gm.GetBoardState(i, j) && gm.GetBlockObject(i, j) != null)
                    {
                        Left += ThreeMatch_DFS(i, j, 1, -1, gm.Get_ImBlock(i, j).Fruit_Type);
                        Top += ThreeMatch_DFS(i, j, 1, 1, gm.Get_ImBlock(i, j).Fruit_Type);
                    }
                }
            }
            return Left + Top;
        }

        /*
            재귀함수를 돌려 3개 이상 이어져있으면 isMatched에 Stack을 넣어준다.

        */
        public int ThreeMatch_DFS(int x, int y, int stack, int dir, int startFruit)
        {
            int num = 0;
            // 범위를 벗어나면 return 
            // 보드가 닫혀있다면 return
            if (y < 1 || x < 0 || gm.GetBoardState(x, y) == false)
                return 0;
            else if (gm.GetBlockObject(x, y) == null)
                return 0;
            // 다른과일이라면 return 
            else if ((gm.Get_ImBlock(x, y).Fruit_Type % 5) != (startFruit % 5))
                return 0;


            // 왼쪽 방향 체크
            else if (dir == -1)
            {
                num = (int)Mathf.Max(stack, ThreeMatch_DFS(x - 1, y, stack + 1, dir, startFruit));
            }
            // 위쪽 방향 체크
            else if (dir == 1)
            {
                num = (int)Mathf.Max(stack, ThreeMatch_DFS(x, y - 1, stack + 1, dir, startFruit));
            }
            if (num >= 3)
            {
                if (isMatched[x, y] < stack)
                    isMatched[x, y] = stack;
                if (dir == -1)
                {
                    if (stack == 4)
                    {
                        BoomType[x, y] = (int)Functions.e_Boom_Number_Type.HorizontalBoom;
                    }
                    else if (stack == 5)
                    {

                    }
                }
                if (dir == 1)
                {
                    if (stack == 4)
                    {
                        BoomType[x, y] = (int)Functions.e_Boom_Number_Type.VerticalBoom;
                    }
                    else if (stack == 5)
                    {

                    }
                }
                return num;
            }
            return 0;
        }
    }
}
    
