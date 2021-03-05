using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;

namespace Values
{
    [System.Serializable]
    public class C_Value : MonoBehaviour
    {
        public Text PointText;
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
            PointText.text = "점수 : " + iPoint + "점";
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

    public class XY
    {
        public int X = 0;
        public int Y = 0;

    }
    
    
}
