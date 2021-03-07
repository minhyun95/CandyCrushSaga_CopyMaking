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

    // Fill_Empty_Space()를 실행할때 fLogic_Timer 갱신
    // fLogic_Timer 0이하라면 3Match 실행 (1회) 
    // 3Match가 된다면 : Fill재실행
    // 3Match가 안된다면 : 유저의 Swap 기다림
    public float fLogic_Timer = -1;

    // 보드
    [Header("보드")]
    public Transform Board_Pool_Transform;
    public Board c_board = new Board();

    // Some Value
    public C_Value values;
    public Board_Setting board_Setting;
    public Candy_Init candy_Init;
    public Fill_Board fill_Board;
    public Breaking breaking;
    public Match_Finder match_Finder;
    public Swap_Board swap_Board;
    public Check_Predicted_Case check_Predicted_Case;

    private void Awake()
    {
        board_Setting = new Functions.Board_Setting();
        candy_Init = new Functions.Candy_Init();
        fill_Board = new Functions.Fill_Board();
        breaking = new Functions.Breaking();
        match_Finder = new Functions.Match_Finder();
        values = new Functions.C_Value();
        swap_Board = new Functions.Swap_Board();
        check_Predicted_Case = new Functions.Check_Predicted_Case();

        // 아틀라스 이미지 로드
        m_atlas = Resources.Load<SpriteAtlas>("FruitAtlas");
    }
    void Start()
    {
        board_Setting.setting();

        Next_Logic_Ready = true;
        Game_Logic_State = 0;
        StartCoroutine("Game_Routine");
    }

    void Update()
    {
        if (fLogic_Timer >= 0)
            fLogic_Timer -= Time.deltaTime * Time.timeScale;
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
                    check_Predicted_Case.CheckPredictedCase();
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








    /*  
        ---------------------------------------------------------------------------------------------
        --------------- 이 밑은 다른 CS파일에서 GameManager 객체를 참조하거나 변경할때 사용 -------------
        ---------------------------------------------------------------------------------------------
    */

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


    // Board_Setting에서 NewBlock을 만들기 위해 
    // (Board_Setting은 MonoBehaviour이 아니라 만들 수 없어서 여기서 생성 기능 만듬)
    public GameObject MakeObject(string name)
    {
        GameObject NewBlock = Instantiate(Resources.Load<GameObject>(name),
                           transform.position, Quaternion.identity, Block_Pool_Transform);
        return NewBlock;
    }
}
