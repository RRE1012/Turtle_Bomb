using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//타이틀 화면~스테이지선택까지 사운드 출력 함수
public class LobbySound : MonoBehaviour
{
    // 인스턴스
    public static LobbySound instanceLS;
    // 오디오소스 변수
    AudioSource audioSource;

    // 인스턴싱+파괴하지 않음 설정
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        instanceLS = this;
        DontDestroyOnLoad(this);
    }
    
    public void SoundStop() // BGM Stop
    {
        audioSource.Stop();
    }
    
    public void SoundStart() // BGM Replay
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
