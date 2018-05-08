using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// #define 보스 모드
static class Boss_Mode_List
{
    public const int NORMAL_MODE = 0;
    public const int ANGRY_MODE = 1;
    public const int GROGGY_MODE = 2;
}

public class Boss_Behavior : MonoBehaviour
{
    NavMeshAgent m_NVAgent;
    Adventure_Boss_Data m_Boss_Data;
    Animator m_Boss_Animator;

    GameObject m_Target;
    GameObject m_NavPlane;

    Monster_Player_Detector m_Attack_Detector;
    Transform m_Attack_Collider;
    Detector_Box m_Target_Detector;
    Transform m_Attack_Range_UI;

    IEnumerator m_Do_Behavior;
    IEnumerator m_Current_Behavior;
    IEnumerator m_Behavior_Find;
    IEnumerator m_Behavior_Chase;
    IEnumerator m_Behavior_Attack;
    IEnumerator m_NULLBehavior;
    
    float m_WalkTimer = 0.0f;
    float m_AttackTimer = 0.0f;
    
    bool m_isDead = false;
    bool m_is_First_Attack = true;
    bool m_Attack_is_Done = true;

    float m_Loss_Time = 0.0f;
    float m_Forgot_Time = 4.0f;

    float m_curr_Hurt_Time = 1.0f;
    float m_Hurt_CoolTime = 1.0f;

    float m_idle_sound_Timer = 0.0f;
    float m_idle_sound_Cooltime = 2.0f;
    

    // 기본 위치
    Vector3 Base_Position;

    // 스탯
    int m_curr_Mode_Number; // 현재 모드 번호
    int m_Health; // 체력
    float m_Monster_Move_Speed; // 일반 이동속도
    float m_Attack_Speed_Slow; // 슬로우모션 공격속도
    float m_Attack_Speed; // 일반 공격속도

    void Start()
    {
        // 내비게이터 등록
        m_NVAgent = gameObject.GetComponent<NavMeshAgent>();
        m_NavPlane = GameObject.Find("Navigation_Plane");

        // 애니메이터 등록
        m_Boss_Animator = GetComponent<Animator>();

        // 플레이어 감지기 등록
        m_Target_Detector = GetComponentInChildren<Detector_Box>();
        m_Attack_Detector = GetComponentInChildren<Monster_Player_Detector>();

        // 공격 충돌박스 등록
        m_Attack_Collider = transform.Find("Attack_Range");
        // 비활성화
        m_Attack_Collider.gameObject.SetActive(false);

        // 공격 범위 표시기 등록
        m_Attack_Range_UI = transform.Find("Attack_Range_UI");
        m_Attack_Range_UI.gameObject.SetActive(false);

        m_Boss_Data = StageManager.c_Stage_Manager.Get_Adventure_Boss_Data();


        Base_Position = transform.position;
        
        // ========스탯설정=========
        ModeChange(Boss_Mode_List.NORMAL_MODE); // 최초 시작시 보통모드로 시작
        m_Health = m_Boss_Data.Boss_HP; // 체력 설정
        GameObject.Find("Boss_Health").GetComponentInChildren<Text>().enabled = true; // 체력 표시기 활성화
        GameObject.Find("Boss_Health").GetComponentInChildren<Text>().text = "Boss Health: " + m_Health.ToString();
        // =========================

        


        // 몬스터의 행동 코루틴들을 설정
        m_Do_Behavior = Do_Behavior();
        m_Behavior_Find = FindPlayer();
        m_Behavior_Chase = Chase();
        m_Behavior_Attack = Attack();
        m_NULLBehavior = NULLBehavior();

        // 처음 실행할 행동 설정
        m_Current_Behavior = m_NULLBehavior;
        StartCoroutine(m_Do_Behavior);
    }

    void OnTriggerEnter(Collider other)
    {
        // 불에 닿을 시 데미지 판정
        if (!m_isDead && (other.gameObject.tag == "Flame" || other.gameObject.CompareTag("Flame_Bush")))
        {
            if (m_curr_Hurt_Time > m_Hurt_CoolTime)
                Hurt();
            else m_curr_Hurt_Time += Time.deltaTime;
        }
        //===============================
    }





    // 어떤 행동을 할지 생각하는 함수 (추후 가중치를 두어 어떤 행동을 할지 더 상세하게 구분해야함.)
    void Think()
    {
        if (m_Attack_is_Done) // 공격중이라면 공격이 끝나야 다른 행동을 할 수 있다.
        {
            if (m_Target_Detector.m_isInRange) // 감지 범위 안이라면
            {
                // 추격
                m_Current_Behavior = m_Behavior_Chase;
                m_NVAgent.isStopped = false;
                m_Target = m_Target_Detector.GetComponent<Detector_Box>().Get_Target();
                if (m_Target != null)
                    m_NVAgent.destination = m_Target.transform.position;
                m_Loss_Time = 0.0f;
                m_Boss_Animator.SetBool("Goblman_isWalk", true);
                m_Boss_Animator.SetBool("Goblman_isIdle", false);

                if (m_Attack_Detector.m_isInRange && !is_Blocked_Between_Target_And_Me()) // 그리고 공격 범위 안이라면
                {
                    // 공격

                    if (m_is_First_Attack) // 첫 공격인가?
                    {
                        m_AttackTimer = Monster_AI_Constants.Boss_Attack_Time; // 바로 공격함
                        m_is_First_Attack = false;
                    }

                    m_Current_Behavior = m_Behavior_Attack;
                    m_NVAgent.isStopped = true;
                }
            }
        }
    }





    // 모든 행동의 베이스가 되는 코루틴
    IEnumerator Do_Behavior()
    {
        while (true)
        {
            if (!StageManager.c_Stage_Manager.m_is_Pause && StageManager.c_Stage_Manager.m_is_Intro_Over)
            {
                Think();

                if (m_Current_Behavior != null && m_Current_Behavior.MoveNext())
                    yield return m_Current_Behavior.Current;

                else
                    yield return null;
            }

            else
            {
                if (m_NavPlane.activeSelf)
                    m_NVAgent.isStopped = true;
                yield return null;
            }
        }
    }

    



    // 플레이어 찾기
    IEnumerator FindPlayer()
    {
        while (true)
        {
            if (m_WalkTimer < Monster_AI_Constants.Walk_Time) // 일정 시간동안 걸어다님.
            {
                transform.Translate(new Vector3(0.0f, 0.0f, (m_Monster_Move_Speed * Time.deltaTime)));
                m_Boss_Animator.SetBool("Goblman_isWalk", true);
                m_Boss_Animator.SetBool("Goblman_isIdle", false);
                m_WalkTimer += Time.deltaTime;
            }

            else // idle 상태 진입
            {
                if (MusicManager.manage_ESound != null)
                    MusicManager.manage_ESound.Goblin_Idle_Sound();

                m_Boss_Animator.SetBool("Goblman_isWalk", false);
                m_Boss_Animator.SetBool("Goblman_isIdle", true);

                m_WalkTimer = 0.0f;
                yield return new WaitForSeconds(3.0f); // 3초간 idle 상태 유지

                float AngleY = Random.Range(0.0f, 360.0f);
                transform.localEulerAngles = new Vector3(0.0f, AngleY, 0.0f);
            }

            yield return null;
        }
        
    }





    // 추격
    IEnumerator Chase()
    {
        while(true)
        {
            if (m_Loss_Time < m_Forgot_Time) // 잊어버리기 까지 플레이어의 위치로 이동한다.
            {
                m_Loss_Time += Time.deltaTime;
                m_NVAgent.isStopped = false;
            }

            else // 잊어버렸다면 기본 위치로 돌아간다.
            {
                m_Current_Behavior = m_NULLBehavior;

                if (transform.position != Base_Position) // 기본 위치가 아니면
                {
                    m_NVAgent.isStopped = false;
                    m_NVAgent.destination = Base_Position;
                    m_Boss_Animator.SetBool("Goblman_isWalk", true);
                    m_Boss_Animator.SetBool("Goblman_isIdle", false);
                }
            }

            yield return null;
        }
    }
    




    // 공격
    IEnumerator Attack()
    {
        while (true)
        {
            if (m_AttackTimer < Monster_AI_Constants.Boss_Attack_Time)
            {
                m_AttackTimer += Time.deltaTime; // 공격타이머 증가
            }
            else
            {
                if (StageManager.c_Stage_Manager.m_is_Intro_Over && !StageManager.c_Stage_Manager.Get_Game_Over())
                {
                    if (m_Attack_is_Done)
                    {
                        if (MusicManager.manage_ESound != null && !m_Boss_Animator.GetBool("Goblman_isAttack")) // 공격할때 1번만 소리냄.
                            MusicManager.manage_ESound.Goblin_Attack_Sound();

                        m_Boss_Animator.SetBool("Goblman_isAttack", true); // 마찬가지로 1번만 수행
                        m_Boss_Animator.SetBool("Goblman_isIdle", false);
                        m_Attack_Range_UI.gameObject.SetActive(true); // 범위 표시기를 꺼낸다.

                        m_Target = m_Attack_Detector.GetComponent<Monster_Player_Detector>().Get_Target();

                        if (m_Target != null) // 타겟을 향해 방향 전환
                        {
                            Vector3 dir = m_Target.transform.position - transform.position;
                            Vector3 dirXZ = new Vector3(dir.x, 0.0f, dir.z);

                            if (dirXZ != Vector3.zero)
                            {
                                Quaternion targetRot = Quaternion.LookRotation(dirXZ);

                                transform.rotation = targetRot;
                            }
                        }

                        m_Attack_is_Done = false;
                    }
                    else
                    {
                        if (m_Boss_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Goblman_Attack"))
                        {
                            if (m_Boss_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) // 애니메이션이 끝나면
                            {
                                m_Boss_Animator.SetBool("Goblman_isAttack", false);
                                m_Boss_Animator.SetBool("Goblman_isIdle", true);
                                m_AttackTimer = 0.0f; // 시간도 초기화한다.
                                m_Current_Behavior = m_NULLBehavior;
                                m_Boss_Animator.SetFloat("Attack_Speed", m_Attack_Speed_Slow); // 공격속도를 슬로우모션으로 되돌린다.

                                m_Attack_is_Done = true; // 공격 완료 알림
                            }

                            else if (m_Boss_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f) // 중간 부분
                            {
                                m_Attack_Collider.gameObject.SetActive(false); // 공격용 충돌체를 집어넣는다.
                                m_Attack_Range_UI.gameObject.SetActive(false); // 범위 표시기도 집어넣는다.
                            }

                            else if (m_Boss_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f) // 초기 부분
                            {
                                m_Boss_Animator.SetFloat("Attack_Speed", m_Attack_Speed); // 슬로우모션 후 뒷부분은 빠르게하기 위해..
                                m_Attack_Collider.gameObject.SetActive(true); // 공격용 충돌체를 꺼낸다.

                            }
                        }
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator NULLBehavior()
    {
        while(true)
        {
            Base_Position.y = transform.position.y;
            if (transform.position == Base_Position) // 기본 위치이면 가만히 있는다.
            {
                if (MusicManager.manage_ESound != null)
                {
                    if (m_idle_sound_Timer >= m_idle_sound_Cooltime)
                    {
                        MusicManager.manage_ESound.Goblin_Idle_Sound();
                        m_idle_sound_Timer = 0.0f;
                    }
                    else
                        m_idle_sound_Timer += Time.deltaTime;
                }

                m_NVAgent.isStopped = true;
                m_Boss_Animator.SetBool("Goblman_isWalk", false);
                m_Boss_Animator.SetBool("Goblman_isIdle", true);
                m_is_First_Attack = true; // 기본 위치로 가면 첫 공격 여부 초기화
            }
            yield return null;
        }
    }


    // 폭탄 피격
    void Hurt()
    {
        if (MusicManager.manage_ESound != null)
            MusicManager.manage_ESound.Boss_Goblin_Hurt_Sound();

        if (m_Health - m_Boss_Data.Bomb_Damage >= 0)
            m_Health -= m_Boss_Data.Bomb_Damage;

        if (GameObject.Find("Boss_Health")) // 체력 표시 UI
            GameObject.Find("Boss_Health").GetComponentInChildren<Text>().text = "Boss Health: " + m_Health.ToString();

        // 체력에 따른 모드 전환
        if (m_Health <= m_Boss_Data.Angry_Condition_Start_HP && m_curr_Mode_Number == Boss_Mode_List.NORMAL_MODE)
            ModeChange(Boss_Mode_List.ANGRY_MODE);
        if (m_Health <= m_Boss_Data.Groggy_Condition_Start_HP && m_curr_Mode_Number == Boss_Mode_List.ANGRY_MODE)
            ModeChange(Boss_Mode_List.GROGGY_MODE);

        m_curr_Hurt_Time = 0.0f;

        if (m_Health <= 0)
        {
            if (MusicManager.manage_ESound != null)
                MusicManager.manage_ESound.Boss_Goblin_Dead_Sound();

            StopCoroutine(m_Do_Behavior);

            m_Boss_Animator.SetBool("Goblman_isDead", true);
            Invoke("Dead", 2.0f);
        }
    }





    // 죽음
    void Dead()
    {
        StageManager.c_Stage_Manager.SetBossDead(true);
        Destroy(gameObject);
        StageManager.c_Stage_Manager.Stage_Clear(); // 보스를 잡으면 스테이지 클리어
    }



    // 모드 전환
    void ModeChange(int Mode_Number)
    {
        m_curr_Mode_Number = Mode_Number;

        switch (m_curr_Mode_Number)
        {
            case Boss_Mode_List.NORMAL_MODE:
                m_Monster_Move_Speed = 2.0f; // 일반 이동속도 설정
                m_NVAgent.speed = 2.0f; // 추격시 이동속도 설정
                m_NVAgent.angularSpeed = 360.0f; // 추격시 회전속도 설정
                m_Attack_Speed_Slow = 0.2f; // 슬로우 모션 공격속도 설정
                m_Attack_Speed = 1.0f; // 진짜 공격속도 설정

                // 1. 버프 스킬 지속시간 설정
                // 2. 피격 가능 시간 설정
                break;
                
            case Boss_Mode_List.ANGRY_MODE:
                // 1. 버프스킬 지속시간 증가
                // 2. 피격 가능 시간 축소
                break;

            case Boss_Mode_List.GROGGY_MODE:
                // 아무것도 못하는 상태로 만들기
                break;
        }

        m_Boss_Animator.SetFloat("Attack_Speed", m_Attack_Speed_Slow); // 설정한 공격속도 (슬로우)로 변경
    }

    bool is_Blocked_Between_Target_And_Me() // 타겟과 나 사이에 장애물이 있는가?
    {
        return StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked (
            StageManager.c_Stage_Manager.Find_Own_MCL_Index (
                (m_Target.transform.position.x + transform.position.x) / 2.0f, 
                (m_Target.transform.position.z + transform.position.z) / 2.0f
            ) );
    }
}

