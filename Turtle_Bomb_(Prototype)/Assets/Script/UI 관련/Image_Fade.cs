using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Image_Fade : MonoBehaviour {
    
    float m_Wait_Time_Before_Fade = 0.15f;
    float m_WaitTime = 0.0f;
    bool m_is_wait_Over = false;

    float m_FadeSpeed = 1200.0f;
    float m_Curr_Fading_Time = 0.0f;
    float m_Dest_Fading_Time = 1.0f;
    
    void Update()
    {
        if (m_is_wait_Over)
        {
            // 3. 슬라이더 이동
            if (m_Curr_Fading_Time <= m_Dest_Fading_Time)
            {
                m_Curr_Fading_Time += Time.deltaTime;
                transform.GetComponent<RectTransform>().sizeDelta += new Vector2(m_FadeSpeed * m_Curr_Fading_Time, m_FadeSpeed * m_Curr_Fading_Time * 0.7f);
            }

            // 4. 이동이 끝나면 비활성화
            else
            {
                gameObject.SetActive(false);
            }
        }

        else
        {
            // 1. 일정시간 대기
            if (m_WaitTime < m_Wait_Time_Before_Fade)
                m_WaitTime += Time.deltaTime;

            // 2. 대기 종료
            else m_is_wait_Over = true;
        }
    }
}
