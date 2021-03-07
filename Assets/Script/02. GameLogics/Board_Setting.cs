using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Functions
{
    public class Board_Setting
    {
        C_GameManager gm = C_GameManager.Instance;
        // 기본 보드 세팅
        public void setting()
        {

            // 스테이지 보드 상태, Color 세팅
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (j == 0)
                    {
                        gm.Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<Image>().color = gm.values.BoardOffColor;
                        gm.c_board.V[i].H[j].block.BoardState = false;
                    }
                    else
                    {
                        gm.c_board.V[i].H[j].block.BoardState = true;

                    }
                }
            }

            // 테스트용 보드 false
            gm.c_board.V[3].H[3].block.BoardState = false;
            gm.c_board.V[4].H[4].block.BoardState = false;
            gm.c_board.V[7].H[6].block.BoardState = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (gm.GetBoardState(i, j) == false)
                    {
                        gm.Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<Image>().color = gm.values.BoardOffColor;
                    }
                }
            }
            // 테스트용 보드 색변경


            // Fruit Pool 생성
            for (int i = 0; i < 120; i++)
            {
                GameObject NewBlock = gm.MakeObject("ImBlock");

                gm.candy_Init.Init(NewBlock);
                NewBlock.SetActive(false);
                gm.SleepBlocks.Enqueue(NewBlock);
                NewBlock.GetComponent<C_ImBlock>().SubscribeEvent(); 
                // MatchComplete 메시지에 대해서 구독, 이후 맵을 생성할때 닫혀있는 맵은 이 함수를 호출하면x, 
                //하게 되면 매치이벤트 발생시 검정색이 흰색으로 돌아올것임.
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    gm.c_board.V[i].H[j].block.Block_Transform = gm.Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<RectTransform>();
                    gm.c_board.V[i].H[j].block.xy.X = i;
                    gm.c_board.V[i].H[j].block.xy.Y = j;
                    if (gm.GetBoardState(i, j))
                    {
                        GameObject block = gm.SleepBlocks.Dequeue();
                        block.SetActive(true);
                        gm.c_board.V[i].H[j].block.HereBlockObject = block;
                        gm.c_board.V[i].H[j].block.Here_ImBlock = block.GetComponent<C_ImBlock>();
                        block.transform.position = gm.c_board.V[i].H[j].block.Block_Transform.GetComponent<RectTransform>().position;
                        block.GetComponent<C_ImBlock>().Check_Now_Position(i, j);
                    }
                }
            }
        }
    }
}
