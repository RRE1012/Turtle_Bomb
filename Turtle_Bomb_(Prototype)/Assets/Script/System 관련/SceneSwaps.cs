using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

//씬 전환 함수
public class SceneSwaps : MonoBehaviour {

    public Canvas cv;
    public RawImage FadeSlider;
    IEnumerator m_WaitForFadeSlider;

    // Use this for initialization
    void Start () {
        if (cv != null)
        {
            cv.enabled = true;
        }
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
        //Debug.Log("Clicked");
        SceneManager.LoadScene(1);
    }

    // "모험모드 스테이지 선택 화면"으로 이동

    public void GoTo_Mode_Adventure_StageSelect_Scene()
    {    
        SceneManager.LoadScene(2);
    }
    public void GoTo_Mode_VS_Wait_Scene()
    {
        SceneManager.LoadScene(8);
    }
    // "선택한 해당 스테이지"로 이동
    public void GoTo_Mode_Adventure_Selected_Stage(int stage_ID)
    {
        if (LobbySound.instanceLS != null)
            LobbySound.instanceLS.SoundStop();
        
        // "맵 로드를 위한" 현재 스테이지 번호를 기록.
        PlayerPrefs.SetInt("Mode_Adventure_Stage_ID_For_MapLoad", stage_ID);
        PlayerPrefs.Save();
        FadeSlider.gameObject.SetActive(true);
        FadeSlider.gameObject.GetComponent<Fade_Slider>().m_is_Stage_Select_Scene = true;
        FadeSlider.gameObject.GetComponent<Fade_Slider>().Start_Fade_Slider(1);


        // 페이드 아웃 대기 후 모험모드 씬을 연다
        m_WaitForFadeSlider = WaitForFadeSlider();
        StartCoroutine(m_WaitForFadeSlider);
    }
   
    public void Save_Selected_Stage(int stage_ID)
    {
        // 선택한 스테이지가 몇번인지 PlayerPrefs에 기록!
        PlayerPrefs.SetInt("Mode_Adventure_Current_Stage_ID", stage_ID);
        PlayerPrefs.Save();
    }

    IEnumerator WaitForFadeSlider()
    {
        while (true)
        {
            if (Fade_Slider.c_Fade_Slider.Get_is_Fade_Over())
            {
                StopCoroutine(m_WaitForFadeSlider);
                SceneManager.LoadScene(3);
            }
            yield return null;
        }
    }

}
