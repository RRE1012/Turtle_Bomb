using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{

    Animator m_Goblman_Animator;

    Vector3 m_V3_To_Find_my_rightObject;
    Vector3 m_V3_To_Find_my_leftObject;

    //NavMeshAgent m_NMA;

    int m_My_MCL_Index = 0;
    int m_Right_Object_Index = 0;
    int m_Left_Object_Index = 0;

    bool m_is_Done_Camera_Walking;
    bool m_isFound_Turtleman;

    float m_Monster_Basic_Speed;
    float m_WalkCounter;

    bool m_isStuck;
    bool m_isDead;


    void Start()
    {
        StageManager.m_Left_Monster += 1;
        //m_NMA = GetComponent<NavMeshAgent>();

        // 객체 자신이 가지고 있는 애니메이터를 찾아온다.
        m_Goblman_Animator = GetComponent<Animator>();

        m_WalkCounter = 0.0f;

        m_Monster_Basic_Speed = 2.0f;
        m_isStuck = false;
        m_isDead = false;

        m_is_Done_Camera_Walking = false;
        m_isFound_Turtleman = false;

        Invoke("ReadyToCameraWalking", 6.0f);
    }

    void Update()
    {
        if (m_is_Done_Camera_Walking && !m_isDead)
        {
            MonsterMove();
            Find_My_Coord();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 아래 구문을 통해 몬스터가 막힌 길로 들어서지 않도록 한다.
        if (!m_isStuck && collision.gameObject.CompareTag("Box") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Rock") || collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Bomb"))
        {
            m_WalkCounter = 7.0f;

            // 위치 변환
            Vector3 Loc;
            Loc.x = StageManager.m_Map_Coordinate_List[m_My_MCL_Index].x;
            Loc.y = transform.position.y;
            Loc.z = StageManager.m_Map_Coordinate_List[m_My_MCL_Index].z;
            transform.position = Loc;


            // 방향도 전환시킨다.
            Find_New_Way();

            m_isStuck = true;
        }
        // ==============================
    }


    void OnTriggerEnter(Collider other)
    {
        // 몬스터가 불에 닿으면 사망 판정
        if (!m_isDead && (other.gameObject.tag == "Flame" || other.gameObject.CompareTag("Flame_Bush")) && !StageManager.m_is_Stage_Clear)
        {
            StageManager.m_Left_Monster -= 1;
            m_Goblman_Animator.SetBool("Goblman_isDead", true);
            m_isDead = true;
            Invoke("MonsterDead", 1.5f);
        }
        //===============================



        /*
        if (other.gameObject.tag == "Player")
        {
            //m_NMA.SetDestination(PlayerMove.C_PM.transform.position);
            //m_NMA.isStopped = false;
            m_isFound_Turtleman = true;
            //Debug.Log(m_isFound_Turtleman);
        }
        */
    }

    /*
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //m_NMA.isStopped = true;
            m_isFound_Turtleman = false;
            //Debug.Log(m_isFound_Turtleman);
        }
    }
    */



    void MonsterMove()
    {
        //if (!m_isFound_Turtleman)
        // {

        // 전방으로 최대 6.0f 만큼 이동.
        if (m_WalkCounter < 6.0f)
        {
            m_WalkCounter += m_Monster_Basic_Speed * Time.deltaTime;
            transform.Translate(new Vector3(0.0f, 0.0f, (m_Monster_Basic_Speed * Time.deltaTime)));
            m_Goblman_Animator.SetBool("Goblman_isWalk", true);
        }

        // 최대 거리 이동 후 2초간 이동불가 상태.
        else
        {
            Invoke("Reset_WalkCounter", 2.0f);
            m_Goblman_Animator.SetBool("Goblman_isWalk", false);
        }

        //}

        //else
        //{

        //}
    }

    /*
    void MonsterAttack()
    {

    }
    */


    void MonsterDead()
    {
        Destroy(gameObject);
    }




    // 새로운 길을 찾도록 하는 함수
    void Find_New_Way()
    {
        m_V3_To_Find_my_leftObject = transform.position - transform.right * 2.0f;
        m_V3_To_Find_my_rightObject = transform.position + transform.right * 2.0f;
        

        m_Right_Object_Index = Find_Objects_Coord(m_V3_To_Find_my_rightObject.x, m_V3_To_Find_my_rightObject.z);
        m_Left_Object_Index = Find_Objects_Coord(m_V3_To_Find_my_leftObject.x, m_V3_To_Find_my_leftObject.z);

        
        if (m_Left_Object_Index != -1 && StageManager.m_Map_Coordinate_List[m_Left_Object_Index].isBlocked == false)
        {
            transform.Rotate(-transform.up * 90.0f);
        }

        else if (m_Right_Object_Index != -1 && StageManager.m_Map_Coordinate_List[m_Right_Object_Index].isBlocked == false)
        {
            transform.Rotate(transform.up * 90.0f);
        }

        else transform.Rotate(transform.up * 180.0f);
    }
    // ==============================


    

    // 이동 거리 카운터를 초기화시킨다.
    void Reset_WalkCounter()
    {
        m_WalkCounter = 0.0f;
        m_isStuck = false;
    }
    // ==============================



    // 시작 시 카메라워킹이 완료될 때 까지 대기한다.
    void ReadyToCameraWalking()
    {
        m_is_Done_Camera_Walking = true;
    }
    // ==============================



    // 몬스터 자신의 MCL 인덱스를 받아오는 함수
    void Find_My_Coord()
    {
        if (StageManager.m_is_init_MCL)
        {
            m_My_MCL_Index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z, false);
        }
    }
    // =========================================



    // 오브젝트의 좌표를 통하여 해당 MCL 인덱스를 반환한다.
    int Find_Objects_Coord(float x, float z)
    {
        if (StageManager.m_is_init_MCL)
        {
            return StageManager.Find_Own_MCL_Index(x, z, false);
        }
        return -1;
    }
    // =========================================
}
