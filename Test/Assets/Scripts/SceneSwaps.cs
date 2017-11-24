using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;


public class SceneSwaps : MonoBehaviour {

    
    public Canvas cv;
    

    // Use this for initialization
    void Start () {
        if (cv != null)
        {
            cv.enabled = true;
        }
    }

    public void switchCamera()
    {       
        cv.enabled = !cv.enabled;
    }
    public void GoToStartPage()
    {

        SceneManager.LoadScene(0);

    }
    public void GoToSelectStage()
    {
        SceneManager.LoadScene(1);
    }
    public void GoToSelectStage2()
    {
        Invoke("GoToSelectStage", 3.0f);
    }
    public void GoToStageSelectStage()
    {
        
        SceneManager.LoadScene(2);
    }
    public void SelectStage()
    {
        SceneManager.LoadScene(3);
    }
    public void GameTimeAlways1()
    {
        Time.timeScale = 1;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
