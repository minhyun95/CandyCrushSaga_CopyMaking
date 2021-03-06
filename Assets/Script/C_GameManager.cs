using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Values;
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
    C_Value somes;

    int[] dx = { 1, -1, 0, 0 };
    int[] dy = { 0, 0, 1, -1 };
    void Start()
    {
        somes = GameObject.FindGameObjectWithTag("Header").GetComponent<C_Value>();
        m_atlas = Resources.Load<SpriteAtlas>("FruitAtlas");
        Board_Setting();

        Next_Logic_Ready = true;
        Game_Logic_State = 0;
        StartCoroutine("Game_Routine");
    }

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
                int MatchedNum = Match3_Algorithm();
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
                Debug.Log("로직1 필");
                // Fil_Empty_Space() 실행
                Fill_Empty_Space();
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
        else
        {

        }
    }

    void Board_Setting()
    {

        // 스테이지 보드 상태, Color 세팅
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (j == 0)
                {
                    Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<Image>().color = somes.BoardOffColor;
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
                    Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<Image>().color = somes.BoardOffColor;
                }
            }
        }
        // 테스트용 보드 색변경


        // Fruit Pool 생성
        for (int i = 0; i < 120; i++)
        {
            GameObject NewBlock = Instantiate(Resources.Load<GameObject>("ImBlock"),
                       transform.position, Quaternion.identity, Block_Pool_Transform);

            Candy_Init(NewBlock);
            NewBlock.SetActive(false);
            SleepBlocks.Enqueue(NewBlock);
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


    // 캔디오브젝트의 이미지를 Fruit_Type를 변경시켜주고
    // Fruit_Type에 맞게 이미지를 바꾼다.
    // 2번째 인자인 getnum값이 들어올 경우 랜덤한 값이 아니라
    // 들어온 값으로 Fruit_Type이 결정된다.
    void Candy_Init(GameObject obj, int getnum = -1)
    {
        int fruitRand = 0;
        if (getnum == -1)
            fruitRand = Random.Range((int)e_Block_Type.N_Blue, (int)(e_Block_Type.N_Green + 1));
        else
            fruitRand = getnum;

        switch (fruitRand)
        {
            case (int)e_Block_Type.N_Blue:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_fruit_face_atlas_01_4");
                break;
            case (int)e_Block_Type.N_Red:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_fruit_face_atlas_01_1");
                break;
            case (int)e_Block_Type.N_Orange:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_fruit_face_atlas_01_8");
                break;
            case (int)e_Block_Type.N_Yellow:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_fruit_face_atlas_01_7");
                break;
            case (int)e_Block_Type.N_Green:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_fruit_face_atlas_01_2");
                break;

            case (int)e_Block_Type.V_Blue:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_20");
                break;
            case (int)e_Block_Type.V_Red:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_5");
                break;
            case (int)e_Block_Type.V_Orange:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_12");
                break;
            case (int)e_Block_Type.V_Yellow:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_9");
                break;
            case (int)e_Block_Type.V_Green:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_17");
                break;
            case (int)e_Block_Type.H_Blue:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_22");
                break;
            case (int)e_Block_Type.H_Red:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_7");
                break;
            case (int)e_Block_Type.H_Orange:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_14");
                break;
            case (int)e_Block_Type.H_Yellow:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_11");
                break;
            case (int)e_Block_Type.H_Green:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("sprite_arrow_atlas_19");
                break;
            case 100:
                obj.GetComponent<Image>().sprite = m_atlas.GetSprite("star");
                break;

        }
        obj.GetComponent<C_ImBlock>().Fruit_Type = fruitRand;
    }

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
    void Breaking(int i, int j)
    {
        if(c_board.V[i].H[j].block.BoardState)
        {
            if (c_board.V[i].H[j].block.HereBlockObject != null)
            {
                if(c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().VerticalBoom)
                {
                    c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().VerticalBoom = false;
                    Debug.Log($"VerticalExplode [{i},{j}]");
                    Vertical_Explode(i,j);
                }
                else if(c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().HorizontalBoom)
                {
                    c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().HorizontalBoom = false;
                    Debug.Log($"HorizontalExplode [{i},{j}]");
                    Horizontal_Explode(i, j);
                }
                else
                {
                    Debug.Log($"브레이킹 속에서 제거[{i},{j}]");
                    SleepBlocks.Enqueue(c_board.V[i].H[j].block.HereBlockObject);
                    c_board.V[i].H[j].block.HereBlockObject.SetActive(false);
                    c_board.V[i].H[j].block.HereBlockObject = null;
                    somes.iPoint += 10;
                    somes.SetPointText();
                }
            }
        }
    }


    /*
        채우기 함수
        오른쪽 아래에서부터 9x9x9회 반복하며 실행
        9x9가 아닌 이유는 한번에 1칸씩 내려가기 때문.
        나중에 최적화 필요.
    */
    [ContextMenu("Fill")]
    public void Fill_Empty_Space()
    {
        int[] MakeCount = new int[9];

        int d = 0;
        while(d < 15)
        {
            d++;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 8; j >= 0; j--)
                {
                    XY xy = new XY();
                    // 해당 위치가 활성화 Block
                    if (c_board.V[i].H[j].block.BoardState)
                    {
                        // 현재 칸이 null이 아닐때
                        if (c_board.V[i].H[j].block.HereBlockObject != null)
                        {
                            // 현재 칸의 아래 칸이 비었고, 보드가 true일때
                            if (c_board.V[i].H[j + 1].block.HereBlockObject == null && c_board.V[i].H[j + 1].block.BoardState)
                            {
                                c_board.V[i].H[j + 1].block.HereBlockObject = c_board.V[i].H[j].block.HereBlockObject;
                                c_board.V[i].H[j].block.HereBlockObject = null;
                                xy.X = i;
                                xy.Y = j + 1;
                                c_board.V[i].H[j + 1].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(xy);
                                //Debug.Log("다음 칸 : x - " + i + ", y - " + (j+1));
                                continue;
                            }
                            // 왼쪽 하단 체크
                            if (i >= 1)
                            {
                                // 현재 칸의 왼쪽 하단 칸이 비었고, 보드가 true일때, 현재칸의 왼쪽 칸이 false일때
                                if (c_board.V[i - 1].H[j + 1].block.BoardState && c_board.V[i - 1].H[j + 1].block.HereBlockObject == null && c_board.V[i - 1].H[j].block.BoardState == false)
                                {
                                    c_board.V[i - 1].H[j + 1].block.HereBlockObject = c_board.V[i].H[j].block.HereBlockObject;
                                    c_board.V[i].H[j].block.HereBlockObject = null;
                                    xy.X = i - 1;
                                    xy.Y = j + 1;
                                    c_board.V[i - 1].H[j + 1].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(xy);
                                    continue;
                                }
                            }
                            // 오른쪽 하단 체크
                            if (i == 0)
                            {
                                // 현재 칸의 오른쪽 하단 칸이 비었고, 보드가 true일때, 현재칸의 오른쪽 칸이 false일때
                                if (c_board.V[i + 1].H[j + 1].block.BoardState && c_board.V[i + 1].H[j + 1].block.HereBlockObject == null && c_board.V[i + 1].H[j].block.BoardState == false)
                                {
                                    c_board.V[i + 1].H[j + 1].block.HereBlockObject = c_board.V[i].H[j].block.HereBlockObject;
                                    c_board.V[i].H[j].block.HereBlockObject = null;
                                    xy.X = i + 1;
                                    xy.Y = j + 1;
                                    c_board.V[i + 1].H[j + 1].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(xy);
                                    continue;
                                }
                            }
                        }
                    }
                    // 최상단일때
                    else if (j == 0)
                    {
                        if (c_board.V[i].H[j + 1].block.BoardState && c_board.V[i].H[j + 1].block.HereBlockObject == null)
                        {
                            MakeCount[i] += 1;
                            GameObject obj = SleepBlocks.Dequeue();
                            c_board.V[i].H[j + 1].block.HereBlockObject = obj;
                            obj.SetActive(true);
                            obj.transform.position = c_board.V[i].H[j].block.Block_Transform.position;
                            xy.X = i;
                            xy.Y = j + 1;
                            Candy_Init(obj);
                            obj.GetComponent<C_ImBlock>().MoveEnQueue(xy, MakeCount[i] / 10f);
                            continue;
                        }
                    }
                }
            }
        }
    }

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
        int Left = 0;
        int Top = 0;
        for (int i = 8; i >= 0; i--)
        {
            for (int j = 9; j >= 1; j--)
            {
                // 활성화된 보드만.
                if (c_board.V[i].H[j].block.BoardState && c_board.V[i].H[j].block.HereBlockObject != null)
                {
                    Left += ThreeMatch_DFS(i, j, 1, -1, c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type);
                    Top += ThreeMatch_DFS(i, j, 1, 1, c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type);
                }
            }
        }

        // isMatched 이중배열을 검사해서
        // 0 이상이면 파괴한다.
        // 파괴하면서 세로,가로줄 파괴하는 특수 폭탄인지 검사.
        for (int i = 0; i < 9; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                if(isMatched[i,j] > 0)
                {
                    //Debug.Log($"브레이킹[{i},{j}] 발동 + Matched : {isMatched[i,j]}");
                    if(isMatched[i,j] == 4)
                    {
                        Debug.Log($"4매치 [{i},{j}] 발동");
                        if (BoomType[i, j] == (int)Values.e_Boom_Number_Type.VerticalBoom)
                        {
                            c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().VerticalBoom = true;
                            Candy_Init(c_board.V[i].H[j].block.HereBlockObject, c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type + 5);
                        }
                        else if (BoomType[i, j] == (int)Values.e_Boom_Number_Type.HorizontalBoom)
                        {
                            c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().HorizontalBoom = true;
                            Candy_Init(c_board.V[i].H[j].block.HereBlockObject, c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type + 10);
                        }
                    }
                    else if (isMatched[i, j] == 5)
                    {

                    }
                    else
                    {
                        Breaking(i, j);
                    }
                }
            }
        }
        return Left + Top;
    }

    /*
        재귀함수를 돌려 3개 이상 이어져있으면 isMatched에 Stack을 넣어준다.
    
    */
    int ThreeMatch_DFS(int x, int y, int stack, int dir, int startFruit)
    {
        int num = 0;
        // 범위를 벗어나면 return 
        // 보드가 닫혀있다면 return
        if (y < 1 || x < 0 || c_board.V[x].H[y].block.BoardState == false)
            return 0;
        else if (c_board.V[x].H[y].block.HereBlockObject == null)
            return 0;
        // 다른과일이라면 return 
        else if ((c_board.V[x].H[y].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type % 5) != (startFruit % 5))
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
            if(isMatched[x, y] < stack)
                isMatched[x, y] = stack;
            if(dir == -1)
            {
                if(stack == 4)
                {
                    BoomType[x, y] = (int)Values.e_Boom_Number_Type.HorizontalBoom;
                }
                else if(stack == 5)
                {

                }
            }
            if (dir == 1)
            {
                if (stack == 4)
                {
                    BoomType[x, y] = (int)Values.e_Boom_Number_Type.VerticalBoom;
                }
                else if (stack == 5)
                {

                }
            }
            return num;
        }
        return 0;
    }


    // 가로줄 파괴 폭탄
    void Vertical_Explode(int xline, int yline)
    {
        for (int x = 0; x < 9; x++)
        {
            somes.iPoint += 100;
            Breaking(x, yline);
        }
    }

    // 세로줄 파괴 폭탄
    void Horizontal_Explode(int xline, int yline)
    {
        for (int y = 1; y < 10; y++)
        {
            somes.iPoint += 100;
            Breaking(xline, y);
        }
    }

    // 스왑기능
    public void Swap(int xLine, int yLine, int s_xLine, int s_yLine)
    {
        // 게임 로직이 2일때
        if(Game_Logic_State == 2)
        {
            GameObject tempobj;
            tempobj = c_board.V[xLine].H[yLine].block.HereBlockObject;
            c_board.V[xLine].H[yLine].block.HereBlockObject = c_board.V[s_xLine].H[s_yLine].block.HereBlockObject;
            c_board.V[s_xLine].H[s_yLine].block.HereBlockObject = tempobj;

            XY xy = new XY();
            XY s_xy = new XY();
            xy.X = xLine;
            xy.Y = yLine;
            s_xy.X = s_xLine;
            s_xy.Y = s_yLine;
            int sum = 0;
            for (int i = 8; i >= 0; i--)
            {
                for (int j = 9; j >= 1; j--)
                {
                    // 활성화된 보드만.
                    if (c_board.V[i].H[j].block.BoardState && c_board.V[i].H[j].block.HereBlockObject != null)
                    {
                        sum += ThreeMatch_DFS(i, j, 1, -1, c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type);
                        sum += ThreeMatch_DFS(i, j, 1, 1, c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type);
                    }
                }
            }
            // 바꾸기 가능
            // 애니메이션 실행
            if (sum > 0)
            {
                c_board.V[xLine].H[yLine].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(xy);
                c_board.V[s_xLine].H[s_yLine].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(s_xy);
                ChangeLogicTimer(0.1f);
                Game_Logic_State = 0;
            }
            // 바꾸기 불가능
            // 다시 되돌리기.
            else
            {
                c_board.V[xLine].H[yLine].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(xy);
                c_board.V[s_xLine].H[s_yLine].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(s_xy);
                tempobj = c_board.V[xLine].H[yLine].block.HereBlockObject;
                c_board.V[xLine].H[yLine].block.HereBlockObject = c_board.V[s_xLine].H[s_yLine].block.HereBlockObject;
                c_board.V[s_xLine].H[s_yLine].block.HereBlockObject = tempobj;

                c_board.V[xLine].H[yLine].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(xy, 0.1f);
                c_board.V[s_xLine].H[s_yLine].block.HereBlockObject.GetComponent<C_ImBlock>().MoveEnQueue(s_xy, 0.1f);
                ChangeLogicTimer(0.2f);
                Game_Logic_State = 2;
            }

            // 스왑이 끝나면 
            Next_Logic_Ready = false;
        }
        
    }
    

    public void ChangeLogicTimer(float changeNum)
    {
        fLogic_Timer = changeNum + 0.15f;
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
                    CheckLine(resultX, resultY, k, GetBlock(x,y).Fruit_Type, ref checkList);
                    if (checkList.Count >= 2) //현 블럭을 제외
                    {
                        //반짝이 처리
                        checkList.Add(GetBlock(x, y));  //현 리스트에는 탐색을 시작하는 칸이 없었기에 넣어줌
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
                if (GetBlock(x, i).Fruit_Type == iFruitType)
                    blockList.Add(GetBlock(x, i));
                else break;
            }
            for (int i = y + 1; i <= 9; ++i)
            {
                if (GetBlock(x, i).Fruit_Type == iFruitType)
                    blockList.Add(GetBlock(x, i));
                else break;
            }
        }
        else //수평 처리
        {
            for (int i = x - 1; i >= 0; --i)
            {
                if (GetBlock(i, y).Fruit_Type == iFruitType)
                    blockList.Add(GetBlock(i, y));
                else break;
            }
            for (int i = x + 1; i < 9; ++i)
            {
                if (GetBlock(i, y).Fruit_Type == iFruitType)
                    blockList.Add(GetBlock(i, y));
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
            if (GetBlock(x, y).Fruit_Type != iFruitType)
                break;
            blockList.Add(GetBlock(x, y));
            x += dx[dir];
            y += dy[dir];
        }

        return;
    }

    C_ImBlock GetBlock(int x,int y)
    {
        return c_board.V[x].H[y].block.HereBlockObject.GetComponent<C_ImBlock>();
    }
}

/*
    Unity 인스펙터 창에서 2중배열을 볼 수 없으므로
    Ver(가로) 9개, Hor(세로) 10개 가지고있는 Class 배열을 만들고
    Hor Class 내부에 Block Class를 넣었다.
    9x10의 Block이 생긴셈.
*/
[System.Serializable]
public class Board
{
    public Ver[] V = new Ver[9];
    //public bool[,] b_BoardStates = new bool[9, 10];
}

[System.Serializable]
public class Ver
{
    public Hor[] H = new Hor[10];
}

[System.Serializable]
public class Hor
{
    public Block block = new Block();
}


// 블록이 가진 여러 함수. 추후에 주석 수정 필요.
[System.Serializable]
public class Block
{
    public GameObject HereBlockObject;
    public C_ImBlock Here_ImBlock;
    public int BlockType = -1;
    public XY xy = new XY();
    public Image BlockSprite;
    public RectTransform Block_Transform;
    public bool BoardState = false;
}