using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// #define SceneNumbers
static class PlayerPrefs_Manager_Constants
{
    public const int Title_Start_Scene = 9999;

    public const int Mode_Select_Scene = 0;

    public const int Mode_Adventure = 1;
    public const int Mode_Coop = 2;
    public const int Mode_Competition = 3;

    public const int MAX_STAGE_NUM = 9;
}



public class PlayerPrefs_Manager : MonoBehaviour {

    // 씬 넘버를 받아온다.
    // 빌드 세팅의 씬 넘버와는 다르니 주의!
    public int m_SceneNumber;

    public static List<int> m_Stage_Stars_List;

    void Start()
    {
        // 타이틀 씬
        if (m_SceneNumber == PlayerPrefs_Manager_Constants.Title_Start_Scene)
        {
            // 최초 플레이어 정보 초기화
            if (!PlayerPrefs.HasKey("is_First_Play"))
            {
                PlayerPrefs.SetInt("is_First_Play", 1);
                PlayerPrefs.SetInt("isOpen_Mode_Coop", 0);
                PlayerPrefs.SetInt("isOpen_Mode_Competition", 0);
                PlayerPrefs.SetInt("Mode_Adventure_Playable_Max_Stage", 1);
                PlayerPrefs.SetInt("Mode_Adventure_Selected_Stage_ID", 1);
                PlayerPrefs.Save();
            }
        }


        // 모드 선택 씬
        else if (m_SceneNumber == PlayerPrefs_Manager_Constants.Mode_Select_Scene)
        {
            if (PlayerPrefs.GetInt("isOpen_Mode_Coop") == 1)
            {
                Mode_Select_Scene_Manager.c_Mode_Select_manager.Open_Coop_Mode();
            }

            if (PlayerPrefs.GetInt("isOpen_Mode_Competition") == 1)
            {
                Mode_Select_Scene_Manager.c_Mode_Select_manager.Open_Competition_Mode();
            }
        }


        // 모험모드 스테이지 선택 씬
        else if (m_SceneNumber == PlayerPrefs_Manager_Constants.Mode_Adventure)
        {
            // 별 개수 리스트 초기화;
            //m_Stage_Stars_List = new List<int>();
            //string temp_string;
            //int stage_num = 0;

            int playable_max_stage = PlayerPrefs.GetInt("Mode_Adventure_Playable_Max_Stage");

            for (int i = 1; i <= playable_max_stage; ++i)
            {
                // 버튼도 활성화 시킨다.
                Mode_Adventure_Stage_Select_Scene_Manager.m_Stage_Buttons[i-1].interactable = true;
            }
        }

    }
}
