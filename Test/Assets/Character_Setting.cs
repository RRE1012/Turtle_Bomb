using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Setting : MonoBehaviour {
    int bombindex_X = 0;
    int bombindex_Z = 0;
    float bombLocX = 0.0f;
    float bombLocZ = 0.0f;
    public float speed = 5.0f;
    // Use this for initialization
    void Start () {
		
	}
    public void MoveRight() { transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f)); }
    public void MoveLeft() { transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f)); }
    public void MoveDown() { transform.Translate(new Vector3(0.0f, 0.0f, -speed * Time.deltaTime)); }
    public void MoveUp() { transform.Translate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime)); }

    void Move() // 플레이어 이동 및 회전
    {

        if(Input.GetKey(KeyCode.W)) transform.Translate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime));
        if (Input.GetKey(KeyCode.S)) transform.Translate(new Vector3(0.0f, 0.0f, -speed * Time.deltaTime));
        if (Input.GetKey(KeyCode.A)) transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f));
        if (Input.GetKey(KeyCode.D)) transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f));
    }
    // Update is called once per frame
    void Update () {
		
	}
    void SetBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
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
            GameObject obj = Object_Pool.obj_pool.GetPoolObject_Bomb();
            if (obj == null)
                return;
            obj.transform.position = new Vector3(bombLocX, 0.5f, bombLocZ);
            obj.transform.rotation = transform.rotation;
            obj.SetActive(true);
        }
    }
    void FixedUpdate()
    {
        Move();
    }
    void LateUpdate()
    {
        SetBomb();
    }
}
