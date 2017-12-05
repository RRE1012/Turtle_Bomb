using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    private Collider m_BCollider;
	public GameObject m_Flame;
	public GameObject[] m_Character;
	public GameObject[] m_Bomb;
	public GameObject[] m_Box;

    // 불길이 퍼질때 막힌 공간 판별을 위한 boolean
    // 동서남북 (= E, W, S, N)
	bool m_Blocked_N = false;
	bool m_Blocked_S = false;
	bool m_Blocked_W = false;
	bool m_Blocked_E = false;

    // 폭탄 생명주기
    private float m_BombCountDown = 3.0f;

	public void MakeExplode()
	{
        m_BombCountDown = -1.0f;
	}
    void Update()
    {

        m_Character = GameObject.FindGameObjectsWithTag("Player");
        m_Bomb = GameObject.FindGameObjectsWithTag("Bomb");
        m_Box = GameObject.FindGameObjectsWithTag("Box");
        
        if (m_BombCountDown >= 0.0f)
        {
            m_BombCountDown -= Time.deltaTime;
        }

		else if(m_BombCountDown < 0.0f)
        {
            Destroy(gameObject);
        }
    }
	private void OnDestroy()
	{
        //폭탄 터질 때 화염 이펙트 생성, 그 중에서 만약 박스가 있다면 관통하지 않고 그 박스만 부수도록 설정 -R

        GameObject Instance_Flame = Instantiate(m_Flame);

        Instance_Flame.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z);

		for (int i = 0; i < UI.m_fire_count; ++i)
		{
			GameObject Instance_FlameDir_N = Instantiate(m_Flame);
			GameObject Instance_FlameDir_S = Instantiate(m_Flame);
			GameObject Instance_FlameDir_W = Instantiate(m_Flame);
			GameObject Instance_FlameDir_E = Instantiate(m_Flame);

            if (!m_Blocked_N)
                Instance_FlameDir_N.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z + (2.0f * (i + 1)));
            if (!m_Blocked_S)
                Instance_FlameDir_S.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z - (2.0f * (i + 1)));
            if (!m_Blocked_W)
                Instance_FlameDir_W.transform.position = new Vector3(gameObject.transform.position.x - (2.0f * (i + 1)), 0.0f, gameObject.transform.position.z);
            if (!m_Blocked_E)
                Instance_FlameDir_E.transform.position = new Vector3(gameObject.transform.position.x + (2.0f * (i + 1)), 0.0f, gameObject.transform.position.z);
			
            // 화염과 박스의 충돌
			foreach (GameObject box in m_Box)
			{
                if (transform.position.x == box.transform.position.x && transform.position.z + (2.0f * (i + 1)) == box.transform.position.z)
                {
                    m_Blocked_N = true;
                }
                if (transform.position.x == box.transform.position.x && transform.position.z - (2.0f * (i + 1)) == box.transform.position.z)
                {
                    m_Blocked_S = true;
                }
                if (transform.position.x - (2.0f * (i + 1)) == box.transform.position.x && transform.position.z == box.transform.position.z)
                {
                    m_Blocked_W = true;
                }
                if (transform.position.x + (2.0f * (i + 1)) == box.transform.position.x && transform.position.z == box.transform.position.z)
				{
                    m_Blocked_E = true;
				}
			}

            // 화염과 폭탄의 충돌
			foreach (GameObject bomb in m_Bomb)
			{
				if (bomb != null)
				{
                    if (transform.position.x == bomb.transform.position.x && transform.position.z + (2.0f * (i + 1)) == bomb.transform.position.z)
                    {
                        Destroy(bomb);
                        m_Blocked_N = true;
                        break;
                    }
                    if (transform.position.x == bomb.transform.position.x && transform.position.z - (2.0f * (i + 1)) == bomb.transform.position.z)
                    {
                        Destroy(bomb);
                        m_Blocked_S = true;
                        break;
                    }
                    if (transform.position.x - (2.0f * (i + 1)) == bomb.transform.position.x && transform.position.z == bomb.transform.position.z)
                    {
                        Destroy(bomb);
                        m_Blocked_W = true;
                        break;
                    }
                    if (transform.position.x + (2.0f * (i + 1)) == bomb.transform.position.x && transform.position.z == bomb.transform.position.z)
					{
						Destroy(bomb);
                        m_Blocked_E = true;
						break;
					}
				}
			}
		}

        //폭탄 수 다시 증가
		PlayerMove.C_PM.ReloadUp();
	}

    private void OnTriggerExit(Collider other)
    {
        if (m_BCollider.isTrigger && other.tag == "Player")
        {
            m_BCollider.isTrigger = false;
        }
    }
}
