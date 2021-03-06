using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Functions
{
    public class Candy_Init
    {
        // 캔디오브젝트의 이미지를 Fruit_Type를 변경시켜주고
        // Fruit_Type에 맞게 이미지를 바꾼다.
        // 2번째 인자인 getnum값이 들어올 경우 랜덤한 값이 아니라
        // 들어온 값으로 Fruit_Type이 결정된다.
        public void Init(GameObject obj, int getNum = -1)
        {
            int fruitRand = 0;
            if (getNum == -1)
                fruitRand = Random.Range((int)e_Block_Type.N_Blue, (int)(e_Block_Type.N_Green + 1));
            else
                fruitRand = getNum;

            switch (fruitRand)
            {
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
                case 100:
                    obj.GetComponent<Image>().sprite = C_GameManager.Instance.m_atlas.GetSprite("star");
                    break;

            }
            obj.GetComponent<C_ImBlock>().Fruit_Type = fruitRand;
        }
       

    }


}
