using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
	public Text m_FCT;
	public Text m_BCT;
	public Text m_SCT;
	public Text m_GIT;
    public Text m_TLT;
    public Animator ani;

    // UI에 표시될 변수
    // 캐릭터 마다 다르게 설정해주어야한다.
    public static int m_fire_count = 2;
	public static int m_bomb_count = 2;
	public static int m_speed_count = 2;

	public static string m_getItemText;
	float m_GIT_CoolTime = 0.0f;
    float time_Second = 30.0f;


	void Start() 
	{
		m_getItemText = "";
        time_Second = 30.0f;
    }

	// Update is called once per frame
	void Update () {
        if(time_Second>0)
            time_Second = time_Second - Time.deltaTime;
        
		m_FCT.text = "Fire: " + m_fire_count.ToString();
        m_BCT.text = "X: " + PlayerMove.C_PM.GetPosX();

        m_SCT.text = "Z : " + PlayerMove.C_PM.GetPosZ();
		m_GIT.text = m_getItemText;
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
