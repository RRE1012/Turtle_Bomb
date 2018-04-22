using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Boss_Behavior : MonoBehaviour
{
    NavMeshAgent m_NVAgent;
    Adventure_Boss_Data m_Boss_Data;
    Animator m_Boss_Animator;

    Monster_Player_Detector m_Attack_Detector;
    Transform m_Attack_Collider;
    Detector_Box m_Player_Detector;

    IEnumerator m_Current_Behavior;
    IEnumerator m_Behavior_Find;
    IEnumerator m_Behavior_Chase;
    IEnumerator m_Behavior_Attack;

    float m_Monster_Basic_Speed = 2.0f;
    float m_WalkTimer = 0.0f;
    float m_AttackTimer = 0.0f;

    public bool m_isFound_Turtleman = false;
    bool m_isDead = false;
    bool m_isAttacking = false;
    bool m_is_First_Attack = true;

    float m_Loss_Time = 0.0f;
    float m_Forgot_Time = 3.0f;

    float m_curr_Hurt_Time = 1.0f;
    float m_Hurt_CoolTime = 1.0f;

    // 플레이어 위치
    Transform m_playerTransform;

    // 스탯
    int m_Health;



    void Start()
    {
        // 내비게이터 등록
        m_NVAgent = gameObject.GetComponent<NavMeshAgent>();

        // 애니메이터 등록
        m_Boss_Animator = GetComponent<Animator>();
        
        // 플레이어 감지기 등록
        m_Attack_Detector = GetComponentInChildren<Monster_Player_Detector>();
        m_Player_Detector = GetComponentInChildren<Detector_Box>();

        // 공격 충돌박스 등록
        m_Attack_Collider = transform.Find("Attack_Range");
        // 비활성화
        m_Attack_Collider.gameObject.SetActive(false);


        m_Boss_Data = StageManager.c_Stage_Manager.Get_Adventure_Boss_Data();

        m_Health = m_Boss_Data.Boss_HP;

        m_NVAgent.speed = 2.5f;
        m_NVAgent.angularSpeed = 360.0f;


        
        m_playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        

        // 몬스터의 행동 코루틴들을 설정
        m_Behavior_Find = FindPlayer();
        m_Behavior_Chase = Chase();
        m_Behavior_Attack = Attack();

        // 처음 실행할 행동 설정
        m_Current_Behavior = m_Behavior_Find;
        StartCoroutine(Do_Behavior());
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
        // 감지 범위 안이라면
        if (m_Player_Detector.m_isInRange)
        {
            // 추격
            m_Current_Behavior = m_Behavior_Chase;
            m_NVAgent.isStopped = false;
            m_Loss_Time = 0.0f;

            // 그리고 공격 범위 안이라면
            if (m_Attack_Detector.m_isInRange)
            {
                // 공격
                if (m_is_First_Attack)
                {
                    m_AttackTimer = Monster_AI_Constants.Attack_Time;
                    m_is_First_Attack = false;
                }
                m_Current_Behavior = m_Behavior_Attack;
                m_NVAgent.isStopped = true;
            }
            else m_is_First_Attack = true;
        }

        // 감지 범위 밖이라면
        else
        {
            // 잊어버릴때 까지 따라간다.
            if (m_Current_Behavior != m_Behavior_Find && m_Loss_Time < m_Forgot_Time)
            {
                m_Current_Behavior = m_Behavior_Chase;
                m_NVAgent.isStopped = false;
            }

            // 잊어버렸다면 탐색 수행
            else
            {
                m_Current_Behavior = m_Behavior_Find;
                m_NVAgent.destination = transform.position;
                m_NVAgent.isStopped = true;
            }
        }
    }





    // 모든 행동의 베이스가 되는 코루틴
    IEnumerator Do_Behavior()
    {
        while (true)
        {
            Think();

            Debug.Log(m_Current_Behavior.ToString()); // 현재 상태 출력

            if (!StageManager.c_Stage_Manager.m_is_Pause && m_Current_Behavior != null && m_Current_Behavior.MoveNext())
            {
                yield return m_Current_Behavior.Current;
            }

            else
            {
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
                if (StageManager.c_Stage_Manager.m_is_Intro_Over && PlayerMove.C_PM.Get_IsAlive() && !StageManager.m_is_Stage_Clear)
                {
                    transform.Translate(new Vector3(0.0f, 0.0f, (m_Monster_Basic_Speed * Time.deltaTime)));
                    m_Boss_Animator.SetBool("Goblman_isWalk", true);
                    m_WalkTimer += Time.deltaTime;
                }
            }

            else // idle 상태 진입
            {
                if (MusicManager.manage_ESound != null)
                    MusicManager.manage_ESound.Goblin_Idle_Sound();

                m_Boss_Animator.SetBool("Goblman_isWalk", false);
                
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
                if (StageManager.c_Stage_Manager.m_is_Intro_Over && PlayerMove.C_PM.Get_IsAlive() && !StageManager.m_is_Stage_Clear)
                {
                    m_Boss_Animator.SetBool("Goblman_isWalk", true);
                    m_NVAgent.destination = m_playerTransform.position;
                    m_Loss_Time += Time.deltaTime;
                }
            }

            else // 잊어버리면
            {
                if (MusicManager.manage_ESound != null)
                    MusicManager.manage_ESound.Goblin_Idle_Sound();

                m_Boss_Animator.SetBool("Goblman_isWalk", false);
                m_NVAgent.destination = transform.position;
                m_NVAgent.isStopped = true; // 멈춘다.

                yield return new WaitForSeconds(1.0f); // 1초간 대기 후

                m_Current_Behavior = m_Behavior_Find; // 탐색 시작
            }

            yield return null;
        }
    }





    // 공격
    IEnumerator Attack()
    {
        while (true)
        {
            if (m_AttackTimer < Monster_AI_Constants.Attack_Time) // 공격 시간이 될때까지 시간 증가
            {
                m_AttackTimer += Time.deltaTime;
            }

            else // 시간이 되면
            {
                if (StageManager.c_Stage_Manager.m_is_Intro_Over && PlayerMove.C_PM.Get_IsAlive() && !StageManager.m_is_Stage_Clear && !m_Boss_Animator.GetBool("Goblman_isAttack"))
                {
                    if (MusicManager.manage_ESound != null)
                        MusicManager.manage_ESound.Goblin_Attack_Sound();
                    m_Attack_Collider.gameObject.SetActive(true); // 공격용 충돌체를 꺼낸다.
                    m_Boss_Animator.SetBool("Goblman_isAttack", true);

                    yield return new WaitForSeconds(0.5f); // 대기했다가

                    m_Boss_Animator.SetBool("Goblman_isAttack", false);
                    m_Attack_Collider.gameObject.SetActive(false); // 충돌체를 집어넣는다.
                    m_AttackTimer = 0.0f; // 시간도 초기화한다.
                }
            }

            yield return null;
        }
    }

    




    // 폭탄 피격
    void Hurt()
    {
        if (MusicManager.manage_ESound != null)
            MusicManager.manage_ESound.Boss_Goblin_Hurt_Sound();

        m_Health -= m_Boss_Data.Bomb_Damage;

        m_curr_Hurt_Time = 0.0f;

        if (m_Health < 0.0f)
        {
            if (MusicManager.manage_ESound != null)
                MusicManager.manage_ESound.Boss_Goblin_Dead_Sound();
            StopCoroutine(Do_Behavior());
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






}

