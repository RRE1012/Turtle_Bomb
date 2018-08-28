using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_Boss_1 : MonoBehaviour {

    bool sensored = false;
    bool cooltime = false;
    // Use this for initialization
    void Start () {
        sensored = false;
        StartCoroutine("SendTester");
    }
    void OnTriggerEnter(Collider other)
    {
        if (gameObject.activeInHierarchy && !Performance_Network.instance.ani_is_working)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (Turtle_Move_Coop.instance.alive != 0)
                {
                    if(!cooltime)
                        sensored = true;
                    //Debug.Log("Sensored");
                }
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
       
        if (other.gameObject.CompareTag("Player"))
        {
            if (Turtle_Move_Coop.instance.alive != 0)
            {
                if (!cooltime)
                    sensored = true;

            }
        }
        // ===================

    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            sensored = false;
        }
    }
    void CoolTimeOff()
    {
        cooltime = false;
    }
    // Update is called once per frame
    IEnumerator SendTester()
    {

        for (; ; )
        {
            if (sensored)
            {
                NetManager_Coop.instance.SendBossPacket(VariableManager_Coop.instance.pos_id);
                sensored = false;
                cooltime = true;
                //Debug.Log("Send BossPacket");
                Invoke("CoolTimeOff", 2.0f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

}
