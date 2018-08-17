using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice_Box : Box
{
    int m_Crash_Count; public void Set_Crash_Count(int count) { m_Crash_Count = count; }
    public Material[] m_Materials;

    void Start()
    {
        GetComponentInChildren<MeshRenderer>().material = m_Materials[m_Crash_Count - 1];
        //index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);
        //StageManager.GetInstance().Update_MCL_isBlocked(index, true);
    }


    void OnTriggerEnter(Collider other)
    {
        if (!m_is_Destroyed && other.gameObject.CompareTag("Flame_Remains"))
        {
            if (m_Crash_Count > 1)
            {
                --m_Crash_Count;
                GetComponentInChildren<MeshRenderer>().material = m_Materials[m_Crash_Count - 1];
            }

            else
            {
                m_is_Destroyed = true;

                // MCL 갱신
                index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);
                StageManager.GetInstance().Update_MCL_isBlocked(index, false);

                SetItem(); // 아이템 생성

                Instantiate(m_Particle).transform.position = transform.position; // 파티클 발생

                Destroy(gameObject); // 박스 파괴
            }

            Destroy(other.gameObject); // 화염 잔해 파괴
        }
    }
}
