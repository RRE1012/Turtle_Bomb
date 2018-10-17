using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class PLAYER_SOUND_NUMBER
{
    public const int ITEM_GET = 0;
    public const int BOMB_SET = 1;
    public const int MOVE = 2;
}

public class Player_Sound : Sound_Effect
{
    public AudioClip[] m_AudioClips;
    
    public void Play_Item_Get_Sound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips[PLAYER_SOUND_NUMBER.ITEM_GET]);
    }

    public void Play_Bomb_Set_Sound()
    {
        if (!m_is_SE_Mute) m_AudioSource.PlayOneShot(m_AudioClips[PLAYER_SOUND_NUMBER.BOMB_SET]);
    }

    public void Play_Move_Sound()
    {
        if (!m_is_SE_Mute && !m_AudioSource.isPlaying) m_AudioSource.PlayOneShot(m_AudioClips[PLAYER_SOUND_NUMBER.MOVE]);
    }
}
