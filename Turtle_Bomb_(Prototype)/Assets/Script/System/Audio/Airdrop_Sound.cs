using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airdrop_Sound : Sound_Effect
{
    public AudioClip m_AudioClips;

    public void Play_AirplaneSound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips);
    }
}
