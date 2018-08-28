using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Portal : MonoBehaviour {

    public GameObject m_Normal_Monster;

    float m_Summon_Timer = 0.0f;

    int m_MCL_Index;

    void Start()
    {
        m_MCL_Index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);
    }

    void Update ()
    {
        if (m_Summon_Timer < 5.0f)
            m_Summon_Timer += Time.deltaTime;
        else
        {
            Vector3 pos = transform.position;

            if (!StageManager.GetInstance().Get_MCL_index_is_Blocked(m_MCL_Index)) pos.y = 0.0f;
            else pos.y = 1.5f;

            Instantiate(m_Normal_Monster).transform.position = pos;
            Destroy(gameObject);
        }
	}

    public void Set_Monster_Speed(float s)
    {
        m_Normal_Monster.GetComponent<MonsterAI>().Set_Basic_Speed(s);
    }
}
