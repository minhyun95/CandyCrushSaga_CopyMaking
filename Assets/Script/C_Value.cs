using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;

namespace Functions
{
    [System.Serializable]
    public class C_Value
    {
        public int iPoint = 0;

        [Header("점수 구분")]
        public int iBrokePoint = 30;

        // UI 색 
        public Color BoardOffColor = Color.black;  // 비활성화된 보드판 sprite color

        public XY GetXY(int x, int y)
        {
            XY xy = new XY();
            xy.X = x;
            xy.Y = y;
            return xy;
        }

        public void SetPointText()
        {
            UI_Manager.Instance.PointText.text = "점수 : " + iPoint + "점";
        }
    }

    // Fruit가 가지는 값
    enum e_Block_Type
    {
        // N : Normal
        N_Blue,
        N_Red,
        N_Orange,
        N_Yellow,
        N_Green,
        // VB : VerticalBoom
        V_Blue,
        V_Red,
        V_Orange,
        V_Yellow,
        V_Green,

        // HB : HorizontalBoom
        H_Blue,
        H_Red,
        H_Orange,
        H_Yellow,
        H_Green,
    }

    // 가로,세로 파괴일때 0,1로 구분해준다.
    enum e_Boom_Number_Type
    {
        VerticalBoom,
        HorizontalBoom
    }

}

/*
    Unity 인스펙터 창에서 2중배열을 볼 수 없으므로
    Ver(가로) 9개, Hor(세로) 10개 가지고있는 Class 배열을 만들고
    Hor Class 내부에 Block Class를 넣었다.
    9x10의 Block이 생긴셈.
*/

public class XY
{
    public int X = 0;
    public int Y = 0;

}
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