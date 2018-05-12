using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Portal : MonoBehaviour {

    public GameObject m_Normal_Monster;

    float m_Summon_Timer = 0.0f;

	void Update ()
    {
        if (m_Summon_Timer < 5.0f)
            m_Summon_Timer += Time.deltaTime;
        else
        {
            Vector3 pos = transform.position;
            pos.y = 0.0f;

            Instantiate(m_Normal_Monster).transform.position = pos;
            Destroy(gameObject);
        }
	}
}
