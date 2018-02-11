using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//폭탄 함수
public class Bomb : MonoBehaviour
{
    private Collider m_BCollider;
    public GameObject m_Flame;
    public GameObject m_Flame_Remains;
    public GameObject m_Boom_Effect;

    public static Bomb c_Bomb;

    int m_FlameCount = UI.m_fire_count;

    int m_My_MCL_Index = -1;

    // 불길이 퍼질때 막힌 공간 판별을 위한 boolean
    // 동서남북 (= E, W, S, N)
    bool m_Blocked_N = false;
    bool m_Blocked_S = false;
    bool m_Blocked_W = false;
    bool m_Blocked_E = false;

    // 폭탄 생명주기
    private float m_BombCountDown = 3.0f;

    // 폭탄 애니메이션
    float m_Reverse_Scale = 1.0f;
    float m_Scale_Timer = 0.0f;
    float m_Weight_Time_XY = 0.0f;
    float m_Weight_Time_Z = 0.0f;
    float m_Weight_Time_Pos = 0.0f;

    float m_Kicked_Bomb_Speed = 10.0f;
    float m_Thrown_Bomb_Speed = 15.0f;
    
    public bool m_isKicked = false;
    public bool m_isThrown = false;
    public bool m_is_Thrown_Bomb_Moving = false;
    public bool m_isDestroyed = false;
    bool m_isinvalidated = false;

    public bool m_is_Rising_Start = false;
    bool m_is_Done_Rising = false;
    float m_Rising_Limit = 2.0f;
    float m_Down_Limit = -0.8f;
    float m_Rising_Speed = 10.0f;
    float m_Down_Speed = 15.0f;

    float m_Escape_Time;

    public void MakeExplode()
    {
        m_BombCountDown = -1.0f;
    }

    void Start()
    {
        m_My_MCL_Index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z, false);
        StageManager.Update_MCL_isBlocked(m_My_MCL_Index, true);
        c_Bomb = this;

        m_Escape_Time = 0.0f;
        m_is_Rising_Start = false;
        m_is_Done_Rising = false;

        m_Blocked_N = false;
        m_Blocked_S = false;
        m_Blocked_W = false;
        m_Blocked_E = false;
    }

    void Update()
    {
        if (m_BombCountDown > 0.0f)
        {
            BombAnimate(Time.deltaTime);
            if (m_isKicked)
                Kicked_Bomb_Move();
            if (m_is_Thrown_Bomb_Moving)
                Thrown_Bomb_Move();
            else m_BombCountDown -= Time.deltaTime;
        }

        //폭발 전 사운드 출력(풀링으로 수정 예정)
        else if (m_BombCountDown <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Flame_Remains") || other.gameObject.CompareTag("Flame") || other.gameObject.CompareTag("Flame_Bush"))
        {
            // 폭탄 파괴
            if (m_isKicked || m_is_Thrown_Bomb_Moving)
            {
                transform.position = new Vector3(other.transform.position.x, 0.0f, other.transform.position.z);
                Bomb_MCL_And_Position_Update();
            }
            m_is_Thrown_Bomb_Moving = false;
            m_isKicked = false;
            Destroy(gameObject);
        }
    }

    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Land"))
        {
            if (m_is_Thrown_Bomb_Moving && !m_is_Rising_Start)
            {
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
                Bomb_MCL_And_Position_Update();
                m_is_Thrown_Bomb_Moving = false;
            }
        }


        // 발차기로 이동중인 폭탄이
        // 오브젝트와 충돌시 정지
        if (//collision.gameObject.CompareTag("Player")
            collision.gameObject.CompareTag("Rock")
            || collision.gameObject.CompareTag("Box")
            || collision.gameObject.CompareTag("Monster")
            || collision.gameObject.CompareTag("Wall")
            || collision.gameObject.CompareTag("Bomb"))
        {
            if (m_isKicked)
            {
                Bomb_MCL_And_Position_Update();
                m_isKicked = false;
            }
            if (m_is_Thrown_Bomb_Moving && !m_is_Rising_Start && transform.position.y > 0.5f)
            {
                // ReRising 해야한다.
                m_isThrown = false;
                m_Thrown_Bomb_Speed = 3.0f;
                m_is_Done_Rising = false;
            }
        }

        
    }

    
    void OnCollisionStay(Collision collision)
    {
        // 폭탄 끼임 탈출
        if (collision.gameObject.CompareTag("Monster"))
        {
            if (m_Escape_Time < 0.1f)
            {
                m_Escape_Time += Time.deltaTime;
            }
            else
            {
                StageManager.Update_MCL_isBlocked(m_My_MCL_Index, false);
                transform.Translate(new Vector3(0.0f, 0.0f, -2.0f));
                m_My_MCL_Index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z, false);
                StageManager.Update_MCL_isBlocked(m_My_MCL_Index, true);

                m_Escape_Time = 0.0f;
            }
        }
    }


    void OnDestroy()
    {
        if (!StageManager.c_Stage_Manager.m_is_Map_Changing && !m_is_Thrown_Bomb_Moving)
        {
            // 폭발 사운드 출력
            MusicManager.manage_ESound.soundE();
            if (transform.gameObject.GetComponentInChildren<RingEffectCollider>().m_isInRange)
                PlayerMove.C_PM.AniBomb_Start();

            Instantiate(m_Boom_Effect).transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z);

            // 폭발 전 위치 조정
            Bomb_MCL_And_Position_Update();

            GameObject Instance_Flame = Instantiate(m_Flame);

            Instance_Flame.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z);

            for (int i = 1; i <= m_FlameCount; ++i)
            {
                GameObject Instance_FlameDir_N;
                GameObject Instance_FlameDir_S;
                GameObject Instance_FlameDir_W;
                GameObject Instance_FlameDir_E;

                if (!m_Blocked_N)
                {
                    if (StageManager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].isBlocked == false)
                    {
                        Instance_FlameDir_N = Instantiate(m_Flame);
                        Instance_FlameDir_N.transform.position = new Vector3(StageManager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].x, 0.0f, StageManager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].z);
                    }
                    else
                    {
                        m_Blocked_N = true;
                        GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                        Instance_Flame_Remains.transform.position = new Vector3(StageManager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].x, 0.0f, StageManager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].z);
                    }
                }

                if (!m_Blocked_S)
                {
                    if (StageManager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].isBlocked == false)
                    {
                        Instance_FlameDir_S = Instantiate(m_Flame);
                        Instance_FlameDir_S.transform.position = new Vector3(StageManager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].x, 0.0f, StageManager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].z);
                    }
                    else
                    {
                        m_Blocked_S = true;
                        GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                        Instance_Flame_Remains.transform.position = new Vector3(StageManager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].x, 0.0f, StageManager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].z);
                    }
                }

                if (!m_Blocked_W)
                {
                    if (StageManager.m_Map_Coordinate_List[m_My_MCL_Index - i].isBlocked == false)
                    {
                        Instance_FlameDir_W = Instantiate(m_Flame);
                        Instance_FlameDir_W.transform.position = new Vector3(StageManager.m_Map_Coordinate_List[m_My_MCL_Index - i].x, 0.0f, StageManager.m_Map_Coordinate_List[m_My_MCL_Index - i].z);
                    }
                    else
                    {
                        m_Blocked_W = true;
                        GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                        Instance_Flame_Remains.transform.position = new Vector3(StageManager.m_Map_Coordinate_List[m_My_MCL_Index - i].x, 0.0f, StageManager.m_Map_Coordinate_List[m_My_MCL_Index - i].z);
                    }
                }
                if (!m_Blocked_E)
                {
                    if (StageManager.m_Map_Coordinate_List[m_My_MCL_Index + i].isBlocked == false)
                    {
                        Instance_FlameDir_E = Instantiate(m_Flame);
                        Instance_FlameDir_E.transform.position = new Vector3(StageManager.m_Map_Coordinate_List[m_My_MCL_Index + i].x, 0.0f, StageManager.m_Map_Coordinate_List[m_My_MCL_Index + i].z);
                    }
                    else
                    {
                        m_Blocked_E = true;
                        GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                        Instance_Flame_Remains.transform.position = new Vector3(StageManager.m_Map_Coordinate_List[m_My_MCL_Index + i].x, 0.0f, StageManager.m_Map_Coordinate_List[m_My_MCL_Index + i].z);
                    }
                }
            }

            // MCL 갱신
            StageManager.Update_MCL_isBlocked(m_My_MCL_Index, false);
        }
        //폭탄 수 다시 증가
        PlayerMove.C_PM.ReloadUp();
    }


    void BombAnimate(float time)
    {
        m_Scale_Timer += time;
        m_Weight_Time_XY = time * 0.05f;
        m_Weight_Time_Z = time * 0.45f;
        m_Weight_Time_Pos = time * 0.1f;

        if (m_Scale_Timer >= 1.2f)
        {
            m_Reverse_Scale *= -1.0f;
            m_Scale_Timer = 0.0f;
        }

        gameObject.transform.localScale += new Vector3(m_Reverse_Scale * m_Weight_Time_XY, m_Reverse_Scale * m_Weight_Time_XY, m_Reverse_Scale * m_Weight_Time_Z);
        gameObject.transform.localPosition += new Vector3(0.0f, m_Reverse_Scale * m_Weight_Time_Pos, 0.0f);
    }



    public void SetBombDir()
    {
        // 폭탄의 방향 설정
        float angleY = PlayerMove.C_PM.transform.localEulerAngles.y;
        float bombAngleY = 0.0f;

        if (angleY >= 315.0f && angleY < 45.0f)
            bombAngleY = 0.0f;
        else if (angleY >= 45.0f && angleY < 135.0f)
            bombAngleY = 90.0f;
        else if (angleY >= 135.0f && angleY < 225.0f)
            bombAngleY = 180.0f;
        else if (angleY >= 225.0f && angleY < 315.0f)
            bombAngleY = 270.0f;


        transform.Rotate(0.0f, bombAngleY, 0.0f);
    }

    void Kicked_Bomb_Move()
    {
        transform.Translate(new Vector3(0.0f, 0.0f, (m_Kicked_Bomb_Speed * Time.deltaTime)));
    }

    void Thrown_Bomb_Move()
    {
        if (m_isThrown)
        {
            m_Rising_Limit = 4.0f;
        }

        else
        {
            m_Rising_Limit = 2.5f;
        }

        if (m_is_Rising_Start)
        {
            // 폭탄 일정량 상승
            transform.position = new Vector3(transform.position.x, (transform.position.y + (m_Rising_Speed * Time.deltaTime)), transform.position.z);
            if (transform.position.y > 2.0f)
            {
                m_is_Rising_Start = false;
                GetComponent<MeshRenderer>().enabled = true;
            }
        }

        else
        {
            // 폭탄 전방 이동
            transform.Translate(new Vector3(0.0f, 0.0f, (m_Thrown_Bomb_Speed * Time.deltaTime)));
        }

        if (!m_is_Done_Rising && transform.position.y < m_Rising_Limit)
        {
            // 폭탄 상승
            transform.Translate(new Vector3(0.0f, (m_Rising_Speed * Time.deltaTime), 0.0f));
        }

        if (transform.position.y >= m_Rising_Limit)
        {
            // 폭탄 상승 끝
            m_is_Done_Rising = true;
        }

        if (m_is_Done_Rising && transform.position.y > m_Down_Limit)
        {
            // 폭탄 하강
            transform.Translate(new Vector3(0.0f, -(m_Down_Speed * Time.deltaTime), 0.0f));
        }

        
        if (transform.position.x < -0.5f || transform.position.x > 28.5f
            || transform.position.z < 49.5f || transform.position.z > 78.5f)
        {
            // 맵 범위 밖으로 나가면 제거
            StageManager.Update_MCL_isBlocked(m_My_MCL_Index, false);
            Destroy(gameObject);
        }
    }

    void Bomb_MCL_And_Position_Update()
    {
        StageManager.Update_MCL_isBlocked(m_My_MCL_Index, false);
        m_My_MCL_Index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z, false);
        Vector3 pos;
        pos.x = StageManager.m_Map_Coordinate_List[m_My_MCL_Index].x;
        pos.y = transform.position.y;
        pos.z = StageManager.m_Map_Coordinate_List[m_My_MCL_Index].z;
        transform.position = pos;
        StageManager.Update_MCL_isBlocked(m_My_MCL_Index, true);
    }
    
}