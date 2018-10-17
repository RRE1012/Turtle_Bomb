using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class BGM_NUMBER
{
    public const int NORMAL = 0;
    public const int BOSS = 1;
}

public class Audio_Manager : MonoBehaviour
{
    static Audio_Manager m_Instance; public static Audio_Manager GetInstance() { return m_Instance; }

    static bool m_is_Vib_Mute; public bool Get_is_Vibration_Mute() { return m_is_Vib_Mute; }

    AudioSource m_BGM_Audio_Source;

    public AudioClip[] m_BGM_List;

    IEnumerator m_Fadeout;
    IEnumerator m_Fadein;

    float m_FadeSpeed = 0.4f;
    float m_Volume = 1.0f;

    void Awake ()
    {
        m_Instance = this;
        m_BGM_Audio_Source = GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("System_Option_Vib_ON") == 0) m_is_Vib_Mute = true;
        else m_is_Vib_Mute = false;

        if (Sound_Effect.GetInstance() != null)
        {
            if (PlayerPrefs.GetInt("System_Option_SE_ON") == 0)
                Sound_Effect.GetInstance().Set_SE_Mute(true);
            else Sound_Effect.GetInstance().Set_SE_Mute(false);
        }

        m_Fadeout = Sound_Fadeout();
        m_Fadein = Sound_Fadein();
    }

    public void Sound_Fadeout_Start() { StartCoroutine(m_Fadeout); }
    public void Sound_Fadein_Start() { StartCoroutine(m_Fadein); }

    IEnumerator Sound_Fadeout()
    {
        while (true)
        {
            m_Volume = Mathf.Lerp(m_Volume, 0.0f, m_FadeSpeed * Time.deltaTime);
            m_BGM_Audio_Source.volume = m_Volume;
            Sound_Effect.GetInstance().SetVolume(m_Volume);

            if (m_Volume <= 0.01f) StopCoroutine(m_Fadeout);

            yield return null;
        }
    }

    IEnumerator Sound_Fadein()
    {
        while (true)
        {
            m_Volume = Mathf.Lerp(m_Volume, 1.0f, m_FadeSpeed * Time.deltaTime);
            m_BGM_Audio_Source.volume = m_Volume;
            Sound_Effect.GetInstance().SetVolume(m_Volume);

            if (m_Volume >= 0.99f) StopCoroutine(m_Fadein);

            yield return null;
        }
    }

    public void BGM_Play(int bgm_number)
    {
        m_BGM_Audio_Source.Stop();
        m_BGM_Audio_Source.clip = m_BGM_List[bgm_number];
        if (PlayerPrefs.GetInt("System_Option_BGM_ON") != 0) m_BGM_Audio_Source.Play();
    }
}
