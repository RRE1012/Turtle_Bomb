using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Effect : MonoBehaviour
{
    static Sound_Effect m_Instance; public static Sound_Effect GetInstance() { return m_Instance; }

    protected static bool m_is_SE_Mute; public void Set_SE_Mute(bool b) { m_is_SE_Mute = b; }

    public void SetVolume(float v) { m_AudioSource.volume = v; }

    protected AudioSource m_AudioSource;

    void Awake ()
    {
        m_Instance = this;
        m_AudioSource = GetComponent<AudioSource>();
    }
}
