using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static class NOTICE_NUMBER
{
    public const int AIR_DROP = 0;
    public const int DANGER = 1;
    public const int BOSS_INTRO_1_DANGER = 2;
}

public class Notice_UI : MonoBehaviour
{
    static Notice_UI m_Instance;

    Animation m_Animations;

    Notice_Sound m_Notice_Sound;

    public RawImage m_Child_Image;
    public Texture m_AirDrop_Image_Font;
    public Texture m_Danger_Image_Font;

    void Awake()
    {
        m_Instance = this;
        m_Animations = GetComponent<Animation>();
        m_Notice_Sound = GetComponentInChildren<Notice_Sound>();
    }

    public static Notice_UI GetInstance()
    {
        return m_Instance;
    }

    public void Notice_Play(int num)
    {
        switch(num)
        {
            case NOTICE_NUMBER.AIR_DROP:
                m_Child_Image.texture = m_AirDrop_Image_Font;
                m_Animations.Play(m_Animations.GetClip("Air_Drop").name);
                break;

            case NOTICE_NUMBER.DANGER:
                m_Child_Image.texture = m_Danger_Image_Font;
                m_Animations.Play(m_Animations.GetClip("Danger").name);
                break;

            case NOTICE_NUMBER.BOSS_INTRO_1_DANGER:
                m_Child_Image.texture = m_Danger_Image_Font;
                m_Animations.Play(m_Animations.GetClip("Boss_Intro_1_Danger").name);
                Invoke("RedAlert_Sound", 0.9f);
                break;
        }
    }

    void RedAlert_Sound() { m_Notice_Sound.Play_RedAlertSound(); }
}
