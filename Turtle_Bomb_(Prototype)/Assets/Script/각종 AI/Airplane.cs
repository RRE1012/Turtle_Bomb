using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    static Airplane m_Instance; public static Airplane GetInstance() { return m_Instance; }
    public GameObject Airdrop_Item;

    Animation m_Animations;
    float m_Airdrop_Time; // 테이블로 관리됨. StageManager에서 받아온다.

    int m_Airdrop_Count = 0;

    [HideInInspector]
    public bool m_is_able_to_Drop = false;

    IEnumerator m_Airdrop;
    IEnumerator m_Time_Checker;
    

    void Start()
    {
        m_Instance = this;
        m_Animations = GetComponent<Animation>();

        m_Airdrop = AirDrop();
        m_Time_Checker = AirDrop_Time_Check();

        StartCoroutine(Wait_For_Intro());

        m_Airdrop_Time = StageManager.GetInstance().Get_AirDrop_Time();
    }

    IEnumerator AirDrop()
    {
        while (true)
        {
            if (m_Animations["Air_Drop"].normalizedTime >= 0.8f)
            {
                for (int i = 0; i < m_Airdrop_Count; ++i)
                {
                    int index;

                    while (true) // 루프를 돌면서 지형 탐색
                    {
                        index = Random.Range(17, 271); // 맵 범위
                        if (!StageManager.GetInstance().Get_MCL_index_is_Blocked(index)) // 막혀있지 않으면
                            break; // 탈출
                    }

                    Vector3 pos;
                    pos.x = 0.0f;
                    pos.y = 15.0f;
                    pos.z = 0.0f;
                    StageManager.GetInstance().Get_MCL_Coordinate(index, ref pos.x, ref pos.z);

                    Instantiate(Airdrop_Item).transform.position = pos; // 설정한 위치에 아이템 투하
                }

                StopCoroutine(m_Airdrop); // 에어드랍 종료
            }
            yield return null;
        }
    }


    IEnumerator AirDrop_Time_Check()
    {
        while(true)
        {
            if (UI.GetInstance().Get_Elapsed_Time() >= m_Airdrop_Time)
            {
                Dispatch_Airplane();
            }
            yield return null;
        }
    }

    IEnumerator Wait_For_Intro()
    {
        while(true)
        {
            if (StageManager.GetInstance().Get_is_Intro_Over())
            {
                StopAllCoroutines();
                StartCoroutine(m_Time_Checker);
            }
            yield return null;
        }
    }

    void Dispatch_Airplane()
    {
        Notice_UI.GetInstance().Notice_Play(NOTICE_NUMBER.AIR_DROP);

        StopCoroutine(m_Time_Checker);

        m_Animations.Play(m_Animations.GetClip("Air_Drop").name);
        StartCoroutine(m_Airdrop);
    }

    public void Set_Airdrop_Count(int c)
    {
        m_Airdrop_Count = c;
    }
}
