using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    public GameObject Boomb;
    GameObject B;
    public float speed;
    public float minX = -360.0f;
    public float maxX = 360.0f;

    public float minY = -45.0f;
    public float maxY = 45.0f;

    public float sensX = 100.0f;
    public float sensY = 100.0f;

    float rotationY = 0.0f;
    float rotationX = 0.0f;

    float bombLocX = 0.0f;
    float bombLocZ = 0.0f;

    int bombindex_X = 0;
    int bombindex_Z = 0;


    // ======== Functions ========

    void FixedUpdate () {
        Move();
    }

    void LateUpdate()
    {
        SetBomb();
    }




    // ======== UDF =========

    void Move() // 플레이어 이동 및 회전
    {
        if (Input.GetKey(KeyCode.W)) transform.Translate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime));
        if (Input.GetKey(KeyCode.S)) transform.Translate(new Vector3(0.0f, 0.0f, -speed * Time.deltaTime));
        if (Input.GetKey(KeyCode.A)) transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f));
        if (Input.GetKey(KeyCode.D)) transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f));

        if (Input.GetMouseButton(0))
        {
            rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
            //rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0.0f);
        }
    }

    void SetBomb() // 폭탄 설치
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            B = Instantiate(Boomb);

            bombindex_X = (int)transform.position.x;
            bombindex_Z = (int)transform.position.z;

            if (bombindex_X % 2 == 1)
            {
                bombLocX = bombindex_X + 1.0f;
            }
            else if (bombindex_X % 2 == -1)
            {
                bombLocX = bombindex_X - 1.0f;
            }
            else bombLocX = bombindex_X;


            if (bombindex_Z % 2 == 1)
            {
                bombLocZ = bombindex_Z + 1.0f;
            }
            else if (bombindex_Z % 2 == -1)
            {
                bombLocZ = bombindex_Z - 1.0f;
            }
            else bombLocZ = bombindex_Z;

            Debug.Log(bombLocX);
            Debug.Log(bombLocZ);
            B.transform.position = new Vector3(bombLocX, 0.5f, bombLocZ);
        }
    }
}
