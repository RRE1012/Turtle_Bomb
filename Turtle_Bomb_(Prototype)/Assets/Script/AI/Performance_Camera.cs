using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Performance_Camera : MonoBehaviour {

    public Camera m_PlayerView;
    public static Performance_Camera c_PerfCamera;
    public Animator m_CameraAnimator;
    IEnumerator m_Intro_Normal_Performance;

    void Start()
    {
        c_PerfCamera = this;
        m_Intro_Normal_Performance = WaitFor_EndOf_Intro_Normal_Performance(); // 코루틴 함수 등록

        Player_To_Perf();

        Intro_Performance(); // 인트로 수행

    }

    IEnumerator WaitFor_EndOf_Intro_Normal_Performance() // 일반 인트로 모션 완료를 기다린다.
    {
        while (true)
        {
            if (m_CameraAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Camera_Performance_Intro_Normal"))
            {
                if (m_CameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) // 완료되면
                {
                    StopCoroutine(m_Intro_Normal_Performance); // 코루틴 종료
                    Intro_Performance_Over(); // 인트로 모션 완료 알림
                }
            }
            yield return null;
        }
        
    }

    public void Player_To_Perf() // 카메라 전환 (플레이어->퍼포먼스)
    {
        m_PlayerView.enabled = false;
        m_PlayerView.GetComponent<AudioListener>().enabled = false;
        gameObject.GetComponent<Camera>().enabled = true;
        gameObject.GetComponent<AudioListener>().enabled = true;
    }
    public void Perf_To_Player() // 카메라 전환 (퍼포먼스->플레이어)
    {
        m_PlayerView.enabled = true;
        m_PlayerView.GetComponent<AudioListener>().enabled = true;
        gameObject.GetComponent<Camera>().enabled = false;
        gameObject.GetComponent<AudioListener>().enabled = false;
    }

    public void Intro_Performance()
    {
        if (StageManager.GetInstance().Get_is_Boss_Stage())
        {
            //m_CameraAnimator.SetTrigger("Intro_Boss");
            Intro_Performance_Over();
        }
        else
        {
            m_CameraAnimator.SetTrigger("Intro_Normal"); // 일반 인트로 모션 시작
            StartCoroutine(m_Intro_Normal_Performance); // 일반 인트로 대기 코루틴 실행
        }
    }

    public void Intro_Performance_Over()
    {
        StageManager.GetInstance().Set_is_Intro_Over(true);
        Perf_To_Player();
    }
}
