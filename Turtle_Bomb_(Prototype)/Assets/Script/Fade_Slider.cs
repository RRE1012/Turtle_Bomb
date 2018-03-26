using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade_Slider : MonoBehaviour {

    float FadeSpeed = 600.0f;
    public int slider_num;
    float m_fade_wait_Time = 0.5f;
    float m_tempTime = 0.0f;
    bool m_is_wait_Over = false;

    void Update()
    {
        if (m_is_wait_Over)
        {
            if (transform.position.x <= 600.0f)
                transform.Translate(new Vector3(FadeSpeed * Time.deltaTime, 0.0f, 0.0f));
            else
            {
                if (slider_num == 0)
                    gameObject.SetActive(false);
            }
        }

        else
        {
            if (m_tempTime < m_fade_wait_Time)
                m_tempTime += Time.deltaTime;
            else m_is_wait_Over = true;
        }
    }
}
