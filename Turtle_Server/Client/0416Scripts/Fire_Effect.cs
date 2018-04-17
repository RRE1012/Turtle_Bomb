using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_Effect : MonoBehaviour {

    Collider m_Fire_Collider;

    //화염 지속시간-  다만 이펙트 이기 때문에 파티클은 사라지지 않을 수 있으므로 값을 조종하려면 파티클 inspector에서 조정 -R
    public float bombCountDown = 1.0f;
    void OnTriggerEnter(Collider other)
    {
        if (gameObject.activeInHierarchy)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("TTaGawa");
                if (Turtle_Move.instance.alive != 0)
                {
                    Turtle_Move.instance.alive = 0;
                    NetTest.instance.SetmoveTrue();
                }
            }
        }
    }
    // Update is called once per frame
    void Update () {

        if (bombCountDown >= 0.0f)
        {
            bombCountDown -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
