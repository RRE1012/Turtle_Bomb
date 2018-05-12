using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//화염 이펙트 클래스
public class Fire_Effect : MonoBehaviour {
    //화염 지속시간-  다만 이펙트 이기 때문에 파티클은 사라지지 않을 수 있으므로 값을 조종하려면 파티클 inspector에서 조정 -R

    float FlameLifeTime;

    void Start()
    {
        FlameLifeTime = 0.7f;
    }

    void Update ()
    {
        if (!StageManager.c_Stage_Manager.Get_is_Pause())
        {
            if (FlameLifeTime >= 0.0f)
            {
                FlameLifeTime -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
                //StageManager.c_Stage_Manager.Set_Off_Fire_Range(StageManager.c_Stage_Manager.Find_Own_MCL_Index(gameObject.transform.position.x, gameObject.transform.position.z)); // 범위도 꺼준다.
            }
        }
    }

    public void ResetLifeTime()
    {
        FlameLifeTime = 0.7f;
    }
}
