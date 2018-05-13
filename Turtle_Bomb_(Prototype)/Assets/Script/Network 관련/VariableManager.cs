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
    public byte game_mode = 4;
    public byte map_type = 0;
    public byte map_num = 0;
    //MapManager의 변수
    public byte[] copy_map_info = new byte[225];
    public byte[] bombexplode_list = new byte[225];
    public byte[] firepower_list = new byte[225];
    byte[] roomIDarray = new byte[20]; //0이면 안만들어짐 
    public TB_Room[] roominfo = new TB_Room[20];
    public byte[] team_Turtle = new byte[4];
    public byte[] ready_Turtle = new byte[4];
    public bool forceout = false;
    public float m_time=60.0f;
    public byte myteam;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    public void SetGameMode(int mode)
    {
        game_mode = (byte)mode;
    }
    public void SetMapMode(int mode)
    {
        map_type = (byte)mode;
    }
    public void SetMapNum(int num)
    {
        map_num = (byte)num;
    }
    public void SetTeam(byte mode)
    {
        myteam = (byte)mode;
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
        m_time = 60.0f;
        for (int i = 0; i < 4; ++i)
        {
            team_Turtle[i] = 0;
            people_inRoom[i] = 0;
            ready_Turtle[i] = 0;
        }
        
        //Debug.Log(is_guardian);
    }
    public void F_OutRoom()
    {
        forceout = true;
        m_roomid = 0;
        pos_inRoom = 0;
        pos_guardian = 0;
        is_guardian = 0;
        m_time = 60.0f;
        for (int i = 0; i < 4; ++i)
        {
            team_Turtle[i] = 0;
            people_inRoom[i] = 0;
            ready_Turtle[i] = 0;
        }
    }
    public void ChangeTime(float time)
    {
        m_time = time;
    }
    public void GetReadyState(byte roomid,byte pos, byte ready)
    {
        if (roomid == m_roomid)
        {
            ready_Turtle[pos-1] = ready;
        }
        switch (pos) {
            case 1:
                roominfo[roomid - 1].ready1 = ready;
                break;
            case 2:
                roominfo[roomid - 1].ready2 = ready;
                break;
            case 3:
                roominfo[roomid - 1].ready3 = ready;
                break;
            case 4:
                roominfo[roomid - 1].ready4 = ready;
                break;

        }

    }
    public byte GetRoomNum()
    {
        return m_roomid;
    }
    public void Check_Map(byte[] mapinfo)
    {
        Buffer.BlockCopy(mapinfo, 2, copy_map_info, 0, 225);

    }
    public void Check_BombMap(int x,int z,byte a)
    {
        copy_map_info[(z * 15) + x] = a;
    }
    public void Check_FireMap(int x, int z, byte f)
    {
        firepower_list[(z * 15) + x] = f;
    }
    public void SetRoomState(byte[] b)
    {
        byte[] tempArray = new byte[26];

        Buffer.BlockCopy(b, 0, tempArray, 0, 26);
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
        byte tempmode = tempArray[5];
        roominfo[temproomID-1].roomtype = tempArray[12];
        byte tempmap = tempArray[13];
        byte tempmap_num = tempArray[14];
        roominfo[temproomID - 1].team1 = tempArray[15];
        roominfo[temproomID - 1].team2 = tempArray[16];
        roominfo[temproomID - 1].team3 = tempArray[17];
        roominfo[temproomID - 1].team4 = tempArray[18];
        roominfo[temproomID - 1].ready1 = tempArray[19];
        roominfo[temproomID - 1].ready2 = tempArray[20];
        roominfo[temproomID - 1].ready3 = tempArray[21];
        roominfo[temproomID - 1].ready4 = tempArray[22];

        team_Turtle[0] = tempArray[15];
        team_Turtle[1] = tempArray[16];
        team_Turtle[2] = tempArray[17];
        team_Turtle[3] = tempArray[18];

        game_mode = tempmode;
        map_type = tempmap;
        map_num = tempmap_num;


        if (SceneChange.instance.GetSceneState() == 6 || SceneChange.instance.GetSceneState() == 7)
        {

            GameRoom.instance.SetMode_2(game_mode);
            GameRoom.instance.SetMap_2(map_type);
            GameRoom.instance.SetMapNum2(map_num);

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
        byte[] tempArray = new byte[10];

        Buffer.BlockCopy(b, 0, tempArray, 0, 10);
        m_roomid = tempArray[3];
        pos_inRoom = tempArray[4];
        Debug.Log(pos_inRoom);
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
