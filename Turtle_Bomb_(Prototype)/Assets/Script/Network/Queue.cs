using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class Queue {

    struct PacketInfo
    {
        public int offset;
        public int size;
    };

    private MemoryStream m_streamBuffer;
    private Object lockObj = new Object();
    private List<PacketInfo> m_offsetList;

    private int m_offset = 0;

    //class 생성자
    public Queue()
    {
        m_streamBuffer = new MemoryStream();
        m_offsetList = new List<PacketInfo>();
    }

    public int InputQueue(byte[] d, int size)
    {
        PacketInfo info = new PacketInfo();
        info.offset = m_offset;
        info.size = size;
        lock (lockObj)
        {
            // 패킷 저장 정보를 보존.
            m_offsetList.Add(info);

            // 패킷 데이터를 보존.
            m_streamBuffer.Position = m_offset;
            m_streamBuffer.Write(d, 0, size);
            m_streamBuffer.Flush();
            m_offset = m_offset + size;
        }
        return size;
    }
    

    public bool IsPacket()
    {
        if (m_offsetList.Count != 0)
            return true;
        else return false;
    }
    public int Dequeue(ref byte[] buffer, int size)
    {

        if (m_offsetList.Count <= 0)
        {
            return -1;
        }

        int recvSize = 0;
        lock (lockObj)
        {
            PacketInfo info = m_offsetList[0];

            // 버퍼에서 해당하는 패킷 데이터를 획득한다.
            int dataSize = Math.Min(size, info.size);
            m_streamBuffer.Position = info.offset;
            recvSize = m_streamBuffer.Read(buffer, 0, dataSize);

            // 큐 데이터를 추출했으므로 선두 요소를 삭제.
            if (recvSize > 0)
            {
                m_offsetList.RemoveAt(0);
            }

            // 모든 큐 데이터를 추출했을 때는 스트림을 클리어해서 메모리를 절약한다.
            if (m_offsetList.Count == 0)
            {
                Clear();
                m_offset = 0;
            }
        }

        return recvSize;
    }
    //큐 비우기
    public void Clear()
    {
        byte[] buffer = m_streamBuffer.GetBuffer();
        Array.Clear(buffer, 0, buffer.Length);

        m_streamBuffer.Position = 0;
        m_streamBuffer.SetLength(0);
    }
}
