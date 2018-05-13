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

public class Big_Boss_Behavior : MonoBehaviour
{
    NavMeshAgent m_NVAgent; // 내비에이전트
    Animator m_Boss_Animator; // 애니메이터

    GameObject m_Target; // 타겟
    GameObject m_NavPlane; // 내비메쉬 플레인

    Monster_Player_Detector m_Attack_Detector; // 공격 감지기
    Transform m_Attack_Collider; // 공격 충돌체
    Detector_Box m_Target_Detector; // 타겟 감지기
    Transform m_Attack_Range_UI; // 공격 범위 표시기

    public GameObject m_Normal_Monster_Portal; // 일반몹 소환기
    public GameObject m_Glider_Goblin; // 공중 몬스터
    public GameObject m_Flame_Crash_Portal; // 화염폭발 소환기


    // 행동들
    IEnumerator m_Do_Behavior;
    IEnumerator m_Current_Behavior;
    IEnumerator m_Behavior_Find;
    IEnumerator m_Behavior_Chase;
    IEnumerator m_Behavior_Attack;
    IEnumerator m_Behavior_Return;
    IEnumerator m_Behavior_Skill_Normal_Monster_Summon;
    IEnumerator m_Behavior_Skill_Glider_Goblin_Summon;
    IEnumerator m_Behavior_Skill_Flame_Crash;

    // 기본 위치
    Vector3 Base_Position;

    // 스탯
    int m_Boss_Number; // 어떤 보스인지 (애니메이션이 다르므로)
    int m_curr_Mode_Number; // 현재 모드 번호
    int m_curr_Turn_Number; // 현재 스킬 턴 번호

    int m_Health; // 체력
    float m_Move_Speed; // 일반 이동속도
    float m_Attack_Speed_Slow; // 슬로우모션 공격속도
    float m_Attack_Speed; // 일반 공격속도

    float m_Skill_Time; // 스킬 연출 시간
    float m_curr_Skill_Time = 0.0f; // 현재 스킬 시간

    int m_Curr_Skill = 0;

    float m_Turn_Duration;
    float m_curr_Turn_Duration = 0.0f; // 현재 턴의 경과된 지속시간

    int m_Normal_Monster_Count; // 일반 몬스터 소환개수
    int m_Glider_Monster_Count; // 공중 몬스터 소환개수
    float m_Normal_Monster_Speed_Value; // 소환한 일반 몬스터 이동속도 (몬스터에게 넘겨주자)
    float m_Glider_Monster_Speed_Value; // 소환한 공중 몬스터 이동속도 (몬스터에게 넘겨주자)
    int m_Glider_Goblin_Bomb_Value; // 공중 몬스터 폭탄개수 (몬스터에게 넘겨주자)
    int m_Glider_Goblin_Fire_Value; // 공중 몬스터 화염크기 (몬스터에게 넘겨주자)
    int m_Flame_Crash_Range; // 화염폭발스킬 범위
    int m_Flame_Crash_Count; // 화염폭발스킬 화염개수


    Adventure_Boss_Data m_Boss_Data; // 보스 데이터

    // 빅보스 AI 데이터들
    Adventure_Big_Boss_Normal_Mode_AI_Data m_Adv_Big_Boss_Normal_AI = new Adventure_Big_Boss_Normal_Mode_AI_Data();
    Adventure_Big_Boss_Angry_Mode_AI_Data m_Adv_Big_Boss_Angry_AI = new Adventure_Big_Boss_Angry_Mode_AI_Data();
    Adventure_Big_Boss_Groggy_Mode_AI_Data m_Adv_Big_Boss_Groggy_AI = new Adventure_Big_Boss_Groggy_Mode_AI_Data();


    float m_WalkTimer = 0.0f;
    float m_AttackTimer = 0.0f;
    
    bool m_isDead = false; // 죽었는가?
    bool m_is_First_Attack = true; // 첫 공격인가?
    bool m_Attack_is_Done = true; // 공격이 완료됐는가?
    bool m_is_Summonning = false; // 소환중인가?

    float m_Loss_Time = 0.0f; // 타겟을 놓친 후 경과 시간
    float m_Forgot_Time = 4.0f; // 잊어버리게 되는 시간

    float m_curr_Hurt_Time = 1.0f; // 데미지를 받지 않도록
    float m_Hurt_CoolTime = 1.0f; // 데미지 쿨타임

    float m_idle_sound_Timer = 0.0f; // idle상태 사운드 타이머
    float m_idle_sound_Cooltime = 2.0f; // idle상태 사운드 쿨타임


    // 애니 이름
    string m_Animation_idle;
    string m_Animation_Walk;
    string m_Animation_Attack;
    string m_Animation_Dead;
    string m_Animation_Summon;

    string m_Attack_Motion_Checker; // 공격 모션 점검을 위한..
    

    void Start()
    {
        // 내비게이터 등록
        m_NVAgent = gameObject.GetComponent<NavMeshAgent>();
        m_NavPlane = GameObject.Find("Navigation_Plane");

        // 애니메이션 등록
        m_Boss_Animator = GetComponent<Animator>();
        m_Animation_idle = "OrkBoss_isIdle";
        m_Animation_Walk = "OrkBoss_isWalk";
        m_Animation_Attack = "OrkBoss_isAttack";
        m_Animation_Summon = "OrkBoss_isSummon";
        m_Animation_Dead = "OrkBoss_isDead";

        //m_Boss_Number = StageManager.c_Stage_Manager.Get_Boss_ID();
        /*
        switch (m_Boss_Number)
        {
            case 1:
                
                m_Animation_idle = "Goblman_isIdle";
                m_Animation_Walk = "Goblman_isWalk";
                m_Animation_Attack = "Goblman_isAttack";
                m_Animation_Dead = "Goblman_isDead";
                break;

            case 2:
                m_Animation_idle = "OrkBoss_isIdle";
                m_Animation_Walk = "OrkBoss_isWalk";
                m_Animation_Attack = "OrkBoss_isAttack";
                m_Animation_Dead = "OrkBoss_isDead";
                break;
        }*/

        m_Attack_Motion_Checker = "Base Layer.OrkBoss_Attack";

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
        m_Health = m_Boss_Data.Boss_HP; // 체력 설정
        GameObject.Find("Boss_Health").GetComponentInChildren<Text>().enabled = true; // 체력 표시기 활성화
        GameObject.Find("Boss_Health").GetComponentInChildren<Text>().text = "Boss Health: " + m_Health.ToString();

        Big_Boss_Data_Allocation(); // 보스 AI 데이터의 리스트들을 할당
        CSV_Manager.GetInstance().Get_Adventure_Big_Boss_AI_Data(ref m_Adv_Big_Boss_Normal_AI, ref m_Adv_Big_Boss_Angry_AI, ref m_Adv_Big_Boss_Groggy_AI); // 보스 AI 테이블을 받아온다.
        // =========================




        // 몬스터의 행동 코루틴들을 설정
        m_Do_Behavior = Do_Behavior();
        m_Behavior_Find = FindPlayer();
        m_Behavior_Chase = Chase();
        m_Behavior_Attack = Attack();
        m_Behavior_Return = Return();
        m_Behavior_Skill_Normal_Monster_Summon = Skill_Normal_Monster_Summon();
        m_Behavior_Skill_Glider_Goblin_Summon = Skill_Glider_Goblin_Summon();
        m_Behavior_Skill_Flame_Crash = Skill_Flame_Crash();

        // 처음 실행할 행동 설정
        m_Current_Behavior = m_Behavior_Return;
        StartCoroutine(m_Do_Behavior);


        ModeChange(Boss_Mode_List.NORMAL_MODE); // 최초 시작시 노말모드로 시작
    }



    void OnTriggerEnter(Collider other)
    {
        // 불에 닿을 시 데미지 판정
		if (!m_isDead && (other.gameObject.CompareTag("Flame") || other.gameObject.CompareTag("Flame_Bush")))
        {
            if (m_curr_Hurt_Time > m_Hurt_CoolTime)
                Hurt();
        }
        //===============================
    }





    
    void Think() // 어떤 행동을 할지 생각하는 함수 (추후 가중치를 두어 어떤 행동을 할지 더 상세하게 구분해야함.)
    {
		if (m_curr_Mode_Number != Boss_Mode_List.GROGGY_MODE && m_Current_Behavior == m_Behavior_Chase/*(m_Curr_Skill != 0 && (m_curr_Skill_Time >= m_Skill_Time))*/) // 공격중이라면 공격이 끝나야 다른 행동을 할 수 있다. 또는 스킬 사용중이라면..
        {
            if (m_Target_Detector.m_isInRange) // 감지 범위 안이라면
            {
                m_Current_Behavior = m_Behavior_Chase; // 추격 상태로 전환

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





    
    IEnumerator Do_Behavior() // 모든 행동의 베이스가 되는 코루틴
    {
        while (true)
        {
            if (!StageManager.c_Stage_Manager.Get_is_Pause() && StageManager.c_Stage_Manager.Get_is_Intro_Over())
            {
                Think();

                m_curr_Turn_Duration += Time.deltaTime; // 턴 지속시간을 잰다.
				m_curr_Hurt_Time += Time.deltaTime;

                if (m_curr_Turn_Duration > m_Turn_Duration && m_curr_Skill_Time >= m_Skill_Time)
                    Set_Next_Turn_Skill();

                if (m_Current_Behavior != null && m_Current_Behavior.MoveNext())
                    yield return m_Current_Behavior.Current;

                else
                    yield return null;
            }

            else
            {
                if (StageManager.c_Stage_Manager.Get_is_Intro_Over() && m_NavPlane.activeSelf)
                    m_NVAgent.isStopped = true;
                yield return null;
            }
        }
    }

    



    
    IEnumerator FindPlayer() // 플레이어 찾기 (일단 뺌)
    {
        while (true)
        {
            if (m_WalkTimer < Monster_AI_Constants.Walk_Time) // 일정 시간동안 걸어다님.
            {
                transform.Translate(new Vector3(0.0f, 0.0f, (m_Move_Speed * Time.deltaTime)));
                m_Boss_Animator.SetBool(m_Animation_Walk, true);
                m_Boss_Animator.SetBool(m_Animation_idle, false);
                m_WalkTimer += Time.deltaTime;
            }

            else // idle 상태 진입
            {
                if (MusicManager.manage_ESound != null)
                    MusicManager.manage_ESound.Goblin_Idle_Sound();

                m_Boss_Animator.SetBool(m_Animation_Walk, false);
                m_Boss_Animator.SetBool(m_Animation_idle, true);

                m_WalkTimer = 0.0f;
                yield return new WaitForSeconds(3.0f); // 3초간 idle 상태 유지

                float AngleY = Random.Range(0.0f, 360.0f);
                transform.localEulerAngles = new Vector3(0.0f, AngleY, 0.0f);
            }

            yield return null;
        }
        
    }





    
    IEnumerator Chase() // 추격
    {
        while(true)
        {
            if (m_Loss_Time < m_Forgot_Time) // 잊어버리기 까지 플레이어의 위치로 이동한다.
            {
                m_Loss_Time += Time.deltaTime;
                m_NVAgent.isStopped = false;
                m_Target = m_Target_Detector.GetComponent<Detector_Box>().Get_Target();
                if (m_Target != null)
                    m_NVAgent.destination = m_Target.transform.position;
                m_Loss_Time = 0.0f;
                m_Boss_Animator.SetBool(m_Animation_Walk, true);
                m_Boss_Animator.SetBool(m_Animation_idle, false);
            }

            else // 잊어버렸다면 기본 위치로 돌아간다.
            {
                m_Current_Behavior = m_Behavior_Return;

                if (transform.position != Base_Position) // 기본 위치가 아니면
                {
                    m_NVAgent.isStopped = false;
                    m_NVAgent.destination = Base_Position;
                    m_Boss_Animator.SetBool(m_Animation_Walk, true);
                    m_Boss_Animator.SetBool(m_Animation_idle, false);
                }
            }

            yield return null;
        }
    }
    




    
    IEnumerator Attack() // 공격
    {
        while (true)
        {
            if (m_AttackTimer < Monster_AI_Constants.Boss_Attack_Time)
                m_AttackTimer += Time.deltaTime; // 공격타이머 증가

            else
            {
                if (StageManager.c_Stage_Manager.Get_is_Intro_Over() && !StageManager.c_Stage_Manager.Get_Game_Over())
                {
                    if (m_Attack_is_Done)
                    {
                        if (MusicManager.manage_ESound != null && !m_Boss_Animator.GetBool(m_Animation_Attack)) // 공격할때 1번만 소리냄.
                            MusicManager.manage_ESound.Goblin_Attack_Sound();

                        m_Boss_Animator.SetBool(m_Animation_Attack, true); // 마찬가지로 1번만 수행
                        m_Boss_Animator.SetBool(m_Animation_Walk, false);
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
                        Debug.Log("공격중");
                        if (m_Boss_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash(m_Attack_Motion_Checker))
                        {
                            if (m_Boss_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) // 애니메이션이 끝나면
                            {
                                m_Boss_Animator.SetBool(m_Animation_Attack, false);
                                m_Boss_Animator.SetBool(m_Animation_Walk, true);
                                m_AttackTimer = 0.0f; // 시간도 초기화한다.
                                m_Current_Behavior = m_Behavior_Return;
                                m_Boss_Animator.SetFloat("Attack_Speed", m_Attack_Speed_Slow); // 공격속도를 슬로우모션으로 되돌린다.

                                m_Attack_is_Done = true; // 공격 완료 알림
                            }

                            else if (m_Boss_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f) // 중간 부분
                            {
                                m_Attack_Collider.gameObject.SetActive(false); // 공격용 충돌체를 집어넣는다.
                                m_Attack_Range_UI.gameObject.SetActive(false); // 범위 표시기도 집어넣는다.
                            }

                            else if (m_Boss_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.15f) // 초기 부분
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









    IEnumerator Return() // 복귀
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
                m_Boss_Animator.SetBool(m_Animation_Walk, false);
                m_Boss_Animator.SetBool(m_Animation_idle, true);
                m_is_First_Attack = true; // 기본 위치로 가면 첫 공격 여부 초기화
            }
            yield return null;
        }
    }


    void Set_Next_Turn_Skill() // 다음 턴의 스킬을 설정하는 함수
    {
        ++m_curr_Turn_Number;

        m_curr_Turn_Duration = 0.0f; // 턴 경과 시간 초기화
        m_curr_Skill_Time = 0.0f; // 스킬 경과 시간 초기화

        int random = Random.Range(70, 100);
        Debug.Log(m_curr_Turn_Number);

        switch (m_curr_Mode_Number)
        {
            case Boss_Mode_List.NORMAL_MODE:

                if (m_curr_Turn_Number > m_Adv_Big_Boss_Normal_AI.Skill_Percentage.Count)
                    m_curr_Turn_Number = 1;

                m_Turn_Duration = m_Adv_Big_Boss_Normal_AI.Skill_Duration[m_curr_Turn_Number - 1];
                
                if (random <= m_Adv_Big_Boss_Normal_AI.Skill_Percentage[m_curr_Turn_Number - 1][0])
                {
                    // m_Behavior_Chase 초기화 작업
                    Debug.Log("Chase");
                    m_Skill_Time = m_Adv_Big_Boss_Normal_AI.Skill_Time[0];
                    m_Current_Behavior = m_Behavior_Chase;
                }

                else if (random <= m_Adv_Big_Boss_Normal_AI.Skill_Percentage[m_curr_Turn_Number - 1][0] + m_Adv_Big_Boss_Normal_AI.Skill_Percentage[m_curr_Turn_Number - 1][1])
                {
                    // m_Behavior_Skill_Normal_Monster_Summon 초기화 작업
                    Debug.Log("Normal_Spawn");
                    m_Skill_Time = m_Adv_Big_Boss_Normal_AI.Skill_Time[1];
                    m_Normal_Monster_Count = Random.Range(m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Value_Min[0], m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Value_Max[0]);
                    m_Normal_Monster_Speed_Value = m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Speed_Value[0];

                    m_Current_Behavior = m_Behavior_Skill_Normal_Monster_Summon;
                }

                else if (random <= m_Adv_Big_Boss_Normal_AI.Skill_Percentage[m_curr_Turn_Number - 1][0] + m_Adv_Big_Boss_Normal_AI.Skill_Percentage[m_curr_Turn_Number - 1][1] + m_Adv_Big_Boss_Normal_AI.Skill_Percentage[m_curr_Turn_Number - 1][2])
                {
                    // m_Behavior_Skill_Glider_Goblin_Summon 초기화 작업
                    Debug.Log("Glider_Spawn");
                    m_Skill_Time = m_Adv_Big_Boss_Normal_AI.Skill_Time[2];
                    m_Glider_Monster_Count = Random.Range(m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Value_Min[1], m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Value_Max[1]);
                    m_Glider_Monster_Speed_Value = m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Speed_Value[1];
                    m_Glider_Goblin_Bomb_Value = m_Adv_Big_Boss_Normal_AI.Glider_Goblin_Bomb_Value;
                    m_Glider_Goblin_Fire_Value = m_Adv_Big_Boss_Normal_AI.Glider_Goblin_Fire_Value;
                    m_Current_Behavior = m_Behavior_Skill_Glider_Goblin_Summon;
                }
                
                else
                {
                    // m_Behavior_Skill_Flame_Crash 초기화 작업
                    Debug.Log("Flame_Crash");
                    m_Skill_Time = m_Adv_Big_Boss_Normal_AI.Skill_Time[3];
                    m_Flame_Crash_Range = Random.Range(m_Adv_Big_Boss_Normal_AI.Skill_Fire_Range_Min, m_Adv_Big_Boss_Normal_AI.Skill_Fire_Range_Max);
                    m_Flame_Crash_Count = Random.Range(m_Adv_Big_Boss_Normal_AI.Fire_In_Range_Min, m_Adv_Big_Boss_Normal_AI.Fire_In_Range_Max);
                    m_Current_Behavior = m_Behavior_Skill_Flame_Crash;
                }

                break;





            case Boss_Mode_List.ANGRY_MODE:

                if (m_curr_Turn_Number > m_Adv_Big_Boss_Angry_AI.Skill_Percentage.Count)
                    m_curr_Turn_Number = 1;

                m_Turn_Duration = m_Adv_Big_Boss_Angry_AI.Skill_Duration[m_curr_Turn_Number - 1];

                if (random <= m_Adv_Big_Boss_Angry_AI.Skill_Percentage[m_curr_Turn_Number - 1][0])
                {
                    // m_Behavior_Chase 초기화 작업
                    m_Skill_Time = m_Adv_Big_Boss_Angry_AI.Skill_Time[0];
                    m_Current_Behavior = m_Behavior_Chase;
                }

                else if (random <= m_Adv_Big_Boss_Angry_AI.Skill_Percentage[m_curr_Turn_Number - 1][0] + m_Adv_Big_Boss_Angry_AI.Skill_Percentage[m_curr_Turn_Number - 1][1])
                {
                    // m_Behavior_Skill_Normal_Monster_Summon 초기화 작업
                    m_Skill_Time = m_Adv_Big_Boss_Angry_AI.Skill_Time[1];
                    m_Normal_Monster_Count = Random.Range(m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Value_Min[0], m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Value_Max[0]);
                    m_Normal_Monster_Speed_Value = m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Speed_Value[0];

                    m_Current_Behavior = m_Behavior_Skill_Normal_Monster_Summon;
                }

                else if (random <= m_Adv_Big_Boss_Angry_AI.Skill_Percentage[m_curr_Turn_Number - 1][0] + m_Adv_Big_Boss_Angry_AI.Skill_Percentage[m_curr_Turn_Number - 1][1] + m_Adv_Big_Boss_Angry_AI.Skill_Percentage[m_curr_Turn_Number - 1][2])
                {
                    // m_Behavior_Skill_Glider_Goblin_Summon 초기화 작업
                    m_Skill_Time = m_Adv_Big_Boss_Angry_AI.Skill_Time[2];
                    m_Glider_Monster_Count = Random.Range(m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Value_Min[1], m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Value_Max[1]);
                    m_Glider_Monster_Speed_Value = m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Speed_Value[1];
                    m_Glider_Goblin_Bomb_Value = m_Adv_Big_Boss_Angry_AI.Glider_Goblin_Bomb_Value;
                    m_Glider_Goblin_Fire_Value = m_Adv_Big_Boss_Angry_AI.Glider_Goblin_Fire_Value;

                    m_Current_Behavior = m_Behavior_Skill_Glider_Goblin_Summon;
                }
                
                else
                {
                    // m_Behavior_Skill_Flame_Crash 초기화 작업
                    m_Skill_Time = m_Adv_Big_Boss_Angry_AI.Skill_Time[3];
                    m_Flame_Crash_Range = Random.Range(m_Adv_Big_Boss_Angry_AI.Skill_Fire_Range_Min, m_Adv_Big_Boss_Angry_AI.Skill_Fire_Range_Max);
                    m_Flame_Crash_Count = Random.Range(m_Adv_Big_Boss_Angry_AI.Fire_In_Range_Min, m_Adv_Big_Boss_Angry_AI.Fire_In_Range_Max);
                    m_Current_Behavior = m_Behavior_Skill_Flame_Crash;
                }

                break;
        }
    }


    IEnumerator Skill_Normal_Monster_Summon() // 일반몹 소환 스킬
    {
        while (true)
        {
            if (m_curr_Skill_Time < m_Skill_Time)
            {
                m_curr_Skill_Time += Time.deltaTime;

                m_NVAgent.isStopped = true;

                // 여기서 소환 애니메이션 수행
                m_Boss_Animator.SetBool(m_Animation_Summon, true);


                if (!m_is_Summonning) // 소환중이 아니라면
                {
                    m_is_Summonning = true; // 소환중이라 알림

                    int index; // 소환 위치 (인덱스)
                    Vector3 pos;
                    pos.y = m_Normal_Monster_Portal.transform.position.y;

                    for (int i = 0; i < m_Normal_Monster_Count; ++i)
                    {
                        while (true) // 루프를 돌면서 지형 탐색
                        {
                            index = Random.Range(17, 271); // 맵 범위
                            if (!StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(index)) // 막혀있지 않으면
                                break; // 탈출
                        }
                        
                        pos.x = 0.0f;
                        pos.z = 0.0f;
                        StageManager.c_Stage_Manager.Get_MCL_Coordinate(index, ref pos.x, ref pos.z);

                        GameObject temp = Instantiate(m_Normal_Monster_Portal);
                        temp.transform.position = pos; // 설정한 위치에 포탈 소환
                        temp.GetComponent<Monster_Portal>().Set_Monster_Speed(m_Normal_Monster_Speed_Value);
                    }
                }
            }
            
            else
            {
                m_is_Summonning = false;
                m_Boss_Animator.SetBool(m_Animation_Summon, false);
                Debug.Log("소환 끝");
                m_Current_Behavior = m_Behavior_Chase; // 남은 시간동안 추격-공격 패턴
            }

            yield return null;
        }
    }




    IEnumerator Skill_Glider_Goblin_Summon() // 글라이더 고블린 소환 스킬
    {
        while (true)
        {
            if (m_curr_Skill_Time < m_Skill_Time)
            {
                m_curr_Skill_Time += Time.deltaTime;
                m_NVAgent.isStopped = true;

                // 여기서 소환 애니메이션 수행
                m_Boss_Animator.SetBool(m_Animation_Summon, true);


                if (!m_is_Summonning) // 소환중이 아니라면
                {
                    m_is_Summonning = true; // 소환중이라 알림

                    int index;
                    Vector3 pos;
                    pos.y = m_Glider_Goblin.transform.position.y;

                    for (int i = 0; i < m_Glider_Monster_Count; ++i)
                    {
                        index = Random.Range(17, 271); // 소환 위치 (인덱스)
                        
                        pos.x = 0.0f;
                        pos.z = 0.0f;
                        StageManager.c_Stage_Manager.Get_MCL_Coordinate(index, ref pos.x, ref pos.z);

                        GameObject temp = Instantiate(m_Glider_Goblin);
                        temp.transform.position = pos; // 설정한 위치에 글라이더 소환
                        temp.GetComponent<Boss_AI_JetGoblin>().Set_Bomb_info(m_Glider_Goblin_Bomb_Value, m_Glider_Goblin_Fire_Value);
                        temp.GetComponent<Boss_AI_JetGoblin>().Set_Glider_Speed(m_Glider_Monster_Speed_Value);
                    }
                }
            }

            else
            {
                m_is_Summonning = false;
                m_Boss_Animator.SetBool(m_Animation_Summon, false);
                Debug.Log("소환 끝");
                m_Current_Behavior = m_Behavior_Chase; // 남은 시간동안 추격-공격 패턴
            }

            yield return null;
        }
    }





    IEnumerator Skill_Flame_Crash() // 화염폭발 스킬
    {
        while (true)
        {
            if (m_curr_Skill_Time < m_Skill_Time)
            {
                m_curr_Skill_Time += Time.deltaTime;
                m_NVAgent.isStopped = true;
               

                // 여기서 소환 애니메이션 수행
                m_Boss_Animator.SetBool(m_Animation_Summon, true);
                
                if (!m_is_Summonning) // 소환중이 아니라면
                {
                    m_is_Summonning = true; // 소환중이라 알림

                    float range_X;
                    float range_Z;
                    int index;
                    Vector3 pos;
                    pos.y = m_Flame_Crash_Portal.transform.position.y;

                    for (int i = 0; i < m_Flame_Crash_Count; ++i)
                    {
                        while (true)
                        {
                            range_X = Random.Range(transform.position.x - m_Flame_Crash_Range, transform.position.x + m_Flame_Crash_Range); // 소환 위치 (X값)
                            range_Z = Random.Range(transform.position.z - m_Flame_Crash_Range, transform.position.z + m_Flame_Crash_Range); // 소환 위치 (Z값)
                            if (range_X < -1.0f || range_X > 29.0f || range_Z < 49.0f || range_Z > 79.0f) // 맵을 벗어나면 다시
                                continue;
                            index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(range_X, range_Z);
                            if (!StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(index)) // 막혀있지 않으면 탈출
                                break;
                        }
                        
                        pos.x = 0.0f;
                        pos.z = 0.0f;
                        StageManager.c_Stage_Manager.Get_MCL_Coordinate(index, ref pos.x, ref pos.z);

                        Instantiate(m_Flame_Crash_Portal).transform.position = pos; // 설정한 위치에 화염 마법진 소환
                        
                    }
                }
            }

            else
            {
                m_is_Summonning = false;
                m_Boss_Animator.SetBool(m_Animation_Summon, false);
                Debug.Log("소환 끝");
                m_Current_Behavior = m_Behavior_Chase; // 남은 시간동안 추격-공격 패턴
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

            m_Boss_Animator.SetBool(m_Animation_Dead, true);
            Invoke("Dead", 2.0f);
        }

		Debug.Log ("피격!");
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
        m_curr_Turn_Number = 0; // 모드전환 시 초기화

        switch (m_curr_Mode_Number)
        {
            case Boss_Mode_List.NORMAL_MODE:
                m_Move_Speed = (float)m_Adv_Big_Boss_Normal_AI.Boss_Speed_Value; // 이동속도 설정
                m_NVAgent.speed = m_Move_Speed; // 추격시 이동속도 설정
                m_NVAgent.angularSpeed = 360.0f * m_Move_Speed; // 추격시 회전속도 설정
                m_Attack_Speed_Slow = 0.2f; // 슬로우 모션 공격속도 설정
                m_Attack_Speed = 1.0f; // 진짜 공격속도 설정

                // 1. 버프 스킬 지속시간 설정
                // 2. 피격 가능 시간 설정
                break;
                
            case Boss_Mode_List.ANGRY_MODE:
                m_Move_Speed = (float)m_Adv_Big_Boss_Angry_AI.Boss_Speed_Value; // 이동속도 설정
                m_NVAgent.speed = m_Move_Speed; // 추격시 이동속도 설정
                m_NVAgent.angularSpeed = 360.0f * m_Move_Speed; // 추격시 회전속도 설정
                m_Attack_Speed_Slow = 0.4f; // 슬로우 모션 공격속도 설정
                m_Attack_Speed = 2.0f; // 진짜 공격속도 설정

                // 1. 버프스킬 지속시간 증가
                // 2. 피격 가능 시간 축소
                break;

            case Boss_Mode_List.GROGGY_MODE:
                // 아무것도 못하는 상태로 만들기
                m_Move_Speed = (float)m_Adv_Big_Boss_Groggy_AI.Boss_Speed_Value; // 이동속도 설정
                m_NVAgent.speed = m_Move_Speed; // 추격시 이동속도 설정
                m_NVAgent.angularSpeed = 360.0f * m_Move_Speed; // 추격시 회전속도 설정
                break;
        }

        m_Boss_Animator.SetFloat("Attack_Speed", m_Attack_Speed_Slow); // 설정한 공격속도 (슬로우)로 변경

        Set_Next_Turn_Skill(); // 다음 턴 스킬 설정
    }

    bool is_Blocked_Between_Target_And_Me() // 타겟과 나 사이에 장애물이 있는가?
    {
        return StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked (
            StageManager.c_Stage_Manager.Find_Own_MCL_Index (
                (m_Target.transform.position.x + transform.position.x) / 2.0f, 
                (m_Target.transform.position.z + transform.position.z) / 2.0f
            ) );
    }


    void Big_Boss_Data_Allocation() // 빅보스 데이터 메모리 할당작업
    {
        // Normal
        m_Adv_Big_Boss_Normal_AI.Skill_Time = new int[4];
        m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Value_Min = new int[2];
        m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Value_Max = new int[2];
        m_Adv_Big_Boss_Normal_AI.Spawn_Monster_Speed_Value = new int[2];
        m_Adv_Big_Boss_Normal_AI.Skill_Percentage = new List<int[]>();
        m_Adv_Big_Boss_Normal_AI.Skill_Duration = new List<int>();
        m_Adv_Big_Boss_Normal_AI.Link_Skill = new List<int>();

        // ===============================================================




        // Angry
        m_Adv_Big_Boss_Angry_AI.Skill_Time = new int[4];
        m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Value_Min = new int[2];
        m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Value_Max = new int[2];
        m_Adv_Big_Boss_Angry_AI.Spawn_Monster_Speed_Value = new int[2];
        m_Adv_Big_Boss_Angry_AI.Skill_Percentage = new List<int[]>();
        m_Adv_Big_Boss_Angry_AI.Skill_Duration = new List<int>();
        m_Adv_Big_Boss_Angry_AI.Link_Skill = new List<int>();
    }
}

