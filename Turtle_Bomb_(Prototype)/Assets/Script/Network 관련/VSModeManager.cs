using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VSModeManager : MonoBehaviour {
    byte win_or_lose = 0; //1이면 win, 2면 lose
    public static VSModeManager instance;
    public RawImage winner_image;
    public RawImage loser_image;
    public RawImage draw_image;
    bool m_isClicked = false;
    public bool game_set;
    public Text time_Text;
    float time;
    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start () {
        time = 60.0f;
        win_or_lose = 0;
        game_set = false;
    }
    public void isClicked()
    {
        m_isClicked = true;
    }
    public void isClickedOff()
    {
        m_isClicked = false;
    }

    public bool Get_isClicked()
    {
        return m_isClicked;
    }
    public void GameOver_Set(byte id)
    {
        if (Turtle_Move.instance.GetId() == id)
        {
            win_or_lose = 1;
            game_set = true;
        }
        else if (id == 4)
        {
            win_or_lose = 3;
            game_set = true;
        }
        else
        {
            win_or_lose = 2;
            game_set = true;
        }

    }

    public void ChangeTime(float a)
    {
        time = a;
        Debug.Log("Time" + (time));
    }
	// Update is called once per frame
	void Update () {
        time = VariableManager.instance.m_time;
        if (time < 0)
            time = 0;
        if(time%60>=10)
            time_Text.text = "0"+(int)time/60+":"+ (int)time%60;
        else
            time_Text.text = "0" + (int)time / 60 + ":0" + (int)time % 60;
        if (win_or_lose == 1)
        {

            winner_image.gameObject.SetActive(true);
            win_or_lose = 0;
            Debug.Log("Win!!!!!!");
            //Time.timeScale = 0;

        }
        else if (win_or_lose == 2)
        {
            
            loser_image.gameObject.SetActive(true);
            win_or_lose = 0;
            Debug.Log("Lose!!!!!!");
            //Time.timeScale = 0;
            
        }
        else if (win_or_lose == 3)
        {
            draw_image.gameObject.SetActive(true);
            win_or_lose = 0;
        }
    }
    
}
