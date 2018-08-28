using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine;

public class NetManager_Coop : MonoBehaviour {
    public static NetManager_Coop instance = null;
    int bomb_posx;
    int bomb_posz;
    byte[] m_bomb_posx = new byte[4];
    byte[] m_bomb_posz = new byte[4];
    byte[] m_turtle_posx = new byte[4];
    byte[] m_turtle_posz = new byte[4];
    byte[] m_turtle_roty = new byte[4];
    bool m_set_bomb = false;
    bool m_is_move = false;
    bool m_ingame = false;
    float deltaTime = 0.0f;
    public static float time_Second = 30.0f;
    private string m_address = "13.124.255.57";
    byte access_id = 254;
    byte game_id = 254;
    private const int m_port = 9000;
    private const int m_packetSize = 4000;
    private Socket m_socket = null;
    private bool m_isConnected = false;
    public Thread m_thread = null;
    public bool m_threadLoop = false;
    public ClientID client_id;
    byte firepower;
    byte[] m_firepower = new byte[1];
    // 송신 버퍼.
    private Queue m_sendQueue = new Queue();
    int remain_size = 0;
    // 수신 버퍼.
    private Queue m_recvQueue = new Queue();
    Byte[] temp_buffer = new Byte[m_packetSize];
    // 수신한 데이터를 복사할 데이터
    Byte[] copy_data = new Byte[m_packetSize];

    // 수신 데이터
    Byte[] recv_data = new Byte[m_packetSize];

    public CharInfo[] m_chardata = new CharInfo[4];
    // Use this for initialization
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        //m_address = Client_IP();
        //Debug.Log("This is CoopMode");
        InitializePos();
        client_id.size = 15;
        Connect();
        StartCoroutine("SendTester");
        //어플리케이션 프레임레이트를 30으로
        //Application.targetFrameRate = 30;
        //StartCoroutine("DebugTest");
    }

    public void InitializePos()
    {
        m_chardata[0].x = 0;
        m_chardata[0].z = 0;
        m_chardata[0].rotateY = 0;
        m_chardata[1].x = 28;
        m_chardata[1].z = 0;
        m_chardata[1].rotateY = 0;
        m_chardata[2].x = 0;
        m_chardata[2].z = 28;
        m_chardata[2].rotateY = 180;
        m_chardata[3].x = 28;
        m_chardata[3].z = 28;
        m_chardata[3].rotateY = 180;

    }
    public string Client_IP()
    {
        //dns.gethostentry = 호스트ip주소를 확인
        //dns.gethostname - 로컬컴퓨터의 호스트 이름을 return
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        //임시변수 초기화
        string ClientIP = string.Empty;
        for (int i = 0; i < host.AddressList.Length; i++)
        {
            if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
            {
                ClientIP = host.AddressList[i].ToString();
            }
        }
        return ClientIP;
    }
    public byte GetId()
    {
        return access_id;
    }

    public byte GetGameId()
    {
        return game_id;
    }
    public void Connect()
    {
        try
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
            IPAddress serverIP = System.Net.IPAddress.Parse(m_address);
            IPEndPoint ipEndPoint = new System.Net.IPEndPoint(serverIP, m_port);
            //m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
            // m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
            m_socket.Connect(ipEndPoint);
            m_socket.NoDelay = true;

            m_isConnected = true;

            m_socket.SendBufferSize = 0;

            //byte[] recvBuffer = new byte[m_packetSize];
            m_socket.BeginReceive(this.recv_data, 0, recv_data.Length, SocketFlags.None,
                             new AsyncCallback(OnReceiveCallBack), m_socket);
            SceneChange.instance.GoTo_Matching_Scene();

        }
        catch (SocketException e)
        {
            m_socket = null;
            m_isConnected = false;
            Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
            ////Debug.Log("Socket Connect Error.");
            ////Debug.Log("End client communication.");
        }
        if (m_socket == null)
        {
            Disconnect();
        }


    }


    public void Disconnect()
    {
        m_isConnected = false;

        if (m_socket != null)
        {
            // 소켓 닫기.
            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
            m_socket = null;
            //DestroyThread();
        }
    }
    private void SendCallBack(IAsyncResult IAR)
    {
        string message = (string)IAR.AsyncState;
    }

    public void BeginSend(byte[] buffer)
    {
        try
        {
            // 연결 성공시
            if (m_socket.Connected)
            {
                m_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                      new AsyncCallback(SendCallBack), buffer);
            }
        }
        catch (SocketException e)
        {
            ////Debug.Log("전송 에러 : " + e.Message);
        }
    }

    public void Receive()
    {
        //////Debug.Log("Recv!!");
        ////Debug.Log("Receive Try");
        m_socket.BeginReceive(this.recv_data, 0, recv_data.Length, SocketFlags.None,
                             new AsyncCallback(OnReceiveCallBack), m_socket);
    }
    private void OnReceiveCallBack(IAsyncResult IAR)
    {
        try
        {
            Socket tempSock = (Socket)IAR.AsyncState;
            int nReadSize = tempSock.EndReceive(IAR);

            Buffer.BlockCopy(recv_data, 0, copy_data, 0 + remain_size, nReadSize);
            remain_size = remain_size + nReadSize;
            if (nReadSize != 0)
            {
                ProcessPacket(copy_data);

            }
            if (remain_size >= 2)
            {
                ProcessPacket(copy_data);
            }
            this.Receive();
        }
        catch (SocketException se)
        {
            if (se.SocketErrorCode == SocketError.ConnectionReset)
            {
                //Debug.Log(se);
                //this.BeginConnect();
            }
        }
    }
    //버퍼를 교체한다.
    void SwapBuffer(byte size)
    {
        int temp = size;
        //Debug.Log("Delete Size: "+temp+ " ");
        Array.Clear(temp_buffer, 0, temp_buffer.Length);
        //Debug.Log("Clear Temp Array");
        remain_size = remain_size - temp;
        Buffer.BlockCopy(copy_data, temp, temp_buffer, 0, remain_size);
       //Debug.Log("Copy to Temp Array");

        Array.Clear(copy_data, 0, copy_data.Length);
        //Debug.Log("Clear Copy Array");
        Buffer.BlockCopy(temp_buffer, 0, copy_data, 0, temp_buffer.Length);
        //Debug.Log("Swap Array");
        //Debug.Log("Remain_Size2 : " + remain_size);

    }

    void ProcessPacket(byte[] recv_buff)
    {
        ////Debug.Log("Process P");
        while (remain_size >= 2)
        {
            //Debug.Log("This Data : " + copy_data[1] + "\nThis Size:" + copy_data[0]);
            switch (copy_data[1])
            {
                case (byte)PacketInfo.ClientID:
                    {
                        access_id = copy_data[2];
                        VariableManager_Coop.instance.SetID(copy_data[2]);
                        //////Debug.Log("Get Id!! : " + (byte)access_id);


                        SwapBuffer(copy_data[0]);
                        //////Debug.Log("Remain_Size : " + remain_size);
                    }
                    break;
                case (byte)PacketInfo.CharPos:
                    //다른 유저 위치정보를 전송
                    {
                        byte tempid = copy_data[2];
                        m_chardata[tempid].ani_state = copy_data[3];
                        //Turtle_Move_Coop.instance.Move_Case(tempid);
                        m_chardata[tempid].x = BitConverter.ToSingle(recv_buff, 10);
                        m_chardata[tempid].z = BitConverter.ToSingle(recv_buff, 14);
                        m_chardata[tempid].rotateY = BitConverter.ToSingle(recv_buff, 18);

                        SwapBuffer(copy_data[0]);
                    }
                    //Debug.Log("Get CharPos");
                    break;

                case (byte)PacketInfo.BombPos:
                    //폭탄정보 전송(터지는 폭탄)
                    byte tempid2 = copy_data[2];
                    MapManager.instance.Check_Map(recv_buff);
                    //SetCharPos(recv_buff);
                    SwapBuffer(copy_data[0]);
                    //////Debug.Log("Remain_Size : " + remain_size);
                    break;
                case (byte)PacketInfo.BombExplode:
                    //폭탄정보 전송(터지는 폭탄)
                    {

                        byte reloadid = copy_data[6];
                        Turtle_Move_Coop.instance.ReloadBomb(reloadid);
                        int tempx = BitConverter.ToInt32(copy_data, 7);
                        int tempz = BitConverter.ToInt32(copy_data, 11);

                        byte[] tempfire_array = new byte[4];
                        Buffer.BlockCopy(copy_data, 2, tempfire_array, 0, 4);
                        //Debug.Log("UpFire : " + tempfire_array[0]);
                       // Debug.Log("RightFire : " + tempfire_array[1]);
                        //Debug.Log("DownFire : " + tempfire_array[2]);
                        //Debug.Log("LeftFire : " + tempfire_array[3]);
                        MapManager_COop.instance.Explode_Bomb_v2(tempx, tempz, tempfire_array);
                        SwapBuffer(copy_data[0]);
                    }
                    ///////Debug.Log("Explode Bomb : " + remain_size);
                    break;
                
                case (byte)PacketInfo.MapData:
                    //////Debug.Log("Get MapData!! : ");
                    VariableManager_Coop.instance.Check_Map(copy_data);
                    SwapBuffer(copy_data[0]);

                    break;

                case (byte)PacketInfo.ItemData:
                    byte tempidt = copy_data[2];
                    byte tempitemt = copy_data[3];

                    Turtle_Move_Coop.instance.SetItem_Ability(tempidt, tempitemt);
                    SwapBuffer(copy_data[0]);
                    break;
                case (byte)PacketInfo.DeadNotice:
                    byte tempid_d = copy_data[2];
                    Turtle_Move_Coop.instance.Dead_Case(tempid_d);
                    SwapBuffer(copy_data[0]);
                    break;
                case (byte)PacketInfo.GameOver:
                    //Debug.Log("Game Over!!!");
                    byte winnerid = copy_data[2];
                    byte loserid = copy_data[3];
                    //VariableManager_Coop.instance.GameOver_Set(winnerid, loserid);

                    SwapBuffer(copy_data[0]);
                    break;
                case (byte)PacketInfo.ReadyData:

                    GameRoom.instance.GetReadyState(copy_data[2], copy_data[3]);
                    VariableManager_Coop.instance.GetReadyState(copy_data[4], copy_data[2], copy_data[3]);
                    SwapBuffer(copy_data[0]);
                    break;

                case (byte)PacketInfo.EnemyData:
                    //CoOpManager.instance.boss
                    Boss_AI_Coop.instance.SetBossPoss(copy_data);
                    SwapBuffer(copy_data[0]);
                    break;
                case (byte)PacketInfo.RoomAccept:
                    {
                        Boss_AI_Coop.instance.SetBossState(copy_data);
                        SwapBuffer(copy_data[0]);
                    }
                    break;
                case (byte)PacketInfo.GameStart:
                    //Debug.Log("Get Game Start Data");
                    byte temp_boolG_S = copy_data[2];
                    if (temp_boolG_S==1)
                    {
                        m_ingame = true;
                        Debug.Log("Get In");
                        Matching_Room.instance.SetMatchingInfo(copy_data);
                        //Matching_Room.instance.MathcingCorrect(copy_data[2]);
                       
                        Debug.Log("Change Value2");
                    }
                    SwapBuffer(copy_data[0]);
                    break;

                case (byte)PacketInfo.SetBomb:
                    byte tempfp2 = copy_data[2];
                    int tempxBB = BitConverter.ToInt32(copy_data, 3);
                    int tempzBB = BitConverter.ToInt32(copy_data, 7);
                    VariableManager_Coop.instance.Check_BombMap(tempxBB, tempzBB, 5);
                    VariableManager_Coop.instance.Check_FireMap(tempxBB, tempzBB, tempfp2);

                    SwapBuffer(copy_data[0]);
                    break;
                case (byte)PacketInfo.OUTRoom:

                    VariableManager_Coop.instance.OutRoom();
                    GameRoom.instance.OutRoom();

                    SceneChange.instance.GoTo_Wait_Scene();
                    SwapBuffer(copy_data[0]);
                    break;
               
                case (byte)PacketInfo.ThrowBomb:

                    // MapManager.instance.Throw_BombSet();
                    //Debug.Log("Throw!!Get");
                    byte direction2 = copy_data[2];
                    byte tempthrowid = copy_data[3];
                    int tempx2 = BitConverter.ToInt32(copy_data, 4);
                    int tempz2 = BitConverter.ToInt32(copy_data, 8);
                    
                    int tempdx2 = BitConverter.ToInt32(copy_data, 12);
                    int tempdz2 = BitConverter.ToInt32(copy_data, 16);
                    ////Debug.Log("Get convert complete");
                    MapManager_COop.instance.Throw_BombSet(tempx2, tempz2, tempdx2, tempdz2, direction2);
                    Turtle_Move_Coop.instance.Throw_Case(tempthrowid);
                    SwapBuffer(copy_data[0]);
                    break;
                case (byte)PacketInfo.KickBomb:
                    byte kickornot = copy_data[2];
                    if (kickornot == 1)
                    {
                        byte kickid = copy_data[3];
                        byte directionPB = copy_data[4];
                        int tempxpb = BitConverter.ToInt32(copy_data, 5);
                        int tempzpb = BitConverter.ToInt32(copy_data, 9);
                        int tempxpbd = BitConverter.ToInt32(copy_data, 13);
                        int tempzpbd = BitConverter.ToInt32(copy_data, 17);
                        MapManager_COop.instance.Kick_BombSet(tempxpb, tempzpb, tempxpbd, tempzpbd, directionPB);
                        Turtle_Move_Coop.instance.Kick_Case(kickid);
                    }
                    SwapBuffer(copy_data[0]);
                    break;

                case (byte)PacketInfo.SetMap2:
                    byte tiletype = copy_data[2];
                    int tempxB = BitConverter.ToInt32(copy_data, 3);
                    int tempzB = BitConverter.ToInt32(copy_data, 7);
                    VariableManager_Coop.instance.Check_BombMap(tempxB, tempzB, tiletype);
                    SwapBuffer(copy_data[0]);

                    break;
                case (byte)PacketInfo.PushBox:
                    byte pushornot = copy_data[2];
                    if (pushornot == 1)
                    {
                        byte pushid = copy_data[3];
                        byte directionPB = copy_data[4];
                        int tempxpb = BitConverter.ToInt32(copy_data, 5);
                        int tempzpb = BitConverter.ToInt32(copy_data, 9);
                        int tempxpbd = BitConverter.ToInt32(copy_data, 13);
                        int tempzpbd = BitConverter.ToInt32(copy_data, 17);
                        MapManager_COop.instance.Push_BoxSet(tempxpb, tempzpb, tempxpbd, tempzpbd, directionPB);
                        Turtle_Move_Coop.instance.Push_Case(pushid);
                    }
                    SwapBuffer(copy_data[0]);
                    break;

                case (byte)PacketInfo.GetTime:

                    float t_time = BitConverter.ToSingle(copy_data, 2);
                    ////Debug.Log("I got Time"+t_time);
                    VariableManager_Coop.instance.ChangeTime(t_time);
                    SwapBuffer(copy_data[0]);
                    break;
                case (byte)PacketInfo.ConnectSuccess:
                    SendMyIDAndPassword();
                    //Debug.Log("Swag");
                    SwapBuffer(copy_data[0]);
                    break;
                case (byte)PacketInfo.DBInfo1:
                    {
                        byte is_connected = copy_data[2];
                        if (is_connected == 1 || is_connected == 2)
                        {
                            VariableManager_Coop.instance.SetMyDBInfo(copy_data);
                            Debug.Log("Get DBDATA!!!");

                        }
                        else
                        {
                            Debug.Log("Rejected By DB");
                            /*
                            if (is_connected == 0)
                                WaitRoom.instance.cannotConnect = 1;
                            else if (is_connected == 3)
                                WaitRoom.instance.cannotConnect = 2;
                                */
                            //연결 실패
                        }
                        SwapBuffer(copy_data[0]);
                    }
                    break;
                
                case (byte)PacketInfo.ForceOutRoom:
                    {
                        byte is_connected = copy_data[2];
                        if (is_connected == 1)
                        {
                            //그대로 매칭

                        }
                        else
                        {
                            //매칭 실패 메시지
                        }
                        SwapBuffer(copy_data[0]);
                    

                    }
                    break;
                default:
                    //Debug.Log(copy_data[1] + "Unknown PacketType!");
                    if (copy_data[0] == 0)
                        SwapBuffer(17);

                    break;
            }
        }


    }

    public virtual void WorkerThread()
    {
        const int fps = (int)(1000 / 30);
        int frameTick = Environment.TickCount + fps;
        while (m_threadLoop)
        {
            // 초당 FPS 만큼만 송, 수신 처리
            if (Environment.TickCount >= frameTick)
            {
                frameTick = Environment.TickCount + fps;
            }
        }
    }

    void DispatchReceive()
    {
        if (m_socket == null)
        {
            return;
        }

        // 수신처리.
        try
        {
            while (m_socket.Poll(0, SelectMode.SelectRead))
            {
                byte[] buffer = new byte[m_packetSize];

                int recvSize = m_socket.Receive(buffer, buffer.Length, SocketFlags.None);

                if (recvSize == 0)
                {
                    // 연결 끊기.
                    ////Debug.Log("Disconnect recv from other.");
                    Disconnect();
                }
                else if (recvSize > 0)
                {
                    m_recvQueue.InputQueue(buffer, recvSize);
                }
            }
        }
        catch
        {
            return;
        }
    }
    void SetCharPos(CharInfo c)
    {
        switch (c.id)
        {
            case 1:
                if (Turtle_Move.instance.GetId() == 1)
                {
                    break;
                }
                else
                {
                    NetUser.instance.SetPos(c.x, c.rotateY, c.z);
                }
                break;
            case 2:
                if (Turtle_Move.instance.GetId() == 2)
                {
                    break;
                }
                else
                {
                    NetUser2.instance.SetPos(c.x, c.rotateY, c.z);
                }
                break;
            case 3:
                if (Turtle_Move.instance.GetId() == 3)
                {
                    break;
                }
                else
                {
                    NetUser3.instance.SetPos(c.x, c.rotateY, c.z);
                }
                break;
            case 4:
                if (Turtle_Move.instance.GetId() == 4)
                {
                    break;
                }
                else
                {
                    NetUser4.instance.SetPos(c.x, c.rotateY, c.z);
                }
                break;
            default:
                break;


        }
    }

    public void SetBombPos(int a, int b)
    {
        bomb_posx = a / 2;
        bomb_posz = b / 2;
        m_bomb_posx = BitConverter.GetBytes(bomb_posx);
        m_bomb_posz = BitConverter.GetBytes(bomb_posz);
        m_set_bomb = true;
    }
    public void SetBombPos(int a, int b, byte c)
    {
        bomb_posx = a / 2;
        bomb_posz = b / 2;
        firepower = c;
        m_bomb_posx = BitConverter.GetBytes(bomb_posx);
        m_bomb_posz = BitConverter.GetBytes(bomb_posz);
        m_set_bomb = true;
    }
    public void SetMyPos(float x, float y, float z)
    {
        m_turtle_posx = BitConverter.GetBytes(x);
        m_turtle_posz = BitConverter.GetBytes(z);
        m_turtle_roty = BitConverter.GetBytes(y);
        m_is_move = true;
    }
    public void SetmoveTrue()
    {
        m_is_move = true;
    }
    public float GetNetPosx(int i)
    {
        if (i >= 0 && i <= 3)
            return m_chardata[i].x;
        else
            return 0;
    }
    public float GetNetRoty(int i)
    {
        if (i >= 0 && i <= 3)
            return m_chardata[i].rotateY;
        else
            return 0;
    }
    public float GetNetPosz(int i)
    {
        if (i >= 0 && i <= 3)
            return m_chardata[i].z;
        else
            return 0;
    }
    void SendMyIDAndPassword()
    {
        byte[] myInfo = new byte[44];
        byte t_size = 44;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte t_type = 26;
        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        // string a = 
        byte t_id = VariableManager_Coop.instance.m_accessid;
        byte[] m_packet_id = BitConverter.GetBytes(t_id);
        byte t_login = GetID.instance.login_type;
        byte[] m_packet_login = BitConverter.GetBytes(t_login);
        byte[] m_idString = new byte[20];
        //Debug.Log(t_login);
        string a = GetID.instance.GetIDD();
        //Debug.Log(a);
        for (int i = 0; i < a.Length; ++i)
        {
            //Debug.Log(i);
            m_idString[i] = Convert.ToByte(a[i]);
        }
        //Debug.Log(m_idString);
        //m_idString = BitConverter.GetBytes(a);
        string b = GetID.instance.GETPW();
        //Debug.Log(b);
        byte[] m_pwString = new byte[20];
        for (int i = 0; i < b.Length; ++i)
            m_pwString[i] = Convert.ToByte(b[i]);

        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_packet_id, 0, myInfo, 2, m_packet_id.Length);
        Buffer.BlockCopy(m_packet_login, 0, myInfo, 3, m_packet_login.Length);
        Buffer.BlockCopy(m_idString, 0, myInfo, 4, m_idString.Length);
        Buffer.BlockCopy(m_pwString, 0, myInfo, 24, m_pwString.Length);
        m_socket.Send(myInfo);
        //Debug.Log("Send DBPacket");

    }
    public void GetOutRoom()
    {
        SceneChange.instance.GoTo_Select_Scene();
    }
    IEnumerator SendTester()
    {

        for (; ; )
        {
            if (m_ingame)
            {
                SendTurtlePacket();
                //SendBombPacket();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    // FPS 출력
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        // Rect rect2 = new Rect(0, h * 10 / 100, w, h * 20 / 100);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);


        GUI.Label(rect, text, style);

    }
    public void SendBomb_TPacket(byte direction, int x, int z)
    {
        byte[] myInfo = new byte[13];
        byte t_size = 13;
        byte t_type = 17;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_roomnum = GameRoom.instance.GetRoomID();
        byte[] m_room_num = BitConverter.GetBytes(t_roomnum);
        byte t_ingameid = Turtle_Move.instance.GetId();
        byte[] m_player_id = BitConverter.GetBytes(t_ingameid);
        byte[] m_direction = BitConverter.GetBytes(direction);
        byte[] m_bombx = BitConverter.GetBytes(x);
        byte[] m_bombz = BitConverter.GetBytes(z);

        //Debug.Log("Direction :" + direction);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_room_num, 0, myInfo, 2, m_room_num.Length);
        Buffer.BlockCopy(m_player_id, 0, myInfo, 3, m_player_id.Length);
        Buffer.BlockCopy(m_direction, 0, myInfo, 4, m_direction.Length);

        Buffer.BlockCopy(m_bombx, 0, myInfo, 5, m_bombx.Length);
        Buffer.BlockCopy(m_bombz, 0, myInfo, 9, m_bombz.Length);
        m_socket.Send(myInfo);
    }
    //발차기 패킷
    public void SendBomb_KPacket(byte direction, int x, int z)
    {
        byte[] myInfo = new byte[13];
        byte t_size = 13;
        byte t_type = 18;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_roomnum = GameRoom.instance.GetRoomID();
        byte[] m_room_num = BitConverter.GetBytes(t_roomnum);
        byte t_ingameid = Turtle_Move.instance.GetId();
        byte[] m_player_id = BitConverter.GetBytes(t_ingameid);
        byte[] m_direction = BitConverter.GetBytes(direction);
        byte[] m_bombx = BitConverter.GetBytes(x);
        byte[] m_bombz = BitConverter.GetBytes(z);

        //Debug.Log("Direction :" + direction);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_room_num, 0, myInfo, 2, m_room_num.Length);
        Buffer.BlockCopy(m_player_id, 0, myInfo, 3, m_player_id.Length);
        Buffer.BlockCopy(m_direction, 0, myInfo, 4, m_direction.Length);

        Buffer.BlockCopy(m_bombx, 0, myInfo, 5, m_bombx.Length);
        Buffer.BlockCopy(m_bombz, 0, myInfo, 9, m_bombz.Length);
        m_socket.Send(myInfo);

    }

    public void PushBox_Packet(byte direction, int x, int z)
    {
        byte[] myInfo = new byte[13];
        byte t_size = 13;
        byte t_type = 22;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_roomnum = GameRoom.instance.GetRoomID();
        byte[] m_room_num = BitConverter.GetBytes(t_roomnum);
        byte t_ingameid = Turtle_Move.instance.GetId();
        byte[] m_player_id = BitConverter.GetBytes(t_ingameid);

        byte[] m_direction = BitConverter.GetBytes(direction);
        byte[] m_bombx = BitConverter.GetBytes(x);
        byte[] m_bombz = BitConverter.GetBytes(z);

        //Debug.Log("Direction :" + direction);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_room_num, 0, myInfo, 2, m_room_num.Length);
        Buffer.BlockCopy(m_player_id, 0, myInfo, 3, m_player_id.Length);
        Buffer.BlockCopy(m_direction, 0, myInfo, 4, m_direction.Length);

        Buffer.BlockCopy(m_bombx, 0, myInfo, 5, m_bombx.Length);
        Buffer.BlockCopy(m_bombz, 0, myInfo, 9, m_bombz.Length);
        m_socket.Send(myInfo);
    }
    public void SendBomb_TCPacket(int x, int z)
    {
        byte[] myInfo = new byte[11];
        byte t_size = 11;
        byte t_type = 19;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_roomnum = GameRoom.instance.GetRoomID();
        byte[] m_room_num = BitConverter.GetBytes(t_roomnum);

        byte[] m_bombx = BitConverter.GetBytes(x);
        byte[] m_bombz = BitConverter.GetBytes(z);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_room_num, 0, myInfo, 2, m_room_num.Length);
        Buffer.BlockCopy(m_bombx, 0, myInfo, 3, m_bombx.Length);
        Buffer.BlockCopy(m_bombz, 0, myInfo, 7, m_bombz.Length);
        m_socket.Send(myInfo);
    }
    public void SendBox_Packet(int x, int z)
    {
        byte[] myInfo = new byte[11];
        byte t_size = 11;
        byte t_type = 23;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_roomnum = GameRoom.instance.GetRoomID();
        byte[] m_room_num = BitConverter.GetBytes(t_roomnum);

        byte[] m_bombx = BitConverter.GetBytes(x);
        byte[] m_bombz = BitConverter.GetBytes(z);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_room_num, 0, myInfo, 2, m_room_num.Length);
        Buffer.BlockCopy(m_bombx, 0, myInfo, 3, m_bombx.Length);
        Buffer.BlockCopy(m_bombz, 0, myInfo, 7, m_bombz.Length);
        m_socket.Send(myInfo);

    }
    public void SendKickCom_Packet(int x, int z)
    {
        byte[] myInfo = new byte[11];
        byte t_size = 11;
        byte t_type = 20;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_roomnum = GameRoom.instance.GetRoomID();
        byte[] m_room_num = BitConverter.GetBytes(t_roomnum);

        byte[] m_bombx = BitConverter.GetBytes(x);
        byte[] m_bombz = BitConverter.GetBytes(z);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_room_num, 0, myInfo, 2, m_room_num.Length);
        Buffer.BlockCopy(m_bombx, 0, myInfo, 3, m_bombx.Length);
        Buffer.BlockCopy(m_bombz, 0, myInfo, 7, m_bombz.Length);
        m_socket.Send(myInfo);

    }
    public void SendBombPacket()
    {
        bool tempbool = m_set_bomb;
        if (tempbool)
        {
            byte[] TurtleBomb_Bomb = new byte[13];

            byte t_size = 13;
            byte t_type = 2;
            byte t_ingameid = Turtle_Move_Coop.instance.GetId();
            byte[] m_player_id = BitConverter.GetBytes(t_ingameid);
            byte[] m_packet_size = BitConverter.GetBytes(t_size);

            byte[] m_packet_type = BitConverter.GetBytes(t_type);

            byte t_room = VariableManager_Coop.instance.m_roomid;
            byte[] m_roomid = BitConverter.GetBytes(t_room);
            byte[] t_turtlefire = BitConverter.GetBytes(Turtle_Move_Coop.instance.GetFirePower());


            Buffer.BlockCopy(m_packet_size, 0, TurtleBomb_Bomb, 0, m_packet_size.Length);
            Buffer.BlockCopy(m_packet_type, 0, TurtleBomb_Bomb, 1, m_packet_type.Length);
            Buffer.BlockCopy(t_turtlefire, 0, TurtleBomb_Bomb, 2, t_turtlefire.Length);
            Buffer.BlockCopy(m_roomid, 0, TurtleBomb_Bomb, 3, m_roomid.Length);
            Buffer.BlockCopy(m_player_id, 0, TurtleBomb_Bomb, 4, m_player_id.Length);

            Buffer.BlockCopy(m_bomb_posx, 0, TurtleBomb_Bomb, 5, m_bomb_posx.Length);
            Buffer.BlockCopy(m_bomb_posz, 0, TurtleBomb_Bomb, 9, m_bomb_posz.Length);


            m_socket.Send(TurtleBomb_Bomb);

            m_set_bomb = false;
            //////Debug.Log("폭탄보냄");
        }
    }
    public void SendMatchPacket(byte difficulty, byte boss, byte map, byte how_many)
    {
        byte[] myInfo = new byte[7];
        byte t_size = 7;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte t_type = 14;
        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte[] t_id= BitConverter.GetBytes(access_id);
        byte[] m_difficulty = BitConverter.GetBytes(difficulty);
        byte[] m_boss = BitConverter.GetBytes(boss);
        byte[] m_map = BitConverter.GetBytes(map);
        byte[] m_many = BitConverter.GetBytes(how_many);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(t_id, 0, myInfo, 2, t_id.Length);

        Buffer.BlockCopy(m_difficulty, 0, myInfo, 3, m_difficulty.Length);
        Buffer.BlockCopy(m_boss, 0, myInfo, 4, m_boss.Length);
        Buffer.BlockCopy(m_map, 0, myInfo,5, m_map.Length);
        Buffer.BlockCopy(m_many, 0, myInfo, 6, 1);
        


        m_socket.Send(myInfo);

    }
    public void SendItemPacket(int x, int z, byte t)
    {
        byte[] myInfo = new byte[13];
        byte t_size = 13;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte t_type = 6;
        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_roomid = VariableManager_Coop.instance.m_roomid;
        byte t_ingameid = Turtle_Move_Coop.instance.GetId();
        byte[] m_packet_roomid = BitConverter.GetBytes(t_roomid);
        byte[] m_packet_ingameid = BitConverter.GetBytes(t_ingameid);
        int tempx = x / 2;
        int tempz = z / 2;
        byte[] m_item_type = BitConverter.GetBytes(t);
        byte[] m_packet_x = BitConverter.GetBytes(tempx);
        byte[] m_packet_z = BitConverter.GetBytes(tempz);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_packet_roomid, 0, myInfo, 2, m_packet_roomid.Length);
        Buffer.BlockCopy(m_packet_ingameid, 0, myInfo, 3, m_packet_ingameid.Length);
        Buffer.BlockCopy(m_item_type, 0, myInfo, 4, m_item_type.Length);
        Buffer.BlockCopy(m_packet_x, 0, myInfo, 5, m_packet_x.Length);
        Buffer.BlockCopy(m_packet_z, 0, myInfo, 9, m_packet_z.Length);


        m_socket.Send(myInfo);

    }

    public void SendReadyCoopPacket()
    {
        byte[] myInfo = new byte[5];
        byte t_size = 5;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte t_type = 28;
        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_ready = 1;
        byte[] m_packet_ready = BitConverter.GetBytes(t_ready);
        byte t_roomid = VariableManager_Coop.instance.m_roomid;
        byte t_ingameid = Turtle_Move_Coop.instance.GetId();
        byte[] m_packet_roomid = BitConverter.GetBytes(t_roomid);
        byte[] m_packet_ingameid = BitConverter.GetBytes(t_ingameid);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_packet_ready, 0, myInfo, 2, m_packet_ready.Length);
        Buffer.BlockCopy(m_packet_roomid, 0, myInfo, 3, m_packet_roomid.Length);
        Buffer.BlockCopy(m_packet_ingameid, 0, myInfo, 4, 1);
        m_socket.Send(myInfo);

    }
    public void SendBossPacket(int a)
    {
        byte[] myInfo = new byte[18];
        byte t_size = 18;
        byte[] m_packet_size = BitConverter.GetBytes(t_size);

        byte t_type = 16;
        byte[] m_packet_type = BitConverter.GetBytes(t_type);
        byte t_ani = Boss_AI_Coop.instance.bossani;
        byte t_alive = Boss_AI_Coop.instance.boss_alive;
        byte t_target = (byte)(a);
        byte[] m_ani = BitConverter.GetBytes(t_ani);
        byte[] m_alive = BitConverter.GetBytes(t_alive);
        byte[] m_target = BitConverter.GetBytes(t_target);
        byte t_roomid = VariableManager_Coop.instance.m_roomid;
        byte[] m_roomid = BitConverter.GetBytes(t_roomid);
        byte[] m_posx = BitConverter.GetBytes(Boss_AI_Coop.instance.bossX);
        byte[] m_posz = BitConverter.GetBytes(Boss_AI_Coop.instance.bossZ);
        byte[] m_roty = BitConverter.GetBytes(Boss_AI_Coop.instance.boss_rotY);
        Buffer.BlockCopy(m_packet_size, 0, myInfo, 0, m_packet_size.Length);
        Buffer.BlockCopy(m_packet_type, 0, myInfo, 1, m_packet_type.Length);
        Buffer.BlockCopy(m_ani, 0, myInfo, 2, m_ani.Length);
        Buffer.BlockCopy(m_alive, 0, myInfo, 3, m_alive.Length);//애니메이션 상태지만 일단 보류
        Buffer.BlockCopy(m_target, 0, myInfo, 4, m_target.Length);
        Buffer.BlockCopy(m_roomid, 0, myInfo, 5, m_roomid.Length);

        Buffer.BlockCopy(m_posx, 0, myInfo, 6, m_posx.Length);
        Buffer.BlockCopy(m_posz, 0, myInfo, 10, m_posz.Length);
        Buffer.BlockCopy(m_roty, 0, myInfo, 14, m_roty.Length);
        m_socket.Send(myInfo);


    }
    void SendTurtlePacket()
    {
        bool tempbool = m_is_move;
        if (tempbool)
        {
            byte[] ReceivebTurtles_Pos = new byte[22];
            byte t_size = 22;
            byte[] m_packet_size = BitConverter.GetBytes(t_size);

            byte t_type = 1;
            byte[] m_packet_type = BitConverter.GetBytes(t_type);
            byte t_id = (byte)(VariableManager_Coop.instance.pos_id - 1);

            byte[] m_turtle_id = BitConverter.GetBytes(t_id);
            byte t_roomid = VariableManager_Coop.instance.m_roomid;


            byte t_alive = Turtle_Move_Coop.instance.alive;
            byte[] t_turtle_alive = BitConverter.GetBytes(t_alive);
            byte[] m_room_id = BitConverter.GetBytes(t_roomid);
            //m_Socket.Send(m_packet_type);
            //MemoryStream memStream = new MemoryStream(17);
            Buffer.BlockCopy(m_packet_size, 0, ReceivebTurtles_Pos, 0, m_packet_size.Length);
            Buffer.BlockCopy(m_packet_type, 0, ReceivebTurtles_Pos, 1, m_packet_type.Length);
            Buffer.BlockCopy(m_turtle_id, 0, ReceivebTurtles_Pos, 2, m_turtle_id.Length);
            Buffer.BlockCopy(m_packet_type, 0, ReceivebTurtles_Pos, 3, m_packet_type.Length);//애니메이션 상태지만 일단 보류
            Buffer.BlockCopy(t_turtle_alive, 0, ReceivebTurtles_Pos, 4, t_turtle_alive.Length);
            Buffer.BlockCopy(m_room_id, 0, ReceivebTurtles_Pos, 5, m_room_id.Length);
            Buffer.BlockCopy(m_turtle_id, 0, ReceivebTurtles_Pos, 6, m_turtle_id.Length);
            Buffer.BlockCopy(m_turtle_id, 0, ReceivebTurtles_Pos, 7, m_turtle_id.Length);
            Buffer.BlockCopy(m_turtle_id, 0, ReceivebTurtles_Pos, 8, m_turtle_id.Length);
            Buffer.BlockCopy(m_turtle_id, 0, ReceivebTurtles_Pos, 9, m_turtle_id.Length);
            Buffer.BlockCopy(m_turtle_posx, 0, ReceivebTurtles_Pos, 10, m_turtle_posx.Length);
            Buffer.BlockCopy(m_turtle_posz, 0, ReceivebTurtles_Pos, 14, m_turtle_posz.Length);
            Buffer.BlockCopy(m_turtle_roty, 0, ReceivebTurtles_Pos, 18, m_turtle_roty.Length);


            //////Debug.Log(n);
            m_socket.Send(ReceivebTurtles_Pos);
            Debug.Log("Send");

            m_is_move = false;
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
