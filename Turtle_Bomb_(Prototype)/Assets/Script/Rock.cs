using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    int index;
    
    void Start()
    {
        // 최초 시작 시 자신의 위치의 isBlocked를 true로 갱신
        index = StageManager.Find_Own_MCL_Index(transform.position.x, transform.position.z, false);
        StageManager.Update_MCL_isBlocked(index, true);
    }
	
}
