using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Terrain_Net : MonoBehaviour {
    public Terrain m_terrain;
    public Texture2D[] m_terrainmat;

	// Use this for initialization
	void Start () {
        SplatPrototype[] tex = new SplatPrototype[1];
        Debug.Log(VariableManager.instance.map_type);
        tex[0] = new SplatPrototype();
        tex[0].texture = m_terrainmat[VariableManager.instance.map_type];
        tex[0].tileSize = new Vector2(15, 15);
        //tex[0].texture = m_terrainmat[1];
        m_terrain.terrainData.splatPrototypes = tex;
		//VariableManager.instance.map_type
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
