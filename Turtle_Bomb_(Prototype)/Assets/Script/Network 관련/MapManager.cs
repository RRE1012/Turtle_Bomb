using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
//explode에 gameid를 넣어 폭탄 수 reload
public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public GameObject m_bomb;
    public GameObject m_Tbomb;
    public GameObject[] m_terrain;
    public GameObject[] m_box;
    public GameObject[] m_rock;
    public GameObject m_flame_effect;
    public GameObject m_bush;
    public GameObject m_item_speed;
    public GameObject m_item_bomb;
    public GameObject m_item_fire;
    public GameObject m_item_kick;
    public GameObject m_item_throw;
    public GameObject m_item_glider;
    public GameObject m_item_airdrop;
    public GameObject m_explode_warn_range;
    public GameObject m_explode_effect;
    public GameObject parent;
    public GameObject m_airplane;
    public GameObject[] m_tile;
    public Text g_text;

    byte[] bombexplode_list = new byte[225];
    byte[] up_bombexplode_list = new byte[225];
    byte[] right_bombexplode_list = new byte[225];
    byte[] down_bombexplode_list = new byte[225];
    byte[] left_bombexplode_list = new byte[225];
    GameObject[] bomb_list = new GameObject[225];
    GameObject[] box_list = new GameObject[225];
    GameObject[] rock_list = new GameObject[50];
    GameObject[] bombT_list = new GameObject[32];
    GameObject[] bombK_list = new GameObject[32];
    GameObject[] range_List = new GameObject[225];
    List<GameObject> LiveList = new List<GameObject>();
    GameObject[] push_box_list = new GameObject[8];
    WaitForSeconds delay = new WaitForSeconds(0.1f);
    bool[] is_alive_kick = new bool[32];
    bool[] set_pos_kick = new bool[32];
    int[] dx_kick = new int[32];
    int[] dz_kick = new int[32];
    byte[] dirc_kick = new byte[32];
    int[] sx_kick = new int[32];
    int[] sz_kick = new int[32];

    bool[] is_alive_box = new bool[8];
    bool[] set_pos_box = new bool[8];
    bool[] rock_set = new bool[225];
    bool[] bush_set = new bool[225];

    bool[] item_set = new bool[225];
    int[] dx_box = new int[8];
    int[] dz_box = new int[8];
    byte[] dirc_box = new byte[8];
    int[] sx_box = new int[8];
    int[] sz_box = new int[8];


    bool[] m_is_rising_start = new bool[32];
    bool[] m_is_donerising = new bool[32];
    bool[] is_alive = new bool[32];
    bool[] set_pos = new bool[32];
    int[] dx = new int[32];
    int[] dz = new int[32];
    byte[] dirc = new byte[32];
    int[] sx = new int[32];
    int[] sz = new int[32];
    GameObject[] land1 = new GameObject[225];

    GameObject terrain;
    GameObject[] bush_list = new GameObject[50];
    GameObject plane;
    GameObject[] item_s_list = new GameObject[32];
    GameObject[] item_f_list = new GameObject[32];
    GameObject[] item_b_list = new GameObject[32];
    GameObject[] item_t_list = new GameObject[32];
    GameObject[] item_k_list = new GameObject[32];
    GameObject[] item_g_list = new GameObject[32];
    GameObject[] item_air_list = new GameObject[32];
    GameObject[] ex_list = new GameObject[32];
    GameObject[] fire_list = new GameObject[200];
    byte[] copy_map_info = new byte[225];
    bool is_airdrop = false;
    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //NetTest.instance.Receive();
        terrain = Instantiate(m_terrain[VariableManager.instance.map_type]);
        terrain.transform.position = new Vector3(0.0f, 0.0f, -50.0f);
        terrain.transform.parent = parent.transform;
        terrain.SetActive(true);
        //terrain.transform.position == new Vector3(100, 0, 100);
        LobbySound.instanceLS.SoundStop();

        is_airdrop = false;
        plane = Instantiate(m_airplane);
        //plane.transform.parent = parent.transform;
        plane.SetActive(false);
       
        
        for (int i = 0; i < 50; ++i)
        {
            rock_list[i] = Instantiate(m_rock[VariableManager.instance.map_type]);

            rock_list[i].SetActive(false);
            rock_list[i].transform.parent = parent.transform;
            
            bush_list[i] = Instantiate(m_bush);
            bush_list[i].transform.parent = parent.transform;
            bush_list[i].SetActive(false);
            if (i < 32)
            {
                if (i < 8)
                {
                    
                    is_alive_box[i] = false;
                    set_pos_box[i] = false;
                    push_box_list[i] = Instantiate(m_box[VariableManager.instance.map_type]);
                    push_box_list[i].transform.position = new Vector3(100, 0, 100);
                    push_box_list[i].SetActive(false);
                    push_box_list[i].transform.parent = parent.transform;
                    dirc_box[i] = 0;
                }
                ex_list[i] = Instantiate(m_explode_effect);
                ex_list[i].transform.parent = parent.transform;
                ex_list[i].SetActive(false);
                item_s_list[i] = Instantiate(m_item_speed);
                item_s_list[i].transform.parent = parent.transform;
                item_s_list[i].SetActive(false);
                item_f_list[i] = Instantiate(m_item_fire);
                item_f_list[i].transform.parent = parent.transform;
                item_f_list[i].SetActive(false);
                item_b_list[i] = Instantiate(m_item_bomb);
                item_b_list[i].transform.parent = parent.transform;
                item_b_list[i].SetActive(false);
                item_t_list[i] = Instantiate(m_item_throw);
                item_k_list[i] = Instantiate(m_item_kick);
                item_k_list[i].transform.parent = parent.transform;

                item_k_list[i].SetActive(false);
                item_t_list[i].SetActive(false);
                item_t_list[i].transform.parent = parent.transform;
                item_g_list[i] = Instantiate(m_item_glider);
                item_g_list[i].transform.parent = parent.transform;
                item_air_list[i] = Instantiate(m_item_airdrop);
                item_air_list[i].transform.position = new Vector3(100, 20, 100);
                item_air_list[i].SetActive(false);
                item_air_list[i].transform.parent = parent.transform;
                item_g_list[i].transform.position = new Vector3(100, 0, 100);
                item_g_list[i].SetActive(false);
                bombT_list[i] = Instantiate(m_Tbomb);
                bombT_list[i].transform.position = new Vector3(100, 0, 100);
                bombT_list[i].SetActive(false);
                bombT_list[i].transform.parent = parent.transform;
                bombK_list[i] = Instantiate(m_Tbomb);
                bombK_list[i].transform.position = new Vector3(100, 0, 100);
                bombK_list[i].transform.parent = parent.transform;
                //bombK_list[i].transform.rotation = new Quaternion(0, 0, 90.0f,0);
                bombK_list[i].SetActive(false);
                is_alive[i] = false;
                set_pos[i] = false;
                is_alive_kick[i] = false;
                set_pos_kick[i] = false;
                m_is_donerising[i] = false;
                m_is_rising_start[i] = true;
                dirc[i] = 0;
                dirc_kick[i] = 0;
            }
        }
        for(int i = 0; i < 200; ++i)
        {
            fire_list[i] = Instantiate(m_flame_effect);
            fire_list[i].SetActive(false);
            fire_list[i].transform.parent = parent.transform;
        }
        for (int z = 0; z < 15; ++z)
        {
            for (int x = 0; x < 15; ++x)
            {
                range_List[(z * 15) + x] = Instantiate(m_explode_warn_range);
                range_List[(z * 15) + x].transform.position = new Vector3(x * 2, -0.7f, z * 2);
                range_List[(z * 15) + x].transform.parent = parent.transform;
                range_List[(z * 15) + x].SetActive(false);
                item_set[(z * 15) + x] = false;
                rock_set[(z * 15) + x] = false;
                /*
                if (((z * 15) + x) % 2 == 0)
                {

                    land1[(z * 15) + x] = Instantiate(m_tile[VariableManager.instance.map_type * 2]);
                    land1[(z * 15) + x].transform.position = new Vector3(x * 2, -0.75f, z * 2);
                    land1[(z * 15) + x].SetActive(true);
                    land1[(z * 15) + x].transform.parent = parent.transform;
                }
                else
                {
                    land1[(z * 15) + x] = Instantiate(m_tile[(VariableManager.instance.map_type * 2) + 1]);
                    land1[(z * 15) + x].transform.position = new Vector3(x * 2, -0.75f, z * 2);
                    land1[(z * 15) + x].SetActive(true);
                    land1[(z * 15) + x].transform.parent = parent.transform;
                }
                */
                bomb_list[(z * 15) + x] = Instantiate(m_bomb);
                bomb_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                bomb_list[(z * 15) + x].SetActive(false);
                bomb_list[(z * 15) + x].transform.parent = parent.transform;
                box_list[(z * 15) + x] = Instantiate(m_box[VariableManager.instance.map_type]);
                box_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                box_list[(z * 15) + x].SetActive(false);

                box_list[(z * 15) + x].transform.parent = parent.transform;

                bombexplode_list[(z * 15) + x] = 0;
                up_bombexplode_list[(z * 15) + x] = 0;
                down_bombexplode_list[(z * 15) + x] = 0;

                right_bombexplode_list[(z * 15) + x] = 0;
                left_bombexplode_list[(z * 15) + x] = 0;
            }
        }
        StartCoroutine("CheckMap_v2");
        StartCoroutine("CheckFire");
    }

    public void AirdropStart()
    {
        is_airdrop = true;

    }
    void AirplaneOff()
    {
        plane.SetActive(false);
    }
    public void Push_Box_Move(int g, int x, int z, int x_dest, int z_dest, byte direction)
    {
        if (g >= 0)
        {

            float m_PushBox_Speed = 4.0f;
            if (x_dest == (int)push_box_list[g].gameObject.transform.position.x && z_dest == (int)push_box_list[g].gameObject.transform.position.z)
            {
                //Debug.Log(g + "번째 투척완료!!!");
                push_box_list[g].gameObject.SetActive(false);

                is_alive_box[g] = false;
                NetTest.instance.SendBox_Packet(x_dest / 2, z_dest / 2);
            }
            switch (direction)
            {
                case (byte)1:
                    ////Debug.Log("bombBombDown1");

                    if (push_box_list[g].gameObject.transform.position.x <= x_dest)
                        push_box_list[g].gameObject.transform.Translate(new Vector3((m_PushBox_Speed * Time.deltaTime), 0.0f, 0.0f));
                    else
                        push_box_list[g].gameObject.transform.position = (new Vector3(x_dest, 0, z_dest));
                    break;
                case (byte)2:
                    ////Debug.Log("bombBombDown2");

                    if (push_box_list[g].gameObject.transform.position.x >= x_dest)
                        push_box_list[g].gameObject.transform.Translate(new Vector3(-(m_PushBox_Speed * Time.deltaTime), 0.0f, 0.0f));
                    else
                        push_box_list[g].gameObject.transform.position = (new Vector3(x_dest, 0, z_dest));
                    break;
                case (byte)3:
                    ////Debug.Log("bombBombDown3");

                    if (push_box_list[g].gameObject.transform.position.z <= z_dest)
                        push_box_list[g].gameObject.transform.Translate(new Vector3(0.0f, 0.0f, (m_PushBox_Speed * Time.deltaTime)));
                    else
                        push_box_list[g].gameObject.transform.position = (new Vector3(x_dest, 0, z_dest));
                    break;
                case (byte)4:
                    ////Debug.Log("bombBombDown4");

                    if (push_box_list[g].gameObject.transform.position.z >= z_dest)
                        push_box_list[g].gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -(m_PushBox_Speed * Time.deltaTime)));
                    else
                        push_box_list[g].gameObject.transform.position = (new Vector3(x_dest, 0, z_dest));
                    break;
                default:
                    //Debug.Log("bombBombDownDefault");
                    break;
            }
        }
    }
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
            if (bombT_list[g].gameObject.activeInHierarchy)
            {
                if (x_dest == (int)bombT_list[g].gameObject.transform.position.x && z_dest == (int)bombT_list[g].gameObject.transform.position.z && bombT_list[g].gameObject.transform.position.y == 0)
                {
                    //Debug.Log(g + "번째 투척완료!!!");
                    bombT_list[g].gameObject.SetActive(false);
                    m_is_rising_start[g] = true;
                    m_is_donerising[g] = false;
                    is_alive[g] = false;
                    NetTest.instance.SendBomb_TCPacket(x_dest / 2, z_dest / 2);
                }
                if (m_is_rising_start[g])
                {
                    // 폭탄 일정량 상승
                    bombT_list[g].gameObject.transform.position = new Vector3(bombT_list[g].gameObject.transform.position.x, (bombT_list[g].gameObject.transform.position.y + (m_Rising_Speed * Time.deltaTime)), bombT_list[g].gameObject.transform.position.z);
                    //Debug.Log("bombBombUp");
                    if (bombT_list[g].gameObject.transform.position.y > 2.0f)
                    {

                        m_is_rising_start[g] = false;
                        //GetComponentInChildren<MeshRenderer>().enabled = true;

                    }
                }

                else
                {

                    // 폭탄 전방 이동
                    switch (direction)
                    {
                        case (byte)1:
                            //Debug.Log("bombBombDown1");
                            tempBool = bombT_list[g].gameObject.transform.position.x >= x_dest - 3;
                            if (bombT_list[g].gameObject.transform.position.x <= x_dest)
                                bombT_list[g].gameObject.transform.Translate(new Vector3((m_Thrown_Bomb_Speed * Time.deltaTime), 0.0f, 0.0f));
                            else
                                bombT_list[g].gameObject.transform.position = (new Vector3(x_dest, bombT_list[g].gameObject.transform.position.y, bombT_list[g].gameObject.transform.position.z));
                            break;
                        case (byte)2:
                            //Debug.Log("bombBombDown2");
                            tempBool = bombT_list[g].gameObject.transform.position.x <= x_dest + 3;
                            if (bombT_list[g].gameObject.transform.position.x >= x_dest)
                                bombT_list[g].gameObject.transform.Translate(new Vector3(-(m_Thrown_Bomb_Speed * Time.deltaTime), 0.0f, 0.0f));
                            else
                                bombT_list[g].gameObject.transform.position = (new Vector3(x_dest, bombT_list[g].gameObject.transform.position.y, bombT_list[g].gameObject.transform.position.z));
                            break;
                        case (byte)3:
                            //Debug.Log("bombBombDown3");
                            tempBool = bombT_list[g].gameObject.transform.position.z >= z_dest - 3;
                            if (bombT_list[g].gameObject.transform.position.z <= z_dest)
                                bombT_list[g].gameObject.transform.Translate(new Vector3(0.0f, 0.0f, (m_Thrown_Bomb_Speed * Time.deltaTime)));
                            //else
                            //    bomb_list[g].gameObject.transform.position = (new Vector3(bomb_list[g].gameObject.transform.position.x, bomb_list[g].gameObject.transform.position.y, z_dest));
                            break;
                        case (byte)4:
                            //Debug.Log("bombBombDown4");
                            tempBool = bombT_list[g].gameObject.transform.position.z <= z_dest + 3;
                            if (bombT_list[g].gameObject.transform.position.z >= z_dest)
                                bombT_list[g].gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -(m_Thrown_Bomb_Speed * Time.deltaTime)));
                            else
                                bombT_list[g].gameObject.transform.position = (new Vector3(bombT_list[g].gameObject.transform.position.x, bombT_list[g].gameObject.transform.position.y, z_dest));
                            break;
                        default:
                            //Debug.Log("bombBombDownDefault");
                            break;
                    }
                }

                if (!m_is_donerising[g] && bombT_list[g].gameObject.transform.position.y < m_Rising_Limit)
                {
                    // 폭탄 상승
                    ////Debug.Log("올라간다");
                    bombT_list[g].gameObject.transform.Translate(new Vector3(0.0f, (m_Rising_Speed * Time.deltaTime), 0.0f));
                }

                if (bombT_list[g].gameObject.transform.position.y >= m_Rising_Limit)
                {
                    // 폭탄 상승 끝
                    m_is_donerising[g] = true;
                }

                //if (m_is_Done_Rising && transform.position.y > m_Down_Limit)
                if (m_is_donerising[g] && bombT_list[g].gameObject.transform.position.y > m_Down_Limit && tempBool)
                {
                    // 폭탄 하강
                    // //Debug.Log("떨어진다");
                    bombT_list[g].gameObject.transform.Translate(new Vector3(0.0f, -(m_Rising_Speed * Time.deltaTime), 0.0f));
                }

                if (bombT_list[g].gameObject.transform.position.y < m_Down_Limit)
                {
                    bombT_list[g].gameObject.transform.position = new Vector3(bombT_list[g].gameObject.transform.position.x, 0.0f, bombT_list[g].gameObject.transform.position.z);
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
    public void Kick_Bomb_Move(int g, int x, int z, int x_dest, int z_dest, byte direction)
    {
        if (g >= 0)
        {

            float m_PushBox_Speed = 15.0f;

            if (x_dest == (int)bombK_list[g].gameObject.transform.position.x && z_dest == (int)bombK_list[g].gameObject.transform.position.z)
            {
                //Debug.Log(g + "번째 투척완료!!!");
                bombK_list[g].gameObject.SetActive(false);

                is_alive_kick[g] = false;
                NetTest.instance.SendKickCom_Packet(x_dest / 2, z_dest / 2);
            }


            switch (direction)
            {
                case (byte)1:
                    ////Debug.Log("bombBombDown1");

                    if (bombK_list[g].gameObject.transform.position.x <= x_dest)
                    {

                        bombK_list[g].gameObject.transform.Translate(new Vector3((m_PushBox_Speed * Time.deltaTime), 0.0f, 0.0f));
                    }
                    else
                        bombK_list[g].gameObject.transform.position = (new Vector3(x_dest, 0, z_dest));
                    break;
                case (byte)2:
                    ////Debug.Log("bombBombDown2");

                    if (bombK_list[g].gameObject.transform.position.x >= x_dest)
                    {

                        bombK_list[g].gameObject.transform.Translate(new Vector3(-(m_PushBox_Speed * Time.deltaTime), 0.0f, 0.0f));
                    }
                    else
                        bombK_list[g].gameObject.transform.position = (new Vector3(x_dest, 0, z_dest));
                    break;
                case (byte)3:
                    ////Debug.Log("bombBombDown3");

                    if (bombK_list[g].gameObject.transform.position.z <= z_dest)
                    {

                        bombK_list[g].gameObject.transform.Translate(new Vector3(0.0f, 0.0f, (m_PushBox_Speed * Time.deltaTime)));
                    }
                    else
                        bombK_list[g].gameObject.transform.position = (new Vector3(x_dest, 0, z_dest));
                    break;
                case (byte)4:
                    ////Debug.Log("bombBombDown4");

                    if (bombK_list[g].gameObject.transform.position.z >= z_dest)
                    {

                        bombK_list[g].gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -(m_PushBox_Speed * Time.deltaTime)));
                    }
                    else
                        bombK_list[g].gameObject.transform.position = (new Vector3(x_dest, 0, z_dest));
                    break;
                default:
                    //Debug.Log("bombBombDownDefault");
                    break;
            }
        }
    }
    public void Set_Bomb(int x, int z)
    {
        bomb_list[((z * 15) + x)].SetActive(true);
    }
    public void Check_Map(byte[] mapinfo)
    {
        Buffer.BlockCopy(mapinfo, 2, copy_map_info, 0, 225);

    }
    int GetGameObject()
    {
        for (int i = 0; i < 32; ++i)
        {
            if (!is_alive[i])
            {
                is_alive[i] = true;
                ////Debug.Log("I get Bomb~To~Throw");
                return i;
            }
        }
        return -1;
    }
    int GetKickBomb()
    {
        for (int i = 0; i < 32; ++i)
        {
            if (!is_alive_kick[i])
            {
                is_alive_kick[i] = true;
                ////Debug.Log("I get Bomb~To~Throw");
                return i;
            }
        }
        return -1;
    }
    int GetPushBox()
    {
        for (int i = 0; i < 8; ++i)
        {
            if (!is_alive_box[i])
            {
                is_alive_box[i] = true;
                ////Debug.Log("I get Bomb~To~Throw");
                return i;
            }
        }
        return -1;
    }
   
    public void Explode_Bomb(int x, int z, byte f)
    {
        //Explode(f, bomb_list[((z * 15) + x)]);
        bombexplode_list[((z * 15) + x)] = f;
    }
    public void Explode_Bomb_v2(int x, int z, byte[] f)
    {
        up_bombexplode_list[((z * 15) + x)] = f[0];
        right_bombexplode_list[((z * 15) + x)] = f[1];
        down_bombexplode_list[((z * 15) + x)] = f[2];
        left_bombexplode_list[((z * 15) + x)] = f[3];

    }
    public bool Check_BombSet(int x, int z)
    {
        if (bomb_list[((z * 15) + x)].activeInHierarchy)
            return true;
        else
            return false;
    }
    public void Push_BoxSet(int x, int z, int dxx, int dzz, byte dircc)
    {
        int tempx = GetPushBox();
        if (tempx >= 0)
        {
            sx_box[tempx] = x * 2;
            sz_box[tempx] = z * 2;
            dx_box[tempx] = dxx * 2;
            dz_box[tempx] = dzz * 2;
            dirc_box[tempx] = dircc;
            set_pos_box[tempx] = true;
            ////Debug.Log("SetBombThrow" +dirc[tempx]);
        }
        else
        {
            //Debug.Log("Don't have bomb");
        }
    }
    public void Kick_BombSet(int x, int z, int dxx, int dzz, byte dircc)
    {
        int tempx = GetKickBomb();
        if (tempx >= 0)
        {
            sx_kick[tempx] = x * 2;
            sz_kick[tempx] = z * 2;
            dx_kick[tempx] = dxx * 2;
            dz_kick[tempx] = dzz * 2;
            dirc_kick[tempx] = dircc;
            set_pos_kick[tempx] = true;
            ////Debug.Log("SetBombThrow" +dirc[tempx]);
        }
        else
        {
            //Debug.Log("Don't have bomb");
        }
    }
    public void Throw_BombSet(int x, int z, int dxx, int dzz, byte dircc)
    {
        int tempx = GetGameObject();
        ////Debug.Log("GetGameObjComplete");
        if (tempx >= 0)
        {
            sx[tempx] = x * 2;
            sz[tempx] = z * 2;
            dx[tempx] = dxx * 2;
            dz[tempx] = dzz * 2;
            dirc[tempx] = dircc;
            set_pos[tempx] = true;
            ////Debug.Log("SetBombThrow" +dirc[tempx]);
        }
        else
        {
            //Debug.Log("Don't have bomb");
        }
        //bombT_list[tempx].SetActive(true);
    }

    void Explode(byte fire_power, int x, int z)
    {
        range_List[(z * 15) + (x)].SetActive(false);
        for (byte y = 1; y <= fire_power; ++y)
        {
            if (x + y < 15)
                range_List[(z * 15) + (x + y)].SetActive(false);
            if (x - y >= 0)
                range_List[(z * 15) + (x - y)].SetActive(false);
            if (z + y < 15)
                range_List[((z + y) * 15) + x].SetActive(false);
            if (z - y >= 0)
                range_List[((z - y) * 15) + x].SetActive(false);
        }
    }
    void Explode_V2(GameObject bomb, byte up, byte right, byte down, byte left)
    {
        GameObject Instance_FlameDir_N;
        GameObject Instance_FlameDir_S;
        GameObject Instance_FlameDir_W;
        GameObject Instance_FlameDir_E;
        GameObject Instance_FlameDir_M;
        GameObject Instance_FlameDir_M2;
        Instance_FlameDir_M2 = GetExplodeEffect();
        Instance_FlameDir_M2.transform.position = new Vector3(bomb.transform.position.x, 0.0f, bomb.transform.position.z);
        Instance_FlameDir_M2.SetActive(true);
        Instance_FlameDir_M2.GetComponentInChildren<BoxCollider_x>().ChangeBoxColX(left, right);
        Instance_FlameDir_M2.GetComponentInChildren<BoxCollider_z>().ChangeBoxColZ(down,up);
        
        Instance_FlameDir_M2.GetComponent<ParticleSystem>().Play();
        
        Instance_FlameDir_M = GetFireEffect();
        Instance_FlameDir_M.transform.position = new Vector3(bomb.transform.position.x, 0.0f, bomb.transform.position.z);
        Instance_FlameDir_M.SetActive(true);
        Instance_FlameDir_M.GetComponent<ParticleSystem>().Play();
        
        for (byte i = 0; i < up; ++i)
        {
            Instance_FlameDir_N = GetFireEffect();
            Instance_FlameDir_N.transform.position = new Vector3(bomb.transform.position.x, 0.0f, bomb.transform.position.z + (2.0f * (i + 1)));
            Instance_FlameDir_N.SetActive(true);
            Instance_FlameDir_N.GetComponent<ParticleSystem>().Play();
            

        }
        for (byte i = 0; i < down; ++i)
        {
            Instance_FlameDir_S = GetFireEffect();
            Instance_FlameDir_S.transform.position = new Vector3(bomb.transform.position.x, 0.0f, bomb.transform.position.z - (2.0f * (i + 1)));
            Instance_FlameDir_S.SetActive(true);
            Instance_FlameDir_S.GetComponent<ParticleSystem>().Play();
            
        }
        for (byte i = 0; i < left; ++i)
        {
            Instance_FlameDir_W = GetFireEffect();
            Instance_FlameDir_W.transform.position = new Vector3(bomb.transform.position.x - (2.0f * (i + 1)), 0.0f, bomb.transform.position.z);
            Instance_FlameDir_W.SetActive(true);
            Instance_FlameDir_W.GetComponent<ParticleSystem>().Play();
            
        }
        for (byte i = 0; i < right; ++i)
        {
            Instance_FlameDir_E = GetFireEffect();
            Instance_FlameDir_E.transform.position = new Vector3(bomb.transform.position.x + (2.0f * (i + 1)), 0.0f, bomb.transform.position.z);
            Instance_FlameDir_E.SetActive(true);
            
            Instance_FlameDir_E.GetComponent<ParticleSystem>().Play();
            
        }
        MusicManager.manage_ESound.soundE();
        bomb.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        //g_text.text = "Player1 x :" + NetTest.instance.GetNetPosx(0) + ", z :" + NetTest.instance.GetNetPosz(0) + "\nPlayer2 x :" + NetTest.instance.GetNetPosx(1) + ", z:" + NetTest.instance.GetNetPosz(1) + "\nPlayer3 x :" + NetTest.instance.GetNetPosx(2) + ", z:" + NetTest.instance.GetNetPosz(2) + "\nPlayer4 x :" + NetTest.instance.GetNetPosx(3) + ", z:" + NetTest.instance.GetNetPosz(3);
        for (int i = 0; i < 32; ++i)
        {
            if (i < 8)
            {
                if (set_pos_box[i])
                {
                    is_alive_box[i] = true;

                    push_box_list[i].SetActive(true);
                    push_box_list[i].transform.position = new Vector3(sx_box[i], 0, sz_box[i]);
                    set_pos_box[i] = false;
                }
                if (push_box_list[i].gameObject.activeInHierarchy)
                {
                    Push_Box_Move(i, sx_box[i], sz_box[i], dx_box[i], dz_box[i], dirc_box[i]);
                }
            }
            if (set_pos_kick[i])
            {
                is_alive_kick[i] = true;

                bombK_list[i].SetActive(true);
                bombK_list[i].transform.position = new Vector3(sx_kick[i], 0, sz_kick[i]);
                set_pos_kick[i] = false;
            }
            if (set_pos[i])
            {
                ////Debug.Log("get Throw!!!!");

                is_alive[i] = true;

                bombT_list[i].SetActive(true);
                bombT_list[i].transform.position = new Vector3(sx[i], 0, sz[i]);
                set_pos[i] = false;
                ////Debug.Log("Destination  x: " + dx[i] + "z:" + dz[i] + "direction:" + dirc[i]);
            }

            if (bombT_list[i].gameObject.activeInHierarchy)
            {

                ////Debug.Log(dirc[i]);
                Thrown_Bomb_Move(i, sx[i], sz[i], dx[i], dz[i], dirc[i]);
            }
            if (bombK_list[i].gameObject.activeInHierarchy)
            {

                ////Debug.Log(dirc[i]);
                Kick_Bomb_Move(i, sx_kick[i], sz_kick[i], dx_kick[i], dz_kick[i], dirc_kick[i]);
            }
        }
    }

    public void GotoRoom()
    {
        //Time.timeScale = 1;
        //Debug.Log("Go to Room");
        LobbySound.instanceLS.SoundStart();
        for (int i = 0; i < 4; ++i)
            VariableManager.instance.ready_Turtle[i] = 0;
        SceneChange.instance.GoTo_ModeSelect_Scene();
    }
    GameObject GetRock()
    {
        for (int i = 0; i < 50; ++i)
        {
            if (!rock_list[i].activeInHierarchy)
                return rock_list[i];
        }
        return null;
    }
    GameObject GetBush()
    {
        for (int i = 0; i < 50; ++i)
        {
            if (!bush_list[i].activeInHierarchy)
                return bush_list[i];
        }
        return null;
    }
    GameObject GetExplodeEffect()
    {
        for (int i = 0; i < 32; ++i)
        {
            if (!ex_list[i].activeInHierarchy)
                return ex_list[i];
        }
        return null;
    }
    GameObject GetFireEffect()
    {
        for (int i = 0; i < 200; ++i)
        {
            if (!fire_list[i].activeInHierarchy)
                return fire_list[i];
        }
        return null;
    }
    GameObject GetItem(int type)
    {
        switch (type)
        {
            case 1:
                for (int i = 0; i < 50; ++i)
                {
                    if (!item_b_list[i].activeInHierarchy)
                        return item_b_list[i];
                }
                break;
            case 2:
                for (int i = 0; i < 50; ++i)
                {
                    if (!item_f_list[i].activeInHierarchy)
                        return item_f_list[i];
                }
                break;
            case 3:
                for (int i = 0; i < 50; ++i)
                {
                    if (!item_s_list[i].activeInHierarchy)
                        return item_s_list[i];
                }
                break;
            case 4:
                for (int i = 0; i < 50; ++i)
                {
                    if (!item_k_list[i].activeInHierarchy)
                        return item_k_list[i];
                }
                break;

            case 5:
                for (int i = 0; i < 50; ++i)
                {
                    if (!item_t_list[i].activeInHierarchy)
                        return item_t_list[i];
                }
                break;
            case 6:
                for (int i = 0; i < 32; ++i)
                {
                    if (!item_g_list[i].activeInHierarchy)
                        return item_g_list[i];
                }
                break;
            case 7:
                for(int i = 0; i <32; ++i)
                {
                     if (!item_air_list[i].activeInHierarchy)
                        return item_air_list[i];
                }
                break;
            default:
                return null;

        }
        return null;
    }
    void DeleteItem(int x, int z)
    {
        if (LiveList.Count > 0)
        {
            for (int i = 0; i < LiveList.Count; ++i)
            {
                if (LiveList[i].activeInHierarchy && LiveList[i].transform.position.x == x * 2 && LiveList[i].transform.position.z == z * 2)
                {
                    LiveList[i].SetActive(false);
                    LiveList.Remove(LiveList[i]);
                }
            }
        }
    }

    public void GotoLobby()
    {
        //Time.timeScale = 1;
        //Debug.Log("Go to GotoLobby");
        NetTest.instance.SendOUTPacket();
        //난 살아있다를 증명하자
        LobbySound.instanceLS.SoundStart();
        for (int i = 0; i < 4; ++i)
            VariableManager.instance.ready_Turtle[i] = 0;
        SceneChange.instance.GoTo_Wait_Scene();
    }
    IEnumerator CheckFire()
    {
        
        for (; ; )
        {


            for (int z = 0; z < 15; ++z)
            {
                for (int x = 0; x < 15; ++x)
                {
                    byte fireinfo2 = VariableManager.instance.firepower_list[(z * 15) + (x)];
                    if (fireinfo2 != 0)
                    {

                        range_List[(z * 15) + (x)].SetActive(true);
                        bool upBlocked = false;
                        bool downBlocked = false;
                        bool rightBlocked = false;
                        bool leftBlocked = false;
                        for (byte y = 1; y <= fireinfo2; ++y)
                        {
                            if (x + y < 15 && !rightBlocked)
                            {
                                if (rock_set[(z * 15) + (x + y)] || box_list[(z * 15) + (x + y)].activeInHierarchy)
                                    rightBlocked = true;
                                if (!rightBlocked)
                                    range_List[(z * 15) + (x + y)].SetActive(true);
                            }
                            if (x - y >= 0 && !leftBlocked)
                            {
                                if (rock_set[(z * 15) + (x - y)] || box_list[(z * 15) + (x - y)].activeInHierarchy)
                                    leftBlocked = true;
                                if (!leftBlocked)
                                    range_List[(z * 15) + (x - y)].SetActive(true);
                            }
                            if (z + y < 15 && !upBlocked)
                            {
                                if (rock_set[((z + y) * 15) + x] || box_list[((z + y) * 15) + x].activeInHierarchy)
                                    upBlocked = true;
                                if (!upBlocked)
                                    range_List[((z + y) * 15) + x].SetActive(true);
                            }
                            if (z - y >= 0 && !downBlocked)
                            {
                                if (rock_set[((z - y) * 15) + x] || box_list[((z - y) * 15) + x].activeInHierarchy)
                                    downBlocked = true;
                                if (!downBlocked)
                                    range_List[((z - y) * 15) + x].SetActive(true);
                            }
                        }
                    }
                    //range_List

                }
            }
            yield return delay;
        }
    }
    IEnumerator CheckMap_v2()
    {
        
        for (; ; )
        {
            if (is_airdrop)
            {
                Notice_UI.GetInstance().Notice_Play(NOTICE_NUMBER.AIR_DROP);
                plane.transform.position = new Vector3(24, 20, 24);
                plane.SetActive(true);
                plane.GetComponent<Animation>().Play(plane.GetComponent<Animation>().GetClip("Air_Drop").name);
                plane.GetComponentInChildren<Airdrop_Sound>().Play_AirplaneSound();
                //Debug.Log("AirPlane Active");
                Invoke("AirplaneOff", 5.0f);
                is_airdrop = false;
            }
            for (int z = 0; z < 15; ++z)
            {
                for (int x = 0; x < 15; ++x)
                {
                    byte Tile_Info2 = VariableManager.instance.copy_map_info[(z * 15) + (x)];
                    bool tempbool = (down_bombexplode_list[(z * 15) + (x)] != 0) || (up_bombexplode_list[(z * 15) + (x)] != 0) || (left_bombexplode_list[(z * 15) + (x)] != 0) || (right_bombexplode_list[(z * 15) + (x)] != 0);
                    switch (Tile_Info2)
                    {
                        case 5: //Bomb
                            if (!bomb_list[(z * 15) + (x)].activeInHierarchy)
                            {
                                bomb_list[(z * 15) + (x)].SetActive(true);
                                MusicManager.manage_ESound.BombSetSound();

                                ////Debug.Log("Made Bomb");
                            }/*
                            else if (bombexplode_list[(z * 15) + (x)] != 0)
                            {
                                Explode(bombexplode_list[(z * 15) + (x)], bomb_list[((z * 15) + x)]);
                                bombexplode_list[(z * 15) + (x)] = 0;
                            }*/

                            if (tempbool)
                            {
                                Explode_V2(bomb_list[((z * 15) + x)], up_bombexplode_list[((z * 15) + x)], right_bombexplode_list[((z * 15) + x)], down_bombexplode_list[((z * 15) + x)], left_bombexplode_list[((z * 15) + x)]);
                                down_bombexplode_list[((z * 15) + x)] = 0;
                                up_bombexplode_list[((z * 15) + x)] = 0;
                                left_bombexplode_list[((z * 15) + x)] = 0;
                                right_bombexplode_list[((z * 15) + x)] = 0;
                            }
                            break;
                        case 0: //Nothing
                                ////Debug.Log("Nothing:" + x+","+y);
                            if (VariableManager.instance.firepower_list[(z * 15) + (x)] != 0)
                            {
                                Explode(VariableManager.instance.firepower_list[(z * 15) + (x)], x, z);
                                VariableManager.instance.firepower_list[(z * 15) + (x)] = 0;
                            }
                            if (tempbool)
                            {

                                Explode_V2(bomb_list[((z * 15) + x)], up_bombexplode_list[((z * 15) + x)], right_bombexplode_list[((z * 15) + x)], down_bombexplode_list[((z * 15) + x)], left_bombexplode_list[((z * 15) + x)]);
                                down_bombexplode_list[((z * 15) + x)] = 0;
                                up_bombexplode_list[((z * 15) + x)] = 0;
                                left_bombexplode_list[((z * 15) + x)] = 0;
                                right_bombexplode_list[((z * 15) + x)] = 0;
                            }
                            bomb_list[((z * 15) + x)].SetActive(false);
                            box_list[(z * 15) + (x)].SetActive(false);
                            item_set[(z * 15) + x] = false;

                            DeleteItem(x, z);
                            //bush_list[(z * 15) + (x)].SetActive(false);

                            break;
                        case 1: //Cube(Box)로
                            if (!box_list[(z * 15) + (x)].activeInHierarchy)
                                box_list[(z * 15) + (x)].SetActive(true);
                            break;
                        case 2: //Rock
                            if (!rock_set[(z * 15) + (x)])
                            {
                                GameObject tempb = GetRock();
                                tempb.SetActive(true);
                                tempb.transform.position = new Vector3(x * 2, 0, z * 2);
                                rock_set[(z * 15) + (x)] = true;
                            }
                            break;
                        case 11: //Item_Bomb
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_set[(z * 15) + (x)])
                            {
                                GameObject tempib = GetItem(1);
                                tempib.SetActive(true);
                                tempib.transform.position = new Vector3(x * 2, 0, z * 2);

                                LiveList.Add(tempib);

                                item_set[(z * 15) + (x)] = true;
                            }
                            break;
                        case 22: //Glider
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_set[(z * 15) + (x)])
                            {

                                GameObject tempib = GetItem(6);
                                tempib.SetActive(true);
                                tempib.transform.position = new Vector3(x * 2, 0, z * 2);

                                LiveList.Add(tempib);

                                item_set[(z * 15) + (x)] = true;
                            }
                            break;
                        case 23://AirDrop
                            if (!item_set[(z * 15) + (x)])
                            {

                                GameObject tempib = GetItem(7);
                                tempib.SetActive(true);
                                tempib.transform.position = new Vector3(x * 2, 20, z * 2);
                                tempib.GetComponent<Item_Network_Airdrop>().IsGen();
                                LiveList.Add(tempib);

                                item_set[(z * 15) + (x)] = true;
                            }
                            break;
                        case 3:
                            if (!bush_set[(z * 15) + (x)])
                            {
                                GameObject tempbush = GetBush();
                                tempbush.SetActive(true);
                                tempbush.transform.position = new Vector3(x * 2, -0.7f, z * 2);
                                LiveList.Add(tempbush);
                                bush_set[(z * 15) + (x)] = true;
                            }

                            break;
                        case 13:
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_set[(z * 15) + (x)])
                            {
                                GameObject tempib = GetItem(2);
                                tempib.SetActive(true);
                                tempib.transform.position = new Vector3(x * 2, 0, z * 2);
                                LiveList.Add(tempib);
                                item_set[(z * 15) + (x)] = true;
                            }
                            break;
                        case 12:
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_set[(z * 15) + (x)])
                            {
                                GameObject tempib = GetItem(3);
                                tempib.SetActive(true);
                                tempib.transform.position = new Vector3(x * 2, 0, z * 2);
                                LiveList.Add(tempib);
                                item_set[(z * 15) + (x)] = true;
                            }
                            break;
                        case 14:
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_set[(z * 15) + (x)])
                            {
                                GameObject tempib = GetItem(4);
                                tempib.SetActive(true);
                                tempib.transform.position = new Vector3(x * 2, 0, z * 2);
                                LiveList.Add(tempib);
                                item_set[(z * 15) + (x)] = true;
                            }
                            break;

                        case 15:
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_set[(z * 15) + (x)])
                            {
                                GameObject tempib = GetItem(5);
                                tempib.SetActive(true);
                                tempib.transform.position = new Vector3(x * 2, 0, z * 2);
                                LiveList.Add(tempib);
                                item_set[(z * 15) + (x)] = true;
                            }
                            break;
                        default:

                            //rock_list[(z * 15) + (x)].SetActive(true);
                            break;

                    }
                }
            }

            yield return delay;
        }
    }

}
