using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Normal_Fade : MonoBehaviour
{
    Animation m_Animation;
    IEnumerator m_Fade_Out_Checker;
    bool m_is_Fade_Out_Over = false; public bool Get_is_Fade_Out_Over () { return m_is_Fade_Out_Over; }

    void Awake()
    {
        m_Animation = GetComponent<Animation>();
        m_Fade_Out_Checker = Fade_Out_Check();
        FadeIn();
    }

    public void FadeIn()
    {
        m_Animation.Play(m_Animation.GetClip("Normal_Fade_In").name);
    }

    public void FadeOut()
    {
        m_Animation.Play(m_Animation.GetClip("Normal_Fade_Out").name);
        StartCoroutine(m_Fade_Out_Checker);
    }

    IEnumerator Fade_Out_Check()
    {
        while(true)
        {
            if (m_Animation["Normal_Fade_Out"].normalizedTime >= 0.95f)
            {
                StopCoroutine(m_Fade_Out_Checker);
                m_is_Fade_Out_Over = true;
            }
            yield return null;
        }
    }
}
