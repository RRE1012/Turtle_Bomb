using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VSModeManager : MonoBehaviour {
    byte win_or_lose = 0; //1이면 win, 2면 lose
    public static VSModeManager instance;
    public RawImage winner_image;
    public RawImage loser_image;
    public bool game_set;
    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start () {
        win_or_lose = 0;
        game_set = false;
    }
	public void GameOver_Set(byte id)
    {
        if (Turtle_Move.instance.GetId() == id)
        {
            win_or_lose = 1;
            game_set = true;
        }
        else
        {
            win_or_lose = 2;
            game_set = true;
        }

    }
	// Update is called once per frame
	void Update () {
        if (win_or_lose == 1)
        {
            
            winner_image.gameObject.SetActive(true);
            win_or_lose = 0;
            Debug.Log("Win!!!!!!");
            Time.timeScale = 0;
            
        }
        if (win_or_lose == 2)
        {
            
            loser_image.gameObject.SetActive(true);
            win_or_lose = 0;
            Debug.Log("Lose!!!!!!");
            Time.timeScale = 0;
            
        }
    }
    
}
