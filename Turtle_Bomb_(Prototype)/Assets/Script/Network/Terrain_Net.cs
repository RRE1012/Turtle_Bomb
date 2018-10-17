using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Terrain_Net : MonoBehaviour
{
    public Terrain m_terrain;
    public Texture2D[] m_terrainmat;
    List<TreeInstance> tree_list = new List<TreeInstance>();
    public GameObject[] m_objects;
    GameObject[] mapObject = new GameObject[12];
    // Use this for initialization
    void Start()
    {

        SplatPrototype[] tex = new SplatPrototype[1];
        //Debug.Log(VariableManager.instance.map_type);
        tex[0] = new SplatPrototype();
        tex[0].texture = m_terrainmat[VariableManager.instance.map_type];
        tex[0].tileSize = new Vector2(15, 15);

        //tex[0].texture = m_terrainmat[1];
        m_terrain.terrainData.splatPrototypes = tex;
        for (int i = 0; i < 6; ++i)
        {

            mapObject[i] = Instantiate(m_objects[VariableManager.instance.map_type * 2]);
            if (i % 3 == 0)
                mapObject[i].transform.position = new Vector3(39.0f, 0.0f, i * 4);
            else if (i % 3 == 1)
                mapObject[i].transform.position = new Vector3(-10.0f + i, 0.0f, i * 3);
            else
                mapObject[i].transform.position = new Vector3(i * 3, 0.0f, 32.0f + i);
            mapObject[i + 6] = Instantiate(m_objects[VariableManager.instance.map_type * 2 + 1]);
            if (i % 3 == 2)
            {
                mapObject[i + 6].transform.position = new Vector3(38.0f, 0.0f, i * 4);
                mapObject[i + 6].transform.Rotate(0, -90.0f, 0);
            }
            else if (i % 3 == 0)
            {
                mapObject[i + 6].transform.position = new Vector3(-7.0f, 0.0f, i * 3);
                mapObject[i + 6].transform.Rotate(0, 90.0f, 0);
            }
            else
            {
                mapObject[i + 6].transform.position = new Vector3(i * 5, 0.0f, 32.0f);
                mapObject[i + 6].transform.Rotate(0, 180.0f, 0);
            }

        }

        //VariableManager.instance.map_type
        if (VariableManager.instance.map_type > 0)
        {

            for (int i = 0; i < m_terrain.terrainData.treeInstanceCount; ++i)
            {
                TreeInstance treeInstance = m_terrain.terrainData.GetTreeInstance(i);
                treeInstance.heightScale = 0.0f;
                treeInstance.widthScale = 0.0f;
                tree_list.Add(treeInstance);
                //Destroy(treeInstance);
            }
            tree_list.RemoveRange(0, m_terrain.terrainData.treeInstanceCount);
            float[,] heights = m_terrain.terrainData.GetHeights(0, 0, 0, 0);
            m_terrain.terrainData.SetHeights(0, 0, heights);

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
