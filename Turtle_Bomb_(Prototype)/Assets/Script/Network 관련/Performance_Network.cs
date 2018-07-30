using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Performance_Network : MonoBehaviour {
    public Camera m_PlayerView;
    public Camera m_PerformanceView;
	public Camera[] m_OtherPlayerView;
	public Animator[] m_OtherPlayerAnimator;
    public static Performance_Network instance;
    public Animator m_CameraAnimator;
    public bool ani_is_working;
    // Use this for initialization

    void Awake()
    {
        instance = this;
    }
    void Start () {
        ani_is_working = true;
        if (m_OtherPlayerView.Length>0)
        {
            for (int i = 0; i < 4; ++i)
                m_OtherPlayerView[i].enabled = false;
        }
        
    }
	public void DeadAnimation(byte id){
		m_PlayerView.enabled = false;
		m_OtherPlayerView[id].enabled = true;
		switch (id) {
		case 0:
			m_OtherPlayerAnimator[0].SetTrigger("GameOver");
			ani_is_working = true;
			break;
		case 1:
			m_OtherPlayerAnimator[1].SetTrigger("GameOver");
			ani_is_working = true;
			break;
		case 2:
			m_OtherPlayerAnimator[2].SetTrigger("GameOver");
			ani_is_working = true;
			break;
		case 3:
			m_OtherPlayerAnimator[3].SetTrigger("GameOver");
			ani_is_working = true;
			break;
		
		}
		Invoke("CameraSwitch", 9.3f);
	}
	void CameraSwitch()
    {
        m_PlayerView.enabled = true;
        m_PerformanceView.enabled = false;
        if (m_OtherPlayerView.Length > 0)
        {
            for (int i = 0; i < 4; ++i)
                m_OtherPlayerView[i].enabled = false;
        }
		ani_is_working = false;
    }
	// Update is called once per frame
	void Update () {
        
    }
    public void Intro_Performance(byte id)
    {
        m_PlayerView.enabled = false;
        m_PerformanceView.enabled = true;
        switch (id)
        {
            case 0:
                m_CameraAnimator.SetTrigger("Intro_Normal");
                ani_is_working = true;
                break;
            case 1:
                m_CameraAnimator.SetTrigger("Intro_Normal2");
                ani_is_working = true;
                break;
            case 2:
                m_CameraAnimator.SetTrigger("Intro_Normal3");
                ani_is_working = true;
                break;
            case 3:
                m_CameraAnimator.SetTrigger("Intro_Normal4");
                ani_is_working = true;
                break;

        }
        Invoke("CameraSwitch", 5.9f);
    }
       
}
