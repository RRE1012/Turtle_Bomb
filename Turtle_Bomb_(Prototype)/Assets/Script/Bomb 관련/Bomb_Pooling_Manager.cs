using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CALL_BOMB_STATE
{
    public const int NORMAL = 0;
    public const int DROP = 1;
}

static class BOMB_POOL_VALUES
{
    public const int BOMB_MAX_COUNT = 40;
}

public class Bomb_Pooling_Manager : MonoBehaviour
{
    static Bomb_Pooling_Manager m_Instance; public static Bomb_Pooling_Manager GetInstance() { return m_Instance; }

    Queue<GameObject> m_Bomb_Pool;

    public GameObject m_Bomb_Object;

    GameObject m_Temp_Bomb;

    void Awake()
    {
        m_Instance = this;

        m_Bomb_Pool = new Queue<GameObject>();
        GameObject temp;
        for (int i = 0; i < BOMB_POOL_VALUES.BOMB_MAX_COUNT; ++i)
        {
            temp = Instantiate(m_Bomb_Object);
            temp.transform.parent = transform;
            m_Bomb_Pool.Enqueue(temp);
        }
    }

    public GameObject Dequeue_Bomb(GameObject who, int state) // 꺼내기
    {
        m_Temp_Bomb = null;
        if (who.GetComponent<Bomb_Setter>().Get_Curr_Bomb_Count() > 0)
        {
            m_Temp_Bomb = m_Bomb_Pool.Dequeue();
            m_Temp_Bomb.SetActive(true);
            m_Temp_Bomb.GetComponent<Bomb_Remaster>().Get_Out_Of_Pool(who, state);
            who.GetComponent<Bomb_Setter>().Decrease_Bomb_Count();
        }

        return m_Temp_Bomb;
    }

    public void Enqueue_Bomb(GameObject bomb) { m_Bomb_Pool.Enqueue(bomb); } // 넣기
}
