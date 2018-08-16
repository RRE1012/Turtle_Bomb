using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//게임씬에서의 사운드 조정 함수
public class MusicManager : MonoBehaviour {

    //인스턴스로 사용
    public static MusicManager manage_ESound;

    //아이템 획득, 폭발사운드, 폭탄설치사운드
    public AudioClip itemGetSound;
    public AudioClip explodeSound;
    public AudioClip bombSetSound;

    public AudioClip m_Goblin_Idle_Sound;
    public AudioClip m_Goblin_Attack_Sound;
    public AudioClip m_Goblin_Dead_Sound;
    public AudioClip m_Boss_Goblin_Throw_Sound;
    public AudioClip m_Boss_Goblin_Fall_Sound;
    public AudioClip m_Boss_Goblin_Dead_Sound;
    public AudioClip m_Boss_Goblin_Hurt_Sound;
    public AudioClip m_Boss_Goblin_Wall_Crush_Sound;
    

    //게임 씬에서의 bgm 오디오소스
    public AudioSource bgmSource;

    //게임 씬에서의 효과음 오디오소스
    AudioSource source;

    //시간 측정 변수
    float deltatime;

    bool gameover = false;
    
    bool is_SE_Mute = false;
    bool is_Vib_Mute = false;

    void Awake()
    {
        manage_ESound = this;
        source = GetComponent<AudioSource>();
        deltatime = 0.0f;

        if (PlayerPrefs.GetInt("System_Option_BGM_ON") == 0)
            bgmSource.Stop();
        if (PlayerPrefs.GetInt("System_Option_SE_ON") == 0)
            is_SE_Mute = true;
        if (PlayerPrefs.GetInt("System_Option_Vib_ON") == 0)
            is_Vib_Mute = true;
    }


    void Update () {
        //게임오버 시 게임 씬 브금 서서히 감소
        if (gameover)
        {
            deltatime = deltatime + (Time.deltaTime);
            bgmSource.volume = 1.0f - deltatime / 10;
            source = FindObjectOfType<AudioSource>();
            source.volume = 1.0f - deltatime / 10;
        }
    }

    //폭탄 폭발 사운드
    public void soundE()
    {
        if (!is_SE_Mute) source.PlayOneShot(explodeSound, 1.0f);
        if (!is_Vib_Mute)
        {
#if UNITY_ANDROID
            Handheld.Vibrate(); // 1초간 진동
#endif
        }
    }
    public void soundE2()
    {
        if (!is_SE_Mute) bgmSource.PlayOneShot(explodeSound, 1.0f);
        if (!is_Vib_Mute)
        {
#if UNITY_ANDROID
            Handheld.Vibrate(); // 1초간 진동
#endif
        } 
    }

    public void TryMute()
    {
        gameover = true;
    }
    
    //아이템 획득 사운드
    public void ItemGetSound()
    {
        if (!is_SE_Mute) source.PlayOneShot(itemGetSound,1.0f);
        //AudioSource.PlayClipAtPoint(itemGetSound, transform.position, 1.0f);
    }
    public void ItemGetSound2()
    {
        bgmSource.PlayOneShot(itemGetSound, 1.0f);
        //AudioSource.PlayClipAtPoint(itemGetSound, transform.position, 1.0f);
    }
    //폭탄 설치 사운드
    public void BombSetSound()
    {
        if (!is_SE_Mute) source.PlayOneShot(bombSetSound);
    }

    public void BombSetSound2()
    {
        bgmSource.PlayOneShot(bombSetSound);
    }
    // 고블린 기본
    public void Goblin_Idle_Sound()
    {
        if (!is_SE_Mute) source.PlayOneShot(m_Goblin_Idle_Sound, 0.6f);
    }

    // 고블린 공격
    public void Goblin_Attack_Sound()
    {
        if (!is_SE_Mute) source.PlayOneShot(m_Goblin_Attack_Sound);
    }

    // 고블린 죽음
    public void Goblin_Dead_Sound()
    {
        if (!is_SE_Mute) source.PlayOneShot(m_Goblin_Dead_Sound, 0.6f);
    }


    // 보스 고블린 폭탄 투척
    public void Boss_Goblin_Throw_Sound()
    {
        if (!is_SE_Mute) source.PlayOneShot(m_Boss_Goblin_Throw_Sound, 0.7f);
    }

    // 보스 고블린 벽 충돌
    public void Boss_Goblin_Wall_Crush_Sound()
    {
        if (!is_SE_Mute) source.PlayOneShot(m_Boss_Goblin_Wall_Crush_Sound, 0.5f);
    }

    // 보스 고블린 추락
    public void Boss_Goblin_Fall_Sound()
    {
        if (!is_SE_Mute) source.PlayOneShot(m_Boss_Goblin_Fall_Sound, 0.5f);
    }

    // 보스 고블린 피격
    public void Boss_Goblin_Hurt_Sound()
    {
        if (!is_SE_Mute) source.PlayOneShot(m_Boss_Goblin_Hurt_Sound);
    }

    // 보스 고블린 죽음
    public void Boss_Goblin_Dead_Sound()
    {
        if (!is_SE_Mute) source.PlayOneShot(m_Boss_Goblin_Dead_Sound);
    }
    
}
