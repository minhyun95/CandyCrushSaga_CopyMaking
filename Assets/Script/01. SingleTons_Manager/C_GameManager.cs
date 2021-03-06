using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Functions;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;
using singleton;
public class C_GameManager : Singleton<C_GameManager>
{
    public SpriteAtlas m_atlas;

    // 블록들
    [Header("블록")]
    public Transform Block_Pool_Transform;

    // 파괴될때 SleepBlocks에 넣고 필요할때 새로 꺼냄.
    public Queue<GameObject> SleepBlocks = new Queue<GameObject>();

    // 0 : 3Match , 1 : Fill , 2 : Player Swap
    public int Game_Logic_State = 0;
    public bool Next_Logic_Ready = false;

    // Fill_Empty_Space()를 실행할때 FillTimer를 0.1f
    // FillTimer가 0이하라면 3Match 실행 (1회) 
    // 3Match가 된다면 : Fill재실행
    // 3Match가 안된다면 : 유저의 Swap 기다림
    public float fLogic_Timer = -1;

    // 보드
    [Header("보드")]
    public Transform Board_Pool_Transform;
    public Board c_board = new Board();

    // Some Value
    public C_Value values;
    public Candy_Init candy_Init;
    public Fill_Board fill_Board;
    public Breaking breaking;
    public Match_Finder match_Finder;
    public Swap_Board swap_Board;
    int[] dx = { 1, -1, 0, 0 };
    int[] dy = { 0, 0, 1, -1 };

    private void Awake()
    {
        candy_Init = new Functions.Candy_Init();
        fill_Board = new Functions.Fill_Board();
        breaking = new Functions.Breaking();
        match_Finder = new Functions.Match_Finder();
        values = new Functions.C_Value();
        swap_Board = new Functions.Swap_Board();
    }
    void Start()
    {
        m_atlas = Resources.Load<SpriteAtlas>("FruitAtlas");
        Board_Setting();

        Next_Logic_Ready = true;
        Game_Logic_State = 0;
        StartCoroutine("Game_Routine");
    }

    // 게임 동작 루틴
    IEnumerator Game_Routine()
    {
        Next_Logic_Ready = true;
        // 무한 루프 게임플레이
        // 로직 변수
        // 0 : 3Match , 1 : Fill , 2 : Player Swap
        yield return new WaitUntil(() => fLogic_Timer < 0);
        if (Next_Logic_Ready)
        {
            if (Game_Logic_State == 0)
            {
                Debug.Log("로직0 매치");
                // 3Match 실행
                int MatchedNum = match_Finder.Match3_Algorithm();
                if(MatchedNum > 0)
                {
                    // Fill 실행
                    Game_Logic_State = 1;
                }
                else
                {
                    // 스왑으로
                    Game_Logic_State = 2;
                    CheckPredictedCase();
                }
                Next_Logic_Ready = false;
            }
            else if (Game_Logic_State == 1)
            {
                Debug.Log("로직1 채우기");
                fill_Board.Fill();
                Next_Logic_Ready = false;
                Game_Logic_State = 0;

            }
            else if (Game_Logic_State == 2)
            {           
                Debug.Log("로직2 스왑");
                // 스왑 가능
            }
        }
        // 다음 로직이 true가 된다면 새로 실행
        Debug.Log("기다림");
        yield return new WaitUntil(() => Next_Logic_Ready == false);
        Debug.Log("다음로직 실행");
        StartCoroutine("Game_Routine");
    }

    void Update()
    {
        if(fLogic_Timer >= 0)
            fLogic_Timer -= Time.deltaTime * Time.timeScale;
    }

    // 기본 보드 세팅
    void Board_Setting()
    {

        // 스테이지 보드 상태, Color 세팅
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (j == 0)
                {
                    Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<Image>().color = values.BoardOffColor;
                    c_board.V[i].H[j].block.BoardState = false;
                }
                else
                {
                    c_board.V[i].H[j].block.BoardState = true;
                    
                }
            }
        }

        // 테스트용 보드 false
        c_board.V[4].H[4].block.BoardState = true;
        c_board.V[1].H[5].block.BoardState = true;
        c_board.V[7].H[2].block.BoardState = true;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (c_board.V[i].H[j].block.BoardState == false)
                {
                    Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<Image>().color = values.BoardOffColor;
                }
            }
        }
        // 테스트용 보드 색변경


        // Fruit Pool 생성
        for (int i = 0; i < 120; i++)
        {
            GameObject NewBlock = Instantiate(Resources.Load<GameObject>("ImBlock"),
                       transform.position, Quaternion.identity, Block_Pool_Transform);

            candy_Init.Init(NewBlock);
            NewBlock.SetActive(false);
            SleepBlocks.Enqueue(NewBlock);
            NewBlock.GetComponent<C_ImBlock>().SubscribeEvent(); // MatchComplete 메시지에 대해서 구독, 이후 맵을 생성할때 닫혀있는 맵은 이 함수를 호출하면x, 
            //하게 되면 매치이벤트 발생시 검정색이 흰색으로 돌아올것임.
        }
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                c_board.V[i].H[j].block.Block_Transform = Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<RectTransform>();
                c_board.V[i].H[j].block.xy.X = i;
                c_board.V[i].H[j].block.xy.Y = j;
                if (c_board.V[i].H[j].block.BoardState)
                {
                    GameObject block = SleepBlocks.Dequeue();
                    block.SetActive(true);
                    c_board.V[i].H[j].block.HereBlockObject = block;
                    c_board.V[i].H[j].block.Here_ImBlock = block.GetComponent<C_ImBlock>();
                    block.transform.position = c_board.V[i].H[j].block.Block_Transform.GetComponent<RectTransform>().position;
                    block.GetComponent<C_ImBlock>().Check_Now_Position(i, j);
                }
            }
        }
    }
    
    public bool CheckPredictedCase()
    {
        
        List <C_ImBlock> checkList = new List<C_ImBlock>();

        for(int x = 0; x<9; ++x)
        {
            for(int y =1; y<= 9; ++y)
            {
                for(int k = 0; k<4; ++k)
                {
                    int resultX = x + dx[k];
                    int resultY = y + dy[k];
                    // 우좌 상하 순으로 이동했을때의 경우에서 매치가 되는부분을 탐색 
                    if (resultX < 0 || resultX >= 9 || resultY <= 0 || resultY >= 10) //넘어가는지
                        continue;
                    CheckLine(resultX, resultY, k, Get_ImBlock(x,y).Fruit_Type, ref checkList);
                    if (checkList.Count >= 2) //현 블럭을 제외
                    {
                        //반짝이 처리
                        checkList.Add(Get_ImBlock(x, y));  //현 리스트에는 탐색을 시작하는 칸이 없었기에 넣어줌
                        foreach(C_ImBlock tBlock in checkList)  //리스트에 들어있는 블럭들의 색 변화 fill함수가 호출될때마다 색을 리셋 시켜줘야함
                        {
                            tBlock.GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f,0.3f);
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
    void CheckLine(int x,int y,int dir,int iFruitType, ref List<C_ImBlock> blockList)
    {
        // dir < 2인 부분은 좌우로 타일을 교체했을 경우, 2이상은 상하로 타일을 교체했을 경우
        //dir<2 인부분은 좌우 이동했으니 수직탐색, 2 이상은 상하 이동으로 수평탐색을 해야함, 여기서 조건이 만족된다면 리턴
        //그리고 각자 dir방향으로 지나면서 같은 종의 block계산할것

        if (dir < 2) //수직처리
        {
            for(int i = y - 1; i > 0; --i)
            {
                if (Get_ImBlock(x, i).Fruit_Type == iFruitType)
                    blockList.Add(Get_ImBlock(x, i));
                else break;
            }
            for (int i = y + 1; i <= 9; ++i)
            {
                if (Get_ImBlock(x, i).Fruit_Type == iFruitType)
                    blockList.Add(Get_ImBlock(x, i));
                else break;
            }
        }
        else //수평 처리
        {
            for (int i = x - 1; i >= 0; --i)
            {
                if (Get_ImBlock(i, y).Fruit_Type == iFruitType)
                    blockList.Add(Get_ImBlock(i, y));
                else break;
            }
            for (int i = x + 1; i < 9; ++i)
            {
                if (Get_ImBlock(i, y).Fruit_Type == iFruitType)
                    blockList.Add(Get_ImBlock(i, y));
                else break;
            }
        }
        if(blockList.Count >= 2) //같은 블럭을 2개 이상 찾았다는건 3match가 가능하니 리턴
            return;

        blockList.Clear(); //다른 탐색을 위한 리셋

        x += dx[dir];
        y += dy[dir];

        //각 방향으로 이동하면서 같은 블럭체크
        while (x > 0 && x < 10 && y > 0 && y < 10)
        {
            if (Get_ImBlock(x, y).Fruit_Type != iFruitType)
                break;
            blockList.Add(Get_ImBlock(x, y));
            x += dx[dir];
            y += dy[dir];
        }

        return;
    }

    // 애니메이션 시간 + 0.15초(여유시간) 만큼 대기 후 다음 동작 실행
    public void ChangeLogicTimer(float changeNum)
    {
        fLogic_Timer = changeNum + 0.15f;
    }


    // x y 좌표의 C_ImBlock 가져오기.
    public C_ImBlock Get_ImBlock(int x,int y)
    {
        return c_board.V[x].H[y].block.HereBlockObject.GetComponent<C_ImBlock>();
    }

    // x y 좌표의 BoardState 가져오기
    public bool GetBoardState(int x,int y)
    {
        return c_board.V[x].H[y].block.BoardState;
    }

    // x y 좌표의 Object 가져오기.
    public GameObject GetBlockObject(int x, int y)
    {
        return c_board.V[x].H[y].block.HereBlockObject;
    }

    // 1. sx sy 오브젝트에 x y 오브젝트 대입 후
    // 2. x y 오브젝트 null로 만들어주고
    // 3. 이동한 오브젝트의 UI를 이동시켜주기위해 MoveEnQueue
    public void PutBackObject_To_Front(int x, int y, int sx, int sy, XY _xy)
    {
        c_board.V[sx].H[sy].block.HereBlockObject = c_board.V[x].H[y].block.HereBlockObject;
        c_board.V[x].H[y].block.HereBlockObject = null;
        _xy.X = sx;
        _xy.Y = sy;
        Get_ImBlock(sx, sy).MoveEnQueue(_xy);
    }
}
