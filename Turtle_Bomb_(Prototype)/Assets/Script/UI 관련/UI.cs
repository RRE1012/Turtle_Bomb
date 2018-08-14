using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 게임 씬 UI 함수
public class UI : MonoBehaviour {

    static UI m_Instance; public static UI GetInstance() { return m_Instance; }

    
    // ========= Notice UI =========
    public GameObject m_Option_UI; // 옵션 버튼

    public Text m_FireCountText; // 플레이어 화력 텍스트
    public Text m_BombCountText; // 플레이어 폭탄 텍스트
    public Text m_SpeedCountText; // 플레이어 스피드 텍스트
    public Text m_GetItemText; // 아이템 획득 UI 텍스트
    public Text m_TimeLimitText; // 시간 텍스트

    public RawImage m_GetItemBackground; // 아이템 획득시 출력 이미지
    public RawImage m_GetItemImage; // 아이템 획득시 출력 이미지
    



    public Texture m_Bomb_Icon;
    public Texture m_Fire_Icon;
    public Texture m_Speed_Icon;
    public Texture m_Kick_Icon;
    public Texture m_Throw_Icon;
    public Texture m_AirDrop_Icon;

    public Animator m_KickIcon_Animator; // 발차기 아이콘 애니메이터
    public Animator m_ThrowIcon_Animator; // 던지기 아이콘 애니메이터
    public Animator m_GetItem_Animator; // 아이템 획득 UI 애니메이터

    public Image m_InBushImage; // 부쉬 입장 효과

    public Button m_PushButton; // 밀기 버튼
    public Button m_Set_Bomb_Button; // 폭탄 설치 버튼
    public Button m_Throw_Bomb_Button; // 폭탄 던지기 버튼
    
    public Animator ani; // 애니메이터
    
    float deltaTime = 0.0f; // 시간 변수
    
    // =============================





        
    // ==== 스테이지 클리어 UI =====
    public GameObject m_Stage_Clear_UI;
    
    public Texture m_Activated_Star_Texture; // 활성화된 별 텍스쳐
    public Texture m_Deactivated_Star_Texture; // 비활성화된 별 텍스쳐

    public RawImage m_Star_Image1;
    public RawImage m_Star_Image2;
    public RawImage m_Star_Image3;
    // =============================





        
    // ===========미션 UI===========
    public Text[] m_MissionText;

    public RawImage m_Mission_Star_Image1;
    public RawImage m_Mission_Star_Image2;
    public RawImage m_Mission_Star_Image3;
    
    int m_timeLimit = 0;
    int m_monsterKill = 0;
    bool m_is_Goal = false;
    bool m_is_Boss_Kill = false;

    List<Adventure_Quest_Data> m_QuestList; // 퀘스트 목록
    // =============================


    
    // ======== 게임오버 UI ========
    GameObject m_GameOver_UI;
    // =============================




    // UI에 표시될 변수
    public int m_fire_count;
    public int m_releasable_bomb_count;
    public int m_speed_count;
    public int m_cur_Max_Bomb_Count;

    float time_Second;


    // 경과시간
    float m_Elapsed_Time = 0.0f;

    // 클릭
    bool m_isClicked = false;


    void Awake()
    {
        m_Instance = this;
        StageManager.GetInstance().Init_Left_Time();
        Application.targetFrameRate = 30;
    }
    

    void Start()
    {
        m_GameOver_UI = GetComponentInChildren<GameOver_UI>().gameObject;
        m_Option_UI = GameObject.FindGameObjectWithTag("Option_UI");
        
        m_Option_UI.SetActive(false);
        
        
        m_fire_count = 1;
        m_releasable_bomb_count = 1;
        m_speed_count = 1;
        m_cur_Max_Bomb_Count = 1;

        // 폭탄 설치/던지기 활성/비활성
        m_Set_Bomb_Button.gameObject.SetActive(true);
        m_Throw_Bomb_Button.gameObject.SetActive(false);

        Stat_UI_Management();
        GetItemUI_Deactivate();


        m_QuestList = new List<Adventure_Quest_Data>();
        StageManager.GetInstance().GetQuestList(ref m_QuestList);


        for (int i = 0; i < 3; ++i)
        {
            // 퀘스트 id가 1번 (잔여시간) 이면 시간을 받아온다.
            if (m_QuestList[i].Quest_ID == 1)
                m_timeLimit = m_QuestList[i].Quest_Goal;
            // 퀘스트 id가 2번 (일반 몬스터 킬) 이면 몬스터 수를 받아온다.
            else if (m_QuestList[i].Quest_ID == 2)
                m_monsterKill = m_QuestList[i].Quest_Goal;
        }
    }


    // FPS 출력
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        
        GUI.Label(rect, text, style);

    }

    

    // 나가기 버튼
    public void StageClear_ExitButton()
    {
        StageManager.GetInstance().Set_is_Pause(true);
        StageManager.GetInstance().Destroy_Objects();
        GameObject.Find("Navigation_Plane").SetActive(false);
        if (LobbySound.instanceLS != null && PlayerPrefs.GetInt("System_Option_BGM_ON") != 0)
            LobbySound.instanceLS.SoundStart();
        SceneManager.LoadScene(2);
    }

    // 재시작 버튼
    public void StageClear_RestartButton()
    {
        StageManager.GetInstance().Set_is_Pause(true);
        GameObject.Find("Navigation_Plane").SetActive(false);
        StageManager.GetInstance().Destroy_Objects();
        SceneManager.LoadScene(3);
    }
    

    // 아이템 스탯 UI 관리
    public void Stat_UI_Management()
    {
        // 화력, 폭탄 설치 갯수, 스피드 정보 출력
        m_FireCountText.text = m_fire_count.ToString();
        if (m_cur_Max_Bomb_Count >= MAX_VALUE_ITEM.retval)
            m_BombCountText.text = m_releasable_bomb_count.ToString() + " / max";
        else
            m_BombCountText.text = m_releasable_bomb_count.ToString() + " / " + m_cur_Max_Bomb_Count.ToString();
        m_SpeedCountText.text = m_speed_count.ToString();
    }


    // 박스밀기 버튼 관리
    void Push_Button_Management()
    {
        ColorBlock cb = m_PushButton.colors;
        Color c;

        if (!PlayerMove.C_PM.m_isAbleToPush)
        {
            c = Color.gray;
            m_PushButton.interactable = false;
        }

        else
        {
            c = Color.white;
            m_PushButton.interactable = true;
        }

        cb.normalColor = c;
        m_PushButton.colors = cb;
    }


    // 부쉬 입장시 색 변화 효과
    void HideInBush_Management()
    {
        if (!PlayerMove.m_isHideinBush)
        {
            m_InBushImage.gameObject.SetActive(false);
        }
        else
        {
            m_InBushImage.gameObject.SetActive(true);
        }
    }


    // 던지기 버튼 관리
    void Throw_Button_Management()
    {
        // 던지기 버튼 활성화
        if (PlayerMove.m_isAbleToThrow)
        {
            m_Set_Bomb_Button.gameObject.SetActive(false);
            m_Throw_Bomb_Button.gameObject.SetActive(true);
        }
        else
        {
            m_Set_Bomb_Button.gameObject.SetActive(true);
            m_Throw_Bomb_Button.gameObject.SetActive(false);
        }
    }

    
    // 미션 UI 관리
    void Mission_UI_Management()
    {
        for (int i = 0; i < 3; ++i)
        {
            if (m_QuestList[i].Quest_ID == 1) // 시간제한
            {
                m_MissionText[i].text = m_QuestList[i].Quest_Script + " " + ((int)time_Second).ToString() + " / " + m_timeLimit.ToString();
                if (time_Second > m_timeLimit)
                    m_Mission_Star_Image1.texture = m_Activated_Star_Texture;
                else m_Mission_Star_Image1.texture = m_Deactivated_Star_Texture;
            }
            else if (m_QuestList[i].Quest_ID == 2) // 일반 몬스터 처치
            {
                m_MissionText[i].text = m_QuestList[i].Quest_Script + " " + StageManager.GetInstance().Get_Left_Normal_Monster_Count().ToString() + " / " + m_monsterKill.ToString();
                if (StageManager.GetInstance().Get_Left_Normal_Monster_Count() >= m_monsterKill)
                    m_Mission_Star_Image2.texture = m_Activated_Star_Texture;
            }
            else // 목표도달, 보스 처치, 튜토리얼
            {
                m_MissionText[i].text = m_QuestList[i].Quest_Script;

                if (m_is_Goal || m_is_Boss_Kill)
                {
                    m_Mission_Star_Image3.texture = m_Activated_Star_Texture;
                }
            }
        }
        
    }

    public void Set_is_Goal(bool b)
    {
        m_is_Goal = b;
        Mission_UI_Management();
    }
    public void Set_is_Boss_Kill(bool b)
    {
        m_is_Boss_Kill = b;
        Mission_UI_Management();
    }

    public void Kick_Icon_Activate()
    {
        m_KickIcon_Animator.SetBool("is_Icon_Activated", true);
    }

    public void Throw_Icon_Activate()
    {
        m_ThrowIcon_Animator.SetBool("is_Icon_Activated", true);
    }

    






    public void GetItemUI_Activate(int Item_Num)
    {
        switch(Item_Num)
        {
            case 0:
                m_GetItemText.text = "Bomb Up !";
                m_GetItemImage.texture = m_Bomb_Icon;
                break;

            case 1:
                m_GetItemText.text = "Fire Up !";
                m_GetItemImage.texture = m_Fire_Icon;
                break;

            case 2:
                m_GetItemText.text = "Speed Up !";
                m_GetItemImage.texture = m_Speed_Icon;
                break;

            case 3:
                m_GetItemText.text = "Kick Activated !";
                m_GetItemImage.texture = m_Kick_Icon;
                break;

            case 4:
                m_GetItemText.text = "Throw Activated !";
                m_GetItemImage.texture = m_Throw_Icon;
                break;

            case 5:
                m_GetItemText.text = "You've Got AirDrop !!";
                m_GetItemImage.texture = m_AirDrop_Icon;
                break;
        }

        Stat_UI_Management(); // 스탯 갱신
        
        
        m_GetItemBackground.gameObject.SetActive(true);
        m_GetItemText.gameObject.SetActive(true);
        m_GetItemImage.gameObject.SetActive(true);
        m_GetItem_Animator.SetBool("is_Got_Item", true);
        Invoke("GetItemUI_Deactivate", 1.4f);
    }

    void GetItemUI_Deactivate()
    {
        m_GetItem_Animator.SetBool("is_Got_Item", false);
        m_GetItemBackground.gameObject.SetActive(false);
        m_GetItemText.gameObject.SetActive(false);
        m_GetItemImage.gameObject.SetActive(false);
    }









    void Update()
    {
        /*
        // 스테이지 클리어 후 별 갯수 적용
        if (StageManager.GetInstance().Get_is_Stage_Clear() && !m_is_Init_Star_Count)
        {
            Star_Count_Apply();
            m_is_Init_Star_Count = true;
        }
        */

        if (!StageManager.GetInstance().Get_is_Pause())
        {
            if (StageManager.GetInstance().Get_is_Intro_Over())
            {
                // 시간 경과
                if (time_Second > 0)
                {
                    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
                    time_Second = time_Second - deltaTime;
                    m_Elapsed_Time += deltaTime;
                }
            }

            // 시간 텍스트 출력
            if(time_Second % 60 < 10)
                m_TimeLimitText.text = "0" + (int)time_Second / 60 + ":0" + (int)time_Second % 60;
            else
                m_TimeLimitText.text = "0" + (int)time_Second / 60 + ":" + (int)time_Second % 60;

            Mission_UI_Management(); // 미션 UI 출력
            
            Push_Button_Management(); // 밀기버튼
            
            HideInBush_Management(); // 부쉬 효과
            
            Throw_Button_Management(); // 던지기 버튼

            StageManager.GetInstance().Summon_SuddenDeath_Glider();

            // 시간촉박 애니매이션 출력, 초기에는 애니매이션을 꺼놨다가 발동 -R
            if (time_Second < 15.0f)
                ani.enabled = true;
            else
                ani.enabled = false;

            // 시간 초과 시 게임오버 - 캐릭터를 죽게 함으로서 처리 -R
            if (time_Second <= 0)
            {
                time_Second = 0;
                PlayerMove.C_PM.Set_Dead();
            }
        }
    }

    public void Stage_Clear_Directing()
    {
        m_Stage_Clear_UI.GetComponent<Stage_Clear>().Stage_Clear_Direction_Play(); // 스테이지 클리어 연출 발동
    }

    public void GameOver_Directing()
    {
        m_GameOver_UI.SetActive(true);
        m_GameOver_UI.GetComponent<GameOver_UI>().GameOver_Direction_Play(); // 게임 오버 연출 발동
    }
    
    // 옵션 UI 비활성화
    public void Option_UI_Deactivate()
    {
        m_Option_UI.SetActive(false);
    }

    // 옵션 버튼 UI 활성화
    public void Option_Button()
    {
        //m_Ingame_Play_UI.SetActive(false);
        m_Option_UI.SetActive(true);
        StageManager.GetInstance().Set_is_Pause(true);
    }

    // 옵션의 Return 버튼 (게임으로 돌아가기)
    public void Option_Return_Button()
    {
        StageManager.GetInstance().Set_is_Pause(false);
        //m_Ingame_Play_UI.SetActive(true);
        m_Option_UI.SetActive(false);
    }

    public float Get_Left_Time()
    {
        return time_Second;
    }
    
    public void Set_Left_Time(float t)
    {
        time_Second = t;
    }


    public float Get_Elapsed_Time()
    {
        return m_Elapsed_Time;
    }
    public void Set_Elapsed_Time(float t)
    {
        m_Elapsed_Time = t;
    }

    public void isClicked()
    {
        m_isClicked = true;
    }
    public void isClickedOff()
    {
        m_isClicked = false;
    }

    public bool Get_isClicked()
    {
        return m_isClicked;
    }
    
}
