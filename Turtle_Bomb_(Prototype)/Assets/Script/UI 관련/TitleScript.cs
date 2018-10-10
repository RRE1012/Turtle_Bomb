using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScript : MonoBehaviour
{
    public Animator m_Animator;
    public GameObject m_Bomb_Sound;
    bool m_is_Clicked = false;

    public void MakeWave()
    {
        if (!m_is_Clicked)
        {
            m_Bomb_Sound.GetComponent<Bomb_Sound>().Play_ExplodeSound();
            m_Animator.SetTrigger("Touched_Start");
            m_is_Clicked = true;

            if (PlayerPrefs.HasKey("System_Option_Vib_ON") && PlayerPrefs.GetInt("System_Option_Vib_ON") == 1)
            {
//#if UNITY_ANDROID
                Handheld.Vibrate(); // 1초간 진동
//#endif
            }
        }
    }
}
