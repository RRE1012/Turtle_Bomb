using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class CoOpManager : MonoBehaviour {
    public static CoOpManager instance;
    public GameObject myturtle;
    public GameObject[] m_net_user;

    public GameObject gameover_Image;
    public GameObject winner_Image;
    bool m_isClicked = false;

    public Text time_Text;

    bool win_game=false;
    bool lose_game = false;
    float time;
    public Text m_GetItemText;
    public RawImage m_GetItemBackground; // 아이템 획득시 출력 이미지
    public RawImage m_GetItemImage; // 아이템 획득시 출력 이미지
    public Animator m_GetItem_Animator; // 아이템 획득 UI 애니메이터
    public Texture m_Bomb_Icon;
    public Texture m_Fire_Icon;
    public Texture m_Speed_Icon;
    public Texture m_Kick_Icon;
    public Texture m_Throw_Icon;
    public Text[] m_idtext;
    public Text[] m_damagepercent_text;
    // Use this for initialization
    void Awake()
    {
        instance = this;
    }
    void Start () {
        //m_Attack_Collider = transform.Find("Attack_Range");
        // 비활성화
        win_game = false;
        lose_game = false;
        time = 0;
        winner_Image.SetActive(false);
    
     
        gameover_Image.SetActive(false);
        m_net_user[0].transform.position = new Vector3(0.0f, -0.35f, 0.0f);
        m_net_user[1].transform.position = new Vector3(28.0f, -0.35f, 0.0f);
        m_net_user[2].transform.position = new Vector3(0.0f, -0.35f, 28.0f);
        m_net_user[3].transform.position = new Vector3(28.0f, -0.35f, 28.0f);
       
        for (int i=0;i< VariableManager_Coop.instance.howmany;i++)
        {
            m_net_user[i].SetActive(true);
        }
        switch (VariableManager_Coop.instance.pos_id)
        {
            case 1:
                m_net_user[0].SetActive(false);
                break;
            case 2:
                m_net_user[1].SetActive(false);
                break;
            case 3:
                m_net_user[2].SetActive(false);
                break;
            case 4:
                m_net_user[3].SetActive(false);
                break;
            default:
                break;

        }
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
    void GetItemUI_Deactivate()
    {
        m_GetItem_Animator.SetBool("is_Got_Item", false);
        m_GetItemBackground.gameObject.SetActive(false);
        m_GetItemText.gameObject.SetActive(false);
        m_GetItemImage.gameObject.SetActive(false);
    }
   
    

	void SetAniFalse(int char_id,int ani_state)
    {
        switch (ani_state)
        {
            case 1:
                //걷는 애니 구현
                m_net_user[char_id].GetComponent<Animator>().SetBool("TurtleMan_isWalk", false);
                

                break;
            case 2:
                //던지는 애니 구현
                m_net_user[char_id].GetComponent<Animator>().SetBool("TurtleMan_isThrow", false);
                
                break;
            case 3:
                //차는 애니 구현
                m_net_user[char_id].GetComponent<Animator>().SetBool("TurtleMan_isKick", false);
                
                break;
            case 4:
                //미는 애니 구현
                m_net_user[char_id].GetComponent<Animator>().SetBool("TurtleMan_isPush", false);
                
                break;
            case 5:
                //죽는 애니 구현
                m_net_user[char_id].GetComponent<Animator>().SetBool("TurtleMan_isDead", false);
                
                break;

        }
    }
    public void GameOver_Set(byte win, byte winner)
    {
        if (win == 1)
        {
            win_game = true;
        }
        else{
            lose_game = true;
        }
    }
    public void ExitRoom()
    {
        SceneChange.instance.GoTo_Matching_Scene();

    }
    // Update is called once per frame
    void Update () {

        //Set_Boss_HP_Bar();
        //m_damagepercent_text[0].text = "HP: "+Boss_AI_Coop.instance.hp;

        if (win_game)
        {
            winner_Image.SetActive(true);
        }
        if (lose_game)
        {
            gameover_Image.SetActive(true);
        }

        m_idtext[0].text = VariableManager_Coop.instance.id_list[0];
        m_idtext[1].text = VariableManager_Coop.instance.id_list[1];
        m_idtext[2].text = VariableManager_Coop.instance.id_list[2];
        m_idtext[3].text = VariableManager_Coop.instance.id_list[3];

        //Debug.Log(boss.transform.rotation.y);
        time = VariableManager_Coop.instance.m_time;
        if (time <= 0)
        {
            time_Text.text = "00:00";
            time = 0;
        }
        else if (time % 60 >= 10)
            time_Text.text = "0" + (int)time / 60 + ":" + (int)time % 60;
        else
        {
            time_Text.text = "0" + (int)time / 60 + ":0" + (int)time % 60;
        }
       
      //  Debug.Log("rotationY"+boss_rotY);
        //boss.transform.rotation= new Quaternion(boss.transform.rotation.x,boss_rotY, boss.transform.rotation.z, boss.transform.rotation.w);
        for (int i=0;i< VariableManager_Coop.instance.howmany; i++)
        {

            if (m_net_user[i].activeInHierarchy)
            {
                m_net_user[i].transform.position =new Vector3(NetManager_Coop.instance.m_chardata[i].x, -0.35f, NetManager_Coop.instance.m_chardata[i].z);
                //m_net_user[i].transform.rotation = new Quaternion(m_net_user[i].transform.rotation.x, NetManager_Coop.instance.m_chardata[i].rotateY, m_net_user[i].transform.rotation.z, m_net_user[i].transform.rotation.w);
                m_net_user[i].transform.eulerAngles = new Vector3(0, NetManager_Coop.instance.m_chardata[i].rotateY,0);
                switch (NetManager_Coop.instance.m_chardata[i].ani_state)
                {
                    case 1:
                        //걷는 애니 구현
                        m_net_user[i].GetComponent<Animator>().SetBool("TurtleMan_isWalk", true);
                        NetManager_Coop.instance.m_chardata[i].ani_state = 0;
                        //SetAniFalse(i, 1);
                        break;
                    case 2:
                        //던지는 애니 구현
                        m_net_user[i].GetComponent<Animator>().SetBool("TurtleMan_isThrow", true);
                        NetManager_Coop.instance.m_chardata[i].ani_state = 0;
                        //SetAniFalse(i, 2);
                        break;
                    case 3:
                        //차는 애니 구현
                        m_net_user[i].GetComponent<Animator>().SetBool("TurtleMan_isKick", true);
                        NetManager_Coop.instance.m_chardata[i].ani_state = 0;
                        //SetAniFalse(i, 3);
                        break;
                    case 4:
                        //미는 애니 구현
                        m_net_user[i].GetComponent<Animator>().SetBool("TurtleMan_isPush", true);
                        NetManager_Coop.instance.m_chardata[i].ani_state = 0;
                       // SetAniFalse(i, 4);
                        break;
                    case 5:
                        //죽는 애니 구현
                        m_net_user[i].GetComponent<Animator>().SetBool("TurtleMan_isDead", true);
                        NetManager_Coop.instance.m_chardata[i].ani_state = 0;
                        //SetAniFalse(i, 5);
                        break;
                    default:
                        break;

                }
            }
        }
	}
}
