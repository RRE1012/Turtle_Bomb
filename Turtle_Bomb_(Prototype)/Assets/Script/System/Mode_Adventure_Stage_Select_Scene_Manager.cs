using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static class THEME_NUMBER
{
    public const int FOREST = 1;
    public const int SNOWLAND = 2;

    //===================================
    public const int MAX_THEME_NUMBER = 2;
}


public class Mode_Adventure_Stage_Select_Scene_Manager : MonoBehaviour {
    
    static Mode_Adventure_Stage_Select_Scene_Manager m_Instance;

    public GameObject m_Prev_Button;
    public GameObject m_Next_Button;
    public GameObject[] m_Theme_Buttons_Parents;
    public Button[] m_Theme_Forest_Buttons;
    public Button[] m_Theme_SnowLand_Buttons;

    [HideInInspector]
    public List<Button> m_Stage_Button_List;

    int m_Curr_Theme_Number;

    void Awake()
    {
        m_Instance = this;

        m_Stage_Button_List = new List<Button>();
        Add_Buttons_in_List(ref m_Theme_Forest_Buttons); // 숲테마 추가
        Add_Buttons_in_List(ref m_Theme_SnowLand_Buttons); // 설원테마 추가

        // 이전에 선택한 스테이지를 보여주도록.
        switch (PlayerPrefs.GetInt("Mode_Adventure_Current_Stage_ID"))
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                m_Curr_Theme_Number = THEME_NUMBER.FOREST;
                break;
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
                m_Curr_Theme_Number = THEME_NUMBER.SNOWLAND;
                break;
        }

        Set_Stage_Buttons_with_Theme_Num(m_Curr_Theme_Number);
    }

    public static Mode_Adventure_Stage_Select_Scene_Manager GetInstance()
    {
        return m_Instance;
    }


    void Add_Buttons_in_List(ref Button[] buttons)
    {
        for (int i = 0; i < buttons.Length; ++i)
        {
            m_Stage_Button_List.Add(buttons[i]);
        }
    }





    void Set_Stage_Buttons_with_Theme_Num(int theme_num) // 선택한 테마의 버튼들 활성화
    {
        for (int i = 0; i < m_Theme_Buttons_Parents.Length; ++i) // 다 끄고
            m_Theme_Buttons_Parents[i].SetActive(false);

        m_Theme_Buttons_Parents[theme_num - 1].SetActive(true); // 현재 테마만 켠다.

        m_Prev_Button.SetActive(true); // 이전 테마 버튼 활성화
        m_Next_Button.SetActive(true); // 다음 테마 버튼 활성화

        if (0 == m_Curr_Theme_Number - 1) // 맨 처음 테마라면 "이전 버튼" 비활성화
        {
            m_Prev_Button.SetActive(false);
        }
        if (THEME_NUMBER.MAX_THEME_NUMBER == m_Curr_Theme_Number) // 맨 마지막 테마라면 "다음 버튼" 비활성화
        {
            m_Next_Button.SetActive(false);
        }
    }


    public void Set_Prev_Theme()
    {
        if (0 <= m_Curr_Theme_Number-1)
        {
            --m_Curr_Theme_Number;
            Set_Stage_Buttons_with_Theme_Num(m_Curr_Theme_Number);
        }
    }

    public void Set_Next_Theme()
    {
        if (THEME_NUMBER.MAX_THEME_NUMBER > m_Curr_Theme_Number)
        {
            ++m_Curr_Theme_Number;
            Set_Stage_Buttons_with_Theme_Num(m_Curr_Theme_Number);
        }
    }
}
