using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//타이틀 화면~스테이지선택까지 사운드 출력 함수
public class LobbySound : MonoBehaviour {
    //인스턴스용
    public static LobbySound instanceLS;
    //오디오소스 변수
    AudioSource audioSource;
   
    //인스턴싱+파괴하지 않음 설정
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        instanceLS = this;
        DontDestroyOnLoad(this);
    }
    
    //게임씬 들어갈 시 음악 정지
    public void SoundStop()
    {
        audioSource.Stop();
    }
    //다시 음악 시작
    public void SoundStart()
    {
        audioSource.Play();
    } 
}
