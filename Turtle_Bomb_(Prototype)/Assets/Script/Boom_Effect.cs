﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom_Effect : MonoBehaviour {

    float m_TotalLifeTime = 3.0f;
    float m_TimeCount = 0.0f;
    	
	void Update ()
    {
        if (!StageManager.c_Stage_Manager.m_is_Pause)
        {
            if (m_TimeCount < m_TotalLifeTime)
            {
                m_TimeCount += Time.deltaTime;
            }
            else
                Destroy(gameObject);
        }
    }
}
