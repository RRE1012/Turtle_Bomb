using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Performance_Camera : MonoBehaviour {

    public Camera m_PlayerView;
    public static Performance_Camera c_PerfCamera;
    public Animator m_CameraAnimator;

    void Start()
    {
        c_PerfCamera = this;

        Player_To_Perf();

        Intro_Performance();

    }

    public void Player_To_Perf()
    {
        m_PlayerView.enabled = false;
        m_PlayerView.GetComponent<AudioListener>().enabled = false;
        gameObject.GetComponent<Camera>().enabled = true;
        gameObject.GetComponent<AudioListener>().enabled = true;
    }
    public void Perf_To_Player()
    {
        m_PlayerView.enabled = true;
        m_PlayerView.GetComponent<AudioListener>().enabled = true;
        gameObject.GetComponent<Camera>().enabled = false;
        gameObject.GetComponent<AudioListener>().enabled = false;
    }

    public void Intro_Performance()
    {
        if (StageManager.c_Stage_Manager.m_is_Boss_Stage)
        {
            //m_CameraAnimator.SetTrigger("Intro_Boss");
            //Invoke("Intro_Performance_Over", 12.0f);
            Intro_Performance_Over();
        }
        else
        {
            m_CameraAnimator.SetTrigger("Intro_Normal");
            Invoke("Intro_Performance_Over", 7.0f);
        }
    }

    public void Intro_Performance_Over()
    {
        StageManager.c_Stage_Manager.m_is_Intro_Over = true;
        Perf_To_Player();
    }
}
