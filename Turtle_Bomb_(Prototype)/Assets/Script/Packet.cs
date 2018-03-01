using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

public class CharacterInfoPacket : IPacket<CharInfo>
{
    class CharacterInfoSerializer : Serial_Script
    {
        public bool Serialize(CharInfo packet)
        {
            bool ret = true;
            ret &= Serialize(packet.size);
            ret &= Serialize(packet.packet_Type);
            ret &= Serialize(packet.id);
            ret &= Serialize(packet.x);
            ret &= Serialize(packet.z);
            ret &= Serialize(packet.rotateY);
            ret &= Serialize(packet.hp);
            ret &= Serialize(packet.ani_state);
            return ret;
        }

        public bool Deserialize(ref CharInfo element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }
            bool ret = true;
            ret &= Deserialize(ref element.size, 1);
            ret &= Deserialize(ref element.packet_Type, 1);
            ret &= Deserialize(ref element.id, 1);
            
            ret &= Deserialize(ref element.x);
            
            ret &= Deserialize(ref element.z);
            
            ret &= Deserialize(ref element.rotateY);
            ret &= Deserialize(ref element.hp);
          
            ret &= Deserialize(ref element.ani_state);
       

            return ret;
        }
    }

    CharInfo m_packet;

    //패킷 데이터를 시리얼라이즈 하기 위한 생성자
    public CharacterInfoPacket(CharInfo data)
    {
        m_packet = data;
    }

    // 바이너리 데이터를 패킷 데이터로 디시리얼라이즈하기 위한 생성자
    public CharacterInfoPacket(byte[] data)
    {
        CharacterInfoSerializer serializer = new CharacterInfoSerializer();
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref m_packet);
        //serializer.Clear();
    }

    //패킷 id 얻기
    public PacketInfo GetPacketId()
    {
        return PacketInfo.CharPos;
    }

    //게임에서 쓸 패킷 데이터 얻음
    public CharInfo GetPacket()
    {
        return m_packet;
    }
   

    //송신용 byte[]형 데이터 얻기
    public byte[] GetData()
    {
        CharacterInfoSerializer serializer = new CharacterInfoSerializer();
        serializer.Serialize(m_packet);

        return serializer.GetSerializedData();
    }
}