using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

//씬 전환 함수
public class SceneChange : MonoBehaviour
{

    public Canvas cv;
    public RawImage FadeSlider;
    static public SceneChange instance;
    int scene = 8;
    bool swap_scene = false;
    // Use this for initialization

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        if (cv != null)
        {
            cv.enabled = true;
        }
        StartCoroutine("SceneSwap");
        GoTo_Connect_Scene();
    }

    public int GetSceneState()
    {
        return scene;
    }
    public void GoTo_Connect_Scene()
    {
        //SceneManager.LoadScene(0);
        scene = 4;
        swap_scene = true;
    }
    public void GoTo_Connect_Scene_Coop()
    {
        //SceneManager.LoadScene(0);
        scene = 10;
        swap_scene = true;
    }
    public void GoTo_Matching_Scene()
    {
        //SceneManager.LoadScene(0);
        scene = 11;
        swap_scene = true;
    }
    public void GoTo_CoopBoss_Scene()
    {
        //SceneManager.LoadScene(0);
        scene = 12;
        swap_scene = true;
    }

    // "타이틀 화면"으로 이동
    public void GoTo_Wait_Scene()
    {
        //SceneManager.LoadScene(0);
        scene = 5;
        swap_scene = true;
    }

    // "모드 선택 화면"으로 이동
    public void GoTo_ModeSelect_Scene()
    {
        ////Debug.Log("Clicked");
        scene = 6;
        swap_scene = true;

        //SceneManager.LoadScene(1);
    }

    // "모험모드 스테이지 선택 화면"으로 이동
    public void GoTo_Game_Scene()
    {
        scene = 7;

        swap_scene = true;

        //SceneManager.LoadScene(2);
    }
    public void GoTo_Select_Scene()
    {
        scene = 1;
        swap_scene = true;
    }
    // "선택한 해당 스테이지"로 이동
    public void GoTo_Mode_Adventure_Selected_Stage(int stage_ID)
    {
        //if (LobbySound.instanceLS != null)
        //  LobbySound.instanceLS.SoundStop();

        // 선택한 스테이지가 몇번인지 PlayerPrefs에 기록!
        PlayerPrefs.SetInt("Mode_Adventure_Selected_Stage_ID", stage_ID);

        FadeSlider.gameObject.SetActive(true);
        // 모험모드 씬을 연다
        //Invoke("WaitForFadeSlider", 2.0f);
    }

    void WaitForFadeSlider()
    {
        SceneManager.LoadScene(7);
    }
    IEnumerator SceneSwap()
    {
        for (; ; )
        {
            if (swap_scene)
            {
                SceneManager.LoadScene(scene);
                swap_scene = false;
                ////Debug.Log("Go!!");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void DisConnect()
    {

        SceneManager.LoadScene(1);
    }
}
