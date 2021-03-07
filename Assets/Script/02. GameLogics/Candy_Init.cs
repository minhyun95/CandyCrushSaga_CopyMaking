using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Functions
{
    public class Candy_Init
    {
        C_GameManager gm = C_GameManager.Instance;
        // 캔디오브젝트의 이미지를 Fruit_Type를 변경시켜주고
        // Fruit_Type에 맞게 이미지를 바꾼다.
        // 2번째 인자인 getnum값이 들어올 경우 랜덤한 값이 아니라
        // 들어온 값으로 Fruit_Type이 결정된다.
        public void Init(GameObject obj, int getNum = -1)
        {
            int fruitRand = 0;
            if (getNum == -1)
                fruitRand = Random.Range((int)e_Block_Type.N_Blue, (int)(e_Block_Type.N_Green));
            else
                fruitRand = getNum;

            switch (fruitRand)
            {
                    // Normal Image
                case (int)e_Block_Type.N_Blue:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_fruit_face_atlas_01_4");
                    break;
                case (int)e_Block_Type.N_Red:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_fruit_face_atlas_01_1");
                    break;
                case (int)e_Block_Type.N_Orange:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_fruit_face_atlas_01_8");
                    break;
                case (int)e_Block_Type.N_Yellow:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_fruit_face_atlas_01_7");
                    break;
                case (int)e_Block_Type.N_Green:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_fruit_face_atlas_01_2");
                    break;


                    // Vertical Image
                case (int)e_Block_Type.V_Blue:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_20");
                    break;
                case (int)e_Block_Type.V_Red:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_5");
                    break;
                case (int)e_Block_Type.V_Orange:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_12");
                    break;
                case (int)e_Block_Type.V_Yellow:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_9");
                    break;
                case (int)e_Block_Type.V_Green:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_17");


                    // Horizontal Image
                    break;
                case (int)e_Block_Type.H_Blue:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_22");
                    break;
                case (int)e_Block_Type.H_Red:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_7");
                    break;
                case (int)e_Block_Type.H_Orange:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_14");
                    break;
                case (int)e_Block_Type.H_Yellow:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_11");
                    break;
                case (int)e_Block_Type.H_Green:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_19");
                    break;
                    
                    // BagBoom Image
                case (int)e_Block_Type.B_Blue:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_4");
                    break;
                case (int)e_Block_Type.B_Red:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_1");
                    break;
                case (int)e_Block_Type.B_Orange:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_3");
                    break;
                case (int)e_Block_Type.B_Yellow:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_2");
                    break;
                case (int)e_Block_Type.B_Green:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("sprite_arrow_atlas_0");
                    break;

                case 100:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("star");
                    break;

            }
            candy_special_setting(obj.GetComponent<C_ImBlock>(), ((fruitRand / 5) * 5));
            obj.GetComponent<C_ImBlock>().Fruit_Type = fruitRand;
        }

        // 특정 색상 모두 캔디 종류 변경하기
        public void OneColor_Fruit_Change_All_Special(int get_fruit_Type, e_Boom_Number_Type _enum)
        {
            int WhatBoom = 0;

            if (_enum == e_Boom_Number_Type.VerticalBoom)
                WhatBoom = 5;
            if (_enum == e_Boom_Number_Type.HorizontalBoom)
                WhatBoom = 10;
            if (_enum == e_Boom_Number_Type.BagBoom)
                WhatBoom = 15;

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (gm.GetBlockObject(x,y) != null && gm.Get_ImBlock(x, y).Fruit_Type % 5 == get_fruit_Type % 5)
                    {
                        gm.candy_Init.Init(gm.GetBlockObject(x, y), (get_fruit_Type % 5) + WhatBoom);
                    }
                }
            }
        }

        public void candy_special_setting(C_ImBlock c_Im, int get_num)
        {
            c_Im.VerticalBoom = 0;
            c_Im.HorizontalBoom = 0;
            c_Im.BagBoom = 0;
            c_Im.ColorBoom = 0;
            if (get_num == 100)
                c_Im.ColorBoom = 1;
            else if (get_num == 5)
                c_Im.VerticalBoom = 1;
            else if (get_num == 10)
                c_Im.HorizontalBoom = 1;
            else if (get_num == 15)
                c_Im.BagBoom = 1;
        }
    }


}
