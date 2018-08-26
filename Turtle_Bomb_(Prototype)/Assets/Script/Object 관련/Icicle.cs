using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ICICLE_OFFSET_TIME
{
    public const float ICICLE_1 = 0.0f;
    public const float ICICLE_2 = 2.0f;

    public const float WAIT_TIME = 3.0f;
}

public class Icicle : MonoBehaviour
{
    Animation m_Animations;

    float m_Offset_Time;

    IEnumerator m_Behavior;
    IEnumerator m_Up_Checker;
    IEnumerator m_Down_Checker;

    int m_MCL_Index;

	void Start ()
    {
        m_Animations = GetComponent<Animation>();

        m_Behavior = Behavior();
        m_Up_Checker = Perfectly_Up_Check();
        m_Down_Checker = Perfectly_Down_Check();

        m_MCL_Index = StageManager.GetInstance().Find_Own_MCL_Index(transform.position.x, transform.position.z); // 인덱스 찾기
    }

    void OnDestroy()
    {
        // MCL 갱신
        StageManager.GetInstance().Update_MCL_isBlocked(m_MCL_Index, false);
    }

    public void Start_With_Offset_Time(float time) // 인자로 받은 시간만큼 대기 후 첫 시작.
    {
        Invoke("Icicle_Behavior_Start", time);
    }

    IEnumerator Behavior()
    {
        while(true)
        {
            Icicle_Up();
            yield return new WaitForSeconds(ICICLE_OFFSET_TIME.WAIT_TIME);
            
            Icicle_Down();
            yield return new WaitForSeconds(ICICLE_OFFSET_TIME.WAIT_TIME);
        }
    }

    IEnumerator Perfectly_Up_Check()
    {
        while (true)
        {
            if (m_Animations["icicle_Up"].normalizedTime >= 0.7f) // 어느정도 올라오면
            {
                GetComponentInChildren<BoxCollider>().isTrigger = false; // 트리거를 꺼버린다.

                StopCoroutine(m_Up_Checker); // '올라가는 과정 체크' 일시정지
            }
            yield return null;
        }
    }

    IEnumerator Perfectly_Down_Check()
    {
        while (true)
        {
            if (m_Animations["icicle_Down"].normalizedTime >= 0.9f) // 거의 완전히 내려가면
            {
                StageManager.GetInstance().Update_MCL_isBlocked(m_MCL_Index, false); // 열어줌.
                StopCoroutine(m_Down_Checker); // '완전히 내려갔는지 체크' 일시정지
            }

            else if (m_Animations["icicle_Down"].normalizedTime >= 0.3f) // 어느정도 내려가면
            {
                GetComponentInChildren<BoxCollider>().isTrigger = true; // 트리거를 켜준다.
            }
            yield return null;
        }
    }

    void Icicle_Up()
    {
        m_Animations.Play(m_Animations.GetClip("icicle_Up").name);
        StageManager.GetInstance().Update_MCL_isBlocked(m_MCL_Index, true); // 고드름이 올라가기 시작하면 막아줌.
        StartCoroutine(m_Up_Checker); // '올라가는 과정 체크' 시작
    }
    void Icicle_Down()
    {
        m_Animations.Play(m_Animations.GetClip("icicle_Down").name);
        StartCoroutine(m_Down_Checker); // '완전히 내려갔는지 체크' 시작
    }
    

    public void Icicle_Behavior_Start()
    {
        StartCoroutine(m_Behavior);
    }

    public void Icicle_Behavior_Stop()
    {
        StopCoroutine(m_Behavior);
    }
    
}