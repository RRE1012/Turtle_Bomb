using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//폭탄 함수
public class Bomb : MonoBehaviour
{
    private Collider m_BCollider;
    public GameObject m_Flame;
    public GameObject m_Flame_Remains;
    public GameObject[] m_Player;

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
    


    public void MakeExplode()
    {
        m_BombCountDown = -1.0f;
    }

    void Start()
    {
        m_My_MCL_Index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z, false);
        StageManager.Update_MCL_isBlocked(m_My_MCL_Index, true);
        m_Blocked_N = false;
        m_Blocked_S = false;
        m_Blocked_W = false;
        m_Blocked_E = false;
    }

    void Update()
    {
        if (m_BombCountDown > 0.0f)
        {
            m_BombCountDown -= Time.deltaTime;
            BombAnimate(Time.deltaTime);
        }

        //폭발 전 사운드 출력(풀링으로 수정 예정)
        else if (m_BombCountDown <= 0.0f)
        {
            MusicManager.manage_ESound.soundE();
            foreach (GameObject p in m_Player)
            {
                bool in_Range = (p.transform.position.x <= transform.position.x + 6.0f) && (p.transform.position.x >= transform.position.x - 6.0f) && (p.transform.position.z <= transform.position.z + 6.0f) && (p.transform.position.z >= transform.position.z - 6.0f);
                if (in_Range)
                {
                    PlayerMove.C_PM.AniBomb_Start();
                }
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Flame_Remains") || other.gameObject.CompareTag("Flame"))
        {
            // 폭탄 파괴
            Destroy(gameObject);
        }
    }


    void OnDestroy()
    {
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
}