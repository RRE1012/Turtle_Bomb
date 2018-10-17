using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade_Slider : MonoBehaviour {

    public static Fade_Slider c_Fade_Slider;

    bool m_is_Fade_Over = false;
    public bool m_is_Stage_Select_Scene = false;

    int m_Fade_Number = 0;
    bool m_is_Fade_Started = false;

    void Awake()
    {
        c_Fade_Slider = this;
    }

    void Update()
    {
        if (m_is_Fade_Started) // 페이드가 시작됐다면
        {
            if (Check_Fade_Over()) // 페이드가 끝났는지 확인해서
            {
                if (m_is_Stage_Select_Scene) // 스테이지 선택 창이면,
                    m_is_Fade_Over = true; // 페이드 끝 알림.
                else gameObject.SetActive(false); // 아니면 그냥 꺼버림.
            }
        }

    }

    public void Start_Fade_Slider(int num)
    {
        m_Fade_Number = num;

        switch (m_Fade_Number)
        {
            case 1:
                gameObject.GetComponent<Animation>().Play("Fade_Slider_Animation_1");
                m_is_Fade_Started = true;
                break;
            case 2:
                gameObject.GetComponent<Animation>().Play("Fade_Slider_Animation_2");
                m_is_Fade_Started = true;
                break;
        }
    }

    bool Check_Fade_Over()
    {
        switch (m_Fade_Number)
        {
            case 1:
                return !gameObject.GetComponent<Animation>().IsPlaying("Fade_Slider_Animation_1");
            case 2:
                return !gameObject.GetComponent<Animation>().IsPlaying("Fade_Slider_Animation_2");
        }

        return false;
    }

    public bool Get_is_Fade_Over()
    {
        return m_is_Fade_Over;
    }
}
