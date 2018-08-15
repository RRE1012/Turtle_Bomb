using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class NOTICE_SOUND_NUMBER
{
    public const int RED_ALERT = 0;
}

public class Notice_Sound : Sound_Effect
{
    public AudioClip[] m_AudioClips;

    public void Play_RedAlertSound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips[NOTICE_SOUND_NUMBER.RED_ALERT]);
    }
}
