using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// #define 보스 모드
static class Boss_Mode
{
    public const int SUDDENDEATH_MODE = 255;
    public const int NORMAL_MODE = 0;
}

// #define 보스 스탯
static class Boss_JetGoblin_Status
{
    public const float JetGoblin__Base__Health = 10.0f;
    public const float JetGoblin__Base__Move_Speed = 5.0f;
    public const int JetGoblin__Base__Bomb_Count = 3;

    public const float JetGoblin__SuddenDeath__Extra_Move_Speed = 8.0f;
    public const float JetGoblin__SuddenDeath__BombDrop_Cooltime = 1.0f;
    public const int JetGoblin__SuddenDeath__Extra_Bomb_Count = 7;

    public const float JetGoblin__Normal__Extra_Health = 90.0f;
    public const float JetGoblin__Normal__Extra_Move_Speed = 5.0f;
    public const int JetGoblin__Normal__Extra_Bomb_Count = 2;
    public const float JetGoblin__Normal__Moving_Cooltime = 4.0f;
    public const float JetGoblin__Normal__BombDrop_Cooltime = 2.0f;
    public const float JetGoblin__Normal__Landing_Cooltime = 10.0f;
}





public class Boss_AI_JetGoblin : MonoBehaviour {

    // =====================================
    // =============변수 선언부=============
    // =====================================

    // 현재 진행중인 "행동"
    IEnumerator m_Current_Behavior;

    // 현재의 모드
    int m_Current_Mode;

    // "행동들"
    IEnumerator m_Behavior_Move;
    IEnumerator m_Behavior_Drop_Bomb;
    IEnumerator m_Behavior_Landing;

    public static Boss_AI_JetGoblin c_JetGoblin; // 이 객체

    public GameObject m_Bomb; // 고블린이 사용할 폭탄

    Rigidbody m_Rigidbody;



    float m_Health = Boss_JetGoblin_Status.JetGoblin__Base__Health; // 체력
    float m_MaxHealth; // 최대 체력
    float m_Move_Speed = Boss_JetGoblin_Status.JetGoblin__Base__Move_Speed; // 이동속도
    float m_Landing_Speed = 10.0f; // 착륙하는 속도
    int m_Total_Bomb_Count = Boss_JetGoblin_Status.JetGoblin__Base__Bomb_Count; // 폭탄 개수
    int m_Usable_Bomb_Count = Boss_JetGoblin_Status.JetGoblin__Base__Bomb_Count; // 사용가능한 폭탄 개수

    float m_Total_Moving_Cooltime; // 이동 쿨타임
    float m_Current_Moving_Cooltime; // 이동 쿨타임 "체크"
    float m_Total_Bomb_Drop_Cooltime; // 폭탄 드랍 쿨타임
    float m_Current_Bomb_Drop_Cooltime; // 폭탄 드랍 쿨타임 "체크"
    float m_Total_Landing_Cooltime; // 이동-착륙 쿨타임
    float m_Current_Landing_Cooltime; // 이동-착륙 쿨타임 "체크"

    
    int m_MCL_Index = 0; // MCL 인덱스

    // =====================================








    // 최초 소환 시 수행하는 메소드
    void Awake ()
    {
        // 객체 설정
        c_JetGoblin = this;

        m_Rigidbody = GetComponent<Rigidbody>();

        // 몬스터의 행동 코루틴들을 설정
        m_Behavior_Move = Behavior_Move();
        m_Behavior_Drop_Bomb = Behavior_Drop_Bomb();
        m_Behavior_Landing = Behavior_Landing();

        // 처음 실행할 행동 설정
        m_Current_Behavior = Behavior_Move();

        // 보스 스테이지인지 판단하여 (StageManager에서 설정)
        // 처음 실행할 모드 설정 및 그에 따른 스탯 설정
        if (StageManager.c_Stage_Manager.m_is_Boss_Stage)
        {
            Mode_Change(Boss_Mode.NORMAL_MODE);
            Invoke("Wait_To_Start_Presentation", 6.0f);
            //StartCoroutine(Do_Behavior());
        }

        else
        {
            Mode_Change(Boss_Mode.SUDDENDEATH_MODE);
            StartCoroutine(Do_Behavior());
        }
    }





    // 시작시 카메라 연출이 끝날 때 까지 대기한 후 수행하는 메소드
    void Wait_To_Start_Presentation()
    {
        // 코루틴 실행
        StartCoroutine(Do_Behavior());
    }



    // ======================================
    // Think ================================

    // 어떤 행동을 할지 생각하기 위한 메소드 (미완성)
    void Think()
    {
        
    }

    // ======================================
    // ======================================







    // ================================
    // 이하는 "행동" 관련 메소드들이다.
    // ================================


    // 모든 행동의 베이스가 되는 코루틴
    IEnumerator Do_Behavior()
    {
        while (true)
        {
            //Think();

            if (m_Current_Behavior != null && m_Current_Behavior.MoveNext())
            {
                yield return m_Current_Behavior.Current;
            }

            else
            {
                yield return null;
            }
        }
    }


    // 이동
    IEnumerator Behavior_Move()
    {
        // 착륙 쿨타임이 다 차기 전까지는 계속 이동한다.
        while (m_Current_Landing_Cooltime < m_Total_Landing_Cooltime)
        {
            Move();
            Drop_Bomb(); // 이동 중에만 폭탄 투하

            if (m_Current_Landing_Cooltime >= m_Total_Landing_Cooltime)
            {
                // 착륙 쿨타임이 다 되면 착륙!
                m_Current_Behavior = m_Behavior_Landing;
            }
            yield return null;
        }


    }

    // 폭탄 드랍
    IEnumerator Behavior_Drop_Bomb()
    {
        yield return null;
    }

    // 착륙
    IEnumerator Behavior_Landing()
    {
        m_Rigidbody.useGravity = true;

        yield return new WaitForSeconds(8.0f); // 8초간 착륙해있는다.

        m_Rigidbody.useGravity = false;

        while (m_Rigidbody.position.y <= 7.0f)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + transform.up * m_Move_Speed * Time.deltaTime);
            yield return null;
        }

        if (m_Rigidbody.position.y >= 7.0f) // 위치 조정
        {
            m_Current_Behavior = m_Behavior_Move;
            Vector3 pos;
            pos.x = m_Rigidbody.position.x;
            pos.y = 7.0f;
            pos.z = m_Rigidbody.position.z;
            m_Rigidbody.position = pos;
            m_Current_Landing_Cooltime = 0.0f;
        }
        
    }

    // 모드 체인지 연출을 위한 행동 추가??

    // ===============================










    // ===============================
    // 이하는 "모드" 관련 메소드들이다.
    // ===============================


    // 모드 전환
    void Mode_Change(int mode_number)
    {
        switch(mode_number)
        {
            //==================

            case Boss_Mode.SUDDENDEATH_MODE:
                Mode_SuddenDeath();
                break;
            
            //==================

            case Boss_Mode.NORMAL_MODE:
                Mode_Normal();
                break;

            //==================
        }
    }

    // 서든데스 모드
    void Mode_SuddenDeath()
    {
        m_Current_Mode = Boss_Mode.SUDDENDEATH_MODE;

        m_Health = 1.0f;
        m_MaxHealth = m_Health;
        m_Move_Speed += Boss_JetGoblin_Status.JetGoblin__SuddenDeath__Extra_Move_Speed;
        m_Total_Bomb_Count += Boss_JetGoblin_Status.JetGoblin__SuddenDeath__Extra_Bomb_Count;
        m_Usable_Bomb_Count += Boss_JetGoblin_Status.JetGoblin__SuddenDeath__Extra_Bomb_Count;


        m_Total_Bomb_Drop_Cooltime = Boss_JetGoblin_Status.JetGoblin__SuddenDeath__BombDrop_Cooltime;
        m_Current_Bomb_Drop_Cooltime = 0.0f;
        m_Total_Landing_Cooltime = 1.0f;
        m_Current_Landing_Cooltime = 0.0f;
    }

    // 기본 모드
    void Mode_Normal()
    {
        m_Current_Mode = Boss_Mode.NORMAL_MODE;

        m_Health += Boss_JetGoblin_Status.JetGoblin__Normal__Extra_Health;
        m_MaxHealth = m_Health;
        m_Move_Speed += Boss_JetGoblin_Status.JetGoblin__Normal__Extra_Move_Speed;
        m_Total_Bomb_Count += Boss_JetGoblin_Status.JetGoblin__Normal__Extra_Bomb_Count;
        m_Usable_Bomb_Count += Boss_JetGoblin_Status.JetGoblin__Normal__Extra_Bomb_Count;


        m_Total_Bomb_Drop_Cooltime = Boss_JetGoblin_Status.JetGoblin__Normal__BombDrop_Cooltime;
        m_Current_Bomb_Drop_Cooltime = 0.0f;
        m_Total_Landing_Cooltime = Boss_JetGoblin_Status.JetGoblin__Normal__Landing_Cooltime;
        m_Current_Landing_Cooltime = 0.0f;
    }

    // ===============================










    // =============================================
    // 이하는 "행동 및 판정에 도움"을 줄 메소드들이다.
    // =============================================


    // 몬스터 자신의 MCL 인덱스를 받아오는 함수
    void Find_My_Coord()
    {
        if (StageManager.m_is_init_MCL)
        {
            m_MCL_Index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z, false);
        }
    }

    // 자신의 MCL인덱스에 따른 올바른 위치로 이동
    void Set_Right_Position()
    {
        if (StageManager.m_is_init_MCL)
        {
            m_MCL_Index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z, false);

            if (m_MCL_Index != -1)
            {
                Vector3 Loc;
                Loc.x = StageManager.m_Map_Coordinate_List[m_MCL_Index].x;
                Loc.y = transform.position.y;
                Loc.z = StageManager.m_Map_Coordinate_List[m_MCL_Index].z;
                transform.position = Loc;
            }
        }
    }



    // 서든데스 모드에서 울타리 충돌체에 닿으면
    // 새로운 위치/방향을 Set한다.
    void Set_New_Position_SuddenDeath()
    {
        // 위치
        Vector3 newPosition;

        newPosition.x = Random.Range(0, 28);
        newPosition.x -= newPosition.x % 2;
        newPosition.y = transform.position.y;
        newPosition.z = Random.Range(50, 78);
        newPosition.z -= newPosition.z % 2;
        transform.position = newPosition;

        // 방향
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        var heading = player.transform.position - transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;

        float Dot = Vector3.Dot(transform.forward, direction);

        float Angle = Mathf.Acos(Dot);
        Angle *= Mathf.Rad2Deg;

        transform.Rotate(transform.up, Angle);

        float AngleY = 0.0f;
        if (transform.localEulerAngles.y >= 315.0f && transform.localEulerAngles.y < 45.0f)
        {
            AngleY = 0.0f;
            newPosition.z = 50.0f;
        }
            
        else if (transform.localEulerAngles.y >= 45.0f && transform.localEulerAngles.y < 135.0f)
        {
            AngleY = 90.0f;
            newPosition.x = 0.0f;
        }
            
        else if (transform.localEulerAngles.y >= 135.0f && transform.localEulerAngles.y < 225.0f)
        {
            AngleY = 180.0f;
            newPosition.z = 78.0f;
        }
        else if (transform.localEulerAngles.y >= 225.0f && transform.localEulerAngles.y < 315.0f)
        {
            AngleY = 270.0f;
            newPosition.x = 28.0f;
        }

        transform.position = newPosition;
        transform.localEulerAngles = new Vector3(0.0f, AngleY, 0.0f);

    }


    // 폭탄 피격
    void Hurt()
    {
        if (m_Health < 0.0f)
        {
            StopCoroutine(Do_Behavior());
            Invoke("Dead", 1.0f);
        }
        else
        {
            m_Health -= 20.0f;
        }
    }

    // 사망
    void Dead()
    {
        StageManager.c_Stage_Manager.m_is_Boss_Dead = true;
        Destroy(gameObject);
    }

    // 폭탄 투하
    void Drop_Bomb()
    {
        if (m_Current_Bomb_Drop_Cooltime < m_Total_Bomb_Drop_Cooltime)
        {
            m_Current_Bomb_Drop_Cooltime += Time.deltaTime;
        }

        else if (m_Usable_Bomb_Count > 0)
        {
            // 폭탄 생성
            GameObject Instance_Bomb = Instantiate(m_Bomb);
            
            // 사용 가능 폭탄 개수 감소
            m_Usable_Bomb_Count -= 1;

            // 위치 조정
            Vector3 pos;
            pos.x = transform.position.x;
            pos.y = transform.position.y - 1.5f;
            pos.z = transform.position.z;
            Instance_Bomb.transform.position = pos;

            // 주인 마킹
            Instance_Bomb.GetComponent<Bomb>().m_Whose_Bomb = gameObject;
            Instance_Bomb.GetComponent<Bomb>().m_Whose_Bomb_Type = WHOSE_BOMB.JETGOBLIN;

            // 방향 설정
            Instance_Bomb.gameObject.GetComponent<Bomb>().SetBombDir();

            // 쿨타임 리셋
            m_Current_Bomb_Drop_Cooltime = 0.0f;
        }
    }

    // 폭탄 재장전
    public void Bomb_Reload()
    {
        m_Usable_Bomb_Count += 1;
        if (m_Usable_Bomb_Count > m_Total_Bomb_Count)
            m_Usable_Bomb_Count = m_Total_Bomb_Count;
    }

    // 이동
    void Move()
    {
        // 서든데스 모드가 아닐 때
        if (m_Current_Mode != Boss_Mode.SUDDENDEATH_MODE)
        {
            m_Current_Landing_Cooltime += Time.deltaTime;
            transform.Rotate(transform.up, 5.0f);
        }

        m_Rigidbody.MovePosition(m_Rigidbody.position + transform.forward * m_Move_Speed * Time.deltaTime);

    }

    void OnCollisionEnter(Collision collision)
    {
        
        // 울타리 밖으로 나가지 않도록
        if (collision.gameObject.CompareTag("Wall"))
        {
            // 서든데스 모드일 때
            if (m_Current_Mode == Boss_Mode.SUDDENDEATH_MODE)
            {
                Set_New_Position_SuddenDeath();
                m_Current_Bomb_Drop_Cooltime = 0.0f;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 몬스터가 불에 닿으면 사망 판정
        if ((other.gameObject.tag == "Flame" || other.gameObject.CompareTag("Flame_Bush")))
        {
            Hurt();
        }
        //===============================
    }

    void OnTriggerExit(Collider other)
    {
        // 폭탄 투하를 매끄럽게 하기 위해
        if (other.gameObject.CompareTag("Bomb"))
        {
            other.isTrigger = false;
        }
    }

    // =============================================
}
