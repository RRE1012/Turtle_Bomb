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
    public float x;
    public float z;
    public float rotateY;
    public float hp;
    public int ani_state;
}

public enum PacketInfo
{
    ClientID = 1,   // 클라이언트 ID값
    CharPos,        // 캐릭터 좌표 및 스테이터스
    EnemyData,      // 적 데이터
    ItemData,       // 아이템 데이터
    ObjData,        // 오브젝트 데이터
    MapData,        // 맵 데이터
    
}