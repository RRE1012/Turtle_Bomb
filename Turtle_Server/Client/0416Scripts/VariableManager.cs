using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableManager : MonoBehaviour {
    public static VariableManager instance;
    public byte m_accessid=254;
    public byte m_roomid = 0;
    public byte[] people_inRoom = new byte[4];
    public byte pos_inRoom = 0;
    public byte pos_guardian = 0;
    public byte is_guardian = 0;
    
    //MapManager의 변수
    public byte[] copy_map_info = new byte[225];
    public byte[] bombexplode_list = new byte[225];
    byte[] roomIDarray = new byte[20]; //0이면 안만들어짐 
    public TB_Room[] roominfo = new TB_Room[20];
    public bool forceout = false;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    // Use this for initialization
    void Start () {
		
	}
    public void SetID(byte a)
    {
        m_accessid = a;
    }
    public void OutRoom()
    {
        m_roomid = 0;
        pos_inRoom = 0;
        pos_guardian = 0;
        is_guardian = 0;
    }
    public void F_OutRoom()
    {
        forceout = true;
        m_roomid = 0;
        pos_inRoom = 0;
        pos_guardian = 0;
        is_guardian = 0;
    }
  
    public byte GetRoomNum()
    {
        return m_roomid;
    }
    public void Check_Map(byte[] mapinfo)
    {
        Buffer.BlockCopy(mapinfo, 2, copy_map_info, 0, 225);

    }
    public void SetRoomState(byte[] b)
    {
        byte[] tempArray = new byte[20];

        Buffer.BlockCopy(b, 0, tempArray, 0, 20);
        //Debug.Log("BlockCopy Completed");
        byte temproomID = tempArray[2];
        roominfo[temproomID - 1].roomID = temproomID;
        roominfo[temproomID - 1].people_count = tempArray[3];
        roominfo[temproomID - 1].game_start = tempArray[4];
        roominfo[temproomID - 1].people_max = tempArray[5];
        roominfo[temproomID - 1].made = tempArray[6];
        roominfo[temproomID - 1].guardian_pos = tempArray[7];
        roominfo[temproomID - 1].people1 = tempArray[8];
        roominfo[temproomID - 1].people2 = tempArray[9];
        roominfo[temproomID - 1].people3 = tempArray[10];
        roominfo[temproomID - 1].people4 = tempArray[11];

        if (SceneChange.instance.GetSceneState() == 2)
        {
            bool tempbool = temproomID == GameRoom.instance.GetRoomID();
            if (tempbool)
            {
                pos_guardian = tempArray[7];
                for (byte i = 0; i < 4; ++i)
                {
                    people_inRoom[i] = tempArray[8 + i];
                }
            }
        }
        //Debug.Log("SEtRoomState Completed");
    }
    public void SetRoomState_Respond(byte[] b)
    {
        byte[] tempArray = new byte[12];

        Buffer.BlockCopy(b, 0, tempArray, 0, 12);
        m_roomid = tempArray[3];
        pos_inRoom = tempArray[4];
        pos_guardian = tempArray[5];
        for (byte i = 0; i < 4; ++i)
        {
            people_inRoom[i] = tempArray[6 + i];
        }
    }
    public void SetRoomCreate_Respond(byte[] b)
    {
        byte[] tempArray = new byte[4];
        people_inRoom[0] = NetTest.instance.GetId();
        pos_inRoom = 1;
        is_guardian = 1;
        pos_guardian = 1;
        m_roomid = b[3];
    }
    // Update is called once per frame
    void Update () {
        
	}
}
