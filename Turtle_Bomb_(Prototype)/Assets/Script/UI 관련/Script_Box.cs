using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Box : MonoBehaviour
{
    public Text m_Text;
    public Button m_Next_Button;
    public static Script_Box c_Script_Box;
    List<string> m_Script_List;

    int m_curr_Script_Number = 0;

    void Awake()
    {
        c_Script_Box = this;
        m_Script_List = new List<string>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameObject.activeSelf) // 켜져있으면
            StageManager.GetInstance().Set_is_Pause(true); // 일시정지
    }

    public void Set_TextList(List<string> s_list)
    {
        m_Script_List.Clear();
        m_Script_List = s_list;
        m_curr_Script_Number = 0;
        if (m_Script_List.Count != 0)
            Next_Button();
    }

    void Set_Text(int num)
    {
        m_Text.text = m_Script_List[num];
    }

    public void Next_Button()
    {
        if (m_curr_Script_Number < m_Script_List.Count)
        {
            Set_Text(m_curr_Script_Number);
            ++m_curr_Script_Number;
        }
        else
        {
            StageManager.GetInstance().Set_is_Pause(false);
            gameObject.SetActive(false); // 끝이므로 닫기
        }
    }
}
