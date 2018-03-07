using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public struct Adventure_Quest_Data
{
    public int isCountable;
    public string Quest_Script;
    public int Quest_Goal;
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
    protected string[] m_data;
    protected string[] m_stringList;
    protected string m_Read_Text;

    StringReader m_stringReader;
    string m_string_Line;
    int m_EOF_Count = 0;

    private void Awake()
    {
        instance = this;
    }

    void Counting_EOF(TextAsset csv)
    {
        m_stringReader = new StringReader(csv.text);
        m_string_Line = m_stringReader.ReadLine();
        m_EOF_Count = 0;

        while (m_string_Line != null)
        {
            m_string_Line = m_stringReader.ReadLine();
            ++m_EOF_Count;
        }
    }

    public List<Adventure_Quest_Data> Get_Adventure_Quest_List(int theme_Num, int stage_Num)
    {
        Adventure_Quest_Data data = new Adventure_Quest_Data();
        List<Adventure_Quest_Data> Quest_List;
        Quest_List = new List<Adventure_Quest_Data>();

        // csv 파일의 길이를 세어준다.
        Counting_EOF(m_Adventure_Quest_csvFile);

        // Adventure Quest Table은 최상단 부터 3줄은 데이터가 아님.
        // = 3줄 빼준다.
        m_EOF_Count -= 3;

        m_Read_Text = m_Adventure_Quest_csvFile.text;
        m_stringList = m_Read_Text.Split('\n');

        for (int i = 3; i <= m_EOF_Count; ++i)
        {
            m_data = m_stringList[i].Split(',');

            // 스테이지 번호를 확인한다.
            if (System.Convert.ToInt32(m_data[1]) == theme_Num && System.Convert.ToInt32(m_data[2]) == stage_Num)
            {
                // 입력한 값과 일치한다면 필요한 데이터를 리스트에 삽입.
                data.isCountable = System.Convert.ToInt32(m_data[4]);
                data.Quest_Script = m_data[5];
                data.Quest_Goal = System.Convert.ToInt32(m_data[6]);
                Quest_List.Add(data);
            }
        }

        return Quest_List;
    }

}
