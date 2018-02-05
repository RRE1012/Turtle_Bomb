using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 게임 씬 UI 함수
public class UI : MonoBehaviour {

    // 인게임에 사용되는 UI들
    public static GameObject[] m_Ingame_Play_UI;
    // 플레이어 화력 텍스트
    public Text m_FCT;
    // 플레이어 폭탄 텍스트
	public Text m_BCT;
    // 플레이어 스피드 텍스트
	public Text m_SCT;

    // 남은 몬스터 텍스트
    public Text m_MLT;

    // 아이템 획득 텍스트
    public Text m_GIT;
    // 시간 텍스트
    public Text m_TLT;

    public Image m_InBushImage;
   
    // 밀기 버튼
    public Button m_PushButton;

    // 애니메이터
    public Animator ani;
    // 시간 변수
    float deltaTime = 0.0f;


    // 스테이지 클리어시 출력할 UI들
    public static GameObject[] m_Stage_Clear_UI;
    // 활성화된 별 텍스쳐
    public Texture m_Activated_Star_Texture;
    static bool m_is_Init_Star_Count = false;

    public RawImage m_Star_Image1;
    public RawImage m_Star_Image2;
    public RawImage m_Star_Image3;


    // 게임오버시 출력할 UI들
    public static GameObject[] m_GameOver_UI;

    // UI에 표시될 변수
    // 캐릭터 마다 다르게 설정해주어야한다.
    public static int m_fire_count;
	public static int m_releasable_bomb_count;
	public static int m_speed_count;
    public static int m_cur_Max_Bomb_Count;


    // 아이템획득 텍스트에 사용할 string변수
    public static string m_getItemText;


	float m_GIT_CoolTime = 0.0f;
    public static float time_Second = 30.0f;



    // 시험중 -fps 고정
    /*
    void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;
    }
    */

	void Start()
    {
        // UIs initializing
        m_Ingame_Play_UI = GameObject.FindGameObjectsWithTag("Ingame_Play_UI");
        m_Stage_Clear_UI = GameObject.FindGameObjectsWithTag("Stage_Clear_UI");
        m_GameOver_UI = GameObject.FindGameObjectsWithTag("GameOver_UI");
        m_is_Init_Star_Count = false;

        Stage_Clear_UI_Deactivate();
        GameOver_Button_DeActivate();

        m_getItemText = "";
        time_Second = 200.0f;
        m_fire_count = 1;
        m_releasable_bomb_count = 1;
        m_speed_count = 1;
        m_cur_Max_Bomb_Count = 1;
    }

    // FPS 출력
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        // Rect rect2 = new Rect(0, h * 10 / 100, w, h * 20 / 100);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        
        GUI.Label(rect, text, style);

    }





    // 게임 클리어/오버 화면 출력
    public static void Draw_StageClearPage()
    {
        Ingame_Play_UI_Deactivate();
        Stage_Clear_UI_Activate();
        StageManager.m_is_Stage_Clear = true;
    }


    // 인게임 UI 활성화
    public static void Ingame_Play_UI_Activate()
    {
        foreach (GameObject Ingame_UI in m_Ingame_Play_UI)
        {
            Ingame_UI.SetActive(true);
        }
    }

    // 인게임 UI 비활성화
    public static void Ingame_Play_UI_Deactivate()
    {
        foreach (GameObject Ingame_UI in m_Ingame_Play_UI)
        {
            Ingame_UI.SetActive(false);
        }
    }

    // 스테이지 클리어 UI 활성화
    public static void Stage_Clear_UI_Activate()
    {
        foreach (GameObject stage_clear_UI in m_Stage_Clear_UI)
        {
            stage_clear_UI.SetActive(true);
        }
    }

    // 스테이지 클리어 UI 비활성화
    public static void Stage_Clear_UI_Deactivate()
    {
        foreach (GameObject stage_clear_UI in m_Stage_Clear_UI)
        {
            stage_clear_UI.SetActive(false);
        }
    }

    public static void GameOver_Button_Activate()
    {
        foreach (GameObject gameOver_UI in m_GameOver_UI)
        {
            gameOver_UI.SetActive(true);
        }
    }

    public static void GameOver_Button_DeActivate()
    {
        foreach (GameObject gameOver_UI in m_GameOver_UI)
        {
            gameOver_UI.SetActive(false);
        }
    }


    // 획득 별 갯수 적용
    void Star_Count_Apply()
    {
        for (int s = 1; s <= StageManager.m_Stars; ++s)
        {
            if (s == 1)
                m_Star_Image1.texture = m_Activated_Star_Texture;
            else if (s == 2)
                m_Star_Image2.texture = m_Activated_Star_Texture;
            else if (s == 3)
                m_Star_Image3.texture = m_Activated_Star_Texture;
        }
    }



    public void StageClear_ExitButton()
    {
        SceneManager.LoadScene(2);
    }

    public void StageClear_RestartButton()
    {
        SceneManager.LoadScene(3);
    }



    void Push_Button_Management()
    {
        ColorBlock cb = m_PushButton.colors;
        Color c;

        if (!PlayerMove.m_isAbleToPush)
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


    void HideInBush_Management()
    {
        /*
        Color c = m_InBushImage.color;

        if (!PlayerMove.m_isHideinBush)
        {
            c.a = 0;
        }
        else
        {
            c.a = 0.3f;
        }

        m_InBushImage.color = c;
        */
        if (!PlayerMove.m_isHideinBush)
        {
            m_InBushImage.gameObject.SetActive(false);
        }
        else
        {
            m_InBushImage.gameObject.SetActive(true);
        }
    }




    void Update()
    {
        // 스테이지 클리어 후 별 갯수 적용
        if (StageManager.m_is_Stage_Clear && !m_is_Init_Star_Count)
        {
            Star_Count_Apply();
            m_is_Init_Star_Count = true;
        }

        if (!StageManager.m_is_Stage_Clear)
        {
            // 게임 시작 카메라 이동 완료 시 시간 경과
            if (PlayerMove.C_PM.GetAnimBool())
            {
                deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            }

            // 죽었을 경우, 사운드 감소
            if (!PlayerMove.C_PM.Get_IsAlive())
            {
                MusicManager.manage_ESound.TryMute();

            }

            else
            {
                // 시간 경과
                if (time_Second > 0)
                    time_Second = time_Second - deltaTime;

                // 화력, 폭탄 설치 갯수, 스피드 정보 출력
                m_FCT.text = " : " + m_fire_count.ToString();
                m_BCT.text = " : " + m_releasable_bomb_count.ToString() + " / " + m_cur_Max_Bomb_Count.ToString();
                m_SCT.text = " : " + m_speed_count.ToString();
                m_MLT.text = "Monsters Left : " + StageManager.m_Left_Monster.ToString();
                // 아이템 획득 텍스트 출력
                m_GIT.text = m_getItemText;

                // 시간 텍스트 출력
                m_TLT.text = "Time: " + (int)time_Second / 60 + ":" + (int)time_Second % 60;


                // 밀기버튼
                Push_Button_Management();

                // 부쉬 효과
                HideInBush_Management();

                // 각 수치 맥스 설정 -R
                if (m_fire_count > 8)
                    m_fire_count = 8;
                if (m_cur_Max_Bomb_Count > 8)
                    m_cur_Max_Bomb_Count = 8;
                if (m_speed_count > 8)
                    m_speed_count = 8;

                // 시간촉박 애니매이션 출력, 초기에는 애니매이션을 꺼놨다가 발동 -R
                if (time_Second < 15.0f)
                {
                    ani.enabled = true;
                }
                else
                {
                    ani.enabled = false;
                }

                // 시간 초과 시 게임오버 - 캐릭터를 죽게 함으로서 처리 -R
                if (time_Second <= 0)
                {
                    time_Second = 0;
                    PlayerMove.C_PM.Set_Dead();
                }

                if (m_GIT.text != "")
                {
                    m_GIT_CoolTime += Time.deltaTime;
                    if (m_GIT_CoolTime >= 2.0f)
                    {
                        m_getItemText = "";
                        m_GIT_CoolTime = 0.0f;
                    }
                }

            }


        }
    }
}
