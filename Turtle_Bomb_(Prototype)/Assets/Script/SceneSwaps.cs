using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

//씬 전환 함수
public class SceneSwaps : MonoBehaviour {

    
    public Canvas cv;
    
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
    //0번째 씬으로 이동(타이틀화면)
    public void GoToStartPage()
    {
        SceneManager.LoadScene(0);
    }
    //1번째 씬으로 이동(모드선택화면)
    public void GoToSelectStage()
    {
        SceneManager.LoadScene(1);
    }

    //3초후에 1번째 씬으로 이동(사용X)
    public void GoToSelectStage2()
    {
        Invoke("GoToSelectStage", 3.0f);
    }
    //2번째 씬으로 이동(스테이지선택화면) 
    public void GoToStageSelectStage()
    {    
        SceneManager.LoadScene(2);
    }
    //로비사운드 정지 +3번째 씬으로 이동(게임화면)
    public void SelectStage()
    {
        LobbySound.instanceLS.SoundStop();
        SceneManager.LoadScene(3);
    }
   
    
}
