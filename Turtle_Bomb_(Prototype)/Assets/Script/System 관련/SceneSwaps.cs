using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

//씬 전환 함수
public class SceneSwaps : MonoBehaviour {

    public Canvas cv;
    public GameObject m_Fade_UI;
    Normal_Fade m_Normal_Fade;
    IEnumerator m_WaitForFade;
    int m_Scene_Number;

    void Start()
    {
        if (m_Fade_UI != null) m_Normal_Fade = m_Fade_UI.GetComponent<Normal_Fade>();
    }

    //쓰지 않는 함수(카메라 스위칭 함수)
    public void switchCamera()
    {       
        cv.enabled = !cv.enabled;
    }

    // "타이틀 화면"으로 이동
    public void GoTo_Title_Scene()
    {
        SceneManager.LoadScene(0);
    }

    // "모드 선택 화면"으로 이동
    public void GoTo_ModeSelect_AfterFewSec()
    {
        Invoke("GoTo_ModeSelect_Scene", 1.0f);
    }
    public void GoTo_ModeSelect_Scene()
    {
        SceneManager.LoadScene(1);
    }
    public void Return_to_Mode_Select_Scene()
    {
        m_Scene_Number = 1;
        m_Fade_UI.SetActive(true);
        m_Normal_Fade.FadeOut();

        m_WaitForFade = WaitForFade();
        StartCoroutine(m_WaitForFade);
    }

    // "모험모드 스테이지 선택 화면"으로 이동
    public void GoTo_Mode_Adventure_StageSelect_Scene()
    {
        m_Scene_Number = 2;
        m_Fade_UI.SetActive(true);
        m_Normal_Fade.FadeOut();

        m_WaitForFade = WaitForFade();
        StartCoroutine(m_WaitForFade);
    }
    public void GoTo_Mode_VS_Wait_Scene()
    {
        SceneManager.LoadScene(8);
    }
    public void GoTo_Mode_CoOp_Wait_Scene()
    {
        SceneManager.LoadScene(9);
    }

    // "선택한 해당 스테이지"로 이동
    public void GoTo_Mode_Adventure_Selected_Stage(int stage_ID)
    {
        if (LobbySound.instanceLS != null) LobbySound.instanceLS.SoundStop();
        
        // "맵 로드를 위한" 현재 스테이지 번호를 기록.
        PlayerPrefs.SetInt("Mode_Adventure_Stage_ID_For_MapLoad", stage_ID);
        PlayerPrefs.Save();

        m_Scene_Number = 3;
        m_Fade_UI.SetActive(true);
        m_Normal_Fade.FadeOut();

        m_WaitForFade = WaitForFade();
        StartCoroutine(m_WaitForFade);
    }
   
    public void Save_Selected_Stage(int stage_ID)
    {
        // 선택한 스테이지가 몇번인지 PlayerPrefs에 기록!
        PlayerPrefs.SetInt("Mode_Adventure_Current_Stage_ID", stage_ID);
        PlayerPrefs.Save();
    }

    IEnumerator WaitForFade()
    {
        while (true)
        {
            if (m_Normal_Fade.Get_is_Fade_Out_Over())
            {
                StopCoroutine(m_WaitForFade);
                SceneManager.LoadScene(m_Scene_Number);
            }
            yield return null;
        }
    }

}
