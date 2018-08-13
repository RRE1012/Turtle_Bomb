using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame_Remains : MonoBehaviour {
    
    float Flame_Remains_LifeTime;
    bool isFlameOnBush;

    void Start()
    {
        Flame_Remains_LifeTime = 1.0f;
        isFlameOnBush = false;
    }

	void Update()
    {
        if (!StageManager.GetInstance().Get_is_Pause())
        {
            if (!isFlameOnBush && Flame_Remains_LifeTime <= 0.0f)
            {
                Destroy(gameObject);
            }
            else Flame_Remains_LifeTime -= Time.deltaTime;
        }
    }
}
