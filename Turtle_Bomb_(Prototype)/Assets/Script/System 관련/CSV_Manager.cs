using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public struct Adventure_Quest_Data
{
    public int ID;
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

public struct Script_Data
{
    public int Spawn_Number;
    public int NPC_ID;
    public string Script;
}

public struct Adventure_Boss_Data
{
    public int Boss_HP;
    public int Bomb_Damage;
    public int Angry_Condition_Start_HP;
    public int Groggy_Condition_Start_HP;
    public int Spawn_MonsterGroup_Id;
    public int Spawn_MonsterGroup_Number;
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
    public TextAsset m_Script_Table_csvFile;
    public TextAsset m_Adventure_Boss_Table_csvFile;
    public TextAsset m_Adventure_Boss_AI_csvFile;

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
    public List<Adventure_Quest_Data> Get_Adventure_Quest_List(ref int[] mission_list)
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
            for (int j = 0; j < 3; ++j)
            {
                if (System.Convert.ToInt32(m_data[0]) == mission_list[j])
                {
                    // 입력한 값과 일치한다면 필요한 데이터를 리스트에 삽입.
                    data.ID = System.Convert.ToInt32(m_data[0]);
                    data.Quest_ID = System.Convert.ToInt32(m_data[1]);
                    data.isCountable = System.Convert.ToInt32(m_data[2]);
                    data.Quest_Script = m_data[3];
                    data.Quest_Goal = System.Convert.ToInt32(m_data[4]);
                    Quest_List.Add(data);
                }
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

            if (stageNum == System.Convert.ToInt32(m_data[5]))
            {
                for (int j = 0; j < SPIL_MAX; ++j)
                {
                    data = System.Convert.ToInt32(m_data[j + 5]);
                    List.Add(data);
                }
            }
        }

        return List;
    }






    // 스크립트(대사) 목록
    public List<Script_Data> Get_Script_List(int scriptID)
    {
        Script_Data data = new Script_Data();
        List<Script_Data> Script_List = new List<Script_Data>();

        // csv 파일의 길이를 세어준다.
        int file_Line_Count = Counting_EOF(m_Script_Table_csvFile);

        m_Read_Text = m_Script_Table_csvFile.text;
        m_stringList = m_Read_Text.Split('\n');

        // 3 부터 시작
        for (int i = 3; i < file_Line_Count; ++i)
        {
            m_data = m_stringList[i].Split(',');

            // 스테이지 번호를 확인한다.
            if (System.Convert.ToInt32(m_data[1]) == scriptID)
            {
                // 입력한 값과 일치한다면 필요한 데이터를 리스트에 삽입.
                data.Spawn_Number = System.Convert.ToInt32(m_data[2]);
                data.NPC_ID = System.Convert.ToInt32(m_data[3]);
                data.Script = m_data[4];
                Script_List.Add(data);
            }
        }

        return Script_List;
    }

    




    // 보스 테이블
    public Adventure_Boss_Data Get_Adventure_Boss_Data(int objectNum)
    {
        Adventure_Boss_Data boss_Data = new Adventure_Boss_Data();

        int file_Line_Count = Counting_EOF(m_Stage_Table_csvFile);

        m_Read_Text = m_Adventure_Boss_Table_csvFile.text;
        m_stringList = m_Read_Text.Split('\n');

        for (int i = 3; i < file_Line_Count; ++i)
        {
            m_data = m_stringList[i].Split(','); // 한 줄씩 읽기
            if (objectNum == System.Convert.ToInt32(m_data[0])) // id는 오브젝트 테이블의 번호가 아닌 보스 테이블의 번호임!
            {
                boss_Data.Boss_HP = System.Convert.ToInt32(m_data[2]);
                boss_Data.Bomb_Damage = System.Convert.ToInt32(m_data[3]);
                boss_Data.Angry_Condition_Start_HP = System.Convert.ToInt32(m_data[6]);
                boss_Data.Groggy_Condition_Start_HP = System.Convert.ToInt32(m_data[7]);
                boss_Data.Spawn_MonsterGroup_Id = System.Convert.ToInt32(m_data[8]);
                boss_Data.Spawn_MonsterGroup_Number = System.Convert.ToInt32(m_data[9]);
                break;
            }
        }
        return boss_Data;
    }






    // 스테이지에 따른 미션번호 리스트
    public void Get_Adv_Mission_Num_List(ref int[] list, int stage_ID)
    {
        int file_Line_Count = Counting_EOF(m_Stage_Table_csvFile);

        m_Read_Text = m_Stage_Table_csvFile.text;
        m_stringList = m_Read_Text.Split('\n');
        for (int i = 3; i < file_Line_Count; ++i)
        {
            m_data = m_stringList[i].Split(',');

            if (stage_ID == System.Convert.ToInt32(m_data[0]))
            {
                list[0] = System.Convert.ToInt32(m_data[2]);
                list[1] = System.Convert.ToInt32(m_data[3]);
                list[2] = System.Convert.ToInt32(m_data[4]);
                break;
            }
        }
    }
}
