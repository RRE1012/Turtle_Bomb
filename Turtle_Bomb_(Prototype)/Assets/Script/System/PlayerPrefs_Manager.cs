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

    static PlayerPrefs_Manager m_Instance;

    public static List<int> m_Stage_Stars_List;

    public Texture Activated_Star_Image;
    public Texture Activated_Bomb_Image;
    public Texture Activated_Boss_Image;

    void Start()
    {
        switch (m_SceneNumber)
        {
            
            case PlayerPrefs_Manager_Constants.Title_Start_Scene: // 타이틀 씬 입장 시

                Pref_Debug_Mode(); // 디버깅용
                //Pref_Init(); // 별, 스테이지 초기화용



                //PlayerPrefs.SetInt("Have_you_been_Play", 0); // 기기 초기화를 한번 해야겠다 할때 쓰는 코드




                // ★ 이하는 릴리즈 모드에서 반드시 활성화 해야함! ★

                //if (!PlayerPrefs.HasKey("Have_you_been_Play")) // 기기상에서 한번도 실행한적이 없다면
                //    Pref_Init(); // 플레이 정보 초기화
                break;






            case PlayerPrefs_Manager_Constants.Mode_Select_Scene: // 모드 선택 씬 입장 시

                if (PlayerPrefs.GetInt("is_Open_Mode_Competition") == 1)
                {
                    Mode_Select_Scene_Manager.c_Mode_Select_manager.Open_Competition_Mode();
                }

                if (PlayerPrefs.GetInt("is_Open_Mode_Coop") == 1)
                {
                    Mode_Select_Scene_Manager.c_Mode_Select_manager.Open_Coop_Mode();
                }
                break;






            case PlayerPrefs_Manager_Constants.Mode_Adventure: // 모험모드 스테이지 선택 씬 입장 시

                int[] mission_nums = new int[3];
                string tempString;
                int tempStars = 0; // 1: 활성화, 0: 비활성화

                // 플레이 가능한 최대 스테이지를 받아온다.
                int playable_max_stage = PlayerPrefs.GetInt("Mode_Adventure_Playable_Max_Stage");

                // 1번부터 순차적으로 최대 스테이지 까지 수행한다.
                for (int i = 0; i <= playable_max_stage; ++i)
                {
                    // 버튼을 활성화 시킨다.
                    Mode_Adventure_Stage_Select_Scene_Manager.GetInstance().m_Stage_Button_List[i].interactable = true;

                    // 버튼 이미지를 변경한다.
                    if (i == 5 || i == 9 || i == 14 || i == 19) // 보스 스테이지
                    {
                        Mode_Adventure_Stage_Select_Scene_Manager.GetInstance().m_Stage_Button_List[i].gameObject.GetComponent<RawImage>().texture = Activated_Boss_Image;
                    }
                    else // 일반 스테이지
                    {
                        Mode_Adventure_Stage_Select_Scene_Manager.GetInstance().m_Stage_Button_List[i].gameObject.GetComponent<RawImage>().texture = Activated_Bomb_Image;
                        // 텍스트도 활성화 시킨다.
                        Mode_Adventure_Stage_Select_Scene_Manager.GetInstance().m_Stage_Button_List[i].transform.Find("Number").gameObject.SetActive(true);
                    }


                    // 획득했던 별을 받아온다.
                    tempStars = 0;
                    CSV_Manager.GetInstance().Get_Adv_Mission_Num_List(ref mission_nums, i);
                    for (int j = 0; j < 3; ++j)
                    {
                        tempString = "Adventure_Stars_ID_" + mission_nums[j].ToString();
                        if (PlayerPrefs.GetInt(tempString) == 1)
                            ++tempStars;
                    }

                    
                    // 받아온 별만큼 활성화 시킨다.
                    for (int j = 0; j < tempStars; ++j)
                        Mode_Adventure_Stage_Select_Scene_Manager.GetInstance().m_Stage_Button_List[i].gameObject.GetComponentsInChildren<RawImage>()[j + 1].texture = Activated_Star_Image;
                    
                }
                break;






            case PlayerPrefs_Manager_Constants.Mode_Competition:

                break;






            case PlayerPrefs_Manager_Constants.Mode_Coop:

                break;
        }
    }


    public static PlayerPrefs_Manager GetInstance()
    {
        return m_Instance;
    }


    // PlayerPrefs 초기화 함수
    void Pref_Init()
    {
        PlayerPrefs.SetInt("Have_you_been_Play", 1);

        PlayerPrefs.SetInt("System_Option_BGM_ON", 1); // BGM ON
        PlayerPrefs.SetInt("System_Option_SE_ON", 1); // Sound Effect ON
        PlayerPrefs.SetInt("System_Option_Vib_ON", 1); // Vibration ON

        PlayerPrefs.SetInt("is_Opened_Mode_Competition", 0); // 대전모드 -> 1 : 열기, 0 : 닫기
        PlayerPrefs.SetInt("is_Opened_Mode_Coop", 0);

        int max_Mission_Num = 0;
        CSV_Manager.GetInstance().Get_Adv_Max_Mission_Num(ref max_Mission_Num);
        string temp;
        for (int i = 1; i <= max_Mission_Num; ++i)
        {
            temp = "Adventure_Stars_ID_" + i.ToString();
            PlayerPrefs.SetInt(temp, 0);
        }

        PlayerPrefs.SetInt("Mode_Adventure_Playable_Max_Stage", 0); // 튜토리얼로 설정 1
        PlayerPrefs.SetInt("Mode_Adventure_Stage_ID_For_MapLoad", 18); // 튜토리얼로 설정 2
        PlayerPrefs.SetInt("Mode_Adventure_Current_Stage_ID", 0); // 튜토리얼로 설정 3
        PlayerPrefs.Save(); // 저장
    }




    void Pref_Debug_Mode()
    {
        PlayerPrefs.SetInt("Have_you_been_Play", 1);
        
        if (!PlayerPrefs.HasKey("System_Option_BGM_ON"))
            PlayerPrefs.SetInt("System_Option_BGM_ON", 1); // BGM ON
        if (PlayerPrefs.GetInt("System_Option_BGM_ON") == 0) LobbySound.instanceLS.SoundStop();
        else LobbySound.instanceLS.SoundStart();
        if (!PlayerPrefs.HasKey("System_Option_SE_ON"))
            PlayerPrefs.SetInt("System_Option_SE_ON", 1); // Sound Effect ON
        if (!PlayerPrefs.HasKey("System_Option_Vib_ON"))
            PlayerPrefs.SetInt("System_Option_Vib_ON", 1); // Vibration ON
        if (!PlayerPrefs.HasKey("System_Option_Auto_Login_ON"))
            PlayerPrefs.SetInt("System_Option_Auto_Login_ON", 1); // Auto_Login ON

        PlayerPrefs.SetInt("is_Opened_Mode_Competition", 0);
        PlayerPrefs.SetInt("is_Opened_Mode_Coop", 0);

        int max_Mission_Num = 0;
        CSV_Manager.GetInstance().Get_Adv_Max_Mission_Num(ref max_Mission_Num);
        string temp;
        for (int i = 1; i <= max_Mission_Num; ++i)
        {
            temp = "Adventure_Stars_ID_" + i.ToString();
            PlayerPrefs.SetInt(temp, 0);
        }

        int max_num = 0, max_num_for_load = 0;
        CSV_Manager.GetInstance().Get_Adv_Max_Stage_Num(ref max_num, ref max_num_for_load);

        PlayerPrefs.SetInt("Mode_Adventure_Playable_Max_Stage", max_num); // 최대로 설정 1
        PlayerPrefs.SetInt("Mode_Adventure_Stage_ID_For_MapLoad", max_num_for_load); // 최대로 설정 2
        PlayerPrefs.SetInt("Mode_Adventure_Current_Stage_ID", 0); // 초기화
        PlayerPrefs.Save(); // 저장
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

// System_Option_BGM_ON :   시스템 옵션의 BGM 기능이 켜졌는가

// System_Option_SE_ON :   시스템 옵션의 효과음 기능이 켜졌는가

// System_Option_Vib_ON :   시스템 옵션의 진동 기능이 켜졌는가

// System_Option_Auto_Login_ON :   시스템 옵션의 자동로그인 기능이 켜졌는가