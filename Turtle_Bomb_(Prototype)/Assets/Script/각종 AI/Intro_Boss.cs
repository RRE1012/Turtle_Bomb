using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_Boss : MonoBehaviour {
    float m_move_Speed = 40.0f;

    bool move1 = true;
    float move1_Time = 2.0f;
    bool move2 = false;
    float move2_Time = 2.0f;
    bool move3 = false;
    float move3_Time = 2.0f;
    bool move4 = true;
    float move4_Time = 2.0f;
    bool move5 = false;
    float move5_Time = 2.0f;

    float tempTime = 0.0f;
    Vector3 pos;

    void Update ()
    {
        if (move1)
        {
            if (tempTime < move1_Time)
            {
                transform.Translate(new Vector3(0.0f, 0.0f, m_move_Speed/2 * Time.deltaTime));
                tempTime += Time.deltaTime;
            }
            else
            {
                move1 = false;
                move2 = true;
                tempTime = 0.0f;
                pos.x = -4.0f; pos.y = 5.0f; pos.z = 90.0f;

                transform.position = pos;
                transform.localEulerAngles = new Vector3(0.0f, 135.0f, 0.0f);
            }
        }

        else if (move2)
        {
            if (tempTime < move2_Time)
            {
                transform.Translate(new Vector3(0.0f, 0.0f, m_move_Speed * Time.deltaTime));
                tempTime += Time.deltaTime;
            }
            else
            {
                move2 = false;
                move3 = true;
                tempTime = 0.0f;
                pos.x = 32.0f; pos.y = 5.0f; pos.z = 54.0f;

                transform.position = pos;
                transform.localEulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
            }
        }

        else if (move3)
        {
            if (tempTime < move3_Time)
            {
                transform.Translate(new Vector3(0.0f, 0.0f, m_move_Speed * Time.deltaTime));
                tempTime += Time.deltaTime;
            }
            else
            {
                move3 = false;
                move4 = true;
                tempTime = 0.0f;
                pos.x = -4.0f; pos.y = 5.0f; pos.z = 54.0f;

                transform.position = pos;
                transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
            }
        }

        else if (move4)
        {
            if (tempTime < move4_Time)
            {
                transform.Translate(new Vector3(0.0f, 0.0f, m_move_Speed * Time.deltaTime));
                tempTime += Time.deltaTime;
            }
            else
            {
                move4 = false;
                move5 = true;
                tempTime = 0.0f;
                pos.x = 15.0f; pos.y = 8.0f; pos.z = 40.0f;

                transform.position = pos;
                transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        else if (move5)
        {
            if (tempTime < move5_Time)
            {
                transform.Translate(new Vector3(0.0f, 0.0f, m_move_Speed * Time.deltaTime));
                transform.Rotate(new Vector3(0.0f, -90.0f * Time.deltaTime, 0.0f));
                tempTime += Time.deltaTime;
            }
            else
            {
                move5 = false;
            }
        }

        if (StageManager.c_Stage_Manager.Get_is_Intro_Over())
        {
            Destroy(gameObject);
        }
    }
}
