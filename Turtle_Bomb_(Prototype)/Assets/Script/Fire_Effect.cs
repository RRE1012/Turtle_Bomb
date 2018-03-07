using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//화염 이펙트 클래스
public class Fire_Effect : MonoBehaviour {
    //화염 지속시간-  다만 이펙트 이기 때문에 파티클은 사라지지 않을 수 있으므로 값을 조종하려면 파티클 inspector에서 조정 -R

    float FlameLifeTime;

    bool is_OnBush = false;

    void Start()
    {
        FlameLifeTime = 0.7f;
    }

    void Update ()
    { 
        if (FlameLifeTime >= 0.0f)
        {
            FlameLifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
