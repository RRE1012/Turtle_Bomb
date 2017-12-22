using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//폭탄 함수
public class Bomb : MonoBehaviour {
    private Collider m_BCollider;
    public GameObject m_Flame;
    public GameObject m_Flame_Remains;
    public GameObject[] m_Character;
	public GameObject[] m_Bomb;
	public GameObject[] m_Box;
    public GameObject[] m_BombItem;
    public GameObject[] m_FireItem;
    public GameObject[] m_SpeedItem;
    public GameObject[] m_Rock;

    int m_FlameCount = UI.m_fire_count;

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
        m_BombItem = GameObject.FindGameObjectsWithTag("Fire_Item");
        m_FireItem = GameObject.FindGameObjectsWithTag("Bomb_Item");
        m_SpeedItem = GameObject.FindGameObjectsWithTag("Speed_Item");
        m_Rock = GameObject.FindGameObjectsWithTag("Rock");

        if (m_BombCountDown > 0.0f)
        {
            m_BombCountDown -= Time.deltaTime;
        }

        //폭발 전 사운드 출력(풀링으로 수정 예정)
		else if(m_BombCountDown <= 0.0f)
        {
            MusicManager.manage_ESound.soundE();
            Destroy(gameObject);
        }
    }

	private void OnDestroy()
	{
        //폭탄 터질 때 화염 이펙트 생성, 그 중에서 만약 박스가 있다면 관통하지 않고 그 박스만 부수도록 설정 -R
       
        GameObject Instance_Flame = Instantiate(m_Flame);
        
        Instance_Flame.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z);

        for (int i = 0; i < m_FlameCount; ++i)
        {
            GameObject Instance_FlameDir_N;
            GameObject Instance_FlameDir_S;
            GameObject Instance_FlameDir_W;
            GameObject Instance_FlameDir_E;

            // 화염과 박스의 충돌
            foreach (GameObject box in m_Box)
            {
                if (box != null)
                {
                    if (transform.position.x == box.transform.position.x && transform.position.z + (2.0f * (i + 1)) == box.transform.position.z)
                    {
                        GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                        Instance_Flame_Remains.transform.position = new Vector3(box.transform.position.x, 0.0f, box.transform.position.z);

                        m_Blocked_N = true;
                    }
                    if (transform.position.x == box.transform.position.x && transform.position.z - (2.0f * (i + 1)) == box.transform.position.z)
                    {
                        GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                        Instance_Flame_Remains.transform.position = new Vector3(box.transform.position.x, 0.0f, box.transform.position.z);

                        m_Blocked_S = true;
                    }
                    if (transform.position.x - (2.0f * (i + 1)) == box.transform.position.x && transform.position.z == box.transform.position.z)
                    {
                        GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                        Instance_Flame_Remains.transform.position = new Vector3(box.transform.position.x, 0.0f, box.transform.position.z);

                        m_Blocked_W = true;
                    }
                    if (transform.position.x + (2.0f * (i + 1)) == box.transform.position.x && transform.position.z == box.transform.position.z)
                    {
                        GameObject Instance_Flame_Remains = Instantiate(m_Flame_Remains);
                        Instance_Flame_Remains.transform.position = new Vector3(box.transform.position.x, 0.0f, box.transform.position.z);

                        m_Blocked_E = true;
                    }
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

            // 화염과 안터지는 박스(돌)의 충돌
            foreach (GameObject rock in m_Rock)
            {
                if (rock != null)
                {
                    if (!m_Blocked_N && transform.position.x == rock.transform.position.x && transform.position.z + (2.0f * (i + 1)) == rock.transform.position.z)
                    {
                        m_Blocked_N = true;
                        break;
                    }
                    if (!m_Blocked_S && transform.position.x == rock.transform.position.x && transform.position.z - (2.0f * (i + 1)) == rock.transform.position.z)
                    {
                        m_Blocked_S = true;
                        break;
                    }
                    if (!m_Blocked_W && transform.position.x - (2.0f * (i + 1)) == rock.transform.position.x && transform.position.z == rock.transform.position.z)
                    {
                        m_Blocked_W = true;
                        break;
                    }
                    if (!m_Blocked_E && transform.position.x + (2.0f * (i + 1)) == rock.transform.position.x && transform.position.z == rock.transform.position.z)
                    {
                        m_Blocked_E = true;
                        break;
                    }
                }
            }

            // 화염과 아이템들의 충돌
            foreach (GameObject bombitem in m_BombItem)
            {
                if (bombitem != null)
                {
                    if (!m_Blocked_N && transform.position.x == bombitem.transform.position.x && transform.position.z + (2.0f * (i + 1)) == bombitem.transform.position.z)
                    {
                        Destroy(bombitem);
                        break;
                    }
                    if (!m_Blocked_S && transform.position.x == bombitem.transform.position.x && transform.position.z - (2.0f * (i + 1)) == bombitem.transform.position.z)
                    {
                        Destroy(bombitem);
                        break;
                    }
                    if (!m_Blocked_W && transform.position.x - (2.0f * (i + 1)) == bombitem.transform.position.x && transform.position.z == bombitem.transform.position.z)
                    {
                        Destroy(bombitem);
                        break;
                    }
                    if (!m_Blocked_E && transform.position.x + (2.0f * (i + 1)) == bombitem.transform.position.x && transform.position.z == bombitem.transform.position.z)
                    {
                        Destroy(bombitem);
                        break;
                    }
                }
            }
            foreach (GameObject fireitem in m_FireItem)
            {
                if (fireitem != null)
                {
                    if (!m_Blocked_N && transform.position.x == fireitem.transform.position.x && transform.position.z + (2.0f * (i + 1)) == fireitem.transform.position.z)
                    {
                        Destroy(fireitem);
                        break;
                    }
                    if (!m_Blocked_S && transform.position.x == fireitem.transform.position.x && transform.position.z - (2.0f * (i + 1)) == fireitem.transform.position.z)
                    {
                        Destroy(fireitem);
                        break;
                    }
                    if (!m_Blocked_W && transform.position.x - (2.0f * (i + 1)) == fireitem.transform.position.x && transform.position.z == fireitem.transform.position.z)
                    {
                        Destroy(fireitem);
                        break;
                    }
                    if (!m_Blocked_E && transform.position.x + (2.0f * (i + 1)) == fireitem.transform.position.x && transform.position.z == fireitem.transform.position.z)
                    {
                        Destroy(fireitem);
                        break;
                    }
                }
            }

            foreach (GameObject speeditem in m_SpeedItem)
            {
                if (speeditem != null)
                {
                    if (!m_Blocked_N && transform.position.x == speeditem.transform.position.x && transform.position.z + (2.0f * (i + 1)) == speeditem.transform.position.z)
                    {
                        Destroy(speeditem);
                        break;
                    }
                    if (!m_Blocked_S && transform.position.x == speeditem.transform.position.x && transform.position.z - (2.0f * (i + 1)) == speeditem.transform.position.z)
                    {
                        Destroy(speeditem);
                        break;
                    }
                    if (!m_Blocked_W && transform.position.x - (2.0f * (i + 1)) == speeditem.transform.position.x && transform.position.z == speeditem.transform.position.z)
                    {
                        Destroy(speeditem);
                        break;
                    }
                    if (!m_Blocked_E && transform.position.x + (2.0f * (i + 1)) == speeditem.transform.position.x && transform.position.z == speeditem.transform.position.z)
                    {
                        Destroy(speeditem);
                        break;
                    }
                }
            }

            // 화염 외부로 확장 제어
            if (!m_Blocked_N && transform.position.z + (2.0f * (i + 1)) >= 30.0f)
            {
                m_Blocked_N = true;
            }
            if (!m_Blocked_S && transform.position.z - (2.0f * (i + 1)) <= -2.0f)
            {
                m_Blocked_S = true;
            }
            if (!m_Blocked_W && transform.position.x - (2.0f * (i + 1)) <= -2.0f)
            {
                m_Blocked_W = true;
            }
            if (!m_Blocked_E && transform.position.x + (2.0f * (i + 1)) >= 30.0f)
            {
                m_Blocked_E = true;
            }



            if (!m_Blocked_N)
            {
                Instance_FlameDir_N = Instantiate(m_Flame);
                Instance_FlameDir_N.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z + (2.0f * (i + 1)));
            }

            if (!m_Blocked_S)
            {
                Instance_FlameDir_S = Instantiate(m_Flame);
                Instance_FlameDir_S.transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z - (2.0f * (i + 1)));
            }

            if (!m_Blocked_W)
            {
                Instance_FlameDir_W = Instantiate(m_Flame);
                Instance_FlameDir_W.transform.position = new Vector3(gameObject.transform.position.x - (2.0f * (i + 1)), 0.0f, gameObject.transform.position.z);
            }

            if (!m_Blocked_E)
            {
                Instance_FlameDir_E = Instantiate(m_Flame);
                Instance_FlameDir_E.transform.position = new Vector3(gameObject.transform.position.x + (2.0f * (i + 1)), 0.0f, gameObject.transform.position.z);
            }
        }


        //폭탄 수 다시 증가
		PlayerMove.C_PM.ReloadUp();
	}
}
