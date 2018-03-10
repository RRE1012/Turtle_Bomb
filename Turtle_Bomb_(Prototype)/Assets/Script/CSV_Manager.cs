using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public struct Adventure_Quest_Data
{
    public int Quest_ID;
    public int isCountable;
    public string Quest_Script;
    public int Quest_Goal;
}

public struct Object_Table_Data
{
    public int ID;
    public string Description;
}

public struct Object_Spawn_Position_Data
{
    public int[] Spawn_Node;
}



public class CSV_Manager : MonoBehaviour {

    private static CSV_Manager instance = null;

    public static CSV_Manager GetInstance()
    {
        if (instance == null)
            instance = new CSV_Manager();
        return instance;
    }

    public TextAsset m_Adventure_Quest_csvFile;
    public TextAsset m_Object_Table_csvFile;
    public TextAsset m_Object_Spawn_Table_csvFile;
    public TextAsset m_Stage_Table_csvFile;

    protected string[] m_data;
    protected string[] m_stringList;
    protected string m_Read_Text;
    

    private void Awake()
    {
        instance = this;
    }


    // CSV 파일의 행 개수를 세어주는 함수
    int Counting_EOF(TextAsset csv)
    {
        StringReader m_stringReader = new StringReader(csv.text);
        string m_string_Line = m_stringReader.ReadLine();
        int count = 0;

        while (m_string_Line != null)
        {
            m_string_Line = m_stringReader.ReadLine();
            ++count;
        }
        return count;
    }



    // ========= 이하는 CSV 파일을 읽어 List로 반환해주는 함수들이다. ========= 



    // 어드벤쳐모드 퀘스트 목록
    public List<Adventure_Quest_Data> Get_Adventure_Quest_List(int stageID)
    {
        Adventure_Quest_Data data = new Adventure_Quest_Data();
        List<Adventure_Quest_Data> Quest_List = new List<Adventure_Quest_Data>();

        // csv 파일의 길이를 세어준다.
        int file_Line_Count = Counting_EOF(m_Adventure_Quest_csvFile);

        m_Read_Text = m_Adventure_Quest_csvFile.text;
        m_stringList = m_Read_Text.Split('\n');

        // 3 부터 시작
        for (int i = 3; i < file_Line_Count; ++i)
        {
            m_data = m_stringList[i].Split(',');

            // 스테이지 번호를 확인한다.
            if (System.Convert.ToInt32(m_data[1]) == stageID)
            {
                // 입력한 값과 일치한다면 필요한 데이터를 리스트에 삽입.
                data.Quest_ID = System.Convert.ToInt32(m_data[2]);
                data.isCountable = System.Convert.ToInt32(m_data[3]);
                data.Quest_Script = m_data[4];
                data.Quest_Goal = System.Convert.ToInt32(m_data[5]);
                Quest_List.Add(data);
            }
        }
        
        return Quest_List;
    }





    // 인게임 오브젝트 목록
    public List<Object_Table_Data> Get_Object_Table_List()
    {
        Object_Table_Data data = new Object_Table_Data();
        List<Object_Table_Data> List = new List<Object_Table_Data>();

        int file_Line_Count = Counting_EOF(m_Object_Table_csvFile);
        
        m_Read_Text = m_Object_Table_csvFile.text;
        m_stringList = m_Read_Text.Split('\n');

        for (int i = 3; i < file_Line_Count; ++i)
        {
            m_data = m_stringList[i].Split(',');
            data.ID = System.Convert.ToInt32(m_data[0]);
            data.Description = m_data[1];
            List.Add(data);
        }
        return List;
    }





    // 오브젝트 스폰 좌표 목록
    public List<Object_Spawn_Position_Data> Get_Object_Spawn_Position_List(int stage_id)
    {
        Object_Spawn_Position_Data data;
        List<Object_Spawn_Position_Data> List = new List<Object_Spawn_Position_Data>();

        int file_Line_Count = Counting_EOF(m_Object_Spawn_Table_csvFile);
        
        m_Read_Text = m_Object_Spawn_Table_csvFile.text;
        m_stringList = m_Read_Text.Split('\n');

        
        for (int i = 3; i < file_Line_Count; ++i)
        {
            m_data = m_stringList[i].Split(',');

            if (stage_id == System.Convert.ToInt32(m_data[2]))
            {
                data = new Object_Spawn_Position_Data();
                data.Spawn_Node = new int[15];
                for (int j = 0; j < 15; ++j)
                {
                    data.Spawn_Node[j] = System.Convert.ToInt32(m_data[j + 5]);
                }
                List.Add(data);
            }
        }

        return List;
    }





    // 스테이지 번호 목록
    public List<int> Get_Stage_Number_List(int stageNum)
    {
        int data = 0;
        List<int> List = new List<int>();
        int SPIL_MAX = 3; // 스테이지 번호 배열의 최대 개수

        int file_Line_Count = Counting_EOF(m_Stage_Table_csvFile);
        
        m_Read_Text = m_Stage_Table_csvFile.text;
        m_stringList = m_Read_Text.Split('\n');

        for (int i = 3; i < file_Line_Count; ++i)
        {
            m_data = m_stringList[i].Split(',');

            if (stageNum == System.Convert.ToInt32(m_data[2]))
            {
                for (int j = 0; j < SPIL_MAX; ++j)
                {
                    data = System.Convert.ToInt32(m_data[j + 2]);
                    List.Add(data);
                }
            }
        }

        return List;
    }
}
