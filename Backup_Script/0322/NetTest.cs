using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine;

public class NetTest : MonoBehaviour {
    public static NetTest instance = null;
    int bomb_posx;
    int bomb_posz;
    byte[] m_bomb_posx = new byte[4];
    byte[] m_bomb_posz = new byte[4];
    byte[] m_turtle_posx = new byte[4];
    byte[] m_turtle_posz = new byte[4];
    byte[] m_turtle_roty = new byte[4];
    bool m_set_bomb = false;
    bool m_is_move = false;
    
    private string m_address2 = "127.0.0.1";
    private string m_address = "192.168.142.123";
    private byte[] R_Map_Info = new byte[225];
    private byte[] Receiveid = new byte[4];

    int id = -1;
    private const int m_port = 9000;
    private const int m_packetSize = 1024;
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
   
    CharInfo[] m_chardata = new CharInfo[4]; 
   

    private void Awake()
    {
        instance = this;
        
    }

    // Use this for initialization
    void Start()
    {
        //m_address = Client_IP();
        //Debug.Log(m_address);
        client_id.size = 15;
        Connect();
        StartCoroutine("SendTester");
    }

    //ip값 받아오기
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


    public int GetId()
    {
        return id;
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
            

        }
        catch (SocketException e)
        {
            m_socket = null;
            m_isConnected = false;
            Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
            Debug.Log("Socket Connect Error.");
            Debug.Log("End client communication.");
        }
        if (m_socket == null)
        {
            Disconnect();
        }

        
    }

    public bool Connect(bool a)
    {
        try
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Debug.Log("1");
            m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
            //Debug.Log("2");
            IPAddress serverIP = System.Net.IPAddress.Parse(m_address);
            //Debug.Log("3");
            IPEndPoint ipEndPoint = new System.Net.IPEndPoint(serverIP, m_port);
            //Debug.Log("4");
            //m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
            // m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
            m_socket.Connect(ipEndPoint);
            //Debug.Log("5");
            m_socket.NoDelay = true;
            //Debug.Log("6");
            m_isConnected = true;
            //Debug.Log("7");
            m_socket.SendBufferSize = 0;
            //Debug.Log("8");
            //byte[] recvBuffer = new byte[m_packetSize];
            m_socket.BeginReceive(this.recv_data, 0, recv_data.Length, SocketFlags.None,
                             new AsyncCallback(OnReceiveCallBack), m_socket);
            //Debug.Log(sizeof(bool));
            id = BitConverter.ToInt32(Receiveid, 0);
           
            //Debug.Log("ID : " + id);
            
        }
        catch (SocketException e)
        {
            m_socket = null;
            m_isConnected = false;
            Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
            Debug.Log("Socket Connect Error.");
            Debug.Log("End client communication.");
        }
        if (m_socket == null)
        {
            return false;
        }

        return true;
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
            Debug.Log("전송 에러 : " + e.Message);
        }
    }

    public void Receive()
    {
        //Debug.Log("Recv!!");
        m_socket.BeginReceive(this.recv_data, 0, recv_data.Length, SocketFlags.None,
                             new AsyncCallback(OnReceiveCallBack), m_socket);
    }
    private void OnReceiveCallBack(IAsyncResult IAR)
    {
        try
        {
            Socket tempSock = (Socket)IAR.AsyncState;
            int nReadSize = tempSock.EndReceive(IAR);
            
            Buffer.BlockCopy(recv_data,0,copy_data,0+ remain_size, nReadSize);
            remain_size = remain_size + nReadSize;
            if (nReadSize != 0)
            {
                ProcessPacket(copy_data);

            }
            
            this.Receive();
        }
        catch (SocketException se)
        {
            if (se.SocketErrorCode == SocketError.ConnectionReset)
            {
                Debug.Log(se);
                //this.BeginConnect();
            }
        }
    }
    //버퍼를 교체한다.
    void SwapBuffer(byte size)
    {
        int temp = size;
        //Debug.Log("Temp int: "+temp+ " ");
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
        
        while (remain_size >= 6)
        {
            //Debug.Log("Remain_Size : " + remain_size);
            switch (copy_data[1])
            {
                case (byte)PacketInfo.ClientID:
                    id = BitConverter.ToInt32(recv_buff, 2);
                    //Debug.Log("Get Id!! : " + (byte)id);
                    

                    SwapBuffer(copy_data[0]);
                    //Debug.Log("Remain_Size : " + remain_size);

                    break;
                case (byte)PacketInfo.CharPos:
                    //다른 유저 위치정보를 전송

                    byte tempid = copy_data[2];
                    m_chardata[tempid].ani_state = BitConverter.ToInt32(recv_buff, 3);
                    m_chardata[tempid].hp = BitConverter.ToSingle(recv_buff, 7);
                    m_chardata[tempid].x = BitConverter.ToSingle(recv_buff, 11);
                    m_chardata[tempid].z = BitConverter.ToSingle(recv_buff, 15);
                    m_chardata[tempid].rotateY = BitConverter.ToSingle(recv_buff, 19);
                    SwapBuffer(copy_data[0]);
                    //Debug.Log("Remain_Size : " + remain_size);
                    break;

                case (byte)PacketInfo.BombPos:
                    //폭탄정보 전송(터지는 폭탄)
                    byte tempid2 = copy_data[2];
                    MapManager.instance.Check_Map(recv_buff);
                    //SetCharPos(recv_buff);
                    SwapBuffer(copy_data[0]);
                    //Debug.Log("Remain_Size : " + remain_size);
                    break;
                case (byte)PacketInfo.BombExplode:
                    //폭탄정보 전송(터지는 폭탄)
                    //byte tempid3 = copy_data[2];

                    byte tempfirepower = copy_data[2];
                    int tempx = BitConverter.ToInt32(copy_data, 3);
                    int tempz = BitConverter.ToInt32(copy_data, 7);
                    //Debug.Log("Start Explode Bomb : " + remain_size);
                    MapManager.instance.Explode_Bomb(tempx, tempz, tempfirepower);
                    SwapBuffer(copy_data[0]);
                    ///Debug.Log("Explode Bomb : " + remain_size);
                    break;
                case (byte)PacketInfo.MapData:
                    //Debug.Log("Get MapData!! : ");
                    MapManager.instance.Check_Map(copy_data);
                    SwapBuffer(copy_data[0]);

                    break;

                case (byte)PacketInfo.ItemData:


                    break;

                case (byte)PacketInfo.ObjData:

                    break;

                case (byte)PacketInfo.EnemyData:

                    break;



                default:
                    Debug.Log(recv_buff[1] + "Unknown PacketType!");
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
                    Debug.Log("Disconnect recv from other.");
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

    void SendTurtlePacket()
    {
        if (m_is_move)
        {
            byte[] ReceivebTurtles_Pos = new byte[17];
            byte t_size = 17;
            byte[] m_packet_size = BitConverter.GetBytes(t_size);

            byte t_type = 1;
            byte[] m_packet_type = BitConverter.GetBytes(t_type);
            byte t_id = (byte)id;
            
            byte[] m_turtle_id = BitConverter.GetBytes(t_id);
            byte t_alive = 1;
            byte[] t_turtle_alive = BitConverter.GetBytes(t_alive);

            //m_Socket.Send(m_packet_type);
            //MemoryStream memStream = new MemoryStream(17);
            Buffer.BlockCopy(m_packet_size, 0, ReceivebTurtles_Pos, 0, m_packet_size.Length);
            Buffer.BlockCopy(m_packet_type, 0, ReceivebTurtles_Pos, 1, m_packet_type.Length);
            Buffer.BlockCopy(m_turtle_id, 0, ReceivebTurtles_Pos, 2, m_turtle_id.Length);
            Buffer.BlockCopy(m_turtle_id, 0, ReceivebTurtles_Pos, 3, m_turtle_id.Length);//애니메이션 상태지만 일단 보류
            Buffer.BlockCopy(t_turtle_alive, 0, ReceivebTurtles_Pos, 4, t_turtle_alive.Length);

            Buffer.BlockCopy(m_turtle_posx, 0, ReceivebTurtles_Pos, 5, m_turtle_posx.Length);
            Buffer.BlockCopy(m_turtle_posz, 0, ReceivebTurtles_Pos, 9, m_turtle_posz.Length);
            Buffer.BlockCopy(m_turtle_roty, 0, ReceivebTurtles_Pos, 13, m_turtle_roty.Length);


            //Debug.Log(n);
            m_socket.Send(ReceivebTurtles_Pos);
            //Debug.Log("Send");

            m_is_move = false;
        }
    }

    void SendBombPacket()
    {
        if (m_set_bomb)
        {
            byte[] TurtleBomb_Bomb = new byte[11];

            byte t_size = 11;
            byte[] m_packet_size = BitConverter.GetBytes(t_size);
            byte t_type = 2;
            byte[] m_packet_type = BitConverter.GetBytes(t_type);
            byte t_id = (byte)id;

            
            byte[] t_turtlefire = BitConverter.GetBytes(Turtle_Move.instance.GetFirePower());
           

            Buffer.BlockCopy(m_packet_size, 0, TurtleBomb_Bomb, 0, m_packet_size.Length);
            Buffer.BlockCopy(m_packet_type, 0, TurtleBomb_Bomb, 1, m_packet_type.Length);
            Buffer.BlockCopy(t_turtlefire, 0, TurtleBomb_Bomb, 2, t_turtlefire.Length);
            Buffer.BlockCopy(m_bomb_posx, 0, TurtleBomb_Bomb, 3, m_bomb_posx.Length);
            Buffer.BlockCopy(m_bomb_posz, 0, TurtleBomb_Bomb, 7, m_bomb_posz.Length);
            

            m_socket.Send(TurtleBomb_Bomb);

            m_set_bomb = false;
            //Debug.Log("폭탄보냄");
        }
    }
    IEnumerator SendTester()
    {

        for (;;)
        {
            SendTurtlePacket();
            SendBombPacket();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
