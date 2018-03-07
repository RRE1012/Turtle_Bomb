using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public interface IPacket<T>
{
    PacketInfo GetPacketId();     // 패킷 id 얻기

    T GetPacket();              // 패킷 데이터 얻기

    byte[] GetData();           // 바이너리 데이터 얻기
}
