using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class JoyStick : MonoBehaviour
{
    C_ImBlock my;
    Vector3 stickFirstPosition;
    public bool Clicked = false;
    public Vector3 joyVec;
    private static JoyStick instance;

    private void Start()
    {
        my = GetComponent<C_ImBlock>();
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

            if (my.gm.c_board.b_BoardStates[my.X, my.Y])
            {
                // 오른쪽이랑 이동
                if (joyVec.x >= 100 && my.X < 8)
                {
                    if (my.gm.c_board.b_BoardStates[my.X + 1, my.Y] && my.gm.c_board.V[my.X + 1].H[my.Y].block.HereBlockObject != null)
                    {
                        my.gm.Swap(my.X, my.Y, my.X + 1, my.Y);
                        Clicked = false;
                    }
                }
                // 왼쪽이랑 이동
                else if (joyVec.x <= -100 && my.X > 0)
                {
                    if (my.gm.c_board.b_BoardStates[my.X - 1, my.Y] && my.gm.c_board.V[my.X - 1].H[my.Y].block.HereBlockObject != null)
                    {
                        my.gm.Swap(my.X, my.Y, my.X - 1, my.Y);
                        Clicked = false;
                    }
                }
                // 위랑 이동
                else if (joyVec.y >= 100 && my.Y > 0)
                {
                    if (my.gm.c_board.b_BoardStates[my.X, my.Y - 1] && my.gm.c_board.V[my.X].H[my.Y - 1].block.HereBlockObject != null)
                    {
                        my.gm.Swap(my.X, my.Y, my.X, my.Y - 1);
                        Clicked = false;
                    }
                }
                // 아래랑 이동
                else if (joyVec.y <= -100 && my.Y < 9)
                {
                    if (my.gm.c_board.b_BoardStates[my.X, my.Y + 1] && my.gm.c_board.V[my.X].H[my.Y + 1].block.HereBlockObject != null)
                    {
                        my.gm.Swap(my.X, my.Y, my.X, my.Y + 1);
                        Clicked = false;
                    }
                }
            }
        }
        
    }

    public void Drop()
    {
        if (my == null) my = GetComponent<C_ImBlock>();

        Clicked = false;

        
        joyVec = Vector3.zero;
    }
}
