using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Texture Activated_Star_Image;
    public Texture Activated_Bomb_Image;
    public Texture Activated_Boss_Image;

    void Start()
    {
        // 타이틀 씬
        if (m_SceneNumber == PlayerPrefs_Manager_Constants.Title_Start_Scene)
        {
            // 초기화
            //PlayerPrefs.SetInt("Have_you_been_Play", 0);
            
            Pref_All_Stage_Open(); // 디버깅용

            // 최초 플레이어 정보 초기화
            if (PlayerPrefs.GetInt("Have_you_been_Play") == 0 || !PlayerPrefs.HasKey("Have_you_been_Play"))
                Pref_Init();
        }


        // 모드 선택 씬
        else if (m_SceneNumber == PlayerPrefs_Manager_Constants.Mode_Select_Scene)
        {
            if (PlayerPrefs.GetInt("is_Open_Mode_Competition") == 1)
            {
                Mode_Select_Scene_Manager.c_Mode_Select_manager.Open_Competition_Mode();
            }

            if (PlayerPrefs.GetInt("is_Open_Mode_Coop") == 1)
            {
                Mode_Select_Scene_Manager.c_Mode_Select_manager.Open_Coop_Mode();
            }
        }


        // 모험모드 스테이지 선택 씬
        else if (m_SceneNumber == PlayerPrefs_Manager_Constants.Mode_Adventure)
        {
            int[] mission_nums = new int[3];
            string tempString;
            int[] tempStars = new int[3]; // 1: 활성화, 0: 비활성화

            // 플레이 가능한 최대 스테이지를 받아온다.
            int playable_max_stage = PlayerPrefs.GetInt("Mode_Adventure_Playable_Max_Stage");

            // 1번부터 순차적으로 최대 스테이지 까지 수행한다.
            for (int i = 0; i <= playable_max_stage; ++i)
            {
                // 버튼을 활성화 시킨다.
                Mode_Adventure_Stage_Select_Scene_Manager.m_Stage_Buttons[i].interactable = true;

                // 버튼 이미지를 변경한다.
                if (i == 5 || i == 9) // 보스 스테이지
                {
                    Mode_Adventure_Stage_Select_Scene_Manager.m_Stage_Buttons[i].gameObject.GetComponent<RawImage>().texture = Activated_Boss_Image;
                }
                else // 일반 스테이지
                {
                    Mode_Adventure_Stage_Select_Scene_Manager.m_Stage_Buttons[i].gameObject.GetComponent<RawImage>().texture = Activated_Bomb_Image;
                    // 텍스트도 활성화 시킨다.
                    Mode_Adventure_Stage_Select_Scene_Manager.m_Stage_Buttons[i].transform.Find("Number").gameObject.SetActive(true);
                }

                // 획득했던 별을 받아온다.
                CSV_Manager.GetInstance().Get_Adv_Mission_Num_List(ref mission_nums, i);
                for (int j = 0; j < 3; ++j)
                {
                    tempString = "Adventure_Stars_ID_" + mission_nums[j].ToString();
                    tempStars[j] = PlayerPrefs.GetInt(tempString);
                }

                // 받아온 별만큼 활성화 시킨다.
                for (int j = 0; j < 3; ++j)
                {
                    if (tempStars[j] == 1)
                    {
                        Mode_Adventure_Stage_Select_Scene_Manager.m_Stage_Buttons[i].gameObject.GetComponentsInChildren<RawImage>()[j + 1].texture = Activated_Star_Image;
                    }
                }
            }
        }
    }

    // PlayerPrefs 초기화 함수
    void Pref_Init()
    {
        PlayerPrefs.SetInt("Have_you_been_Play", 1);
        PlayerPrefs.SetInt("is_Opened_Mode_Competition", 0); // 대전모드 -> 1 : 열기, 0 : 닫기
        PlayerPrefs.SetInt("is_Opened_Mode_Coop", 0);
        string temp;
        for (int i = 1; i <= 28; ++i)
        {
            temp = "Adventure_Stars_ID_" + i.ToString();
            PlayerPrefs.SetInt(temp, 0);
        }
        PlayerPrefs.SetInt("Mode_Adventure_Playable_Max_Stage", 0);
        PlayerPrefs.SetInt("Mode_Adventure_Stage_ID_For_MapLoad", 18);
        PlayerPrefs.SetInt("Mode_Adventure_Current_Stage_ID", 0);
        PlayerPrefs.Save();
    }

    void Pref_All_Stage_Open()
    {
        PlayerPrefs.SetInt("Have_you_been_Play", 1);
        PlayerPrefs.SetInt("is_Opened_Mode_Competition", 0);
        PlayerPrefs.SetInt("is_Opened_Mode_Coop", 0);
        string temp;
        for (int i = 1; i <= 27; ++i)
        {
            temp = "Adventure_Stars_ID_" + i.ToString();
            PlayerPrefs.SetInt(temp, 0);
        }
        PlayerPrefs.SetInt("Mode_Adventure_Playable_Max_Stage", 9);
        PlayerPrefs.SetInt("Mode_Adventure_Stage_ID_For_MapLoad", 18);
        PlayerPrefs.SetInt("Mode_Adventure_Current_Stage_ID", 0);
        PlayerPrefs.Save();
    }

}




// ==========================================
//            PlayerPref 내용물들
// ==========================================

// Have_you_been_Play   :   최초 플레이 여부 확인

// is_Opened_Mode_Competition   :   대전모드 해제 여부 확인

// is_Opened_Mode_Coop  :   협동모드 해제 여부 확인

// Adventure_Stars_ID_??    :   모험모드 퀘스트ID-?? 의 획득 별 개수

// Mode_Adventure_Playable_Max_Stage    :   모험모드 최대 이용가능 스테이지 번호

// Mode_Adventure_Stage_ID_For_MapLoad     :   모험모드 입장시 선택한 스테이지 번호 (맵로드를 위한..)

// Mode_Adventure_Current_Stage_ID      :   모험모드 입장시 선택한 스테이지 번호