using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    public static Airplane c_Airplane;
    public GameObject Airdrop_Item;

    float m_Next_Drop_Time;
    float m_Curr_dropTime = 0.0f;
    float m_Appear_Time = 10.0f;
    float m_Curr_appearTime = 0.0f;
    float m_Moving_Speed = 80.0f;

    int m_Airdrop_Count = 0;

    bool m_is_Able_to_Appear = false;

    void Start()
    {
        c_Airplane = this;

        // 0. 다음 드랍시간을 결정.
        m_Next_Drop_Time = 1.0f;
    }

    void Update()
    {
        // 일시정지 상태가 아닌 경우에만 수행
        if (!StageManager.c_Stage_Manager.Get_is_Pause())
        {
            // 1. 등장하라는 명령이 떨어지면
            if (m_is_Able_to_Appear)
            {
                // 2. 전진한다.
                transform.Translate(new Vector3(0.0f, 0.0f, m_Moving_Speed * Time.deltaTime));

                // 4. 떨굴 시간이 되면
                if (m_Curr_dropTime > m_Next_Drop_Time)
                {
                    // 5. 아이템을 생성한다.
                    for (int i = 0; i < m_Airdrop_Count; ++i)
                    {
                        int index; // 뿌릴 위치 (인덱스)

                        while (true) // 루프를 돌면서 지형 탐색
                        {
                            index = Random.Range(17, 271); // 맵 범위
                            if (!StageManager.c_Stage_Manager.Get_MCL_index_is_Blocked(index)) // 막혀있지 않으면
                                break; // 탈출
                        }

                        Vector3 pos;
                        pos.x = 0.0f;
                        pos.y = 15.0f;
                        pos.z = 0.0f;
                        StageManager.c_Stage_Manager.Get_MCL_Coordinate(index, ref pos.x, ref pos.z);

                        Instantiate(Airdrop_Item).transform.position = pos; // 설정한 위치에 아이템 투하
                    }

                    // 6. 일단 드랍 시간도 초기화한다.
                    m_Curr_dropTime = 0.0f;
                }

                // 3. 떨굴 시간을 잰다.
                else
                    m_Curr_dropTime += Time.deltaTime;


                // 7. 일정거리 전진하면
                if (transform.position.z >= 120.0f)
                {
                    // 8. 완전 초기화한다.
                    m_Curr_appearTime = 0.0f;
                    m_Next_Drop_Time = 1.0f; //Random.Range(1.0f, 2.0f);
                    m_Curr_dropTime = 0.0f;

                    Vector3 pos;
                    pos.x = 20.0f;
                    pos.y = 20.0f;
                    pos.z = -20.0f;
                    transform.position = pos;

                    UI.c_UI.Set_Elapsed_Time(0.0f); // 경과시간 초기화
                    m_is_Able_to_Appear = false; // 끝
                }
            }
        }
    }


    public void Dispatch_Airplane()
    {
        if (!m_is_Able_to_Appear) Notice_UI.GetInstance().Notice_Play(NOTICE_NUMBER.AIR_DROP);
        m_is_Able_to_Appear = true;
    }

    public void Set_Airdrop_Count(int c)
    {
        m_Airdrop_Count = c;
    }
}
