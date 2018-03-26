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
    public void GoTo_ModeSelect_Scene()
    {
        SceneManager.LoadScene(1);
    }

    // "모험모드 스테이지 선택 화면"으로 이동
    public void GoTo_Mode_Adventure_StageSelect_Scene()
    {    
        SceneManager.LoadScene(2);
    }

    // "선택한 해당 스테이지"로 이동
    public void GoTo_Mode_Adventure_Selected_Stage(int stage_ID)
    {
        if (LobbySound.instanceLS != null)
            LobbySound.instanceLS.SoundStop();
        
        // 선택한 스테이지가 몇번인지 PlayerPrefs에 기록!
        PlayerPrefs.SetInt("Mode_Adventure_Selected_Stage_ID", stage_ID);

        FadeSlider.gameObject.SetActive(true);
        // 모험모드 씬을 연다
        Invoke("WaitForFadeSlider", 2.0f);
    }
   
    void WaitForFadeSlider()
    {
        SceneManager.LoadScene(3);
    }
    
}
