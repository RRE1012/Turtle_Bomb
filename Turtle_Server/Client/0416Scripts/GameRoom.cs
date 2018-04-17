using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoom : MonoBehaviour {
    public static GameRoom instance;
    byte my_room_num = 0;
    public byte pos_inRoom=0;
    byte pos_guard=0;
    byte amIguard = 0;
    byte[] people_inRoom = new byte[4];
    public GameObject popup;
    public Text m_text;
    public GameObject[] turtles;
    public TextMesh[] turtle_text;
    byte m_ready; //0일 경우 ready x, 1일 경우 ready o
    byte m_guardian; //0일경우는 유저, 1일 경우 방장
    byte clicked_position;
	// Use this for initialization
    void Awake()
    {
        instance = this;
        //Application.LoadLevel(Application.loadedLevel);
        //DontDestroyOnLoad(this);
    }
	void Start () {
        SetRoomState();


    }
    public byte GetRoomID()
    {

        return my_room_num;
    }
    
    public void StartGame()
    {
        NetTest.instance.SendStartPacket();
        //SceneChange.instance.GoTo_Game_Scene();

    }
    public void ExitRoom()
    {
        //나간다고 패킷 보냄
        NetTest.instance.SendOUTPacket();

    }

    public void Popup()
    {
        popup.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        popup.SetActive(true);
    }
    public void PopupCancel()
    {
        popup.SetActive(false);
    }
    public void SetPos1()
    {
        clicked_position = 1;
    }
    public void SetPos2()
    {
        clicked_position = 2;
    }
    public void SetPos3()
    {
        clicked_position = 3;
    }
    public void SetPos4()
    {
        clicked_position = 4;
    }
    public void BanUser()
    {
        bool tempbool01 = people_inRoom[clicked_position - 1] != 0;
        bool tempbool02 = amIguard == 1;
        bool tempbool03 = clicked_position != pos_guard;
        if (tempbool01&&tempbool02&&tempbool03)
        {
            //벤 패킷 전송
            NetTest.instance.SendBanPacket(clicked_position);
        }
    }
    void SetRoomState()
    {
        
        byte[] temparray = VariableManager.instance.people_inRoom;
        pos_inRoom = VariableManager.instance.pos_inRoom;
        //Debug.Log(pos_inRoom);
        pos_guard = VariableManager.instance.pos_guardian;
        Buffer.BlockCopy(temparray, 0, people_inRoom, 0, 4);
        amIguard = VariableManager.instance.is_guardian;
        my_room_num = VariableManager.instance.m_roomid;


    }
    // Update is called once per frame
    void Update () {
        //Debug.Log(turtles.Length);
        if (SceneChange.instance.GetSceneState() == 2)
        {
            SetRoomState();
            
            m_text.text = "My position : " + VariableManager.instance.pos_inRoom;
            for (byte i = 0; i < turtles.Length; ++i)
            {
                if (people_inRoom[i] != 0)
                {
                    
                    turtles[i].SetActive(true);
                    
                    if (people_inRoom[i] == 1)
                    {
                        turtle_text[i].text = "ID : " + people_inRoom[i];
                    }
                    else if (people_inRoom[i] == 2)
                    {
                        //    turtle_text[i].text = "ID : " + people_inRoom[i]+"\n M A S T E R";
                    }
                    turtle_text[i].text = "ID : " + people_inRoom[i];

                    //Debug.Log("T.ID : "+people_inRoom[i]);
                }
                else
                {
                    turtles[i].SetActive(false);
                }
            }
            turtles[pos_inRoom - 1].transform.localScale = new Vector3(3, 3, 3);
            if (pos_guard != 0)
                turtle_text[pos_guard - 1].text = "ID : " + people_inRoom[pos_guard - 1] + "\n M A S T E R";
        }
    }
}
