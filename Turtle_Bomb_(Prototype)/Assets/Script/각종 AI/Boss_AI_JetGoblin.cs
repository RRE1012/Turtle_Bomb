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
    public const int JetGoblin__Base__Bomb_Count = 0;
    public const int JetGoblin__Base__Fire_Count = 0;


    public const float JetGoblin__SuddenDeath__Extra_Move_Speed = 8.0f;
    public const float JetGoblin__SuddenDeath__BombDrop_Cooltime = 1.0f;
    public const int JetGoblin__SuddenDeath__Extra_Bomb_Count = 7;

    public const float JetGoblin__Normal__Extra_Health = 0.0f;
    public const float JetGoblin__Normal__Extra_Move_Speed = 5.0f;
    public const int JetGoblin__Normal__Extra_Bomb_Count = 2;
    public const float JetGoblin__Normal__Moving_Cooltime = 1.5f;
    public const float JetGoblin__Normal__BombDrop_Cooltime = 2.0f;
    public const float JetGoblin__Normal__Landing_Cooltime = 15.0f;
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
    IEnumerator m_Behavior_Break;
    IEnumerator m_Behavior_WallCrash;
    IEnumerator m_Behavior_Landing;
    IEnumerator m_Intro_Moving;

    public static Boss_AI_JetGoblin c_JetGoblin; // 이 객체

    public GameObject m_Bomb; // 고블린이 사용할 폭탄

    Rigidbody m_Rigidbody;

    Animator m_Boss_JetGoblin_Animator; // 애니메이터

    float m_Health = Boss_JetGoblin_Status.JetGoblin__Base__Health; // 체력
    float m_MaxHealth; // 최대 체력
    float m_Move_Speed = Boss_JetGoblin_Status.JetGoblin__Base__Move_Speed; // 이동속도
    int m_Total_Bomb_Count = Boss_JetGoblin_Status.JetGoblin__Base__Bomb_Count; // 폭탄 개수
    int m_Usable_Bomb_Count = Boss_JetGoblin_Status.JetGoblin__Base__Bomb_Count; // 사용가능한 폭탄 개수
    int m_Fire_Count = Boss_JetGoblin_Status.JetGoblin__Base__Fire_Count;

    float m_Total_Moving_Cooltime; // 이동 쿨타임
    float m_Current_Moving_Cooltime; // 이동 쿨타임 "체크"
    float m_Total_Bomb_Drop_Cooltime; // 폭탄 드랍 쿨타임
    float m_Current_Bomb_Drop_Cooltime; // 폭탄 드랍 쿨타임 "체크"
    float m_Total_Landing_Cooltime; // 이동-착륙 쿨타임
    float m_Current_Landing_Cooltime; // 이동-착륙 쿨타임 "체크"
    float m_Total_Wall_Crash_Move_Cooltime = 2.0f; // 벽 충돌시 밀려남 쿨타임
    float m_Current_Wall_Crash_Move_Cooltime = 0.0f; // 벽 충돌시 밀려남 쿨타임 "체크"

    SkinnedMeshRenderer[] m_BossRenderer;
    MeshRenderer m_JetRenderer;

    int m_MCL_Index = 0; // MCL 인덱스
    // =====================================








    // 최초 소환 시 수행하는 메소드
    void Start ()
    {
        // 객체 설정
        c_JetGoblin = this;

        m_Rigidbody = GetComponent<Rigidbody>();

        m_Boss_JetGoblin_Animator = GetComponent<Animator>();
        m_Boss_JetGoblin_Animator.SetBool("Idle", true);

        // 몬스터의 행동 코루틴들을 설정
        m_Behavior_Move = Behavior_Move();
        m_Behavior_Break = Behavior_Break();
        m_Behavior_Landing = Behavior_Landing();
        m_Behavior_WallCrash = Behavior_WallCrash();

        m_Current_Behavior = Behavior_Move();

        Mode_Change(Boss_Mode.SUDDENDEATH_MODE);

        StartCoroutine(Do_Behavior());

        // 보스 스테이지인지 판단하여 (StageManager에서 설정)
        // 처음 실행할 모드 설정 및 그에 따른 스탯 설정
        /*
        if (StageManager.c_Stage_Manager.Get_is_Boss_Stage())
        {
            m_Current_Behavior = Wait_To_Intro();

            Mode_Change(Boss_Mode.NORMAL_MODE);

            m_BossRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
            m_JetRenderer = GetComponentInChildren<MeshRenderer>();
            //GetComponentInChildren<LineRenderer>().enabled = false; // 실선 표시기 비활성화
            foreach (SkinnedMeshRenderer s in m_BossRenderer)
                s.enabled = false;
            m_JetRenderer.enabled = false;

            StartCoroutine(Do_Behavior());
        }

        else
        {
            m_Current_Behavior = Behavior_Move();

            Mode_Change(Boss_Mode.SUDDENDEATH_MODE);

            StartCoroutine(Do_Behavior());
        }
        */
    }
    



    // ======================================
    // Think ================================

    // 어떤 행동을 할지 생각하기 위한 메소드 (미완성)
    void Think()
    {
        if (m_Current_Landing_Cooltime >= m_Total_Landing_Cooltime * 2.0f / 3.0f && m_Current_Landing_Cooltime < m_Total_Landing_Cooltime)
        {
            // 착륙 쿨타임이 2/3 지나면 제트기가 고장난다.
            m_Current_Behavior = m_Behavior_Break;
            m_Boss_JetGoblin_Animator.SetBool("Move", false);
            m_Boss_JetGoblin_Animator.SetBool("Idle", true);
        }

        if (m_Current_Landing_Cooltime >= m_Total_Landing_Cooltime)
        {
            // 착륙 쿨타임이 다 되면 착륙!
            m_Current_Behavior = m_Behavior_Landing;
        }
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
            Think();

            if (!StageManager.c_Stage_Manager.Get_is_Pause() && m_Current_Behavior != null && m_Current_Behavior.MoveNext())
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
            if (StageManager.c_Stage_Manager.Get_is_Intro_Over())
            {
                Move();
                Drop_Bomb(); // 폭탄 투하
            }
            yield return null;
        }
    }

    // 제트기 고장
    IEnumerator Behavior_Break()
    {
        while (m_Current_Landing_Cooltime < m_Total_Landing_Cooltime)
        {
            BreakMove();
            Drop_Bomb(); // 폭탄 투하
            yield return null;
        }
    }

    // 벽 충돌시 뒤로 살짝 밀려나게
    IEnumerator Behavior_WallCrash()
    {
        while (m_Current_Wall_Crash_Move_Cooltime < m_Total_Wall_Crash_Move_Cooltime)
        {
            WallCrashMove();
            m_Boss_JetGoblin_Animator.SetBool("Move", false);
            m_Boss_JetGoblin_Animator.SetBool("Idle", true);
            yield return null;
        }
    }

    // 착륙
    IEnumerator Behavior_Landing()
    {
        // 착륙=====================================
        m_Rigidbody.useGravity = true;

        if (MusicManager.manage_ESound != null)
            MusicManager.manage_ESound.Boss_Goblin_Fall_Sound();

        m_Boss_JetGoblin_Animator.SetBool("Idle", false);
        Invoke("Motion_Landing_StandUp", 1.0f);
        Invoke("Motion_Landing_idle", 3.0f);
        Invoke("Motion_Ready_to_Rising", 8.0f);

        yield return new WaitForSeconds(10.0f); // 10초간 착륙해있는다.



        // 이륙=====================================
        m_Boss_JetGoblin_Animator.SetBool("Idle", true);


        m_Rigidbody.useGravity = false;

        while (m_Rigidbody.position.y < 7.0f)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + transform.up * m_Move_Speed * Time.deltaTime);
            yield return null;
        }

        if (m_Rigidbody.position.y >= 7.0f) // 위치 조정
        {
            m_Current_Behavior = m_Behavior_Move; // 이동 상태로 변경

            Vector3 pos;
            pos.x = m_Rigidbody.position.x;
            pos.y = 7.0f;
            pos.z = m_Rigidbody.position.z;
            m_Rigidbody.position = pos;

            m_Current_Landing_Cooltime = 0.0f; // 착륙 쿨타임 초기화
            m_Current_Moving_Cooltime = 0.0f; // 이동 쿨타임 초기화

            m_Boss_JetGoblin_Animator.SetBool("Idle", false);
            m_Boss_JetGoblin_Animator.SetBool("Move", true);
            m_Behavior_Landing = Behavior_Landing();
        }
    }


    IEnumerator Wait_To_Intro()
    {
        while (true)
        {
            if (StageManager.c_Stage_Manager.Get_is_Intro_Over())
            {
                m_Current_Behavior = m_Behavior_Move;
                //GetComponentInChildren<LineRenderer>().enabled = true; // 실선 표시기 활성화
                foreach (SkinnedMeshRenderer s in m_BossRenderer)
                    s.enabled = true;
                m_JetRenderer.enabled = true;
            }
            yield return null;
        }
    }


    // 모드 체인지 연출을 위한 행동 추가??

    // ===============================

    void Motion_Landing_StandUp()
    {
        m_Boss_JetGoblin_Animator.SetTrigger("Landing_StandUp");
    }

    void Motion_Landing_idle()
    {
        m_Boss_JetGoblin_Animator.SetBool("Landing_Idle", true);
    }

    void Motion_Ready_to_Rising()
    {
        m_Boss_JetGoblin_Animator.SetTrigger("Ready_to_Rising");
        m_Boss_JetGoblin_Animator.SetBool("Landing_Idle", false);
    }






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

        m_Total_Moving_Cooltime = Boss_JetGoblin_Status.JetGoblin__Normal__Moving_Cooltime;
        m_Current_Moving_Cooltime = 0.0f;
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
            m_MCL_Index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(transform.position.x, transform.position.z);
        }
    }

    // 자신의 MCL인덱스에 따른 올바른 위치로 이동
    void Set_Right_Position()
    {
        if (StageManager.m_is_init_MCL)
        {
            m_MCL_Index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(transform.position.x, transform.position.z);

            if (m_MCL_Index != -1)
            {
                Vector3 Loc;
                Loc.x = StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_MCL_Index].x;
                Loc.y = transform.position.y;
                Loc.z = StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_MCL_Index].z;
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

    // 폭탄 피격
    void Hurt()
    {
        if (MusicManager.manage_ESound != null)
            MusicManager.manage_ESound.Boss_Goblin_Hurt_Sound();

        m_Health -= 20.0f;
        m_Boss_JetGoblin_Animator.SetTrigger("Hurt");

        if (m_Health < 0.0f && StageManager.c_Stage_Manager.GetBossDead() == false)
        {
            StageManager.c_Stage_Manager.SetBossDead(true);

            if (MusicManager.manage_ESound != null)
                MusicManager.manage_ESound.Boss_Goblin_Dead_Sound();
            StopCoroutine(Do_Behavior());
            m_Boss_JetGoblin_Animator.SetTrigger("Dead");
            Invoke("Dead", 2.0f);
        }
    }

    // 사망
    void Dead()
    {
        Destroy(gameObject);
        StageManager.c_Stage_Manager.Stage_Clear(); // 보스를 잡으면 스테이지 클리어
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
            if (MusicManager.manage_ESound != null)
                MusicManager.manage_ESound.Boss_Goblin_Throw_Sound();

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
            Instance_Bomb.GetComponent<Bomb>().SetBombDir();

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
        m_Boss_JetGoblin_Animator.SetBool("Move", true);
        m_Boss_JetGoblin_Animator.SetBool("Idle", false);

        // 서든데스 모드가 아닐 때
        if (m_Current_Mode != Boss_Mode.SUDDENDEATH_MODE)
        {
            m_Current_Landing_Cooltime += Time.deltaTime;

            if (m_Current_Moving_Cooltime < m_Total_Moving_Cooltime)
            {
                m_Current_Moving_Cooltime += Time.deltaTime;
                m_Rigidbody.MovePosition(m_Rigidbody.position + transform.forward * m_Move_Speed * Time.deltaTime);
            }
            else
            {
                if (m_Current_Moving_Cooltime < m_Total_Moving_Cooltime + 1.5f)
                {
                    m_Current_Moving_Cooltime += Time.deltaTime;
                }
                else m_Current_Moving_Cooltime = 0.0f;
            }
        }
        else
            m_Rigidbody.MovePosition(m_Rigidbody.position + transform.forward * m_Move_Speed * Time.deltaTime);

    }

    // 고장났을때의 이동
    void BreakMove()
    {
        // 빙글빙글 돈다
        m_Current_Landing_Cooltime += Time.deltaTime;
        transform.Rotate(transform.up, 5.0f);
        m_Rigidbody.MovePosition(m_Rigidbody.position + transform.forward * m_Move_Speed * Time.deltaTime);
    }

    // 벽 충돌시 밀려남
    void WallCrashMove()
    {
        m_Current_Wall_Crash_Move_Cooltime += Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + -transform.forward * m_Move_Speed * 0.1f * Time.deltaTime);

        if (m_Current_Wall_Crash_Move_Cooltime >= m_Total_Wall_Crash_Move_Cooltime)
        {
            m_Current_Wall_Crash_Move_Cooltime = 0.0f;
            Find_New_Direction();
            m_Current_Behavior = m_Behavior_Move;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 몬스터가 불에 닿으면 히트 판정
        if ((other.gameObject.tag == "Flame" || other.gameObject.CompareTag("Flame_Bush")))
        {
            Hurt();
        }

        // 울타리 밖으로 나가지 않도록
        if (other.gameObject.CompareTag("Wall"))
        {
            // 서든데스 모드일 때
            if (m_Current_Mode == Boss_Mode.SUDDENDEATH_MODE)
            {
                Set_New_Position_SuddenDeath();
                m_Current_Bomb_Drop_Cooltime = 0.0f;
            }

            else
            {
                if (MusicManager.manage_ESound != null)
                    MusicManager.manage_ESound.Boss_Goblin_Wall_Crush_Sound();
                m_Current_Behavior = m_Behavior_WallCrash;
            }
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



    public void Set_Bomb_info(int bombcount, int firecount)
    {
        m_Total_Bomb_Count = bombcount;
        m_Usable_Bomb_Count = bombcount;
        m_Fire_Count = firecount;
    }

    public void Set_Glider_Speed(float s)
    {
        m_Move_Speed = s;
    }

    public int Get_Fire_Count()
    {
        return m_Fire_Count;
    }
}
