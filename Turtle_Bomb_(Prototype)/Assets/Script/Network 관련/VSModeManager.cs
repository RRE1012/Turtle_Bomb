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
    public GameObject m_Option_UI;
    bool m_isClicked = false;
    public bool game_set;
    public Text time_Text;
    public Text m_GetItemText;
    public RawImage m_GetItemBackground; // 아이템 획득시 출력 이미지
    public RawImage m_GetItemImage; // 아이템 획득시 출력 이미지
    public Animator m_GetItem_Animator; // 아이템 획득 UI 애니메이터
    public Texture m_Bomb_Icon;
    public Texture m_Fire_Icon;
    public Texture m_Speed_Icon;
    public Texture m_Kick_Icon;
    public Texture m_Throw_Icon;
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
        GetItemUI_Deactivate();
    }
    void GetItemUI_Deactivate()
    {
        m_GetItem_Animator.SetBool("is_Got_Item", false);
        m_GetItemBackground.gameObject.SetActive(false);
        m_GetItemText.gameObject.SetActive(false);
        m_GetItemImage.gameObject.SetActive(false);
    }
    public void isClicked()
    {
        m_isClicked = true;
    }
    public void isClickedOff()
    {
        m_isClicked = false;
    }
    public void OptionOn()
    {
        m_Option_UI.SetActive(true);
    }
    public void OptionOff()
    {
        m_Option_UI.SetActive(false);
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
        //Debug.Log("Time" + (time));
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
            //Debug.Log("Win!!!!!!");
            //Time.timeScale = 0;

        }
        else if (win_or_lose == 2)
        {
            
            loser_image.gameObject.SetActive(true);
            win_or_lose = 0;
            //Debug.Log("Lose!!!!!!");
            //Time.timeScale = 0;
            
        }
        else if (win_or_lose == 3)
        {
            draw_image.gameObject.SetActive(true);
            win_or_lose = 0;
        }
    }

    public void GetItemUI_Activate(int Item_Num)
    {
        switch (Item_Num)
        {
            case 0:
                m_GetItemText.text = "Bomb Up !";
                m_GetItemImage.texture = m_Bomb_Icon;
                break;

            case 1:
                m_GetItemText.text = "Fire Up !";
                m_GetItemImage.texture = m_Fire_Icon;
                break;

            case 2:
                m_GetItemText.text = "Speed Up !";
                m_GetItemImage.texture = m_Speed_Icon;
                break;

            case 3:
                m_GetItemText.text = "Kick Activated !";
                m_GetItemImage.texture = m_Kick_Icon;
                break;

            case 4:
                m_GetItemText.text = "Throw Activated !";
                m_GetItemImage.texture = m_Throw_Icon;
                break;

            case 5:
                m_GetItemText.text = "You've Got AirDrop !!";
                m_GetItemImage.texture = m_Bomb_Icon;
                break;
        }

        //Stat_UI_Management(); // 스탯 갱신


        m_GetItemBackground.gameObject.SetActive(true);
        m_GetItemText.gameObject.SetActive(true);
        m_GetItemImage.gameObject.SetActive(true);
        m_GetItem_Animator.SetBool("is_Got_Item", true);
        Invoke("GetItemUI_Deactivate", 1.4f);
    }
}
