using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameRoom : MonoBehaviour
{


    public static GameRoom instance;
    int ready_num = 0;
    byte my_room_num = 0;
    public byte pos_inRoom = 0;
    byte pos_guard = 0;
    byte amIguard = 0;//0일경우는 유저, 1일 경우 방장
    byte myteam = 0;
    bool pop_loading = false;
    public RawImage loadingimage;
    int mode = 0;
    int map_mode = 0;
    int map_num = 0;
    byte[] people_inRoom = new byte[4];
    public GameObject popup;
    public GameObject kick_notice; //서버에서 차였을 때
    public Text m_text;
    public GameObject[] turtles;
    public byte m_imready;
    public bool load_on;


    //public TextMesh[] turtle_text;
    byte m_ready; //0일 경우 ready x, 1일 경우 ready o
    public GameObject[] crown;
    public GameObject[] readyimage;
    public Text Map_Num_Text;
    public Text m_count_text;
    public Button[] SRButton;
    public RawImage mapImage_now;
    public Texture[] mapImage;
    bool mode_changed = false;
    byte roomtype = 0;
    byte[] team = new byte[4];
    byte clicked_position;
    // Use this for initialization
    void Awake()
    {
        instance = this;
        //Application.LoadLevel(Application.loadedLevel);
        //DontDestroyOnLoad(this);
    }
    void Start()
    {
       
        SetRoomState();
        m_imready = 0;
        pop_loading = false;
        load_on = false;
    }


    public void OutRoom()
    {
        my_room_num = 0;
        pos_inRoom = 0;
        pos_guard = 0;
        amIguard = 0;
        for (int i = 0; i < 4; ++i)
            people_inRoom[i] = 0;

    }
    public byte GetRoomID()
    {

        return my_room_num;
    }
    public void GetReadyState(byte pos, byte ready)
    {
        if (pos_inRoom == pos)
        {
            m_ready = ready;
        }
        else
        {
            return;
        }
    }
    public void StartGame()
    {
        NetTest.instance.SendStartPacket();
        //SceneChange.instance.GoTo_Game_Scene();

    }
    public void Ready()
    {

        if (amIguard == 0)
            NetTest.instance.SendReadyPacket_v2();
        else
        {

        }

    }
    public void ExitRoom()
    {
        //나간다고 패킷 보냄
        NetTest.instance.SendOUTPacket();

    }
    public void Kick_By_Server()
    {
        kick_notice.SetActive(true);
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
        if (tempbool01 && tempbool02 && tempbool03)
        {
            //벤 패킷 전송
            NetTest.instance.SendBanPacket(clicked_position);
        }
    }
    public void Out_By_Server()
    {
        //Debug.Log("Disconnect");

        SceneChange.instance.DisConnect();
    }
    //방 상태 체크 함수
    void SetRoomState()
    {
        my_room_num = VariableManager.instance.m_roomid;
        byte[] temparray = VariableManager.instance.people_inRoom;
        pos_inRoom = VariableManager.instance.pos_inRoom;

        pos_guard = VariableManager.instance.pos_guardian;

        for (int t = 0; t < 4; ++t)
            people_inRoom[t] = VariableManager.instance.people_inRoom[t];
        //Buffer.BlockCopy(temparray, 0, people_inRoom, 0, 4);
        ////Debug.Log(people_inRoom);
        amIguard = VariableManager.instance.is_guardian;
        
        mode = VariableManager.instance.game_mode;
        map_mode = VariableManager.instance.map_type;
        map_num = VariableManager.instance.map_num;
        //for (int t = 0; t < 4; ++t)
        //{
        //    team[t] = VariableManager.instance.team_Turtle[t];
        //}

        team[0] = VariableManager.instance.roominfo[my_room_num-1].team1;
        team[1] = VariableManager.instance.roominfo[my_room_num - 1].team2;
        team[2] = VariableManager.instance.roominfo[my_room_num - 1].team3;
        team[3] = VariableManager.instance.roominfo[my_room_num - 1].team4;

        if (pos_guard == pos_inRoom)
        {
            amIguard = 1;


        }

    }

    //팀변경 함수
    public void TeamChange(int a)
    {

        VariableManager.instance.SetTeam((byte)a);


        NetTest.instance.SendTeamChangePacket();


    }
    //맵 변경 우측 화살표
    public void SetMap()
    {
        if (amIguard == 1)
        {
            int temp = (map_mode + 1) % 3;
            VariableManager.instance.SetMapMode(temp);
            NetTest.instance.SendRoomStateChangePacket();

        }

    }
    public void SetMapMinus()
    {
        if (amIguard == 1)
        {
            if (mode <= 0)
            {

                VariableManager.instance.SetMapMode(2);
                NetTest.instance.SendRoomStateChangePacket();

            }
            else
            {
                int temp = map_mode - 1;
                if (temp < 0)
                    temp = 2;
                VariableManager.instance.SetMapMode(temp);
                NetTest.instance.SendRoomStateChangePacket();
            }

        }

    }

    //모드 변경 함수
    public void SetMode_2(byte m)
    {
        mode = m;
    }
    public void SetMap_2(byte m)
    {
        map_mode = m;
    }
    public void SetMapNum2(byte m)
    {
        map_num = m;
    }
    //모드 변경 우측 화살표
    public void SetMode()
    {
        if (amIguard == 1)
        {
            int temp = (mode + 1) % 5;
            if (temp < 2)
                temp = 2;
            VariableManager.instance.SetGameMode(temp);
            NetTest.instance.SendRoomStateChangePacket();
            mode_changed = true;
        }
        if (mode == 1)
        {

        }
    }

    //모드 변경 좌측 화살표
    public void SetModeMinus()
    {
        if (amIguard == 1)
        {
            if (mode <= 2)
            {

                VariableManager.instance.SetGameMode(4);
                NetTest.instance.SendRoomStateChangePacket();

            }
            else
            {
                int temp = (mode - 1);
                VariableManager.instance.SetGameMode(temp);
                NetTest.instance.SendRoomStateChangePacket();
            }
        }
    }
    public void SetMapNum()
    {
        if (amIguard == 1)
        {
            int temp = (map_num + 1) % 4;
            VariableManager.instance.SetMapNum(temp);
            NetTest.instance.SendRoomStateChangePacket();
        }
    }
    public void SetMapNumMinus()
    {
        if (amIguard == 1)
        {
            if (map_num <= 0)
            {

                VariableManager.instance.SetMapNum(3);
                NetTest.instance.SendRoomStateChangePacket();

            }
            else
            {
                int temp = map_num - 1;
                if (temp < 0)
                    temp = 3;
                VariableManager.instance.SetMapNum(temp);
                NetTest.instance.SendRoomStateChangePacket();
            }
        }
    }
    //게임모드 변경
    void CheckMode()
    {
        mapImage_now.texture = mapImage[map_mode];
        m_count_text.text = mode + "인 플레이";


    }
    // Update is called once per frame
    void Update()
    {
        ////Debug.Log(turtles.Length);
        ready_num = 0;
        int people = 0;
        if (load_on)
        {

        }
        if (pop_loading)
        {
            loadingimage.gameObject.SetActive(true);
        }
        else
        {
            loadingimage.gameObject.SetActive(false);
        }
        turtles[0].SetActive(true);
        for (int i = 1; i < 13; ++i)
        {
            turtles[i].SetActive(false);
        }
        if (SceneChange.instance.GetSceneState() == 6)
        {
            SetRoomState();
            if (amIguard == 1)
            {
                SRButton[0].gameObject.SetActive(true);
                SRButton[1].gameObject.SetActive(false);

            }
            else
            {
                SRButton[1].gameObject.SetActive(true);
                SRButton[0].gameObject.SetActive(false);
            }


            CheckMode();
            if (mode == 0)
            {

                m_text.text = VariableManager.instance.m_roomid + "번 방";

            }
            else
            {

                m_text.text = VariableManager.instance.m_roomid + "번 방";

            }
            if (m_ready == 1)
            {
                readyimage[0].SetActive(true);
            }
            else
            {
                readyimage[0].SetActive(false);
            }
            switch (pos_inRoom)
            {
                case 1:
                    for (int i = 1; i < 4; ++i)
                    {
                        if (VariableManager.instance.ready_Turtle[i] == 1)
                        {
                            ready_num += 1;
                            readyimage[i].SetActive(true);
                        }
                        else
                            readyimage[i].SetActive(false);
                    }
                    break;
                case 2:
                    for (int i = 0; i < 4; ++i)
                    {
                        if (i != 1)
                        {
                            if (i == 0)
                            {
                                if (VariableManager.instance.ready_Turtle[i] == 1)
                                {
                                    ready_num += 1;
                                    readyimage[i + 1].SetActive(true);
                                }
                                else
                                    readyimage[i + 1].SetActive(false);
                            }
                            else
                            {
                                if (VariableManager.instance.ready_Turtle[i] == 1)
                                {
                                    ready_num += 1;
                                    readyimage[i].SetActive(true);
                                }
                                else
                                    readyimage[i].SetActive(false);
                            }
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < 4; ++i)
                    {
                        if (i != 2)
                        {
                            if (i < 2)
                            {
                                if (VariableManager.instance.ready_Turtle[i] == 1)
                                {
                                    ready_num += 1;
                                    readyimage[i + 1].SetActive(true);
                                }
                                else
                                    readyimage[i + 1].SetActive(false);
                            }
                            else
                            {
                                if (VariableManager.instance.ready_Turtle[i] == 1)
                                {
                                    ready_num += 1;
                                    readyimage[i].SetActive(true);
                                }
                                else
                                    readyimage[i].SetActive(false);
                            }
                        }
                    }
                    break;
                case 4:
                    for (int i = 0; i < 3; ++i)
                    {
                        if (VariableManager.instance.ready_Turtle[i] == 1)
                        {
                            ready_num += 1;
                            readyimage[i + 1].SetActive(true);
                        }
                        else
                            readyimage[i + 1].SetActive(false);
                    }
                    break;
            }
            if (ready_num == VariableManager.instance.roominfo[VariableManager.instance.m_roomid - 1].people_count - 1)
            {
                SRButton[0].interactable = true;
            }
            else
            {
                SRButton[0].interactable = false;
            }
            if (amIguard == 1)
            {
                crown[1].gameObject.SetActive(false);
                crown[2].gameObject.SetActive(false);
                crown[3].gameObject.SetActive(false);
                crown[0].gameObject.SetActive(true);
            }
            else
            {
                crown[0].gameObject.SetActive(false);
                switch (pos_inRoom)
                {
                    case 1:
                        crown[pos_guard - 1].gameObject.SetActive(true);
                        break;
                    case 2:
                        if (pos_guard < 2)
                        {
                            crown[1].gameObject.SetActive(true);

                            crown[2].gameObject.SetActive(false);
                            crown[3].gameObject.SetActive(false);
                        }
                        else
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                crown[i].gameObject.SetActive(false);
                            }
                            crown[pos_guard - 1].gameObject.SetActive(true);
                        }

                        break;
                    case 3:
                        if (pos_guard > 3)
                        {
                            crown[3].gameObject.SetActive(true);
                            crown[1].gameObject.SetActive(false);
                            crown[2].gameObject.SetActive(false);
                            crown[0].gameObject.SetActive(false);
                        }
                        else
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                crown[i].gameObject.SetActive(false);
                            }
                            crown[pos_guard].gameObject.SetActive(true);
                        }
                        break;
                    case 4:
                        for (int i = 0; i < 4; ++i)
                        {
                            crown[i].gameObject.SetActive(false);
                        }
                        crown[pos_guard].gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
            }

            Map_Num_Text.text = "" + (map_num + 1);
            for (byte i = 0; i < 4; ++i)
            {
                if (pos_inRoom - 1 != i) //내가 아닌 유저 찾기
                {
                    people++;
                }

                if (VariableManager.instance.people_inRoom[i] != 0 && pos_inRoom - 1 != i)
                {
                    if (people < 4)
                    {
                        int tempteamcolor = team[i];
                        //Debug.Log(i+"th Color:"+tempteamcolor);
                        turtles[people + (tempteamcolor * 3)].SetActive(true);
                    }
                    ////Debug.Log(people + "th On!!");


                }
                else if (VariableManager.instance.people_inRoom[i] == 0)
                {
                    if (people < 4)
                    {
                        turtles[people].SetActive(false);
                        turtles[people+3].SetActive(false);
                        turtles[people+6].SetActive(false);
                        turtles[people+9].SetActive(false);

                    }
                }
            }
            turtles[0].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);


        }
    }
}
