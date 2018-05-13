using System;

public struct ClientID
{
    public byte size;           // 패킷 크기
    public byte packetType;     // 패킷 id
    public byte id;             // 클라 id
    public byte clientType;     // 클라 타입( 0 = PC / 1 = AR )
    public char[] playerID;     // 입력한 플레이어 id
    public byte firstClient;    // 먼저 접속한 클라이언트인지 아닌지 ( 0 = 먼저 접속 / 1 = 나중에 접속)
}
public struct CharInfo
{
    public byte size;
    public byte packet_Type;
    public byte id;
    public int ani_state;
    public float hp;
    public float x;
    public float z;
    public float rotateY;
    
   
}
public struct TB_Room
{
    public byte size; //19
    public byte type;//8
    public byte roomID;
    public byte people_count;
    public byte game_start;
    public byte people_max; //최대 인원 수
    public byte made; //만들어진 방인가? 0-안 만들어짐, 1- 만들어짐(공개), 2-만들어짐(비공개)
    public byte guardian_pos;
    public byte people1;
    public byte people2;
    public byte people3;
    public byte people4;
    public byte roomtype;
    public byte team1;
    public byte team2;
    public byte team3;
    public byte team4;
    public byte ready1;
    public byte ready2;
    public byte ready3;
    public byte ready4;

    public string password;

};

public enum PacketInfo
{
    
    CharPos = 1,  // 캐릭터 좌표 및 스테이터스
    BombPos,
    BombExplode,     
    MapData,// 맵 데이터
    ClientID,
    ItemData, // 아이템 데이터
    DeadNotice,      // 적 데이터
    RoomData,   
    RoomAccept,
    RoomCreate,
    ReadyData,        // 오브젝트 데이터
    GameStart,
    OUTRoom,
    ForceOutRoom,
    GameOver,
    EnemyData,
    ThrowBomb,
    KickBomb,
    ThrowComp,
    KickComp,
    SetMap2,
    PushBox,
    Nothing,
    GetTime,
    SetBomb,
}