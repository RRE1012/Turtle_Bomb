using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using UnityEngine;


public class NetworkTest2 : MonoBehaviour {
    public static NetworkTest2 instance = null;
    private Socket m_Socket;
    public string ipAddress = "127.0.0.1";
    PlayerMove pm;
    public const int portNum = 9000;
    private int SendDataLength;
    private int ReceiveDataLength;
    
   
    SerializerR serial;
    List<Vector3> bomb_list;
    float netplayer2x = 100.0f;
    float netplayer2z = 100.0f;
    int type=0;
    byte type2 = 0;
    public GameObject p_Bomb;
    public GameObject p_Box;
    public GameObject p_Rock;

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


    int Tile_Info;
    int id = 0;
    private string ReceiveString;
    int readmessage;
    public static NetworkTest2 n_minstance = null;
    //Packet.CharacterPos c_Pos;
    private byte[] Sendbyte;
   
    private byte[] Receiveb = new byte[4];
    byte[] m_turtle_posx = new byte[4];
    byte[] m_turtle_posz = new byte[4];
    byte[] m_turtle_roty = new byte[4];
    byte[] m_packet_type = new byte[1];
    byte[] m_bomb_posx = new byte[4];
    byte[] m_bomb_posz = new byte[4];
    byte[] m_turtle_id = new byte[4];
    private byte[] m_type = new byte[1];
   
    byte[] m_explodebombpos = new byte[8];
    private byte[] ReceivebTurtles_Pos= new byte[17];
    private byte[] R_Map_Info = new byte[900];
    private byte[] Receiveid = new byte[4];
    
    private byte[] R_Tile_Info = new byte[1];
    bool m_is_move = false;
    bool m_set_bomb=false;
    int bomb_posx;
    int bomb_posz;

    public Int32 GetId()
    {
        return id;
    }

    private void Awake()
    {
        //CharacterPos

        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //IPv4,Stream ,TCP 타입으로 
        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 100);
        IPAddress serverIP = System.Net.IPAddress.Parse(ipAddress);
        IPEndPoint ipEndPoint = new System.Net.IPEndPoint(serverIP, portNum);
        m_Socket.Connect(ipEndPoint);
        //ab = m_Socket.EndReceive();
        m_Socket.Receive(Receiveid);
        m_Socket.Receive(R_Map_Info);
        //Debug.Log(sizeof(bool));
        id = BitConverter.ToInt32(Receiveid, 0);

        Debug.Log("ID : " + id);
        //m_Socket.Receive(Receiveb);
        instance = this;
        
        //SendTurtlePacket();
    }

    void Start()
    {
        bomb_list = new List<Vector3>();

        Invoke("CheckMap", 2.0f);
        //Invoke("CheckMap2", 4.0f);
        StartCoroutine("NetworkCheck");
        for(int x = 0; x < 15; ++x)
        {
            for (int y = 0; y < 15; ++y)
            {
                Vector3 bombpos = new Vector3(x * 2, -0.2f, y * 2);
                
                bomb_list.Add(bombpos);
            }

        }
    }


    void SetBomb()
    {
        

    }
    void SendTurtlePacket()
    {
        if (m_is_move)
        {
            type2 = 1;
            m_packet_type = BitConverter.GetBytes(type2);
            m_turtle_id = BitConverter.GetBytes(NetworkTest2.instance.GetId());
            m_Socket.Send(m_packet_type);
            m_Socket.Send(m_turtle_id);
            m_Socket.Send(m_turtle_posx);
            m_Socket.Send(m_turtle_posz);
            m_Socket.Send(m_turtle_roty);
            m_is_move = false;
        }
    }
    
    void SendBombPacket()
    {
        if (m_set_bomb)
        {
            type2 = 2;
            m_packet_type = BitConverter.GetBytes(type2);
            m_Socket.Send(m_packet_type);
            m_Socket.Send(m_bomb_posx);
            m_Socket.Send(m_bomb_posz);
            m_set_bomb = false;
            Debug.Log("폭탄보냄");
        }
    }
    IEnumerator NetworkCheck()
    {
        for (; ; )
        {
            try
            {
                //m_Socket.
                m_Socket.Receive(Receiveb);
                int check = BitConverter.ToInt32(Receiveb, 0);
                //받은 데이터 변환
                switch (check)
                {
                    case 1:
                        m_Socket.Receive(ReceivebTurtles_Pos);
                        Debug.Log("Good");
                        hp = BitConverter.ToSingle(ReceivebTurtles_Pos, 0);//ReceivebTurtles_Pos
                        posx = BitConverter.ToSingle(ReceivebTurtles_Pos, 4);
                        posz = BitConverter.ToSingle(ReceivebTurtles_Pos, 8);
                        roty = BitConverter.ToSingle(ReceivebTurtles_Pos, 12);
                        is_alive = BitConverter.ToBoolean(ReceivebTurtles_Pos, 16);
                        m_Socket.Receive(ReceivebTurtles_Pos);
                        Debug.Log("Good");
                        hp2 = BitConverter.ToSingle(ReceivebTurtles_Pos, 0);
                        posx2 = BitConverter.ToSingle(ReceivebTurtles_Pos, 4);
                        posz2 = BitConverter.ToSingle(ReceivebTurtles_Pos, 8);
                        roty2 = BitConverter.ToSingle(ReceivebTurtles_Pos, 12);
                        is_alive2 = BitConverter.ToBoolean(ReceivebTurtles_Pos, 16);
                        m_Socket.Receive(ReceivebTurtles_Pos);
                        Debug.Log("Good");
                        hp3 = BitConverter.ToSingle(ReceivebTurtles_Pos, 0);
                        posx3 = BitConverter.ToSingle(ReceivebTurtles_Pos, 4);
                        posz3 = BitConverter.ToSingle(ReceivebTurtles_Pos, 8);
                        roty3 = BitConverter.ToSingle(ReceivebTurtles_Pos, 12);
                        is_alive3 = BitConverter.ToBoolean(ReceivebTurtles_Pos, 16);
                        m_Socket.Receive(ReceivebTurtles_Pos);
                        Debug.Log("Good");
                        hp4 = BitConverter.ToSingle(ReceivebTurtles_Pos, 0);
                        posx4 = BitConverter.ToSingle(ReceivebTurtles_Pos, 4);
                        posz4 = BitConverter.ToSingle(ReceivebTurtles_Pos, 8);
                        roty4 = BitConverter.ToSingle(ReceivebTurtles_Pos, 12);
                        is_alive4 = BitConverter.ToBoolean(ReceivebTurtles_Pos, 16);
                        Debug.Log("hp:"+hp);
                        Debug.Log("PosX:"+posx);
                        Debug.Log(is_alive);
                        Debug.Log("HP2:"+hp2);
                        Debug.Log("PosZ"+posz);

                        


                        break;
                    case 2:
                        m_Socket.Receive(R_Map_Info);
                        CheckMap();
                        Debug.Log("Check_Map");
                        //ReceivebyteBombCopy = ReceivebyteBombMap;
                        break;
                    case 3:
                        m_Socket.Receive(m_explodebombpos);
                        int bombx = BitConverter.ToInt32(m_explodebombpos, 0)*2;
                        int bombz = BitConverter.ToInt32(m_explodebombpos, 4)*2;
                        
                        Debug.Log("폭탄 제거!");
                        break;
                }

                //Debug.Log(Encoding.UTF8.GetString(Receivebyte));
            }
            catch (SocketException e)
            {
                //에러 확인
                Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
                //Debug.Log(e.ErrorCode);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    // Use this for initialization
   
    public void SetBombPos(int a, int b)
    {
        bomb_posx = a / 2;
        bomb_posz = b / 2;
        m_bomb_posx = BitConverter.GetBytes(bomb_posx);
        m_bomb_posz = BitConverter.GetBytes(bomb_posz);
        m_set_bomb = true;
    }
    public void SetMyPos(float x,float y, float z)
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

    void CheckMap2()
    {
        int Tile_Info2 = BitConverter.ToInt32(Receiveb, 0);
        switch (Tile_Info2)
        {
            case 1:
                GameObject Instance_Bomb = Instantiate(p_Bomb);
                Instance_Bomb.transform.position = new Vector3(10, -0.2f, 10);
                break;
            case 2:
                GameObject Instance_Box = Instantiate(p_Box);
                Instance_Box.transform.position = new Vector3(10, -0.2f, 10);
                break;
            case 3:
                GameObject Instance_Rock = Instantiate(p_Rock);
                Instance_Rock.transform.position = new Vector3(10, -0.2f, 10);
                break;
        }
    }
    void CheckMap()
    {
        for(int x = 0; x < 15; ++x)
        {
            for(int y = 0; y < 15; ++y)
            {
                
                int Tile_Info2 = BitConverter.ToInt32(R_Map_Info,(x*4)+(y*4));
                
                //Debug.Log(Tile_Info2);
                switch (Tile_Info2)
                {
                    case 1: //Bomb
                        
                        //GameObject Instance_Bomb = Instantiate(p_Bomb);
                        Vector3 bomb_posi = new Vector3(x * 2, -0.2f, y * 2);
                        //bomb_list.Add(Instance_Bomb);
                        int index = bomb_list.BinarySearch(bomb_posi);
                        Debug.Log("Bomb:"+index);
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
    // Update is called once per frame
    void Update () {
        // m_turtle_posx = BitConverter.GetBytes(PlayerMove.C_PM.GetPosX());
        // m_turtle_posz = BitConverter.GetBytes(PlayerMove.C_PM.GetPosZ());
        // m_turtle_roty = BitConverter.GetBytes(PlayerMove.C_PM.GetRotY());
        SendTurtlePacket();
        SendBombPacket();
    }
}
