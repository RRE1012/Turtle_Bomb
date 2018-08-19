using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Setter : MonoBehaviour
{
    protected int m_Max_Bomb_Count; public void Set_Max_Bomb_Count(int count) { m_Max_Bomb_Count = count; }
    public int Get_Max_Bomb_Count() { return m_Max_Bomb_Count; }

    protected int m_Curr_Bomb_Count; public void Set_Curr_Bomb_Count(int count) { m_Curr_Bomb_Count = count; }
    public int Get_Curr_Bomb_Count() { return m_Curr_Bomb_Count; }

    protected int m_Fire_Count; public void Set_Fire_Count(int count) { m_Fire_Count = count; }
    public int Get_Fire_Count() { return m_Fire_Count; }


    protected GameObject Bomb_Set(int state) { return Bomb_Pooling_Manager.GetInstance().Dequeue_Bomb(gameObject, state); }

    public void Decrease_Bomb_Count() { --m_Curr_Bomb_Count; }
    public void Bomb_Reload() { if (m_Curr_Bomb_Count + 1 <= m_Max_Bomb_Count) ++m_Curr_Bomb_Count; }
}
