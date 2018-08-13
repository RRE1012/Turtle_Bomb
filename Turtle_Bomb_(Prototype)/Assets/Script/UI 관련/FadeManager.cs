using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour {
    
    // 인스턴스로 사용
    public static FadeManager instanceFM;

    // 페이드아웃/인 용 이미지
    public Image fadeImage;

    // 페이드 아웃/인 중인지 판단 여부
    private bool isInTransition;

    // 변환 시간 중
    private float transition;

    // 보이게 할지 안 할지(isInTransition이 참일 경우 -  이 변수가 참이면 화면이 검게, 거짓일 경우 다시 전환) 
    private bool isShowing;

    // 변환하는 시간
    private float duration;
    
    // 페이드아웃 함수
    public void FadeOut()
    {
        isShowing = true;
        isInTransition = true;
        this.duration = 4.0f;
        transition = 0.0f;
    }

    // 페이드인 함수
    public void FadeIn()
    {
        isShowing = false;
        isInTransition = true;
        this.duration = 4.0f;
        transition = 1.0f;
    }

    // 게임오버 버튼 활성화
    void Activate_GameOver_Buttons()
    {
        //UI.GameOver_Button_Activate();
    }
    
    void Awake()
    {
        //인스턴싱
        instanceFM = this;
    }

    void Update()
    {

        //캐릭터가 죽고, 변환중이 아니라면 페이드아웃+페이드인+카메라애니메이션 진행
        if (PlayerMove.C_PM != null && !PlayerMove.C_PM.Get_IsAlive() && !isInTransition)
        {
            UI.Ingame_Play_UI_Deactivate();
            UI.Option_UI_Deactivate();
            FadeOut();
            Invoke("Activate_GameOver_Buttons", 4.0f);
            PlayerMove.C_PM.MakeGameOverAni();
        }

        //값 제한 - 1초과가 되거나 0미만일 경우 변환 종료
        if (transition >= 1.0f)
        {
            transition = 1.0f;
        }
        //페이드아웃일 경우 양수로, 페이드인일 경우 음수로 값을 더해줌(duration의 시간만큼 진행)
        else transition += (isShowing) ? Time.deltaTime * (1 / duration) : -Time.deltaTime * (1 / duration);
        
        //이미지 색+투명도 변환
        fadeImage.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, transition);
    }
}

