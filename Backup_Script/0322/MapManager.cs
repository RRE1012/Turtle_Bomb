using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public static MapManager instance;
    public GameObject m_bomb;
    public GameObject m_box;
    public GameObject m_rock;
    public GameObject m_flame_effect;
    public GameObject m_bush;
    public GameObject m_item_speed;
    public GameObject m_item_bomb;
    public GameObject m_item_fire;

    byte[] bombexplode_list = new byte[225];
    GameObject[] bomb_list = new GameObject[225];
    GameObject[] box_list = new GameObject[225];
    GameObject[] rock_list = new GameObject[225];

    GameObject[] bush_list = new GameObject[225];
    GameObject[] item_s_list = new GameObject[225];
    GameObject[] item_f_list = new GameObject[225];
    GameObject[] item_b_list = new GameObject[225];
    byte[] copy_map_info = new byte[225];

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
		for(int z = 0; z < 15; ++z)
        {
            for(int x = 0; x < 15; ++x)
            {
                bomb_list[(z * 15) + x] = Instantiate(m_bomb);
                bomb_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                bomb_list[(z * 15) + x].SetActive(false);
                box_list[(z * 15) + x] = Instantiate(m_box);
                box_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                box_list[(z * 15) + x].SetActive(false);
                rock_list[(z * 15) + x] = Instantiate(m_rock);
                rock_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                rock_list[(z * 15) + x].SetActive(false);
                bush_list[(z * 15) + x] = Instantiate(m_bush);
                bush_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                bush_list[(z * 15) + x].SetActive(false);
                item_s_list[(z * 15) + x] = Instantiate(m_item_speed);
                item_s_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                item_s_list[(z * 15) + x].SetActive(false);
                item_f_list[(z * 15) + x] = Instantiate(m_item_fire);
                item_f_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                item_f_list[(z * 15) + x].SetActive(false);
                item_b_list[(z * 15) + x] = Instantiate(m_item_bomb);
                item_b_list[(z * 15) + x].transform.position = new Vector3(x * 2, 0, z * 2);
                item_b_list[(z * 15) + x].SetActive(false);
                bombexplode_list[(z * 15) + x] = 0;
            }
        }
        StartCoroutine("CheckMap_v2");
	}
	public void Set_Bomb(int x, int z)
    {
        bomb_list[((z * 15) + x)].SetActive(true);
    }
    public void Check_Map(byte[] mapinfo)
    {
        Buffer.BlockCopy(mapinfo, 2, copy_map_info, 0, 225);
        
    }
    public void Initialize_Map(byte[] mapinfo)
    {
        for (int z = 0; z < 15; ++z)
        {
            for (int x = 0; x < 15; ++x)
            {
                byte Tile_Info2 = mapinfo[(z * 15) + (x)];
                switch (Tile_Info2)
                {
                    case 1: //Bomb
                        if (!bomb_list[(z * 15) + (x)].activeInHierarchy)
                            bomb_list[(z * 15) + (x)].SetActive(true);
                        break;
                    case 2: //Nothing
                        //Debug.Log("Nothing:" + x+","+y);
                        break;
                    case 3: //Cube(Box)로
                        if (!box_list[(z * 15) + (x)].activeInHierarchy)
                            box_list[(z * 15) + (x)].SetActive(true);
                        break;
                    case 4: //Rock
                        if (!rock_list[(z * 15) + (x)].activeInHierarchy)
                            rock_list[(z * 15) + (x)].SetActive(true);
                        break;
                    case 5: //Item_Bomb
                        if (!item_b_list[(z * 15) + (x)].activeInHierarchy)
                            item_b_list[(z * 15) + (x)].SetActive(true);
                        break;
                    default:

                        break;

                }
            }
        }
    }
    public void Explode_Bomb(int x, int z, byte f)
    {
        //Explode(f, bomb_list[((z * 15) + x)]);
        bombexplode_list[((z * 15) + x)] = f;
    }

    void Explode(byte fire_power,GameObject bomb)
    {
        GameObject Instance_FlameDir_N;
        GameObject Instance_FlameDir_S;
        GameObject Instance_FlameDir_W;
        GameObject Instance_FlameDir_E;
        GameObject Instance_FlameDir_M;
        Instance_FlameDir_M = Instantiate(m_flame_effect);
        Instance_FlameDir_M.transform.position = new Vector3(bomb.transform.position.x, 0.0f, bomb.transform.position.z );
        for (byte i = 0; i < fire_power; ++i)
        {
            Instance_FlameDir_N = Instantiate(m_flame_effect);
            Instance_FlameDir_N.transform.position = new Vector3(bomb.transform.position.x, 0.0f, bomb.transform.position.z + (2.0f * (i + 1)));
            Instance_FlameDir_S = Instantiate(m_flame_effect);
            Instance_FlameDir_S.transform.position = new Vector3(bomb.transform.position.x, 0.0f, bomb.transform.position.z - (2.0f * (i + 1)));
            Instance_FlameDir_W = Instantiate(m_flame_effect);
            Instance_FlameDir_W.transform.position = new Vector3(bomb.transform.position.x - (2.0f * (i + 1)), 0.0f, bomb.transform.position.z);
            Instance_FlameDir_E = Instantiate(m_flame_effect);
            Instance_FlameDir_E.transform.position = new Vector3(bomb.transform.position.x + (2.0f * (i + 1)), 0.0f, bomb.transform.position.z);

        }
        bomb.SetActive(false);
    }
    // Update is called once per frame
    void Update () {
		
	}
    IEnumerator CheckMap_v2()
    {

        for (;;)
        {
            for (int z = 0; z < 15; ++z)
            {
                for (int x = 0; x < 15; ++x)
                {
                    byte Tile_Info2 = copy_map_info[(z * 15) + (x)];
                    switch (Tile_Info2)
                    {
                        case 1: //Bomb
                            if(!bomb_list[(z * 15) + (x)].activeInHierarchy)
                            {
                                bomb_list[(z * 15) + (x)].SetActive(true);
                                Debug.Log("Made Bomb");
                            }
                            else if (bombexplode_list[(z * 15) + (x)] != 0)
                            {
                                Explode(bombexplode_list[(z * 15) + (x)], bomb_list[((z * 15) + x)]);
                                bombexplode_list[(z * 15) + (x)] = 0;
                            }
                            
                            break;
                        case 2: //Nothing
                                //Debug.Log("Nothing:" + x+","+y);
                            if (bombexplode_list[(z * 15) + (x)] != 0)
                            {
                                Explode(bombexplode_list[(z * 15) + (x)], bomb_list[((z * 15) + x)]);
                                bombexplode_list[(z * 15) + (x)] = 0;
                            }
                            box_list[(z * 15) + (x)].SetActive(false);
                            rock_list[(z * 15) + (x)].SetActive(false);
                            item_b_list[(z * 15) + (x)].SetActive(false);
                            bush_list[(z * 15) + (x)].SetActive(false);
                            item_s_list[(z * 15) + (x)].SetActive(false);
                            item_f_list[(z * 15) + (x)].SetActive(false);
                            break;
                        case 3: //Cube(Box)로
                            if (!box_list[(z * 15) + (x)].activeInHierarchy)
                                box_list[(z * 15) + (x)].SetActive(true);
                            break;
                        case 4: //Rock
                            if (!rock_list[(z * 15) + (x)].activeInHierarchy)
                                rock_list[(z * 15) + (x)].SetActive(true);
                            break;
                        case 5: //Item_Bomb
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_b_list[(z * 15) + (x)].activeInHierarchy)
                                    item_b_list[(z * 15) + (x)].SetActive(true);
                            break;
                        case 6:
                            if (!bush_list[(z * 15) + (x)].activeInHierarchy)
                                bush_list[(z * 15) + (x)].SetActive(true);
                            break;
                        case 7:
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_f_list[(z * 15) + (x)].activeInHierarchy)
                                item_f_list[(z * 15) + (x)].SetActive(true);
                            break;
                        case 8:
                            box_list[(z * 15) + (x)].SetActive(false);
                            if (!item_s_list[(z * 15) + (x)].activeInHierarchy)
                                item_s_list[(z * 15) + (x)].SetActive(true);
                            break;
                        default:
                            
                            //rock_list[(z * 15) + (x)].SetActive(true);
                            break;

                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

}
