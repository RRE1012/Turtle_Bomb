using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Sound : Sound_Effect
{
    public AudioClip m_AudioClips;
    
    public void Play_ExplodeSound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips);
    }
}
