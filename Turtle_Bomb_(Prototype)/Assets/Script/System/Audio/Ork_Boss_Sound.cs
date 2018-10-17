using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ORK_BOSS_SOUND_NUMBER
{
    public const int IDLE = 0;
    public const int ATTACK = 1;
    public const int HURT = 2;
    public const int DEAD = 3;
}

public class Ork_Boss_Sound : Sound_Effect
{
    public AudioClip[] m_AudioClips;

    public void Play_IdleSound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips[ORK_BOSS_SOUND_NUMBER.IDLE]);
    }

    public void Play_AttackSound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips[ORK_BOSS_SOUND_NUMBER.ATTACK]);
    }

    public void Play_HurtSound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips[ORK_BOSS_SOUND_NUMBER.HURT]);
    }

    public void Play_DeadSound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips[ORK_BOSS_SOUND_NUMBER.DEAD]);
    }
}
