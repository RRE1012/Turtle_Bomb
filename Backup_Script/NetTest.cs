using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine;

public class NetTest : MonoBehaviour {
    int bomb_posx;
    int bomb_posz;
    byte[] m_bomb_posx = new byte[4];
    byte[] m_bomb_posz = new byte[4];
    byte[] m_turtle_posx = new byte[4];
    byte[] m_turtle_posz = new byte[4];
    byte[] m_turtle_roty = new byte[4];
    public GameObject p_Bomb;
    public GameObject p_Box;
    public GameObject p_Rock;
    bool m_set_bomb = false;
    bool m_is_move = false;
    public static NetTest instance = null;
    private string m_address = "127.0.0.1";
    private string m_address2 = "127.0.0.1";
    private byte[] R_Map_Info = new byte[900];
    private byte[] Receiveid = new byte[4];
    int id = -1;
    private const int m_port = 9000;
    private const int m_packetSize = 1024;
    private Socket m_socket = null;
    private bool m_isConnected = false;
    public Thread m_thread = null;
    public bool m_threadLoop = false;
    public ClientID client_id;
    // 송신 버퍼.
    private Queue m_sendQueue = new Queue();

    // 수신 버퍼.
    private Queue m_recvQueue = new Queue();
    // 송신 데이터
    Byte[] send_data = new Byte[m_packetSize];

    // 수신 데이터
    Byte[] recv_data = new Byte[m_packetSize];
    float hp;
    float posx;
    float posz;
    float roty;
    bool is_alive;
    float hp2;
    float posx2;
    float posz2;
    float roty2;
    bool is_alive2;
    float hp3;
    float posx3;
    float posz3;
    float roty3;
    bool is_alive3;
    float hp4;
    float posx4;
    float posz4;
    float roty4;
    bool is_alive4;
    List<Vector3> bomb_list;
    // Use this for initialization
    void Start()
    {
        m_address = Client_IP();
        
        client_id.size = 15;
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
    private void OnReceiveCallBack(IAsyncResult IAR)
    {
        try
        {
            Socket tempSock = (Socket)IAR.AsyncState;
            int nReadSize = tempSock.EndReceive(IAR);
            if (nReadSize != 0)
            {
                ProcessPacket(recv_data);
                
            }
            this.Receive();
        }
        catch (SocketException se)
        {
            if (se.SocketErrorCode == SocketError.ConnectionReset)
            {
                //this.BeginConnect();
            }
        }
    }

    public bool Connect()
    {
        try
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
            m_socket.Connect(m_address, m_port);
            m_socket.NoDelay = true;
            m_isConnected = true;
            m_socket.SendBufferSize = 0;

            //byte[] recvBuffer = new byte[m_packetSize];
            m_socket.BeginReceive(this.recv_data, 0, recv_data.Length, SocketFlags.None,
                             new AsyncCallback(OnReceiveCallBack), m_socket);
            m_socket.Receive(Receiveid);
            m_socket.Receive(R_Map_Info);
            //Debug.Log(sizeof(bool));
            id = BitConverter.ToInt32(Receiveid, 0);

            Debug.Log("ID : " + id);
            
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
        m_socket.BeginReceive(this.recv_data, 0, recv_data.Length, SocketFlags.None,
                             new AsyncCallback(OnReceiveCallBack), m_socket);
    }
    void ProcessPacket(byte[] recv_buff)
    {
        switch (recv_buff[1])
        {
            case (byte)PacketInfo.ClientID:
                //클라이언트 ID를 받는 부분
                
                break;

            case (byte)PacketInfo.CharPos:
                //다른 유저 위치정보를 전송

                //SetCharPos(recv_buff);

                break;

            case (byte)PacketInfo.EnemyData:
                //적(NPC) 정보 수신

                break;

            case (byte)PacketInfo.ItemData:
               
                
                break;

            case (byte)PacketInfo.ObjData:
               
                break;

            case (byte)PacketInfo.MapData:
               
                break;

           

            default:
                Debug.Log("Unknown PacketType!");
                break;
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
    public void SetMyPos(float x, float y, float z)
    {
        m_turtle_posx = BitConverter.GetBytes(x);
        m_turtle_posz = BitConverter.GetBytes(z);
        m_turtle_roty = BitConverter.GetBytes(y);
        m_is_move = true;
    }

    public float GetNetPosx(int i)
    {
        if (i == 0)
            return posx;
        else if (i == 1)
            return posx2;
        else if (i == 2)
            return posx3;
        else
            return posx4;
    }
    public float GetNetRoty(int i)
    {
        if (i == 0)
            return roty;
        else if (i == 1)
            return roty2;
        else if (i == 2)
            return roty3;
        else
            return roty4;
    }
    public float GetNetPosz(int i)
    {
        if (i == 0)
            return posz;
        else if (i == 1)
            return posz2;
        else if (i == 2)
            return posz3;
        else
            return posz4;
    }
    void CheckMap()
    {
        for (int x = 0; x < 15; ++x)
        {
            for (int y = 0; y < 15; ++y)
            {

                int Tile_Info2 = BitConverter.ToInt32(R_Map_Info, (x * 4) + (y * 4));

                //Debug.Log(Tile_Info2);
                switch (Tile_Info2)
                {
                    case 1: //Bomb

                        //GameObject Instance_Bomb = Instantiate(p_Bomb);
                        Vector3 bomb_posi = new Vector3(x * 2, -0.2f, y * 2);
                        //bomb_list.Add(Instance_Bomb);
                        int index = bomb_list.BinarySearch(bomb_posi);
                        Debug.Log("Bomb:" + index);
                        break;
                    case 2: //Nothing

                        break;
                    case 3: //Cube(Box)로
                        GameObject Instance_Box = Instantiate(p_Box);
                        Instance_Box.transform.position = new Vector3(x * 2, -0.2f, y * 2);

                        break;
                    case 4: //Rock
                        GameObject Instance_Rock = Instantiate(p_Rock);
                        Instance_Rock.transform.position = new Vector3(x * 2, -0.2f, y * 2);
                        break;
                    case 5: //Item_Bomb
                        break;
                    default:
                        GameObject Instance_Rock2 = Instantiate(p_Rock);
                        Instance_Rock2.transform.position = new Vector3(x * 2, -0.2f, y * 2);
                        break;

                }
            }
        }
        Debug.Log("맵정보 출력 완료");
    }
}
