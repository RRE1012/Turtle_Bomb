using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//게임 씬 UI 함수
public class UI : MonoBehaviour {
    //플레이어 화력 텍스트
	public Text m_FCT;
    //플레이어 폭탄 텍스트
	public Text m_BCT;
    //플레이어 스피드 텍스트
	public Text m_SCT;
    //아이템 획득 텍스트
	public Text m_GIT;
    //시간 텍스트
    public Text m_TLT;
    //애니메이터
    public Animator ani;
    //시간 변수
    float deltaTime = 0.0f;
    // UI에 표시될 변수
    // 캐릭터 마다 다르게 설정해주어야한다.
    public static int m_fire_count = 1;
	public static int m_bomb_count = 1;
	public static int m_speed_count = 1;
    
    
    
    //아이템획득 텍스트에 사용할 string변수
	public static string m_getItemText;
	float m_GIT_CoolTime = 0.0f;
    float time_Second = 30.0f;

    //시험중 -fps 고정
    void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;
    }
	void Start() 
	{
		m_getItemText = "";
        time_Second = 180.0f;
        
    }
    //FPS 출력
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        Rect rect2 = new Rect(0, h * 10 / 100, w, h * 20 / 100);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        
        GUI.Label(rect, text, style);
        //GUI.Label(rect, text2, style);

    }
    // Update is called once per frame
    void Update () {
        //게임 시작 카메라 이동 완료 시 시간 경과
        if (PlayerMove.C_PM.GetAnimBool())
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }
        //죽었을 경우, 사운드 감소
        if (!PlayerMove.C_PM.Get_IsAlive())
        {
            MusicManager.manage_ESound.TryMute();
            
        }
        //시간 경과
        if (time_Second>0)
            time_Second = time_Second - deltaTime;
        //화력, 폭탄 설치 갯수, 스피드 정보 출력
		m_FCT.text = "Fire: " + m_fire_count.ToString();
		m_BCT.text = "Bomb: " + m_bomb_count.ToString();
		m_SCT.text = "Speed: " + m_speed_count.ToString();
        //아이템 획득 텍스트 출력
		m_GIT.text = m_getItemText;
        //시간 텍스트 출력
        m_TLT.text = "Time: " +(int)time_Second/60 + ":"+(int)time_Second%60;

        //각 수치 맥스 설정 -R
        if (m_fire_count > 8)
            m_fire_count = 8;
        if (m_bomb_count > 8)
            m_bomb_count = 8;
        if (m_speed_count > 8)
            m_speed_count = 8;

        //시간촉박 애니매이션 출력, 초기에는 애니매이션을 꺼놨다가 발동 -R
        if (time_Second < 15.0f)
        {
            ani.enabled = true;
        }
        else
        {
            ani.enabled = false;
        }
        //시간 초과 시 게임오버 - 캐릭터를 죽게 함으로서 처리 -R
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
