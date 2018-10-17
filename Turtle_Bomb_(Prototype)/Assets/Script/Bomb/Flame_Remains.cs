using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame_Remains : MonoBehaviour {
    
    float Flame_Remains_LifeTime;

    IEnumerator m_Cycle_Checker;

    void Awake()
    {
        m_Cycle_Checker = Cycle_Check();
    }

    public void Cycle_Start()
    {
        Flame_Remains_LifeTime = 1.0f;
        StartCoroutine(m_Cycle_Checker);
    }

	IEnumerator Cycle_Check()
    {
        while (true)
        {
            if (!StageManager.GetInstance().Get_is_Pause())
            {
                if (Flame_Remains_LifeTime <= 0.0f)
                {
                    StopCoroutine(m_Cycle_Checker);
                    gameObject.SetActive(false);
                }
                else Flame_Remains_LifeTime -= Time.deltaTime;
            }
            yield return null;
        }
    }
}
