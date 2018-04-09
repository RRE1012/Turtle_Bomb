using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade_Slider : MonoBehaviour {

    float m_Wait_Time_Before_Fade = 0.5f;
    float m_WaitTime = 0.0f;
    bool m_is_wait_Over = false;

    float m_FadeSpeed = 80.0f;
    float m_Curr_Fading_Time = 0.0f;
    float m_Dest_Fading_Time = 1.0f;

    public bool m_is_Stage_Select_Scene = false;

    void Start()
    {
        if (m_is_Stage_Select_Scene)
        {
            m_Wait_Time_Before_Fade = 0.0f;
            m_FadeSpeed = 60.0f;
        }
    }

    void Update()
    {
        if (m_is_wait_Over)
        {
            if (m_Curr_Fading_Time <= m_Dest_Fading_Time)
            {
                m_Curr_Fading_Time += Time.deltaTime;
                transform.Translate(new Vector3(m_FadeSpeed * m_Curr_Fading_Time, 0.0f, 0.0f));
            }

            else
            { 
                if (!m_is_Stage_Select_Scene)
                    gameObject.SetActive(false);
            }
        }

        else
        {
            if (m_WaitTime < m_Wait_Time_Before_Fade)
                m_WaitTime += Time.deltaTime;
            else m_is_wait_Over = true;
        }
    }
}
