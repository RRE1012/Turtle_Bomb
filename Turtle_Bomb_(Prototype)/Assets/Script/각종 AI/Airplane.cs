using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{

    public GameObject Airdrop_Item;

    float m_Next_Drop_Time;
    float m_Curr_dropTime = 0.0f;
    float m_Appear_Time = 10.0f;
    float m_Curr_appearTime = 0.0f;
    float m_Moving_Speed = 80.0f;

    void Start()
    {
        // 0. 다음 드랍시간을 결정.
        m_Next_Drop_Time = 1.0f;
    }

    void Update()
    {
        // 인트로가 끝났고, 일시정지 상태가 아닌경우에만 수행
        if (StageManager.c_Stage_Manager.m_is_Intro_Over && !StageManager.c_Stage_Manager.m_is_Pause)
        {
            // 2. 등장시간이 되면
            if (m_Curr_appearTime > m_Appear_Time)
            {
                // 3. 전진한다.
                transform.Translate(new Vector3(0.0f, 0.0f, m_Moving_Speed * Time.deltaTime));

                // 5. 떨굴 시간이 되면
                if (m_Curr_dropTime > m_Next_Drop_Time)
                {
                    Vector3 pos;
                    pos.x = Random.Range(0.0f, 28.0f);
                    pos.y = 15.0f;
                    pos.z = Random.Range(50.0f, 78.0f);

                    // 6. 아이템을 생성한다.
                    Instantiate(Airdrop_Item).transform.position = pos;

                    // 7. 일단 드랍 시간도 초기화한다.
                    m_Curr_dropTime = 0.0f;
                }

                // 4. 떨굴 시간을 잰다.
                else
                    m_Curr_dropTime += Time.deltaTime;


                // 8. 일정거리 전진하면
                if (transform.position.z >= 120.0f)
                {
                    // 9. 완전 초기화한다.
                    m_Curr_appearTime = 0.0f;
                    m_Next_Drop_Time = 1.0f; //Random.Range(1.0f, 2.0f);
                    m_Curr_dropTime = 0.0f;

                    Vector3 pos;
                    pos.x = 20.0f;
                    pos.y = 20.0f;
                    pos.z = -20.0f;
                    transform.position = pos;
                }
            }

            // 1. 등장 시간을 잰다.
            else
                m_Curr_appearTime += Time.deltaTime;
        }
    }
}
