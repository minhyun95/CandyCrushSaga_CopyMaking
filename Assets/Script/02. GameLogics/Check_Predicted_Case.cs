using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Functions
{
    public class Check_Predicted_Case
    {
        C_GameManager gm = C_GameManager.Instance;
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        public bool CheckPredictedCase()
        {
            List<C_ImBlock> checkList = new List<C_ImBlock>();

            for (int x = 0; x < 9; ++x)
            {
                for (int y = 1; y <= 9; ++y)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        if (gm.GetBlockObject(x, y) == null)
                            continue;
                        int resultX = x + dx[k];
                        int resultY = y + dy[k];
                        // 우좌 상하 순으로 이동했을때의 경우에서 매치가 되는부분을 탐색 
                        if (resultX < 0 || resultX >= 9 || resultY <= 0 || resultY >= 10) //넘어가는지
                            continue;
                        CheckLine(resultX, resultY, k, gm.Get_ImBlock(x, y).Fruit_Type, ref checkList);
                        if (checkList.Count >= 2) //현 블럭을 제외
                        {
                            //반짝이 처리
                            checkList.Add(gm.Get_ImBlock(x, y));  //현 리스트에는 탐색을 시작하는 칸이 없었기에 넣어줌
                            foreach (C_ImBlock tBlock in checkList)  //리스트에 들어있는 블럭들의 색 변화 fill함수가 호출될때마다 색을 리셋 시켜줘야함
                            {
                                tBlock.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
                            }
                            return true;
                        }
                        else
                            checkList.Clear();
                    }
                }
            }
            return false; //가능한 경우가 없다는 의미
        }
        void CheckLine(int x, int y, int dir, int iFruitType, ref List<C_ImBlock> blockList)
        {
            // dir < 2인 부분은 좌우로 타일을 교체했을 경우, 2이상은 상하로 타일을 교체했을 경우
            //dir<2 인부분은 좌우 이동했으니 수직탐색, 2 이상은 상하 이동으로 수평탐색을 해야함, 여기서 조건이 만족된다면 리턴
            //그리고 각자 dir방향으로 지나면서 같은 종의 block계산할것

            if (dir < 2) //수직처리
            {
                for (int i = y - 1; i > 0; --i)
                {
                    // 여기 영완님에게 전달필요 혹시 전달못받고 영완님이 이거 발견하시면 아래 주석 참고해주세요!
                    // gm.GetBlockObject(x, i) != null 이거 추가한 이유
                    // 빈 (과일이 갈수없는 장소) 를 Get_ImBlock하려고하면 Object가 Null이라 NullReference에러가 나기때문에
                    // 미리 그 블록오브젝트가 null인지 체크필요합니다
                    // gm.GetBlockObject(x, i) != null 를 제거하고 Board_Setting에가서 테스트 용 보드 false를 해주시면 확인가능.
                    if (gm.GetBlockObject(x, i) != null && gm.Get_ImBlock(x, i).Fruit_Type == iFruitType)
                        blockList.Add(gm.Get_ImBlock(x, i));
                    else break;
                }
                for (int i = y + 1; i <= 9; ++i)
                {
                    if (gm.GetBlockObject(x, i) != null && gm.Get_ImBlock(x, i).Fruit_Type == iFruitType)
                        blockList.Add(gm.Get_ImBlock(x, i));
                    else break;
                }
            }
            else //수평 처리
            {
                for (int i = x - 1; i >= 0; --i)
                {
                    if (gm.GetBlockObject(i, y) != null && gm.Get_ImBlock(i, y).Fruit_Type == iFruitType)
                        blockList.Add(gm.Get_ImBlock(i, y));
                    else break;
                }
                for (int i = x + 1; i < 9; ++i)
                {
                    if (gm.GetBlockObject(i, y) != null && gm.Get_ImBlock(i, y).Fruit_Type == iFruitType)
                        blockList.Add(gm.Get_ImBlock(i, y));
                    else break;
                }
            }
            if (blockList.Count >= 2) //같은 블럭을 2개 이상 찾았다는건 3match가 가능하니 리턴
                return;

            blockList.Clear(); //다른 탐색을 위한 리셋

            x += dx[dir];
            y += dy[dir];

            //각 방향으로 이동하면서 같은 블럭체크
            while (x > 0 && x < 10 && y > 0 && y < 10)
            {
                if (gm.GetBlockObject(x, y) == null)
                    break;
                if (gm.Get_ImBlock(x, y).Fruit_Type != iFruitType)
                    break;
                blockList.Add(gm.Get_ImBlock(x, y));
                x += dx[dir];
                y += dy[dir];
            }

            return;
        }
    }
}
