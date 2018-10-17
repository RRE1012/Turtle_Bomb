using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class Serial_Script {
    private MemoryStream m_buffer = null;


    private int m_offset = 0;

    //Class 생성자
    public Serial_Script()
    {
        // 시리얼라이즈용 버퍼 생성
        m_buffer = new MemoryStream();
        int val = 1;
        byte[] conv = BitConverter.GetBytes(val);
    }
    public byte[] GetSerializedData()
    {
        return m_buffer.ToArray();
    }

    public void Clear()
    {
        byte[] buffer = m_buffer.GetBuffer();
        Array.Clear(buffer, 0, buffer.Length);

        m_buffer.Position = 0;
        m_buffer.SetLength(0);
        m_offset = 0;
    }

    //
    // 디시리얼라이즈할 데이터를 버퍼에 설정한다.
    //
    public bool SetDeserializedData(byte[] data)
    {
        // 설정할 버퍼를 클리어한다.
        Clear();

        try
        {
            // 디시리얼라이즈할 데이터를 설정한다.
            m_buffer.Write(data, 0, data.Length);
        }
        catch
        {
            return false;
        }

        return true;
    }
    //
    // byte형 데이터를 시리얼라이즈 합니다.
    //
    protected bool Serialize(byte element)
    {
        byte[] data = BitConverter.GetBytes(element);

        return WriteBuffer(data, sizeof(byte));
    }

    //
    // byte[]형 데이터를 시리얼라이즈한다.
    //
    protected bool Serialize(byte[] element, int length)
    {
        return WriteBuffer(element, length);
    }

    //
    // bool형 데이터를 시리얼라이즈한다.
    //
    protected bool Serialize(bool element)
    {
        byte[] data = BitConverter.GetBytes(element);

        return WriteBuffer(data, sizeof(bool));
    }

    //
    // char형 데이터를 시리얼라이즈한다.
    //
    protected bool Serialize(char element)
    {
        byte[] data = BitConverter.GetBytes(element);

        return WriteBuffer(data, sizeof(char));
    }
    //
    // string형 데이터를 시리얼라이즈한다.
    //
    protected bool Serialize(string element, int length)
    {
        byte[] data = new byte[length];

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(element);
        int size = Math.Min(buffer.Length, data.Length);
        Buffer.BlockCopy(buffer, 0, data, 0, size);

        return WriteBuffer(data, data.Length);
    }
    // char[]형 데이터를 시리얼라이즈한다.
    //
    protected bool Serialize(char[] element, int length)
    {
        string temp = new string(element);
        return Serialize(temp, length);
    }
    
    //
    // float형 데이터를 시리얼라이즈한다.
    //
    protected bool Serialize(float element)
    {
        byte[] data = BitConverter.GetBytes(element);

        return WriteBuffer(data, sizeof(float));
    }
    //
    // int형 데이터를 시리얼라이즈한다.
    //
    protected bool Serialize(int element)
    {
        byte[] data = BitConverter.GetBytes(element);

        return WriteBuffer(data, sizeof(int));
    }


    // 데이터를 bool형으로 디시리얼라이즈한다.
    //
    protected bool Deserialize(ref bool element)
    {
        int size = sizeof(bool);
        byte[] data = new byte[size];

        // 
        bool ret = ReadBuffer(ref data, data.Length);
        if (ret == true)
        {
            element = BitConverter.ToBoolean(data, 0);
            return true;
        }

        return false;
    }

    //
    // 데이터를 char형으로 디시리얼라이즈한다.
    //
    protected bool Deserialize(ref char element)
    {
        int size = sizeof(char);
        byte[] data = new byte[size];

        // 
        bool ret = ReadBuffer(ref data, data.Length);
        if (ret == true)
        {
            element = BitConverter.ToChar(data, 0);
            return true;
        }

        return false;
    }

    // 데이터를 char[]형으로 디시리얼라이즈한다.
    //
    protected bool Deserialize(ref char[] element, int length)
    {
        //int size = sizeof(char);
        byte[] data = new byte[length];
        bool ret = ReadBuffer(ref data, data.Length);
        if (ret == true)
        {
            element = System.Text.Encoding.UTF8.GetString(data).ToCharArray();
            //element = BitConverter.ToChar(data, 0);
            return true;
        }

        return false;
    }

    //
    // 데이터를 float형으로 디시리얼라이즈한다.
    //
    protected bool Deserialize(ref float element)
    {
        int size = sizeof(float);
        byte[] data = new byte[size];

        // 
        bool ret = ReadBuffer(ref data, data.Length);
        if (ret == true)
        {
            element = BitConverter.ToSingle(data, 0);
            return true;
        }

        return false;
    }
    // 데이터를 int형으로 디시리얼라이즈한다.
    //
    protected bool Deserialize(ref int element)
    {
        int size = sizeof(int);
        byte[] data = new byte[size];

        // 
        bool ret = ReadBuffer(ref data, data.Length);
        if (ret == true)
        {
            element = BitConverter.ToInt32(data, 0);
            return true;
        }
        return false;
    }

    protected bool ReadBuffer(ref byte[] data, int size)
    {
        // 현재 오프셋에서 데이터를 읽어냄
        try
        {
            m_buffer.Position = m_offset;
            m_buffer.Read(data, 0, size);
            m_offset += size;
        }
        catch
        {
            return false;
        }

        return true;
    }
    // byte형 데이터로 디시리얼라이즈한다.
    protected bool Deserialize(ref byte element, int length)
    {
        byte[] data = BitConverter.GetBytes(element);
        bool ret = ReadBuffer(ref data, length);
        element = data[0];
        if (ret == true)
        {
            return true;
        }

        return false;
    }

    //
    // byte[]형 데이터로 디시리얼라이즈한다.
    //
    protected bool Deserialize(ref byte[] element, int length)
    {
        bool ret = ReadBuffer(ref element, length);

        if (ret == true)
        {
            return true;
        }

        return false;
    }
    protected bool WriteBuffer(byte[] data, int size)
    {
        // 현재 오프셋에서 데이터를 써넣음
        try
        {
            m_buffer.Position = m_offset;
            m_buffer.Write(data, 0, size);
            m_offset += size;
        }
        catch
        {
            return false;
        }

        return true;
    }

    public long GetDataSize()
    {
        return m_buffer.Length;
    }
}
