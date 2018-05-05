using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class WHOSE_BOMB
{
    public const int PLAYER = 0;
    public const int JETGOBLIN = 1;
}



//폭탄 함수
public class Bomb : MonoBehaviour
{
    private Collider m_BCollider;
    public GameObject m_Flame;
    public GameObject m_Flame_Remains;
    public GameObject m_Boom_Effect;
    public GameObject m_Explosion_Range;

    GameObject m_Range_Base;
    List<GameObject> m_Range_N = new List<GameObject>();
    List<GameObject> m_Range_S = new List<GameObject>();
    List<GameObject> m_Range_W = new List<GameObject>();
    List<GameObject> m_Range_E = new List<GameObject>();

    public static Bomb c_Bomb;

    int m_FlameCount;

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

    Transform m_Model;


    public GameObject m_Whose_Bomb; // 어떤 객체의 폭탄인가?
    public int m_Whose_Bomb_Type; // 그 객체의 타입은 무엇인가?



    public bool m_isKicked = false; // 폭탄이 차였는가?
    public bool m_isThrown = false; // 폭탄이 던져졌는가?
    public bool m_is_Thrown_Bomb_Moving = false; // 던져진 폭탄이 움직이는 중인가?
    public bool m_isDestroyed = false; // 폭탄이 파괴되었는가?
    public bool m_is_Rising_Start = false; // 폭탄 튕겨오르는것이 시작 되었는가?
    bool m_is_Done_Rising = false; // 폭탄 튕겨오르는것이 완료 되었는가?


    float m_Rising_Limit = 2.0f; // 튕겨오르기 제한 높이
    float m_Down_Limit = -0.8f; // 떨어지기 제한 높이
    float m_Rising_Speed = 10.0f; // 튕겨오르는 속도
    float m_Down_Speed = 15.0f; // 떨어지는 속도

    float m_Escape_Time; 

    public void MakeExplode()
    {
        m_BombCountDown = -1.0f;
    }

    void Start()
    {
        m_My_MCL_Index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(transform.position.x, transform.position.z);
        StageManager.c_Stage_Manager.Update_MCL_isBlocked(m_My_MCL_Index, true);

        c_Bomb = this;

        m_Model = transform.Find("Bomb_리터칭");

        if (m_Whose_Bomb_Type == WHOSE_BOMB.PLAYER)
            m_FlameCount = UI.m_fire_count;
        else if (m_Whose_Bomb_Type == WHOSE_BOMB.JETGOBLIN)
            m_FlameCount = 1;

        m_Escape_Time = 0.0f;
        m_is_Rising_Start = false;
        m_is_Done_Rising = false;

        m_Blocked_N = false;
        m_Blocked_S = false;
        m_Blocked_W = false;
        m_Blocked_E = false;

        // 화력범위 생성
        Set_Explosion_Range();
        Explosion_Range_Pos_Update();
    }

    void Update()
    {
        if (!StageManager.c_Stage_Manager.m_is_Pause)
        {
            if (m_BombCountDown > 0.0f)
            {
                Explosion_Range_Manager();

                if (m_isKicked)
                    Kicked_Bomb_Move();
                else if (m_is_Thrown_Bomb_Moving)
                    Thrown_Bomb_Move();
                else
                {
                    BombAnimate(Time.deltaTime);
                    m_BombCountDown -= Time.deltaTime;
                }
            }

            //폭발 전 사운드 출력(풀링으로 수정 예정)
            else if (m_BombCountDown <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }


    // 충돌감지
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
            m_isThrown = false;
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
                m_is_Thrown_Bomb_Moving = false;
                m_isThrown = false;
                Bomb_MCL_And_Position_Update();
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
                m_Model.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                Bomb_MCL_And_Position_Update();
                m_isKicked = false;
            }
            if (m_is_Thrown_Bomb_Moving && !m_is_Rising_Start && transform.position.y > 0.5f)
            {
                // ReRising 해야한다.
                m_isThrown = false;
                m_is_Rising_Start = true;
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
                StageManager.c_Stage_Manager.Update_MCL_isBlocked(m_My_MCL_Index, false);
                transform.Translate(new Vector3(0.0f, 0.0f, -2.0f));
                m_My_MCL_Index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(transform.position.x, transform.position.z);
                StageManager.c_Stage_Manager.Update_MCL_isBlocked(m_My_MCL_Index, true);

                m_Escape_Time = 0.0f;
            }
        }
    }

    // ==================================


    // "화력 범위 오브젝트"를 생성
    void Set_Explosion_Range()
    {
        // 정위치
        m_Range_Base = Instantiate(m_Explosion_Range);

        // 그리고 방향별로 8개씩 생성
        for (int i = 1; i <= m_FlameCount; ++i)
        {
            m_Range_N.Add(Instantiate(m_Explosion_Range));
            m_Range_S.Add(Instantiate(m_Explosion_Range));
            m_Range_W.Add(Instantiate(m_Explosion_Range));
            m_Range_E.Add(Instantiate(m_Explosion_Range));
        }
    }

    // "화력 범위 오브젝트"의 위치를 갱신
    void Explosion_Range_Pos_Update()
    {
        if (!m_isThrown && !m_is_Thrown_Bomb_Moving)
        {
            m_Range_Base.transform.position = new Vector3(gameObject.transform.position.x, -0.7f, gameObject.transform.position.z);
            
            for (int i = 1; i <= m_FlameCount; ++i)
            {
                if (m_My_MCL_Index + i <= 288)
                    m_Range_N[i - 1].transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + i].x, -0.7f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + i].z);
                
                if (m_My_MCL_Index - i >= 0)
                    m_Range_S[i - 1].transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - i].x, -0.7f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - i].z);
                
                if (m_My_MCL_Index - 17 * i >= 0)
                    m_Range_W[i - 1].transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].x, -0.7f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].z);
                
                if (m_My_MCL_Index + 17 * i <= 288)
                    m_Range_E[i - 1].transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].x, -0.7f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].z);
            }
        }
    }

    void Explosion_Range_Manager()
    {
        if (m_isKicked || m_is_Thrown_Bomb_Moving || m_isThrown)
        {
            m_Range_Base.SetActive(false);
            for (int i = 0; i < m_FlameCount; ++i)
            {
                m_Range_N[i].SetActive(false);
                m_Range_S[i].SetActive(false);
                m_Range_W[i].SetActive(false);
                m_Range_E[i].SetActive(false);
            }
        }

        else
        {
            m_Range_Base.SetActive(true);

            bool is_blocked_N = false;
            bool is_blocked_S = false;
            bool is_blocked_W = false;
            bool is_blocked_E = false;

            for (int i = 1; i <= m_FlameCount; ++i)
            {
                if (!is_blocked_N)
                {
                    if (m_My_MCL_Index + i <= 288)
                    { 
                        if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(m_My_MCL_Index + i) == false)
                        {
                            m_Range_N[i - 1].SetActive(true);
                        }
                        else
                        {
                            is_blocked_N = true;
                            for (int j = i - 1; j < m_FlameCount; ++j)
                                m_Range_N[j].SetActive(false);
                        }
                    }
                }
                if (!is_blocked_S)
                {
                    if (m_My_MCL_Index - i >= 0)
                    {
                        if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(m_My_MCL_Index - i) == false)
                        {
                            m_Range_S[i - 1].SetActive(true);
                        }
                        else
                        {
                            is_blocked_S = true;
                            for (int j = i - 1; j < m_FlameCount; ++j)
                                m_Range_S[j].SetActive(false);
                        }
                    }
                }
                if (!is_blocked_W)
                {
                    if (m_My_MCL_Index - 17 * i >= 0)
                    {
                        if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(m_My_MCL_Index - 17 * i) == false)
                        {
                            m_Range_W[i - 1].SetActive(true);
                        }
                        else
                        {
                            is_blocked_W = true;
                            for (int j = i - 1; j < m_FlameCount; ++j)
                                m_Range_W[j].SetActive(false);
                        }
                    }
                }
                if (!is_blocked_E)
                {
                    if (m_My_MCL_Index + 17 * i <= 288)
                    {
                        if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(m_My_MCL_Index + 17 * i) == false)
                        {
                            m_Range_E[i - 1].SetActive(true);
                        }
                        else
                        {
                            is_blocked_E = true;
                            for (int j = i - 1; j < m_FlameCount; ++j)
                                m_Range_E[j].SetActive(false);
                        }
                    }
                }
            }
        }
    }







    // 터졌을 때
    void OnDestroy()
    {
        if (!StageManager.c_Stage_Manager.Get_Game_Over()) // 게임이 안끝났으면
        {
            if (!StageManager.c_Stage_Manager.m_is_Map_Changing && !m_is_Thrown_Bomb_Moving)
            {
                // 폭발 사운드 출력
                if (MusicManager.manage_ESound != null)
                    MusicManager.manage_ESound.soundE();

                // 플레이어가 감지범위 내에 있을 경우
                // 카메라 흔들림 연출
                if (transform.gameObject.GetComponentInChildren<RingEffectCollider>().m_isInRange)
                    PlayerMove.C_PM.AniBomb_Start();

                // 폭발 이펙트 생성 (큰것)
                Instantiate(m_Boom_Effect).transform.position = new Vector3(gameObject.transform.position.x, -1.0f, gameObject.transform.position.z);

                // 범위 삭제
                Destroy(m_Range_Base);
                m_Range_Base = null;

                for (int i = 1; i <= m_FlameCount; ++i)
                {
                    Destroy(m_Range_N[i - 1]);
                    Destroy(m_Range_S[i - 1]);
                    Destroy(m_Range_W[i - 1]);
                    Destroy(m_Range_E[i - 1]);
                }

                m_Range_N.Clear();
                m_Range_S.Clear();
                m_Range_W.Clear();
                m_Range_E.Clear();

                // 폭발 전 위치 조정
                Bomb_MCL_And_Position_Update();

                // 이하는 화염 생성 구문
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
                        if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(m_My_MCL_Index + i) == false)
                        {
                            Instance_FlameDir_N = Instantiate(m_Flame);
                            Instance_FlameDir_N.transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + i].x, 0.0f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + i].z);
                        }
                        else
                        {
                            m_Blocked_N = true;
                            GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                            Instance_Flame_Remains.transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + i].x, 0.0f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + i].z);
                        }
                    }

                    if (!m_Blocked_S)
                    {
                        if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(m_My_MCL_Index - i) == false)
                        {
                            Instance_FlameDir_S = Instantiate(m_Flame);
                            Instance_FlameDir_S.transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - i].x, 0.0f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - i].z);
                        }
                        else
                        {
                            m_Blocked_S = true;
                            GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                            Instance_Flame_Remains.transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - i].x, 0.0f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - i].z);
                        }
                    }

                    if (!m_Blocked_W)
                    {
                        if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(m_My_MCL_Index - 17 * i) == false)
                        {
                            Instance_FlameDir_W = Instantiate(m_Flame);
                            Instance_FlameDir_W.transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].x, 0.0f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].z);
                        }
                        else
                        {
                            m_Blocked_W = true;
                            GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                            Instance_Flame_Remains.transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].x, 0.0f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index - 17 * i].z);
                        }
                    }
                    if (!m_Blocked_E)
                    {
                        if (StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(m_My_MCL_Index + 17 * i) == false)
                        {
                            Instance_FlameDir_E = Instantiate(m_Flame);
                            Instance_FlameDir_E.transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].x, 0.0f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].z);
                        }
                        else
                        {
                            m_Blocked_E = true;
                            GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                            Instance_Flame_Remains.transform.position = new Vector3(StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].x, 0.0f, StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index + 17 * i].z);
                        }
                    }
                }

                // MCL 갱신
                StageManager.c_Stage_Manager.Update_MCL_isBlocked(m_My_MCL_Index, false);
            }


            //폭탄 수 다시 증가
            if (m_Whose_Bomb_Type == WHOSE_BOMB.PLAYER)
            {
                m_Whose_Bomb.GetComponent<PlayerMove>().ReloadUp();
            }

            else if (m_Whose_Bomb_Type == WHOSE_BOMB.JETGOBLIN)
            {
                m_Whose_Bomb.GetComponent<Boss_AI_JetGoblin>().Bomb_Reload();
            }
        }
    }



    // =======================================










    // 폭탄 늘어났다 줄어드는 애니메이션
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






    // 폭탄의 방향 설정
    public void SetBombDir()
    {
        float angleY = 0.0f;

        if (m_Whose_Bomb_Type == WHOSE_BOMB.PLAYER)
        {
            angleY = m_Whose_Bomb.GetComponent<PlayerMove>().transform.localEulerAngles.y;
        }
        else if (m_Whose_Bomb_Type == WHOSE_BOMB.JETGOBLIN)
        {
            angleY = m_Whose_Bomb.GetComponent<Boss_AI_JetGoblin>().transform.localEulerAngles.y;
        }

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
        transform.Find("Bomb_리터칭").Rotate(0.0f, bombAngleY, 0.0f);
    }





    // 차여진 폭탄의 이동
    void Kicked_Bomb_Move()
    {
        m_Model.Rotate(transform.right, 70.0f);
        transform.Translate(new Vector3(0.0f, 0.0f, (m_Kicked_Bomb_Speed * Time.deltaTime)));
    }




    // 던져진 폭탄의 이동
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
                GetComponentInChildren<MeshRenderer>().enabled = true;
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
            StageManager.c_Stage_Manager.Update_MCL_isBlocked(m_My_MCL_Index, false);
            Destroy(gameObject);
        }
    }






    // 폭탄의 위치와 MCL, 범위 오브젝트의 위치를 Update
    void Bomb_MCL_And_Position_Update()
    {
        StageManager.c_Stage_Manager.Update_MCL_isBlocked(m_My_MCL_Index, false);
        m_My_MCL_Index = StageManager.c_Stage_Manager.Find_Own_MCL_Index(transform.position.x, transform.position.z);

        if (m_My_MCL_Index != -1)
        {
            Vector3 Loc;
            Loc.x = StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index].x;
            Loc.y = transform.position.y;
            Loc.z = StageManager.c_Stage_Manager.m_Map_Coordinate_List[m_My_MCL_Index].z;
            transform.position = Loc;
        }

        StageManager.c_Stage_Manager.Update_MCL_isBlocked(m_My_MCL_Index, true);

        if (m_Range_Base)
            Explosion_Range_Pos_Update();
    }
    
}