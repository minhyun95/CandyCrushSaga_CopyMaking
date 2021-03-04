using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Values;
public class C_ImBlock : MonoBehaviour
{
    public int Fruit_Type;
    public C_GameManager gm;
    public int X, Y;
    public bool VerticalBoom;
    public bool HorizontalBoom;
    public bool ColorBoom;
    Queue<XY> Q_XY = new Queue<XY>();
    Queue<float> Q_Time = new Queue<float>();
    bool Queue_On = false;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("Game_Manager").GetComponent<C_GameManager>();
    }
    public void Check_Now_Position(int _x, int _y)
    {
        X = _x;
        Y = _y;
    }
    public void MoveEnQueue(XY xy, float DelayTime = 0f)
    {
        // 이미 이동중일때 큐에 넣음
        if(Queue_On)
        {
            Q_XY.Enqueue(xy);
            Q_Time.Enqueue(DelayTime);
        }

        // 이동중 != 일때 코루틴 시작
        else
        {
            StartCoroutine(Moving(xy.X, xy.Y, DelayTime));
        }
    }


    // 큐에 다음 이동장소가 없으면 이동.
    IEnumerator Moving(int x, int y, float DelayTime = 0f)
    {
        Queue_On = true;
        // 시작위치 고정
        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(DelayTime);

        // 0.1초간 목적지로 이동
        for (int i = 0; i < 16; i++)
        {
            yield return new WaitForSeconds(0.1f / 16f);
            transform.position = Vector3.Lerp(startPos, gm.c_board.V[x].H[y].block.Block_Transform.position, i / 16f);
        }
        Check_Now_Position(x, y);

        // 목적지로 현재위치 고정
        transform.position = gm.c_board.V[x].H[y].block.Block_Transform.position;

        // Queue가 비어있으면 종료.
        if (Q_XY.Count != 0)
        {
            XY xy;
            xy = Q_XY.Dequeue();
            float dt = Q_Time.Dequeue();
            StartCoroutine(Moving(xy.X, xy.Y, dt));
        }
        else
        {
            Queue_On = false;
        }
    }
}
