using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class CAMERA_NUMBER
{
    public const int PLAYER = 0;
    public const int DIRECTION = 1;
}

static class DIRECTION_NUMBER
{
    public const int INTRO_NORMAL_1 = 0;
    public const int INTRO_BOSS_1 = 1;
}

public class Camera_Directing : MonoBehaviour {
    
    // StageManager가 시작할 때 Instance로 생성.
    // 생성 직후 포지션값을 맵 중앙으로 옮겨주도록 하자.

    static Camera_Directing m_Instance;
    public static Camera_Directing GetInstance() { return m_Instance; }

    Animation m_Animations;

    public Camera m_Player_Camera;

    int m_Curr_Direction_Number;
    string m_Curr_Animation_Name;
    IEnumerator m_Dir_Over_Checker;
    IEnumerator m_Go_to_Target;
    Transform m_Target;
    float m_Going_to_Target_Speed = 20.0f;
    float m_Going_to_Target_Rotate_Angle = 40.0f;

    bool m_is_in_Target = false;

    void Awake ()
    {
        m_Instance = this;
        m_Animations = GetComponent<Animation>();
        m_Dir_Over_Checker = Directing_Over_Check();
        m_Go_to_Target = Go_To_Target();
        m_Target = transform.GetChild(1);
    }


    public void Direction_Play(int direction_num)
    {
        Camera_Switching(CAMERA_NUMBER.DIRECTION); // 연출용 카메라로 전환

        m_Curr_Direction_Number = direction_num;
        switch (m_Curr_Direction_Number)
        {
            case DIRECTION_NUMBER.INTRO_NORMAL_1:
                m_Curr_Animation_Name = m_Animations.GetClip("Stage_Intro_Normal_1").name; // 재생할 애니메이션 이름을 등록!
                break;

            case DIRECTION_NUMBER.INTRO_BOSS_1:

                m_Curr_Animation_Name = m_Animations.GetClip("Stage_Intro_Boss_1").name;
                break;
        }

        m_Animations.Play(m_Curr_Animation_Name); // 연출 시작!
        StartCoroutine(m_Dir_Over_Checker); // 연출 종료 검사 시작!
    }


    IEnumerator Directing_Over_Check()
    {
        while(true)
        {
            if (m_Animations[m_Curr_Animation_Name].normalizedTime >= 0.95f) // 연출이 끝나면
                Direction_Over_Process(); // 연출 종료 처리 수행.
            yield return null;
        }
    }

    void Direction_Over_Process()
    {
        StopCoroutine(m_Dir_Over_Checker);

        switch (m_Curr_Direction_Number)
        {
            case DIRECTION_NUMBER.INTRO_NORMAL_1:
                m_Target.position = m_Player_Camera.transform.position; // 타겟을 플레이어 위치로 옮기고
                m_Target.rotation = m_Player_Camera.transform.rotation; // 회전도 시킨다.
                StartCoroutine(m_Go_to_Target); // 타겟에게 이동.
                break;

            case DIRECTION_NUMBER.INTRO_BOSS_1:

                break;
        }
    }

    IEnumerator Go_To_Target()
    {
        while (true)
        {
            Going_Target();
            yield return null;
        }
    }

    void Going_Target()
    {
        transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, m_Target.position, m_Going_to_Target_Speed * Time.deltaTime);
        transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, m_Target.rotation, m_Going_to_Target_Rotate_Angle * Time.deltaTime);

        if (transform.GetChild(0).position == m_Target.position)
        {
            Going_Target_End();
            StopCoroutine(m_Go_to_Target);
        }
    }

    void Going_Target_End()
    {
        switch (m_Curr_Direction_Number)
        {
            case DIRECTION_NUMBER.INTRO_NORMAL_1:
                StageManager.c_Stage_Manager.Set_is_Intro_Over(true);
                Camera_Switching(CAMERA_NUMBER.PLAYER); // 플레이어 카메라로 전환!
                break;

            case DIRECTION_NUMBER.INTRO_BOSS_1:

                break;
        }
    }


    public void Camera_Switching(int camera_num) // 카메라 전환
    {
        switch (camera_num)
        {
            case CAMERA_NUMBER.PLAYER:
                m_Player_Camera.enabled = true;
                m_Player_Camera.GetComponent<AudioListener>().enabled = true;
                gameObject.GetComponentInChildren<Camera>().enabled = false;
                gameObject.GetComponentInChildren<AudioListener>().enabled = false;
                break;

            case CAMERA_NUMBER.DIRECTION:
                m_Player_Camera.enabled = false;
                m_Player_Camera.GetComponent<AudioListener>().enabled = false;
                gameObject.GetComponentInChildren<Camera>().enabled = true;
                gameObject.GetComponentInChildren<AudioListener>().enabled = true;
                break;
        }
    }
}
