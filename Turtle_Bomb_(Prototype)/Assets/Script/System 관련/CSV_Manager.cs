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
}

public struct Object_Spawn_Position_Data
{
    public int[] Spawn_Node;
}

public struct Adventure_Stage_Data
{
    public int ID;
    public int Use_Terrain_Number;
    public int Use_Tile_Number;
    public int Stage_Time;
    public int AirDrop_Time;
    public int Number_Of_DropItem;
    public int SuddenDeath_Time;
    public int Number_Of_GliderGoblin;
    public int GliderGoblin_Bomb;
    public int GliderGoblin_Fire;
    public int[] Adventure_Quest_ID_List;
    public int[] Stage_Pattern_ID_List;
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

public struct Adventure_Big_Boss_Normal_Mode_AI_Data
{
    public double Boss_Speed_Value;
    public int[] Skill_Time;
    public int[] Spawn_Monster_Value_Min;
    public int[] Spawn_Monster_Value_Max;
    public int[] Spawn_Monster_Speed_Value;
    public int Glider_Goblin_Bomb_Value;
    public int Glider_Goblin_Fire_Value;
    public int Skill_Fire_Range_Min;
    public int Skill_Fire_Range_Max;
    public int Fire_In_Range_Min;
    public int Fire_In_Range_Max;
    public List<int[]> Skill_Percentage;
    public List<int> Skill_Duration;
    public List<int> Link_Skill;
}

public struct Adventure_Big_Boss_Angry_Mode_AI_Data
{
    public double Boss_Speed_Value;
    public int[] Skill_Time;
    public int[] Spawn_Monster_Value_Min;
    public int[] Spawn_Monster_Value_Max;
    public int[] Spawn_Monster_Speed_Value;
    public int Glider_Goblin_Bomb_Value;
    public int Glider_Goblin_Fire_Value;
    public int Skill_Fire_Range_Min;
    public int Skill_Fire_Range_Max;
    public int Fire_In_Range_Min;
    public int Fire_In_Range_Max;
    public List<int[]> Skill_Percentage;
    public List<int> Skill_Duration;
    public List<int> Link_Skill;
}

public struct Adventure_Big_Boss_Groggy_Mode_AI_Data
{
    public double Boss_Speed_Value;
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

    



    // ========= 이하는 CSV 파일을 읽어 List로 반환해주는 함수들이다. ========= 



    // 어드벤쳐모드 퀘스트 목록
    public void Get_Adventure_Quest_List(ref List<Adventure_Quest_Data> list, ref int[] mission_list)
    {
        Adventure_Quest_Data data = new Adventure_Quest_Data();
        
        m_stringList = m_Adventure_Quest_csvFile.text.Split('\n');

        list.Clear();

        // 3 부터 시작
        for (int i = 3; i < m_stringList.Length; ++i)
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
                    list.Add(data);
                }
            }
        }
    }





    // 인게임 오브젝트 목록
    public List<Object_Table_Data> Get_Object_Table_List()
    {
        Object_Table_Data data = new Object_Table_Data();
        List<Object_Table_Data> List = new List<Object_Table_Data>();
        
        m_stringList = m_Object_Table_csvFile.text.Split('\n');

        for (int i = 3; i < m_stringList.Length; ++i)
        {
            m_data = m_stringList[i].Split(',');
            data.ID = System.Convert.ToInt32(m_data[0]);
            List.Add(data);
        }
        return List;
    }





    // 오브젝트 스폰 좌표 목록
    public void Get_Object_Spawn_Position_List(ref List<Object_Spawn_Position_Data> list, int stage_id)
    {
        Object_Spawn_Position_Data data;
        
        m_stringList = m_Object_Spawn_Table_csvFile.text.Split('\n');

        list.Clear();
        
        for (int i = 3; i < m_stringList.Length; ++i)
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
                list.Add(data);
            }
        }
    }





    // 스크립트 하나 가져오기
    public List<string> Get_Script(int scriptID)
    {
        List<string> script_List = new List<string>();
        string temp = "";
        string temp2 = "";
        
        m_stringList = m_Script_Table_csvFile.text.Split('\n');
        
        for (int i = 3; i < m_stringList.Length; ++i)
        {
            m_data = m_stringList[i].Split(',');
            if (scriptID == System.Convert.ToInt32(m_data[0]))
                temp = m_data[2];
        }

        for (int i = 0; i < temp.Length; ++i)
        {
            if (temp[i] == '-' && temp[i + 1] == 'n' && temp[i + 2] == '-') // "-n-" 를 만나면 리스트에 삽입
            {
                script_List.Add(temp2);
                temp2 = "";
                i += 2;
            }
            else
                temp2 += temp[i]; // 임시변수에 한글자씩 담는다.
        }
        if (temp2.Length != 0) // 마지막 라인이 빈 글이 아니면 또 추가
            script_List.Add(temp2);

        return script_List;
    }

    




    // 보스 테이블
    public void Get_Adventure_Boss_Data(ref Adventure_Boss_Data Boss_Data_Structure, int objectNum)
    {
        m_stringList = m_Adventure_Boss_Table_csvFile.text.Split('\n');

        for (int i = 3; i < m_stringList.Length; ++i)
        {
            m_data = m_stringList[i].Split(','); // 한 줄씩 읽기
            if (objectNum == System.Convert.ToInt32(m_data[0])) // id는 오브젝트 테이블의 번호가 아닌 보스 테이블의 번호임!
            {
                Boss_Data_Structure.Boss_HP = System.Convert.ToInt32(m_data[2]);
                Boss_Data_Structure.Bomb_Damage = System.Convert.ToInt32(m_data[3]);
                Boss_Data_Structure.Angry_Condition_Start_HP = System.Convert.ToInt32(m_data[6]);
                Boss_Data_Structure.Groggy_Condition_Start_HP = System.Convert.ToInt32(m_data[7]);
                Boss_Data_Structure.Spawn_MonsterGroup_Id = System.Convert.ToInt32(m_data[8]);
                Boss_Data_Structure.Spawn_MonsterGroup_Number = System.Convert.ToInt32(m_data[9]);
                break;
            }
        }
    }






    // 스테이지 테이블
    public void Get_Adventure_Stage_Data(ref Adventure_Stage_Data Stage_Data_Structure, int stage_ID)
    {
        m_stringList = m_Stage_Table_csvFile.text.Split('\n');

        m_data = m_stringList[3 + stage_ID].Split(',');

        Stage_Data_Structure.ID = System.Convert.ToInt32(m_data[0]);
        Stage_Data_Structure.Use_Terrain_Number = System.Convert.ToInt32(m_data[2]);
        Stage_Data_Structure.Use_Tile_Number = System.Convert.ToInt32(m_data[3]);
        Stage_Data_Structure.Stage_Time = System.Convert.ToInt32(m_data[4]);
        Stage_Data_Structure.AirDrop_Time = System.Convert.ToInt32(m_data[5]);
        Stage_Data_Structure.Number_Of_DropItem = System.Convert.ToInt32(m_data[6]);
        Stage_Data_Structure.SuddenDeath_Time = System.Convert.ToInt32(m_data[7]);
        Stage_Data_Structure.Number_Of_GliderGoblin = System.Convert.ToInt32(m_data[8]);
        Stage_Data_Structure.GliderGoblin_Bomb = System.Convert.ToInt32(m_data[9]);
        Stage_Data_Structure.GliderGoblin_Fire = System.Convert.ToInt32(m_data[10]);
        for (int i = 0; i < 3; ++i)
        {
            if (m_data[11 + i] != "0") // 빈칸이 아니면
                Stage_Data_Structure.Adventure_Quest_ID_List[i] = System.Convert.ToInt32(m_data[11 + i]);
            if (m_data[14 + i] != "0") // 빈칸이 아니면
                Stage_Data_Structure.Stage_Pattern_ID_List[i] = System.Convert.ToInt32(m_data[14 + i]);
        }
    }



    // 스테이지별 퀘스트 번호만 받아오기 (PlayerPref용)
    public void Get_Adv_Mission_Num_List(ref int[] list, int stage_ID)
    {
        m_stringList = m_Stage_Table_csvFile.text.Split('\n');
        if (stage_ID >= 10) stage_ID += -9;
        m_data = m_stringList[3 + stage_ID].Split(',');
        
        for (int i = 0; i < 3; ++i)
        {
            if (m_data[11 + i] != "0") // 빈칸이 아니면
                list[i] = System.Convert.ToInt32(m_data[11 + i]);
        }
    }

    // 구현된 퀘스트의 마지막 번호를 받아오기 (PlayerPref용)
    public void Get_Adv_Max_Mission_Num(ref int max_num)
    {
        m_stringList = m_Adventure_Quest_csvFile.text.Split('\n');

        int num = 0;

        for (int i = 3; i < m_stringList.Length; ++i)
        {
            m_data = m_stringList[i].Split(',');
            if (System.Convert.ToInt32(m_data[0]) > num)
                num = System.Convert.ToInt32(m_data[0]);
        }

        max_num = num;
    }

    // 구현된 스테이지의 마지막 번호를 받아오기 (PlayerPref용)
    public void Get_Adv_Max_Stage_Num(ref int max_num, ref int max_num_for_Load)
    {
        m_stringList = m_Stage_Table_csvFile.text.Split('\n');

        int max = 0;
        int max_for_Load = 0;

        for (int i = 3; i < m_stringList.Length; ++i)
        {
            m_data = m_stringList[i].Split(',');

            if (System.Convert.ToInt32(m_data[0]) > max)
            {
                max = System.Convert.ToInt32(m_data[0]);
            }
            if (System.Convert.ToInt32(m_data[14]) > max_for_Load)
            {
                max_for_Load = System.Convert.ToInt32(m_data[14]);
                if (System.Convert.ToInt32(m_data[15]) > max_for_Load)
                {
                    max_for_Load = System.Convert.ToInt32(m_data[15]);
                    if (System.Convert.ToInt32(m_data[16]) > max_for_Load)
                    {
                        max_for_Load = System.Convert.ToInt32(m_data[16]);
                    }
                }
            }
        }

        max_num = max;
        max_num_for_Load = max_for_Load;
    }

    // 보스 AI 데이터
    public void Get_Adventure_Big_Boss_AI_Data(ref Adventure_Big_Boss_Normal_Mode_AI_Data normal, ref Adventure_Big_Boss_Angry_Mode_AI_Data angry, ref Adventure_Big_Boss_Groggy_Mode_AI_Data groggy)
    {
        string tmpStr = "";
        m_stringList = m_Adventure_Boss_AI_csvFile.text.Split('\n');

        for (int i = 3; i < m_stringList.Length; ++i)
        {
            m_data = m_stringList[i].Split(','); // 한 줄 읽어서 ','로 분해

            //=======================================================================
            // normal

            if (i == 3)
                normal.Boss_Speed_Value = System.Convert.ToDouble(m_data[2]);
            else if (i == 4)
            {
                for (int j = 0; j < 4; ++j)
                {
                    normal.Skill_Time[j] = System.Convert.ToInt32(m_data[j + 2]);
                }
            }
            else if (i == 5)
            {
                for (int j = 0; j < 2; ++j)
                {
                    normal.Spawn_Monster_Value_Min[j] = System.Convert.ToInt32(m_data[j + 3]);
                }
            }
            else if (i == 6)
            {
                for (int j = 0; j < 2; ++j)
                {
                    normal.Spawn_Monster_Value_Max[j] = System.Convert.ToInt32(m_data[j + 3]);
                }
            }
            else if (i == 7)
            {
                for (int j = 0; j < 2; ++j)
                {
                    normal.Spawn_Monster_Speed_Value[j] = System.Convert.ToInt32(m_data[j + 3]);
                }
            }
            else if (i == 8)
            {
                normal.Glider_Goblin_Bomb_Value = System.Convert.ToInt32(m_data[4]);
            }
            else if (i == 9)
            {
                normal.Glider_Goblin_Fire_Value = System.Convert.ToInt32(m_data[4]);
            }
            else if (i == 10)
            {
                normal.Skill_Fire_Range_Min = System.Convert.ToInt32(m_data[5]);
            }
            else if (i == 11)
            {
                normal.Skill_Fire_Range_Max = System.Convert.ToInt32(m_data[5]);
            }
            else if (i == 12)
            {
                normal.Fire_In_Range_Min = System.Convert.ToInt32(m_data[5]);
            }
            else if (i == 13)
            {
                normal.Fire_In_Range_Max = System.Convert.ToInt32(m_data[5]);
            }
            else if (i == 14)
            {
                int[] tmpArray = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    foreach (char c in m_data[j + 2])
                    {
                        if (c != '%')
                            tmpStr += c;
                    }
                    tmpArray[j] = System.Convert.ToInt32(tmpStr);
                    
                    tmpStr = "";
                }
                normal.Skill_Percentage.Add(tmpArray);
            }
            else if (i == 15)
                normal.Skill_Duration.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 16)
                normal.Link_Skill.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 17)
            {
                int[] tmpArray = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    foreach (char c in m_data[j + 2])
                    {
                        if (c != '%')
                            tmpStr += c;
                    }
                    tmpArray[j] = System.Convert.ToInt32(tmpStr);

                    tmpStr = "";
                }
                normal.Skill_Percentage.Add(tmpArray);
            }
            else if (i == 18)
                normal.Skill_Duration.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 19)
                normal.Link_Skill.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 20)
            {
                int[] tmpArray = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    foreach (char c in m_data[j + 2])
                    {
                        if (c != '%')
                            tmpStr += c;
                    }
                    tmpArray[j] = System.Convert.ToInt32(tmpStr);

                    tmpStr = "";
                }
                normal.Skill_Percentage.Add(tmpArray);
            }
            else if (i == 21)
                normal.Skill_Duration.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 22)
                normal.Link_Skill.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 23)
            {
                int[] tmpArray = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    foreach (char c in m_data[j + 2])
                    {
                        if (c != '%')
                            tmpStr += c;
                    }
                    tmpArray[j] = System.Convert.ToInt32(tmpStr);

                    tmpStr = "";
                }
                normal.Skill_Percentage.Add(tmpArray);
            }
            else if (i == 24)
                normal.Skill_Duration.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 25)
                normal.Link_Skill.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 26)
            {
                int[] tmpArray = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    foreach (char c in m_data[j + 2])
                    {
                        if (c != '%')
                            tmpStr += c;
                    }
                    tmpArray[j] = System.Convert.ToInt32(tmpStr);

                    tmpStr = "";
                }
                normal.Skill_Percentage.Add(tmpArray);
            }
            else if (i == 27)
                normal.Skill_Duration.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 28)
                normal.Link_Skill.Add(System.Convert.ToInt32(m_data[2]));






            //=======================================================================
            // angry



            else if (i == 29)
                angry.Boss_Speed_Value = System.Convert.ToDouble(m_data[2]);
            else if (i == 30)
            {
                for (int j = 0; j < 4; ++j)
                    angry.Skill_Time[j] = System.Convert.ToInt32(m_data[j + 2]);
            }
            else if (i == 31)
            {
                for (int j = 0; j < 2; ++j)
                    angry.Spawn_Monster_Value_Min[j] = System.Convert.ToInt32(m_data[j + 3]);
            }
            else if (i == 32)
            {
                for (int j = 0; j < 2; ++j)
                    angry.Spawn_Monster_Value_Max[j] = System.Convert.ToInt32(m_data[j + 3]);
            }
            else if (i == 33)
            {
                for (int j = 0; j < 2; ++j)
                    angry.Spawn_Monster_Speed_Value[j] = System.Convert.ToInt32(m_data[j + 3]);
            }
            else if (i == 34)
            {
                angry.Glider_Goblin_Bomb_Value = System.Convert.ToInt32(m_data[4]);
            }
            else if (i == 35)
            {
                angry.Glider_Goblin_Fire_Value = System.Convert.ToInt32(m_data[4]);
            }
            else if (i == 36)
            {
                angry.Skill_Fire_Range_Min = System.Convert.ToInt32(m_data[5]);
            }
            else if (i == 37)
            {
                angry.Skill_Fire_Range_Max = System.Convert.ToInt32(m_data[5]);
            }
            else if (i == 38)
            {
                angry.Fire_In_Range_Min = System.Convert.ToInt32(m_data[5]);
            }
            else if (i == 39)
            {
                angry.Fire_In_Range_Max = System.Convert.ToInt32(m_data[5]);
            }
            else if (i == 40)
            {
                int[] tmpArray = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    foreach (char c in m_data[j + 2])
                    {
                        if (c != '%')
                            tmpStr += c;
                    }
                    tmpArray[j] = System.Convert.ToInt32(tmpStr);

                    tmpStr = "";
                }
                angry.Skill_Percentage.Add(tmpArray);
            }
            else if (i == 41)
                angry.Skill_Duration.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 42)
                angry.Link_Skill.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 43)
            {
                int[] tmpArray = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    foreach (char c in m_data[j + 2])
                    {
                        if (c != '%')
                            tmpStr += c;
                    }
                    tmpArray[j] = System.Convert.ToInt32(tmpStr);

                    tmpStr = "";
                }
                angry.Skill_Percentage.Add(tmpArray);
            }
            else if (i == 44)
                angry.Skill_Duration.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 45)
                angry.Link_Skill.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 46)
            {
                int[] tmpArray = new int[4];
                for (int j = 0; j < 4; ++j)
                {
                    foreach (char c in m_data[j + 2])
                    {
                        if (c != '%')
                            tmpStr += c;
                    }
                    tmpArray[j] = System.Convert.ToInt32(tmpStr);

                    tmpStr = "";
                }
                angry.Skill_Percentage.Add(tmpArray);
            }
            else if (i == 47)
                angry.Skill_Duration.Add(System.Convert.ToInt32(m_data[2]));
            else if (i == 48)
                angry.Link_Skill.Add(System.Convert.ToInt32(m_data[2]));



            //=======================================================================
            // groggy


            else if (i == 49)
                groggy.Boss_Speed_Value = System.Convert.ToDouble(m_data[2]);
        }
    }
}
