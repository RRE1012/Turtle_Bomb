using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {
    Rigidbody rb;
    Vector3 movement = new Vector3(3.0f,0.0f,0.0f);
    Vector3 rotation_item = new Vector3(3.0f, 3.0f, 0.0f);
    Object_Pool obj_pool; //<--이부분
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}
    void OnEnable()
    {
        Invoke("SetUnable", 3.0f);


    }

    void SetUnable()
    {
        gameObject.SetActive(false);
    }
    void OnDisable()
    {
        CancelInvoke();
    }
    
    // Update is called once per frame
    void Update () {

        transform.Rotate(rotation_item);
        //transform.Translate(movement * Time.fixedDeltaTime);
    }
}
