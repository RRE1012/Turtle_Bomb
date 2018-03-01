using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

//씬 전환 함수
public class SceneSwaps : MonoBehaviour {

    public Canvas cv;

    int m_Selected_Theme_Number;
    int m_Selected_Stage_Number;

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
    public void GoTo_Mode_Adventure_Selected_Stage(string theme_and_stage_Num)
    {
        if (LobbySound.instanceLS != null)
            LobbySound.instanceLS.SoundStop();

        string tempSTRING = "";
        int theme_num = 0;
        int stage_num = 0;

        for (int i = 0; i < theme_and_stage_Num.Length; ++i)
        {
            if (theme_and_stage_Num[i] == '-')
            {
                theme_num = Convert.ToInt32(tempSTRING);
                tempSTRING = "";
            }
            else
            {
                tempSTRING += theme_and_stage_Num[i];
            }
        }

        stage_num = Convert.ToInt32(tempSTRING);



        PlayerPrefs.SetInt("Mode_Adventure_Selected_Theme_Number", theme_num);
        PlayerPrefs.SetInt("Mode_Adventure_Selected_Stage_Number", stage_num);

        SceneManager.LoadScene(3);
    }
   
    
}
