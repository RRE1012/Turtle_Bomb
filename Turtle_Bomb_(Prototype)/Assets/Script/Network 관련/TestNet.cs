using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNet : MonoBehaviour {
    public GameObject m_bomb;
    GameObject[] bomb_list = new GameObject[14];
    // Use this for initialization
    bool m_is_Rising_Start = true;
    bool m_is_Done_Rising = false;
    bool[] m_is_rising_start = new bool[14];
    bool[] m_is_donerising = new bool[14];
    bool[] is_alive = new bool[14];
    int[] dx = new int[14];
    int[] sx = new int[14];
    int[] sz = new int[14];
    int a = 0;
    int[] dz=new int[14];
    byte[] dirc = new byte[14];

    void Start () {
		for(int i = 0; i < 14; ++i)
        {
            bomb_list[i] = Instantiate(m_bomb);
            bomb_list[i].transform.position = new Vector3(2*i, 0, 0);
            m_is_Rising_Start = true;
            m_is_donerising[i] = false;
            m_is_rising_start[i] = true;
            is_alive[i] = true;
            sx[i] = 2 * i;
            sz[i] = 0;
            dx[i] = 2 * i;
            dz[i] = 2 * i;
            dirc[i] = 3;
            //bomb_list[i].SetActive(false);
        }
	}

    public int GetGameObject2()
    {
        for(int i = 0; i < 14; ++i)
        {
            if (!bomb_list[i].activeInHierarchy)
            {
                return i;
            }
        }
        return -1;
    }
    public int GetGameObject()
    {
        for (int i = 0; i < 14; ++i)
        {
            if (!is_alive[i])
            {
                is_alive[i] = true;
                return i;
            }
        }
        return -1;
    }

    void GetPacket(int x,int z, int x_d,int z_d,byte direction)
    {
        int b = GetGameObject();
        if (b >= 0)
        {
            dirc[b] = direction;
            sx[b] = x;
            sz[b] = z;

            dx[b] = x_d;
            dz[b] = z_d;

            bomb_list[b].transform.position = new Vector3(x, 0.0f, z);
            bomb_list[b].gameObject.SetActive(true);
        }
        
    }
    //겜오브젝트로 받지 말고, 인트로 받아보자
    public void Thrown_Bomb_Move(GameObject gBomb, int x, int z, int x_dest, int z_dest, byte direction)
    {
        //gBomb.gameObject.transform.position = new Vector3(x, 0, z);

        float m_Rising_Limit = 4.0f;
        float m_Down_Limit = 0.0f;
        
       
        float m_Thrown_Bomb_Speed = 10.0f;
        float m_Rising_Speed = 4.0f;
        if (m_is_Rising_Start)
        {
            // 폭탄 일정량 상승
            gBomb.gameObject.transform.position = new Vector3(gBomb.gameObject.transform.position.x, (gBomb.gameObject.transform.position.y + (m_Rising_Speed * Time.deltaTime)), gBomb.gameObject.transform.position.z);
            //Debug.Log("bombBombUp");
            if (gBomb.gameObject.transform.position.y > 2.0f)
            {
               
                m_is_Rising_Start = false;
                //GetComponentInChildren<MeshRenderer>().enabled = true;
                
            }
        }

        else
        {
            //Debug.Log("bombBombDown");
            // 폭탄 전방 이동
            switch (direction)
            {
                case 1:
                    if (gBomb.gameObject.transform.position.x <= x_dest)
                        gBomb.gameObject.transform.Translate(new Vector3((m_Thrown_Bomb_Speed * Time.deltaTime), 0.0f, 0.0f));
                    else
                        gBomb.gameObject.transform.position = (new Vector3(x_dest, gBomb.gameObject.transform.position.y, gBomb.gameObject.transform.position.z));
                    break;
                case 2:
                    if (gBomb.gameObject.transform.position.x >= x_dest)
                        gBomb.gameObject.transform.Translate(new Vector3(-(m_Thrown_Bomb_Speed * Time.deltaTime), 0.0f, 0.0f));
                    else
                        gBomb.gameObject.transform.position = (new Vector3(x_dest, gBomb.gameObject.transform.position.y, gBomb.gameObject.transform.position.z));
                    break;
                case 3:
                    if (gBomb.gameObject.transform.position.z <= z_dest)
                        gBomb.gameObject.transform.Translate(new Vector3(0.0f, 0.0f, (m_Thrown_Bomb_Speed * Time.deltaTime)));
                    //else
                    //    gBomb.gameObject.transform.position = (new Vector3(gBomb.gameObject.transform.position.x, gBomb.gameObject.transform.position.y, z_dest));
                    break;
                case 4:
                    if (gBomb.gameObject.transform.position.z >= z_dest)
                        gBomb.gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -(m_Thrown_Bomb_Speed * Time.deltaTime)));
                    else
                        gBomb.gameObject.transform.position = (new Vector3(gBomb.gameObject.transform.position.x, gBomb.gameObject.transform.position.y, z_dest));
                    break;
            }
        }

        if (!m_is_Done_Rising && gBomb.gameObject.transform.position.y < m_Rising_Limit)
        {
            // 폭탄 상승
            Debug.Log("올라간다");
            gBomb.gameObject.transform.Translate(new Vector3(0.0f, (m_Rising_Speed * Time.deltaTime), 0.0f));
        }

        if (gBomb.gameObject.transform.position.y >= m_Rising_Limit)
        {
            // 폭탄 상승 끝
            m_is_Done_Rising = true;
        }

        //if (m_is_Done_Rising && transform.position.y > m_Down_Limit)
        if (m_is_Done_Rising && gBomb.gameObject.transform.position.y > m_Down_Limit)
        {
            // 폭탄 하강
            Debug.Log("떨어진다");
            gBomb.gameObject.transform.Translate(new Vector3(0.0f, -(m_Rising_Speed * Time.deltaTime), 0.0f));
        }

        if (gBomb.gameObject.transform.position.y < m_Down_Limit)
        {
            gBomb.gameObject.transform.position = new Vector3(gBomb.gameObject.transform.position.x,0.0f, gBomb.gameObject.transform.position.z);
        }

        if (transform.position.x < -0.5f || transform.position.x > 28.5f
            || transform.position.z < 49.5f || transform.position.z > 78.5f)
        {
            // 맵 범위 밖으로 나가면 제거
            // StageManager.Update_MCL_isBlocked(m_My_MCL_Index, false);
            //Destroy(gBomb);
        }
    }
    //겜오브젝트로 받지 말고, 인트로 받아보자
    public void Thrown_Bomb_Move(int g, int x, int z, int x_dest, int z_dest, byte direction)
    {
        //gBomb.gameObject.transform.position = new Vector3(x, 0, z);

        if (g >= 0)
        {
            float m_Rising_Limit = 4.0f;
            float m_Down_Limit = 0.0f;
            bool tempBool = false;

            float m_Thrown_Bomb_Speed = 15.0f;
            float m_Rising_Speed = 8.0f;
            if (bomb_list[g].gameObject.activeInHierarchy)
            {
                if (x_dest == (int)bomb_list[g].gameObject.transform.position.x && z_dest == (int)bomb_list[g].gameObject.transform.position.z&& bomb_list[g].gameObject.transform.position.y==0)
                {
                    Debug.Log(g + "번째 투척완료!!!");
                    bomb_list[g].gameObject.SetActive(false);
                    m_is_rising_start[g] = true;
                    m_is_donerising[g] = false;
                    is_alive[g] = false;

                }
                if (m_is_rising_start[g])
                {
                    // 폭탄 일정량 상승
                    bomb_list[g].gameObject.transform.position = new Vector3(bomb_list[g].gameObject.transform.position.x, (bomb_list[g].gameObject.transform.position.y + (m_Rising_Speed * Time.deltaTime)), bomb_list[g].gameObject.transform.position.z);
                    //Debug.Log("bombBombUp");
                    if (bomb_list[g].gameObject.transform.position.y > 2.0f)
                    {

                        m_is_rising_start[g] = false;
                        //GetComponentInChildren<MeshRenderer>().enabled = true;

                    }
                }

                else
                {
                    //Debug.Log("bombBombDown");
                    // 폭탄 전방 이동
                    switch (direction)
                    {
                        case 1:
                            tempBool = bomb_list[g].gameObject.transform.position.x >= x_dest - 3;
                            if (bomb_list[g].gameObject.transform.position.x <= x_dest)
                                bomb_list[g].gameObject.transform.Translate(new Vector3((m_Thrown_Bomb_Speed * Time.deltaTime), 0.0f, 0.0f));
                            else
                                bomb_list[g].gameObject.transform.position = (new Vector3(x_dest, bomb_list[g].gameObject.transform.position.y, bomb_list[g].gameObject.transform.position.z));
                            break;
                        case 2:
                            tempBool = bomb_list[g].gameObject.transform.position.x <= x_dest + 3;
                            if (bomb_list[g].gameObject.transform.position.x >= x_dest)
                                bomb_list[g].gameObject.transform.Translate(new Vector3(-(m_Thrown_Bomb_Speed * Time.deltaTime), 0.0f, 0.0f));
                            else
                                bomb_list[g].gameObject.transform.position = (new Vector3(x_dest, bomb_list[g].gameObject.transform.position.y, bomb_list[g].gameObject.transform.position.z));
                            break;
                        case 3:
                            tempBool = bomb_list[g].gameObject.transform.position.z >= z_dest - 3;
                            if (bomb_list[g].gameObject.transform.position.z <= z_dest)
                                bomb_list[g].gameObject.transform.Translate(new Vector3(0.0f, 0.0f, (m_Thrown_Bomb_Speed * Time.deltaTime)));
                            //else
                            //    bomb_list[g].gameObject.transform.position = (new Vector3(bomb_list[g].gameObject.transform.position.x, bomb_list[g].gameObject.transform.position.y, z_dest));
                            break;
                        case 4:
                            tempBool = bomb_list[g].gameObject.transform.position.z <= z_dest + 3;
                            if (bomb_list[g].gameObject.transform.position.z >= z_dest)
                                bomb_list[g].gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -(m_Thrown_Bomb_Speed * Time.deltaTime)));
                            else
                                bomb_list[g].gameObject.transform.position = (new Vector3(bomb_list[g].gameObject.transform.position.x, bomb_list[g].gameObject.transform.position.y, z_dest));
                            break;
                    }
                }

                if (!m_is_donerising[g] && bomb_list[g].gameObject.transform.position.y < m_Rising_Limit)
                {
                    // 폭탄 상승
                    //Debug.Log("올라간다");
                    bomb_list[g].gameObject.transform.Translate(new Vector3(0.0f, (m_Rising_Speed * Time.deltaTime), 0.0f));
                }

                if (bomb_list[g].gameObject.transform.position.y >= m_Rising_Limit)
                {
                    // 폭탄 상승 끝
                    m_is_donerising[g] = true;
                }

                //if (m_is_Done_Rising && transform.position.y > m_Down_Limit)
                if (m_is_donerising[g] && bomb_list[g].gameObject.transform.position.y > m_Down_Limit && tempBool)
                {
                    // 폭탄 하강
                    //Debug.Log("떨어진다");
                    bomb_list[g].gameObject.transform.Translate(new Vector3(0.0f, -(m_Rising_Speed * Time.deltaTime), 0.0f));
                }

                if (bomb_list[g].gameObject.transform.position.y < m_Down_Limit)
                {
                    bomb_list[g].gameObject.transform.position = new Vector3(bomb_list[g].gameObject.transform.position.x, 0.0f, bomb_list[g].gameObject.transform.position.z);
                }

                if (transform.position.x < -0.5f || transform.position.x > 28.5f
                    || transform.position.z < 49.5f || transform.position.z > 78.5f)
                {
                    // 맵 범위 밖으로 나가면 제거
                    // StageManager.Update_MCL_isBlocked(m_My_MCL_Index, false);
                    //Destroy(gBomb);
                }
            }
        }
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            GetPacket(0,0,0,10+a,3);
            a++;
        }
            // if (Input.GetKeyDown(KeyCode.Space))
            //{
         for (int i = 0; i < 14; ++i)
         {
            if(is_alive[i])
               Thrown_Bomb_Move(i, sx[i], sz[i], dx[i], dz[i], dirc[i]);
         }

       //}

    }
}
