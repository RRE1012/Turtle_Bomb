using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle_Bottom : MonoBehaviour {
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            other.GetComponent<Box>().Save_Icicle_Bottom_Collider(gameObject);
            transform.parent.GetComponent<Icicle>().Icicle_Behavior_Stop();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            other.GetComponent<Box>().Clear_Icicle_Bottom_Collider();
            transform.parent.GetComponent<Icicle>().Icicle_Behavior_Start();
        }
    }

    public void TriggerExit_Ver2()
    {
        transform.parent.GetComponent<Icicle>().Icicle_Behavior_Start();
    }
}
