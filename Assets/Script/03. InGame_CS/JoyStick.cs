using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class JoyStick : MonoBehaviour
{
    C_ImBlock c_MyBlock;
    Vector3 stickFirstPosition;
    public bool Clicked = false;
    public Vector3 joyVec;
    private static JoyStick instance;
    C_GameManager gm;

    // 조이스틱 CS 기능
    // 각 과일은 조이스틱 함수를 가지고있고
    // 클릭하면 PointDown, OnMouseDrag 함수가 실행됨.
    // 클릭한 상태로 일정거리 이상 움직이면 GameManager에서 Swap
    private void Start()
    {
        gm = C_GameManager.Instance;
        c_MyBlock = GetComponent<C_ImBlock>();
    }
    public void PointDown()
    {
        Clicked = true;
        stickFirstPosition = Input.mousePosition;
    }

    public void OnMouseDrag(BaseEventData baseEventData)
    {
        if(Clicked)
        {
            PointerEventData pointerEventData = baseEventData as PointerEventData;
            Vector3 DragPosition = pointerEventData.position;
            joyVec = (DragPosition - stickFirstPosition);

            if (C_GameManager.Instance.c_board.V[c_MyBlock.X].H[c_MyBlock.Y].block.BoardState)
            {
                // 오른쪽이랑 이동
                if (joyVec.x >= 100 && c_MyBlock.X < 8)
                {
                    if (C_GameManager.Instance.c_board.V[c_MyBlock.X + 1].H[c_MyBlock.Y].block.BoardState && C_GameManager.Instance.c_board.V[c_MyBlock.X + 1].H[c_MyBlock.Y].block.HereBlockObject != null)
                    {
                        gm.swap_Board.Swap(c_MyBlock.X, c_MyBlock.Y, c_MyBlock.X + 1, c_MyBlock.Y);
                        Clicked = false;
                    }
                }
                // 왼쪽이랑 이동
                else if (joyVec.x <= -100 && c_MyBlock.X > 0)
                {
                    if (C_GameManager.Instance.c_board.V[c_MyBlock.X - 1].H[c_MyBlock.Y].block.BoardState && C_GameManager.Instance.c_board.V[c_MyBlock.X - 1].H[c_MyBlock.Y].block.HereBlockObject != null)
                    {
                        gm.swap_Board.Swap(c_MyBlock.X, c_MyBlock.Y, c_MyBlock.X - 1, c_MyBlock.Y);
                        Clicked = false;
                    }
                }
                // 위랑 이동
                else if (joyVec.y >= 100 && c_MyBlock.Y > 0)
                {
                    if (C_GameManager.Instance.c_board.V[c_MyBlock.X].H[c_MyBlock.Y - 1].block.BoardState && C_GameManager.Instance.c_board.V[c_MyBlock.X].H[c_MyBlock.Y - 1].block.HereBlockObject != null)
                    {
                        gm.swap_Board.Swap(c_MyBlock.X, c_MyBlock.Y, c_MyBlock.X, c_MyBlock.Y - 1);
                        Clicked = false;
                    }
                }
                // 아래랑 이동
                else if (joyVec.y <= -100 && c_MyBlock.Y < 9)
                {
                    if (C_GameManager.Instance.c_board.V[c_MyBlock.X].H[c_MyBlock.Y + 1].block.BoardState && C_GameManager.Instance.c_board.V[c_MyBlock.X].H[c_MyBlock.Y + 1].block.HereBlockObject != null)
                    {
                        gm.swap_Board.Swap(c_MyBlock.X, c_MyBlock.Y, c_MyBlock.X, c_MyBlock.Y + 1);
                        Clicked = false;
                    }
                }
            }
        }
        
    }

    public void Drop()
    {
        if (c_MyBlock == null) c_MyBlock = GetComponent<C_ImBlock>();

        Clicked = false;

        
        joyVec = Vector3.zero;
    }
}
