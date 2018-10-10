using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class BOMB_TIMER
{
    public const float EXPLODE_TIME = 3.0f;
}

static class BOMB_COLLIDER_SIZE
{
    public const float NORMAL_X = 1.8f;
    public const float NORMAL_Y = 1.5f;
    public const float NORMAL_Z = 1.8f;
    public const float SMALL_X = 1.2f;
    public const float SMALL_Y = 1.3f;
    public const float SMALL_Z = 1.2f;
}

static class THROWN_BOMB_STATE
{
    public const int RISING = 0;
    public const int FALLING = 1;
}

static class THROWN_BOMB_VALUES
{
    public const float IN_AIR_SPEED = 10.0f; // 체공 (상승/하강) 속도
    public const float THROUGH_SPEED = 8.0f; // 체공 (진행) 속도
    public const float OFFSET_Z = 0.7f; // 초기 OFFSET Z
    public const float OFFSET_Y = 0.4f; // 초기 OFFSET Y
    public const float RISING_MAX_Y = 4.0f; // 최대 높이 Y
    public const float RERISING_MAX_Y = 2.5f; // 최대 높이 Y
    public const float DEST_Z = 4.0f; // 최대 y에 도달까지 거리, 이후 낙하 완료 까지 거리
}

static class KICKED_BOMB_VALUES
{
    public const float THROUGH_SPEED = 10.0f; // 이동속도
}

public class Bomb_Remaster : MonoBehaviour
{
    public GameObject m_Bomb_Model;
    public Shake_Checker m_Shake_Checker;
    public Jump_Checker m_Jump_Checker;
    Bomb_Sound m_Bomb_Sound;

    GameObject m_Range_Box;
    public GameObject m_Center_Range;
    List<GameObject> m_N_Ranges;
    List<GameObject> m_S_Ranges;
    List<GameObject> m_W_Ranges;
    List<GameObject> m_E_Ranges;


    GameObject m_Fire_Box;
    public GameObject m_Center_Fire;
    public GameObject m_Fire_Object;
    List<GameObject> m_N_Fires;
    List<GameObject> m_S_Fires;
    List<GameObject> m_W_Fires;
    List<GameObject> m_E_Fires;

    BoxCollider m_Center_Fire_Collider;
    List<BoxCollider> m_N_Fire_Colliders;
    List<BoxCollider> m_S_Fire_Colliders;
    List<BoxCollider> m_W_Fire_Colliders;
    List<BoxCollider> m_E_Fire_Colliders;

    public Flame_Remains[] m_Fire_Remains;

    IEnumerator m_Idle_State;
    IEnumerator m_Thrown_State;
    IEnumerator m_Kicked_State;
    IEnumerator m_Fire_Checker;

    Animation m_Animations;

    ParticleSystem m_Center_Particle;

    BoxCollider m_Bomb_Collider; public void Change_Bomb_Colliders_isTrigger(bool b) { m_Bomb_Collider.isTrigger = b; }

    bool m_is_Set_Complete; // 설치가 완료 되었는가?

    Bomb_Setter m_Whose_Bomb; // 누구의 폭탄인가?
    GameObject m_Who_Throw; // 누가 던졌는가?
    GameObject m_Who_Kick; // 누가 찼는가?

    float m_Curr_Bomb_Timer; // 폭탄의 현재 타이머 값
    float m_FireCount;

    int m_Curr_Thrown_State; // 현재의 던지기 상태
    Vector3 m_Throw_Middle_Point; // 던지기 이후 중간지점.
    Vector3 m_Throw_End_Point; // 던지기 이후 종료지점.

    bool m_is_Thrown; // 던지기 당했는가?
    public bool Get_is_Thrown() { return m_is_Thrown; }
    bool m_is_Rising; // 상승 중인가?
    bool m_is_Kicked; // 발차기 당했는가?
    bool m_is_Hardend; // 굳혀졌는가?
    bool m_is_Explode; // 폭발했는가?

    bool m_is_Fire_Life_Over; // 불꽃의 수명이 다했는가? (불꽃 충돌체 끄기)

    public bool Get_is_Hardend() { return m_is_Hardend; }
    public void Set_is_Hardend(bool b) { m_is_Hardend = b; Change_Bomb_Colliders_isTrigger(!b); }

    int m_MCL_Index; // MCL 인덱스

    void Awake()
    {
        m_Idle_State = Idle_State();
        m_Thrown_State = Thrown_State();
        m_Kicked_State = Kicked_State();
        m_Fire_Checker = Fire_Check();

        m_Center_Particle = m_Center_Fire.GetComponent<ParticleSystem>();
        m_Animations = GetComponent<Animation>();
        m_Bomb_Collider = GetComponent<BoxCollider>();
        m_Bomb_Sound = GetComponentInChildren<Bomb_Sound>();

        m_Curr_Bomb_Timer = 0.0f;
        m_is_Thrown = false;
        m_is_Rising = false;
        m_is_Kicked = false;
        m_is_Set_Complete = false;
        m_is_Explode = false;
        m_is_Hardend = false;
        m_is_Fire_Life_Over = true;

        Range_Object_Init();
        Fire_Object_Init();
        m_Bomb_Model.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flame") || 
            other.CompareTag("Flame_Remains") || 
            other.CompareTag("Flame_Bush") ||
            other.CompareTag("Flame_Crash") ||
            other.CompareTag("icicle_Body"))
        {
            if (m_is_Kicked || m_is_Thrown) Set_Bomb();

            Explode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (m_is_Kicked)
        {
            if (!m_is_Set_Complete && (
                collision.gameObject.CompareTag("Box") ||
                collision.gameObject.CompareTag("Wall") ||
                collision.gameObject.CompareTag("Rock") ||
                collision.gameObject.CompareTag("Bomb") ||
                collision.gameObject.CompareTag("Monster") ||
                collision.gameObject.CompareTag("Boss_Monster") ||
                collision.gameObject.CompareTag("icicle_Body"))
                ) Set_Bomb();
        }

        if (m_is_Thrown)
        {
            if (!m_is_Set_Complete && !m_is_Rising && (
                collision.gameObject.CompareTag("Monster") ||
                collision.gameObject.CompareTag("Boss_Monster") ||
                collision.gameObject.CompareTag("icicle_Body"))
                ) Set_Bomb();
        }
    }

    IEnumerator Idle_State()
    {
        while(true)
        {
            if (!StageManager.GetInstance().Get_is_Pause())
            {
                if (m_Curr_Bomb_Timer >= BOMB_TIMER.EXPLODE_TIME) Explode();
                else
                {
                    m_Curr_Bomb_Timer += Time.deltaTime;
                    m_Animations.Play("Bomb_Pumpin");
                    Range_Reset();
                }
            }
            yield return null;
        }
    }

    IEnumerator Thrown_State()
    {
        while(true)
        {
            if (!StageManager.GetInstance().Get_is_Pause())
            {
                switch (m_Curr_Thrown_State)
                {
                    case THROWN_BOMB_STATE.RISING:
                        m_Animations.Play(m_Animations.GetClip("Bomb_Rollin").name);

                        transform.Translate(new Vector3(0.0f, THROWN_BOMB_VALUES.IN_AIR_SPEED * Time.deltaTime, THROWN_BOMB_VALUES.THROUGH_SPEED * Time.deltaTime));

                        if (transform.position.y >= m_Throw_Middle_Point.y - 0.05f) Set_Falling();
                        break;

                    case THROWN_BOMB_STATE.FALLING:
                        m_Animations.Play(m_Animations.GetClip("Bomb_Rollin").name);

                        transform.Translate(new Vector3(0.0f, -THROWN_BOMB_VALUES.IN_AIR_SPEED * Time.deltaTime, THROWN_BOMB_VALUES.THROUGH_SPEED * Time.deltaTime));

                        if (transform.position.y <= m_Throw_End_Point.y + 0.05f) Set_Bomb();
                        break;
                }
            }
            yield return null;
        }
    }

    IEnumerator Kicked_State()
    {
        while (true)
        {
            if (!StageManager.GetInstance().Get_is_Pause())
            {
                transform.Translate(new Vector3(0.0f, 0.0f, KICKED_BOMB_VALUES.THROUGH_SPEED * Time.deltaTime));
                m_Animations.Play(m_Animations.GetClip("Bomb_Rollin").name);
            }
            yield return null;
        }
    }

    public void Throw_Bomb(GameObject who) // 폭탄 던지기
    {
        if (!m_is_Thrown && !m_is_Kicked)
        {
            m_Who_Throw = who;
            m_Curr_Thrown_State = THROWN_BOMB_STATE.RISING;
            Set_Bomb_Off();

            transform.position = m_Who_Throw.transform.position; // 포지션 전환
            transform.localEulerAngles = new Vector3 (0.0f, m_Who_Throw.transform.localEulerAngles.y, 0.0f); // 방향 전환
            transform.Translate(new Vector3(0.0f, THROWN_BOMB_VALUES.OFFSET_Y, THROWN_BOMB_VALUES.OFFSET_Z)); // 오프셋 적용

            Set_Collider_Size_Small();

            Set_Rising(THROWN_BOMB_VALUES.RISING_MAX_Y);

            m_is_Thrown = true;
            m_is_Set_Complete = false;

            StopCoroutine(m_Idle_State);
            StartCoroutine(m_Thrown_State);
        }
    }

    public void Drop_Bomb(GameObject who) // 폭탄 떨구기
    {
        if (!m_is_Thrown && !m_is_Kicked)
        {
            m_Who_Throw = who;

            Set_Falling();

            Set_Bomb_Off();

            transform.position = m_Who_Throw.transform.position; // 포지션 전환
            transform.localEulerAngles = new Vector3(0.0f, m_Who_Throw.transform.localEulerAngles.y, 0.0f); // 방향 전환

            Set_Collider_Size_Small();

            m_is_Thrown = true;
            m_is_Set_Complete = false;

            StopCoroutine(m_Idle_State);
            StartCoroutine(m_Thrown_State);
        }
    }

    public void Kick_Bomb(GameObject who) // 폭탄 발차기
    {
        if (!m_is_Kicked && !m_is_Thrown)
        {
            m_Who_Kick = who;
            float rotY = m_Who_Kick.transform.localEulerAngles.y;
            if (rotY <= 45.0f && rotY > -45.0f) rotY = 0.0f;
            else if (rotY <= 135.0f && rotY > 45.0f) rotY = 90.0f;
            else if (rotY <= 225.0f && rotY > 135.0f) rotY = 180.0f;
            else rotY = 270.0f;
            transform.localEulerAngles = new Vector3(0.0f, rotY, 0.0f);
            transform.Translate(new Vector3(0.0f, 0.1f, 0.0f));
            
            Set_Bomb_Off();

            m_is_Kicked = true;
            m_is_Set_Complete = false;

            StopCoroutine(m_Idle_State);
            StartCoroutine(m_Kicked_State);
        }
    }

    public void Set_Rising(float height)
    {
        m_Throw_Middle_Point = transform.position; // 던져진 시점의 포인트 저장.
        m_Throw_Middle_Point.y = height;
        m_Throw_Middle_Point.z += THROWN_BOMB_VALUES.DEST_Z;
        
        
        m_is_Rising = true;
        m_Jump_Checker.gameObject.SetActive(false);
        m_Curr_Thrown_State = THROWN_BOMB_STATE.RISING; // 다음으로 간다.
    }

    public void Set_Falling()
    {
        m_Throw_End_Point = transform.position; // 던져진 시점의 포인트 저장.
        m_Throw_End_Point.y = 0.0f;
        m_Throw_End_Point.z += THROWN_BOMB_VALUES.DEST_Z;
        
        m_is_Rising = false;
        m_Jump_Checker.gameObject.SetActive(true);
        m_Curr_Thrown_State = THROWN_BOMB_STATE.FALLING; // 다음으로 간다.
    }

    void Set_Collider_Size_Small()
    {
        m_Bomb_Collider.size = new Vector3(BOMB_COLLIDER_SIZE.SMALL_X, BOMB_COLLIDER_SIZE.SMALL_Y, BOMB_COLLIDER_SIZE.SMALL_Z);
    }

    void Set_Collider_Size_Normal()
    {
        m_Bomb_Collider.size = new Vector3(BOMB_COLLIDER_SIZE.NORMAL_X, BOMB_COLLIDER_SIZE.NORMAL_Y, BOMB_COLLIDER_SIZE.NORMAL_Z);
    }
    

    public void Set_Bomb() // 폭탄 설치 (재조정)
    {
        m_Bomb_Model.SetActive(true);

        m_Bomb_Model.transform.localEulerAngles = Vector3.zero;

        m_MCL_Index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z);
        
        Vector3 pos = transform.position;
        pos.y = 0.0f;
        StageManager.GetInstance().Get_MCL_Coordinate(m_MCL_Index, ref pos.x, ref pos.z);
        transform.position = pos;

        Set_Collider_Size_Normal();

        StageManager.GetInstance().Update_MCL_isBlocked(m_MCL_Index, true);

        Range_Reset();

        m_is_Thrown = false;
        m_Who_Throw = null;
        StopCoroutine(m_Thrown_State);

        m_is_Kicked = false;
        m_Who_Kick = null;
        StopCoroutine(m_Kicked_State);

        m_is_Set_Complete = true;

        StartCoroutine(m_Idle_State);
    }

    void Set_Bomb_Off() // 폭탄 해제 (기능은 살아있는 채로..)
    {
        StageManager.GetInstance().Update_MCL_isBlocked(m_MCL_Index, false);
        Range_OFF();
    }

    public void Get_Out_Of_Pool(GameObject who, int state) // 풀에서 나온다!
    {
        transform.position = who.transform.position;
        Set_Whose_Bomb(who.GetComponent<Bomb_Setter>());
        m_FireCount = m_Whose_Bomb.Get_Fire_Count();
        switch (state)
        {
            case CALL_BOMB_STATE.NORMAL:
                Set_Bomb();
                break;

            case CALL_BOMB_STATE.DROP:
                Drop_Bomb(who);
                break;
        }
    }

    public void Return_To_Pool() // 풀로 복귀시키기 전의 작업들
    {
        Fire_OFF();
        
        m_Shake_Checker.gameObject.SetActive(false);
        m_Whose_Bomb.GetComponent<Bomb_Setter>().Bomb_Reload(); // 주인의 폭탄 개수를 다시 채워준다.
        if (m_Whose_Bomb.GetComponent<Player>() != null) m_Whose_Bomb.GetComponent<Player>().UI_Status_Update();
        m_Whose_Bomb = null; // 주인 마킹을 지운다.

        m_is_Hardend = false; // 굳히기 초기화.
        Change_Bomb_Colliders_isTrigger(true); // 트리거로 변경시킨다.

        m_Curr_Bomb_Timer = 0.0f; // 폭발 타이머 초기화.
        m_is_Explode = false;

        Set_Collider_Size_Normal();
        m_Jump_Checker.gameObject.SetActive(false);

        StopAllCoroutines(); // 모든 행동을 멈춘다.
        m_Center_Particle.Stop(); // 파티클 정지
        gameObject.transform.position = Vector3.zero; // 폭탄위치 초기화.
        Bomb_Pooling_Manager.GetInstance().Enqueue_Bomb(gameObject);// 풀에 채워넣는다.
    }




    public void Set_Whose_Bomb(Bomb_Setter who) { m_Whose_Bomb = who; } // 폭탄 주인 설정





    void Range_Object_Init() // 폭발 범위 오브젝트 초기화
    {
        m_Range_Box = new GameObject();
        m_Range_Box.transform.parent = transform;

        m_N_Ranges = new List<GameObject>();
        m_S_Ranges = new List<GameObject>();
        m_W_Ranges = new List<GameObject>();
        m_E_Ranges = new List<GameObject>();

        Vector3 pos; float unit;
        for (int i = 1; i <= MAX_STATUS.FIRE; ++i)
        {
            m_N_Ranges.Add(Instantiate(m_Center_Range));
            m_S_Ranges.Add(Instantiate(m_Center_Range));
            m_W_Ranges.Add(Instantiate(m_Center_Range));
            m_E_Ranges.Add(Instantiate(m_Center_Range));

            unit = MAP_SIZE.UNIT * i;

            pos = transform.position;
            pos.y = m_Center_Range.transform.position.y;
            pos.z = transform.position.z + unit;
            m_N_Ranges[i - 1].transform.position = pos;
            m_N_Ranges[i - 1].transform.parent = m_Range_Box.transform;

            pos = transform.position;
            pos.y = m_Center_Range.transform.position.y;
            pos.z = transform.position.z - unit;
            m_S_Ranges[i - 1].transform.position = pos;
            m_S_Ranges[i - 1].transform.parent = m_Range_Box.transform;

            pos = transform.position;
            pos.y = m_Center_Range.transform.position.y;
            pos.x = transform.position.x - unit;
            m_W_Ranges[i - 1].transform.position = pos;
            m_W_Ranges[i - 1].transform.parent = m_Range_Box.transform;

            pos = transform.position;
            pos.y = m_Center_Range.transform.position.y;
            pos.x = transform.position.x + unit;
            m_E_Ranges[i - 1].transform.position = pos;
            m_E_Ranges[i - 1].transform.parent = m_Range_Box.transform;
        }

        Range_OFF();
    }

    void Range_OFF() // 범위 끄기
    {
        m_Center_Range.SetActive(false);
        for (int i = 1; i <= MAX_STATUS.FIRE; ++i)
        {
            m_N_Ranges[i - 1].SetActive(false);
            m_S_Ranges[i - 1].SetActive(false);
            m_W_Ranges[i - 1].SetActive(false);
            m_E_Ranges[i - 1].SetActive(false);
        }
    }

    void Range_Reset() // 범위 재설정
    {
        Range_OFF();

        m_Center_Range.SetActive(true);


        bool is_N_Blocked = false; bool is_S_Blocked = false;
        bool is_W_Blocked = false; bool is_E_Blocked = false;
        // 한번이라도 막히는 순간 더이상 진행되지 않는다!

        for (int i = 0; i < m_FireCount; ++i)
        {
            if (!is_N_Blocked && !StageManager.GetInstance().Get_MCL_is_Blocked_With_Coord(m_N_Ranges[i].transform.position.x, m_N_Ranges[i].transform.position.z))
                m_N_Ranges[i].SetActive(true);
            else is_N_Blocked = true; 
            if (!is_S_Blocked && !StageManager.GetInstance().Get_MCL_is_Blocked_With_Coord(m_S_Ranges[i].transform.position.x, m_S_Ranges[i].transform.position.z))
                m_S_Ranges[i].SetActive(true);
            else is_S_Blocked = true;
            if (!is_W_Blocked && !StageManager.GetInstance().Get_MCL_is_Blocked_With_Coord(m_W_Ranges[i].transform.position.x, m_W_Ranges[i].transform.position.z))
                m_W_Ranges[i].SetActive(true);
            else is_W_Blocked = true;
            if (!is_E_Blocked && !StageManager.GetInstance().Get_MCL_is_Blocked_With_Coord(m_E_Ranges[i].transform.position.x, m_E_Ranges[i].transform.position.z))
                m_E_Ranges[i].SetActive(true);
            else is_E_Blocked = true;
        }
    }

    void Fire_Remains_Reset()
    {
        m_Fire_Remains[0].transform.position = Vector3.zero;
        m_Fire_Remains[0].gameObject.SetActive(false);

        m_Fire_Remains[1].transform.position = Vector3.zero;
        m_Fire_Remains[1].gameObject.SetActive(false);

        m_Fire_Remains[2].transform.position = Vector3.zero;
        m_Fire_Remains[2].gameObject.SetActive(false);

        m_Fire_Remains[3].transform.position = Vector3.zero;
        m_Fire_Remains[3].gameObject.SetActive(false);
    }

    void Fire_Object_Init() // 불꽃 오브젝트 초기화
    {
        m_Fire_Box = new GameObject();
        m_Fire_Box.transform.parent = transform;

        m_Center_Fire.SetActive(false);
        m_Center_Fire_Collider = m_Center_Fire.GetComponent<BoxCollider>();

        m_N_Fires = new List<GameObject>();
        m_S_Fires = new List<GameObject>();
        m_W_Fires = new List<GameObject>();
        m_E_Fires = new List<GameObject>();

        m_N_Fire_Colliders = new List<BoxCollider>();
        m_S_Fire_Colliders = new List<BoxCollider>();
        m_W_Fire_Colliders = new List<BoxCollider>();
        m_E_Fire_Colliders = new List<BoxCollider>();

        Vector3 pos; float unit;
        for (int i = 1; i <= MAX_STATUS.FIRE; ++i)
        {
            m_N_Fires.Add(Instantiate(m_Fire_Object));
            m_S_Fires.Add(Instantiate(m_Fire_Object));
            m_W_Fires.Add(Instantiate(m_Fire_Object));
            m_E_Fires.Add(Instantiate(m_Fire_Object));

            unit = MAP_SIZE.UNIT * i;

            pos = transform.position;
            pos.y = m_Fire_Object.transform.position.y;
            pos.z = transform.position.z + unit;
            m_N_Fires[i - 1].transform.position = pos;
            m_N_Fires[i - 1].transform.parent = m_Fire_Box.transform;
            
            m_N_Fire_Colliders.Add(m_N_Fires[i - 1].GetComponent<BoxCollider>());
            m_N_Fires[i - 1].SetActive(false);

            pos = transform.position;
            pos.y = m_Fire_Object.transform.position.y;
            pos.z = transform.position.z - unit;
            m_S_Fires[i - 1].transform.position = pos;
            m_S_Fires[i - 1].transform.parent = m_Fire_Box.transform;

            m_S_Fire_Colliders.Add(m_S_Fires[i - 1].GetComponent<BoxCollider>());
            m_S_Fires[i - 1].SetActive(false);

            pos = transform.position;
            pos.y = m_Fire_Object.transform.position.y;
            pos.x = transform.position.x - unit;
            m_W_Fires[i - 1].transform.position = pos;
            m_W_Fires[i - 1].transform.parent = m_Fire_Box.transform;

            m_W_Fire_Colliders.Add(m_W_Fires[i - 1].GetComponent<BoxCollider>());
            m_W_Fires[i - 1].SetActive(false);

            pos = transform.position;
            pos.y = m_Fire_Object.transform.position.y;
            pos.x = transform.position.x + unit;
            m_E_Fires[i - 1].transform.position = pos;
            m_E_Fires[i - 1].transform.parent = m_Fire_Box.transform;

            m_E_Fire_Colliders.Add(m_E_Fires[i - 1].GetComponent<BoxCollider>());
            m_E_Fires[i - 1].SetActive(false);
        }
    }

    void Fire_OFF() // 불꽃 오브젝트 끄기
    {
        m_Center_Fire.SetActive(false);
        for (int i = 0; i < m_FireCount; ++i)
        {
            m_N_Fires[i].SetActive(false);
            m_S_Fires[i].SetActive(false);
            m_W_Fires[i].SetActive(false);
            m_E_Fires[i].SetActive(false);
        }
    }

    void Fire_Collider_OFF()
    {
        m_Center_Fire_Collider.enabled = false;
        for (int i = 0; i < m_FireCount; ++i)
        {
            m_N_Fire_Colliders[i].enabled = false;
            m_S_Fire_Colliders[i].enabled = false;
            m_W_Fire_Colliders[i].enabled = false;
            m_E_Fire_Colliders[i].enabled = false;
        }
    }

    void Fire_ON() // 불꽃 오브젝트 켜기
    {
        m_Center_Fire.SetActive(true);

        for (int i = 0; i < m_FireCount; ++i)
        {
            if (m_N_Ranges[i].activeSelf)
            {
                m_N_Fires[i].SetActive(true);
                m_N_Fires[i].GetComponentInChildren<ParticleSystem>().Play();
            }
            else
            {
                m_Fire_Remains[0].gameObject.SetActive(true);
                m_Fire_Remains[0].GetComponent<Flame_Remains>().Cycle_Start();
                m_Fire_Remains[0].transform.position = m_N_Fires[i].transform.position;
                break;
            }
        }
        for (int i = 0; i < m_FireCount; ++i)
        {
            if (m_S_Ranges[i].activeSelf)
            {
                m_S_Fires[i].SetActive(true);
                m_S_Fires[i].GetComponentInChildren<ParticleSystem>().Play();
            }
            else
            {
                m_Fire_Remains[1].gameObject.SetActive(true);
                m_Fire_Remains[1].GetComponent<Flame_Remains>().Cycle_Start();
                m_Fire_Remains[1].transform.position = m_S_Fires[i].transform.position;
                break;
            }
        }
        for (int i = 0; i < m_FireCount; ++i)
        {
            if (m_W_Ranges[i].activeSelf)
            {
                m_W_Fires[i].SetActive(true);
                m_W_Fires[i].GetComponentInChildren<ParticleSystem>().Play();
            }
            else
            {
                m_Fire_Remains[2].gameObject.SetActive(true);
                m_Fire_Remains[2].GetComponent<Flame_Remains>().Cycle_Start();
                m_Fire_Remains[2].transform.position = m_W_Fires[i].transform.position;
                break;
            }
        }
        for (int i = 0; i < m_FireCount; ++i)
        {
            if (m_E_Ranges[i].activeSelf)
            {
                m_E_Fires[i].SetActive(true);
                m_E_Fires[i].GetComponentInChildren<ParticleSystem>().Play();
            }
            else
            {
                m_Fire_Remains[3].gameObject.SetActive(true);
                m_Fire_Remains[3].GetComponent<Flame_Remains>().Cycle_Start();
                m_Fire_Remains[3].transform.position = m_E_Fires[i].transform.position;
                break;
            }
        }

        Fire_Collider_ON();
    }

    void Fire_Collider_ON()
    {
        m_Center_Fire_Collider.enabled = true;
        for (int i = 0; i < m_FireCount; ++i)
        {
            m_N_Fire_Colliders[i].enabled = true;
            m_S_Fire_Colliders[i].enabled = true;
            m_W_Fire_Colliders[i].enabled = true;
            m_E_Fire_Colliders[i].enabled = true;
        }
    }

    void Explode() // 폭탄 폭발!
    {
        if (!m_is_Explode)
        {
            m_is_Explode = true;
            m_Bomb_Sound.Play_ExplodeSound();

            m_Bomb_Model.SetActive(false);
            
            if (m_Shake_Checker.Get_Target() != null) m_Shake_Checker.Get_Target().GetComponent<Player>().AniBomb_Start();
            m_Shake_Checker.Set_Out_of_Range();

            Fire_ON();
            m_is_Fire_Life_Over = false;
            Set_Bomb_Off();

            m_Center_Particle.Play();
            StopCoroutine(m_Idle_State);
            StartCoroutine(m_Fire_Checker);
        }
    }

    IEnumerator Fire_Check()
    {
        while (true)
        {
            if (m_Center_Particle.time >= m_Center_Particle.main.duration) // 불꽃 연출이 끝났다면
            {
                Return_To_Pool(); // 폭탄을 풀로 돌려보낸다.
            }
            else if (!m_is_Fire_Life_Over && m_Center_Particle.time >= m_Center_Particle.main.duration * 0.6f)
            {
                Fire_Collider_OFF();
                m_is_Fire_Life_Over = true;
            }
            yield return null;
        }
    }
}
