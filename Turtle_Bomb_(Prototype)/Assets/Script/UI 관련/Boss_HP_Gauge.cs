using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_HP_Gauge : MonoBehaviour
{
    static Boss_HP_Gauge m_Instance; public static Boss_HP_Gauge GetInstance() { return m_Instance; }
    Scrollbar m_HP_Gauge;
    float m_Prev_Gauge_Size;
    float m_Curr_HP;
    float m_Max_HP;
    float m_HP_Decrease_Speed = 2.0f;
    Animation m_Animations;
    Animation m_Child_Animations;

    bool m_is_Angry_Mode_On = false;
    IEnumerator m_Decrease_HP;
    IEnumerator m_Angry_Mode;

	void Start ()
    {
        m_Instance = this;
        m_HP_Gauge = GetComponent<Scrollbar>();
        m_Animations = GetComponent<Animation>();
        m_Child_Animations = GetComponentsInChildren<Animation>()[1];

        m_Angry_Mode = AngryMode();
        m_Decrease_HP = Decrease_Gauge();
        StartCoroutine(Decrease_Gauge());
    }

    IEnumerator AngryMode()
    {
        while(true)
        {
            m_Animations.Play(m_Animations.GetClip("Angry_Mode").name);
            yield return null;
        }
    }
	
    IEnumerator Decrease_Gauge()
    {
        while (true)
        {
            m_Prev_Gauge_Size = m_HP_Gauge.size;
            m_HP_Gauge.size = Mathf.Lerp(m_HP_Gauge.size, m_Curr_HP / m_Max_HP, Time.deltaTime * 1.2f);

            if (!m_is_Angry_Mode_On && m_Prev_Gauge_Size > m_HP_Gauge.size + 0.0005f) m_Child_Animations.Play(m_Child_Animations.GetClip("Gauge_Twinkle").name);
            yield return null;
        }
    }

    public void Set_Max_HP(float max) { m_Max_HP = max; m_Curr_HP = m_Max_HP; m_HP_Gauge.size = 1.0f; }

    public void Set_Curr_HP(int curr_HP) { m_Curr_HP = curr_HP; }

    public void Start_Angry_Mode_Gauge() { StartCoroutine(m_Angry_Mode); m_is_Angry_Mode_On = true; }

    public void Stop_Angry_Mode_Gauge() { StopCoroutine(m_Angry_Mode); }

    public void Dead_Mode_Gague_Play()
    {
        StopCoroutine(m_Angry_Mode);
        m_Animations.Play(m_Animations.GetClip("Dead_Mode").name);
    }
}
