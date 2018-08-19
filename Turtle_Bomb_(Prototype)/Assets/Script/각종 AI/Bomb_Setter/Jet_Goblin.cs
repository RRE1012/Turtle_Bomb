using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// #define 보스 스탯
static class Boss_JetGoblin_Status
{
    public const float JetGoblin__Base__Health = 10.0f;
    public const float JetGoblin__Base__Move_Speed = 5.0f;
    public const float JetGoblin__SuddenDeath__BombDrop_Cooltime = 1.5f;
}





public class Jet_Goblin : Bomb_Setter
{

    // =====================================
    // =============변수 선언부=============
    // =====================================

    // 현재 진행중인 "행동"
    IEnumerator m_Current_Behavior;
    
    // "행동들"
    IEnumerator m_Behavior_Move;
    IEnumerator m_Behavior_Break;
    IEnumerator m_Behavior_WallCrash;
    IEnumerator m_Behavior_Landing;
    IEnumerator m_Intro_Moving;

    public GameObject m_Bomb; // 고블린이 사용할 폭탄

    Rigidbody m_Rigidbody;

    Animator m_Boss_JetGoblin_Animator; // 애니메이터

    float m_Health = Boss_JetGoblin_Status.JetGoblin__Base__Health; // 체력
    float m_MaxHealth = Boss_JetGoblin_Status.JetGoblin__Base__Health; // 최대 체력
    float m_Move_Speed = Boss_JetGoblin_Status.JetGoblin__Base__Move_Speed; // 이동속도

    float m_Total_Moving_Cooltime; // 이동 쿨타임
    float m_Current_Moving_Cooltime; // 이동 쿨타임 "체크"
    float m_Total_Bomb_Drop_Cooltime; // 폭탄 드랍 쿨타임
    float m_Current_Bomb_Drop_Cooltime; // 폭탄 드랍 쿨타임 "체크"

    SkinnedMeshRenderer[] m_BossRenderer;
    MeshRenderer m_JetRenderer;

    Player m_Target;
    GameObject m_Temp_Bomb;

    int m_MCL_Index = 0; // MCL 인덱스
    // =====================================








    // 최초 소환 시 수행하는 메소드
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        m_Boss_JetGoblin_Animator = GetComponent<Animator>();
        m_Boss_JetGoblin_Animator.SetBool("Idle", true);

        // 몬스터의 행동 코루틴들을 설정
        m_Behavior_Move = Behavior_Move();

        m_Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        m_Current_Behavior = Behavior_Move();

        Mode_SuddenDeath();

        StartCoroutine(Wait_For_Intro());
    }

    IEnumerator Wait_For_Intro()
    {
        while (true)
        {
            StopAllCoroutines();
            StartCoroutine(Do_Behavior());
            yield return null;
        }
    }

    // ================================
    // 이하는 "행동" 관련 메소드들이다.
    // ================================


    // 모든 행동의 베이스가 되는 코루틴
    IEnumerator Do_Behavior()
    {
        while (true)
        {
            if (!StageManager.GetInstance().Get_is_Pause() && m_Current_Behavior != null && m_Current_Behavior.MoveNext())
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
        while (true)
        {
            Move(); // 이동
            Drop_Bomb(); // 폭탄 투하
            yield return null;
        }
    }


    // ===============================
    // 이하는 "모드" 관련 메소드들이다.
    // ===============================

       

    // 서든데스 모드
    void Mode_SuddenDeath()
    {
        m_Health = 1.0f;
        m_MaxHealth = m_Health;

        m_Total_Bomb_Drop_Cooltime = Boss_JetGoblin_Status.JetGoblin__SuddenDeath__BombDrop_Cooltime;
        m_Current_Bomb_Drop_Cooltime = 0.0f;
    }
    

    // ===============================










    // =============================================
    // 이하는 "행동 및 판정에 도움"을 줄 메소드들이다.
    // =============================================


    // 몬스터 자신의 MCL 인덱스를 받아오는 함수
    void Find_My_Coord()
    {
        if (StageManager.GetInstance().m_is_init_MCL)
        {
            m_MCL_Index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);
        }
    }

    // 자신의 MCL인덱스에 따른 올바른 위치로 이동
    void Set_Right_Position()
    {
        if (StageManager.GetInstance().m_is_init_MCL)
        {
            m_MCL_Index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);

            if (m_MCL_Index != -1)
            {
                Vector3 Loc;
                Loc.x = StageManager.GetInstance().m_Map_Coordinate_List[m_MCL_Index].x;
                Loc.y = transform.position.y;
                Loc.z = StageManager.GetInstance().m_Map_Coordinate_List[m_MCL_Index].z;
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
        transform.LookAt(m_Target.transform);

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


    void Find_New_Direction()
    {
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0:
                transform.Rotate(transform.up, 90.0f);
                break;

            case 1:
                transform.Rotate(transform.up, 180.0f);
                break;

            case 2:
                transform.Rotate(transform.up, 270.0f);
                break;
        }
    }
    

    // 폭탄 투하
    void Drop_Bomb()
    {
        Debug.Log("Drop_Check_Start");
        Debug.Log(m_Current_Bomb_Drop_Cooltime + " / " + m_Total_Bomb_Drop_Cooltime);
        if (m_Current_Bomb_Drop_Cooltime < m_Total_Bomb_Drop_Cooltime)
            m_Current_Bomb_Drop_Cooltime += Time.deltaTime;

        else
        {
            GetComponentInChildren<Jet_Goblin_Sound>().Play_Bomb_Throw_Sound();

            Bomb_Set(CALL_BOMB_STATE.DROP);

            Debug.Log("Bomb_Drop");

            m_Current_Bomb_Drop_Cooltime = 0.0f; // 쿨타임 리셋
        }
    }

    // 이동
    void Move()
    {
        m_Boss_JetGoblin_Animator.SetBool("Move", true);
        m_Boss_JetGoblin_Animator.SetBool("Idle", false);
        m_Rigidbody.MovePosition(m_Rigidbody.position + transform.forward * m_Move_Speed * Time.deltaTime);

    }

    void OnTriggerEnter(Collider other)
    {
        // 울타리 밖으로 나가지 않도록
        if (other.gameObject.CompareTag("Wall"))
        {
            Set_New_Position_SuddenDeath();
            m_Current_Bomb_Drop_Cooltime = 0.0f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 폭탄 투하를 매끄럽게 하기 위해
        if (other.gameObject.CompareTag("Bomb"))
            other.isTrigger = false;
    }

    // =============================================



    public void Set_Bomb_info(int bombcount, int firecount, float speed_value)
    {
        m_Max_Bomb_Count = bombcount;
        m_Curr_Bomb_Count = bombcount;
        m_Fire_Count = firecount;
        m_Move_Speed = speed_value;
    }
}
