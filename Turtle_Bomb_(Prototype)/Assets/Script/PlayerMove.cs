using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class MAX_VALUE_ITEM
{
    public const int retval = 8;
    
}

public class PlayerMove : MonoBehaviour {
	public GameObject m_Bomb;
	public GameObject[] m_Fire; 
	public static PlayerMove C_PM; 

    public float m_PlayerSpeed;

    // 회전 감도
    float m_RotateSensX = 150.0f;

    // 회전 각
    float m_RotationX = 0.0f;

    // 폭탄 배치를 위한 위치값
    float m_BombLocX = 0.0f;
    float m_BombLocZ = 0.0f;

    // 폭탄 배치를 위한 인덱스값
    int m_Bombindex_X = 0;
    int m_Bombindex_Z = 0;

    // 플레이어 생존 여부
	bool m_isAlive = true;

    // 플레이어 스탯(status) 3가지
	public int m_FireStat = 2;
	public int m_BombStat = 2;
	public int m_SpeedStat = 2;

    // 남은(배치 가능한) 폭탄 수
	int m_Remain_Bombcount;

    // ======== Functions ========
	void Awake()
	{
        m_Remain_Bombcount = m_BombStat;
		C_PM = this;
	}
    void FixedUpdate () {
        Move();
    }

    void LateUpdate()
    {
        SetBomb();
    }
    //다른 스크립트에서 현 캐릭터의 화력을 Get할 때 쓰이는 함수
	public int GetFire()
	{
		return m_FireStat;
	}
    //다른 스크립트에서 폭탄을 장전할 때 쓰는 함수
	public void ReloadUp()
	{
		UI.m_bomb_count += 1;
	}


    // ======== UDF =========

    void Move() // 플레이어 이동 및 회전
    {
        if (Input.GetKey(KeyCode.W)) transform.Translate(new Vector3(0.0f, 0.0f, m_PlayerSpeed * Time.deltaTime));
        if (Input.GetKey(KeyCode.S)) transform.Translate(new Vector3(0.0f, 0.0f, -m_PlayerSpeed * Time.deltaTime));
        if (Input.GetKey(KeyCode.A)) transform.Translate(new Vector3(-m_PlayerSpeed * Time.deltaTime, 0.0f, 0.0f));
        if (Input.GetKey(KeyCode.D)) transform.Translate(new Vector3(m_PlayerSpeed * Time.deltaTime, 0.0f, 0.0f));

        if (Input.GetMouseButton(0))
        {
            m_RotationX += Input.GetAxis("Mouse X") * m_RotateSensX * Time.deltaTime;
            //rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
            //rotationY = Mathf.Clamp(rotationY, minY, maxY);
            transform.localEulerAngles = new Vector3(0.0f, m_RotationX, 0.0f);
        }
    }

    void SetBomb() // 폭탄 설치
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
			if (UI.m_bomb_count > 0) {
                GameObject Instance_Bomb = Instantiate (m_Bomb);

                m_Bombindex_X = (int)transform.position.x;
                m_Bombindex_Z = (int)transform.position.z;

				if (m_Bombindex_X % 2 == 1) {
                    m_BombLocX = m_Bombindex_X + 1.0f;
				} else if (m_Bombindex_X % 2 == -1) {
                    m_BombLocX = m_Bombindex_X - 1.0f;
				} else
                    m_BombLocX = m_Bombindex_X;


				if (m_Bombindex_Z % 2 == 1) {
                    m_BombLocZ = m_Bombindex_Z + 1.0f;
				} else if (m_Bombindex_Z % 2 == -1) {
                    m_BombLocZ = m_Bombindex_Z - 1.0f;
				} else
                    m_BombLocZ = m_Bombindex_Z;

                //폭탄 위치 수정 -R
                Instance_Bomb.transform.position = new Vector3 (m_BombLocX, -0.2f, m_BombLocZ);
				UI.m_bomb_count = UI.m_bomb_count - 1;
			}
        }
    }

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag("Bomb_Item"))
		{
			Destroy (other.gameObject);
            if(UI.m_bomb_count < MAX_VALUE_ITEM.retval)
            {
                UI.m_bomb_count += 1;
                UI.m_getItemText = "Bomb UP~!";
            }

            
		}
		if (other.gameObject.CompareTag("Fire_Item"))
		{
			Destroy (other.gameObject);
            if (UI.m_fire_count < MAX_VALUE_ITEM.retval)
            {
                UI.m_fire_count += 1;
                UI.m_getItemText = "Fire UP~!";
            }
		}
		if (other.gameObject.CompareTag("Speed_Item"))
		{
			Destroy (other.gameObject);
            if (UI.m_speed_count < MAX_VALUE_ITEM.retval)
            {
                UI.m_speed_count += 1;
                UI.m_getItemText = "Speed UP~!";
            }
		}

        //화염과 접촉 시 사망 -R
        if (other.gameObject.tag == "Fire")
        {
            m_isAlive = false;
        }
    }

    //다른 스크립트에서 플레이어를 죽게 하는 함수-R
    public void Set_Dead()
	{
        m_isAlive = false;
	}

    //살아있는지에 대한 여부를 다른 스크립트에서 get하는 함수-R
    public bool Get_IsAlive()
	{
		return m_isAlive;
	}
}
