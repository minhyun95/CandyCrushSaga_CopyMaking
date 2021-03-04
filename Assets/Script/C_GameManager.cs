using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Values;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;

public class C_GameManager : MonoBehaviour
{
    public SpriteAtlas m_atlas;

    // 블록들
    [Header("블록")]
    public Transform Block_Pool_Transform;

    // 파괴될때 SleepBlocks에 넣고 필요할때 새로 꺼냄.
    public Queue<GameObject> SleepBlocks = new Queue<GameObject>();


    // 보드
    [Header("보드")]
    public Transform Board_Pool_Transform;
    public Board c_board = new Board();

    // Some Value
    C_Value somes;
    void Start()
    {
        somes = GameObject.FindGameObjectWithTag("Header").GetComponent<C_Value>();
        m_atlas = Resources.Load<SpriteAtlas>("FruitAtlas");
        Board_Setting();

        //Invoke("Match3_Algorithm", 2);
        //Invoke("Fill_Empty_Space", 4);
        //Invoke("Match3_Algorithm", 6);
        //Invoke("Fill_Empty_Space", 8);
        //Invoke("Match3_Algorithm", 10);
        //Invoke("Fill_Empty_Space", 12);
        //Invoke("Match3_Algorithm", 14);
        //Invoke("Fill_Empty_Space", 16);
        //Breaking(5, 5);
        //Breaking(5, 6);
        //Breaking(5, 7);

        //Breaking(2, 6);
        //Breaking(2, 7);
        //Breaking(2, 8);
        //Breaking(2, 9);

        //Breaking(1, 4);
        //Breaking(1, 7);
        //Breaking(1, 8);
        //Breaking(1, 9);
        //Fill_Empty_Space();

    }

    void Update()
    {
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
                    c_board.b_BoardStates[i, j] = false;
                    Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<Image>().color = somes.BoardOffColor;
                }
                else
                    c_board.b_BoardStates[i, j] = true;
            }
        }

        // 테스트용 보드 false
        c_board.b_BoardStates[4, 4] = false;
        c_board.b_BoardStates[1, 5] = false;
        c_board.b_BoardStates[7, 2] = false;

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if(c_board.b_BoardStates[i,j] == false)
                    Board_Pool_Transform.GetChild(i * 10 + j).gameObject.GetComponent<Image>().color = somes.BoardOffColor;

            }
        }


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
                if (c_board.b_BoardStates[i,j])
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


    void Breaking(int i, int j)
    {
        if(c_board.b_BoardStates[i,j])
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

    [ContextMenu("Fill")]
    public void Fill_Empty_Space()
    {
        int[] MakeCount = new int[9];

        int d = 0;
        while(d < 10)
        {
            d++;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 8; j >= 0; j--)
                {
                    XY xy = new XY();
                    // 해당 위치가 활성화 Block
                    if (c_board.b_BoardStates[i, j])
                    {
                        // 현재 칸이 null이 아닐때
                        if (c_board.V[i].H[j].block.HereBlockObject != null)
                        {
                            // 현재 칸의 아래 칸이 비었고, 보드가 true일때
                            if (c_board.V[i].H[j + 1].block.HereBlockObject == null && c_board.b_BoardStates[i, j + 1])
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
                                if (c_board.b_BoardStates[i - 1, j + 1] && c_board.V[i - 1].H[j + 1].block.HereBlockObject == null && c_board.b_BoardStates[i - 1, j] == false)
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
                                if (c_board.b_BoardStates[i + 1, j + 1] && c_board.V[i + 1].H[j + 1].block.HereBlockObject == null && c_board.b_BoardStates[i + 1, j] == false)
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
                        if (c_board.b_BoardStates[i, j + 1] && c_board.V[i].H[j + 1].block.HereBlockObject == null)
                        {
                            MakeCount[i] += 1;
                            GameObject obj = SleepBlocks.Dequeue();
                            c_board.V[i].H[j + 1].block.HereBlockObject = obj;
                            obj.SetActive(true);
                            obj.transform.position = c_board.V[i].H[j].block.Block_Transform.position;
                            xy.X = i;
                            xy.Y = j + 1;
                            Candy_Init(obj);
                            obj.GetComponent<C_ImBlock>().MoveEnQueue(xy, MakeCount[i] / 5f);
                            continue;
                        }
                    }
                }
            }
        }
    }

    [ContextMenu("매치")]
    public void Match3_Algorithm()
    {
        // 찾기
        for (int i = 0; i < 9; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                isMatched[i, j] = 0;
            }
        }
        for (int i = 8; i >= 0; i--)
        {
            for (int j = 9; j >= 1; j--)
            {
                // 활성화된 보드만.
                if (c_board.b_BoardStates[i, j] && c_board.V[i].H[j].block.HereBlockObject != null)
                {
                    int Left = ThreeMatch_DFS(i, j, 1, -1, c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type);
                    int Top = ThreeMatch_DFS(i, j, 1, 1, c_board.V[i].H[j].block.HereBlockObject.GetComponent<C_ImBlock>().Fruit_Type);
                }
            }
        }

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
    }

    int[,] isMatched = new int[9, 10];
    int[,] BoomType = new int[9, 10];
    int ThreeMatch_DFS(int x, int y, int stack, int dir, int startFruit)
    {
        int num = 0;
        // 범위를 벗어나면 return 
        // 보드가 닫혀있다면 return
        if (y < 1 || x < 0 || c_board.b_BoardStates[x, y] == false)
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

    void Vertical_Explode(int xline, int yline)
    {
        for (int x = 0; x < 9; x++)
        {
            somes.iPoint += 100;
            Breaking(x, yline);
        }
    }

    void Horizontal_Explode(int xline, int yline)
    {
        for (int y = 1; y < 10; y++)
        {
            somes.iPoint += 100;
            Breaking(xline, y);
        }
    }

    public void Swap(int xLine, int yLine, int s_xLine, int s_yLine)
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
                if (c_board.b_BoardStates[i, j] && c_board.V[i].H[j].block.HereBlockObject != null)
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

        }
    }

}

[System.Serializable]
public class Board
{
    public Ver[] V = new Ver[9];
    public bool[,] b_BoardStates = new bool[9, 10];
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

[System.Serializable]
public class Block
{
    // 블록이 없는 부분 int.MaxValue
    public GameObject HereBlockObject;
    public C_ImBlock Here_ImBlock;
    public int BlockType = -1;
    public XY xy = new XY();
    public Image BlockSprite;
    public RectTransform Block_Transform;
}