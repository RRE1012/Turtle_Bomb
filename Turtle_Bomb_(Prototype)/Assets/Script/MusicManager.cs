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
    //게임 씬에서의 bgm 오디오소스
    public AudioSource bgmSource;
    //게임 씬에서의 효과음 오디오소스
    AudioSource source;
    //게임오버 여부 판단 변수
    bool gameover;
    //시간 측정 변수
    float deltatime;
    
    //인스턴싱
    void Awake()
    {
        manage_ESound = this;
    }
    void Start () {
        source = GetComponent<AudioSource>();
        gameover = false;
        deltatime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        //게임오버 시 게임 씬 브금 서서히 감소
        if (gameover)
        {
            deltatime = deltatime + (Time.deltaTime);
            bgmSource.volume = 1.0f - deltatime / 10;
        }
    }
    //폭탄 폭발 사운드
    public void soundE()
    {
        source.PlayOneShot(explodeSound,1.0f);
    }
    //아이템 획득 사운드
    public void ItemGetSound()
    {
        source.PlayOneShot(itemGetSound,1.0f);
        //AudioSource.PlayClipAtPoint(itemGetSound, transform.position, 1.0f);
    }
    //폭탄 설치 사운드
    public void BombSetSound()
    {
        source.PlayOneShot(bombSetSound);
    }
    //게임오버를 참으로 변환
    public void TryMute()
    {
        gameover = true;
    }
}
