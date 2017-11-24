using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Pool : MonoBehaviour {
    
    public static Object_Pool obj_pool;
    public int character_amount = 4; //캐릭터 수
    public int bomb_amount = 11; //캐릭터 별 폭탄 설치 개수
    public int box_amount = 12 * 12*3;
    public int boxu_amount = 12 * 12 * 3;
    public int enemy_amount = 12 * 12 * 3;

    int count = 0;
    public int wall_amount = 36;
    static int poligon_count;
    public GameObject bomb;
    public GameObject character;
    public GameObject box;
    public GameObject boxu;
    public GameObject enemy01;
    public GameObject wall;
    public static Mesh viewedModel;
    List<GameObject> pooled_objects;
    List<GameObject> pooled_objects_Box;
    List<GameObject> pooled_objects_BoxU;

    List<GameObject> pooled_objects_Wall;
    List<GameObject> pooled_objects_Chr;
    List<GameObject> pooled_objects_Enemy;

    MeshFilter viewedModelFilter;
    //싱글톤
    void Awake()
    {
        if (null != obj_pool)
        {
            return;
        }
        obj_pool = this;
        DontDestroyOnLoad(gameObject);
        

    }
    void Start()
    {

        pooled_objects = new List<GameObject>();
        pooled_objects_Box = new List<GameObject>();
        pooled_objects_BoxU = new List<GameObject>();

        pooled_objects_Wall = new List<GameObject>();
        pooled_objects_Chr = new List<GameObject>();
        pooled_objects_Enemy = new List<GameObject>();
        viewedModelFilter = (MeshFilter)character.GetComponent("MeshFilter");
        //viewedModel = viewedModelFilter.mesh;
        for (int i = 0; i < character_amount*bomb_amount; ++i)
        {
            GameObject obj = (GameObject)Instantiate(bomb);
            //poligon_count = poligon_count+ obj.GetComponent<MeshFilter>().mesh.triangles.Length / 3;
            obj.SetActive(false);
            pooled_objects.Add(obj);
        }
        for(int i=0;i<box_amount;++i)
        {
            GameObject obj2 = (GameObject)Instantiate(box);
            poligon_count= poligon_count + obj2.GetComponent<MeshFilter>().mesh.triangles.Length / 3;
            obj2.SetActive(false);
            pooled_objects_Box.Add(obj2);
        }
        for (int i = 0; i < boxu_amount; ++i)
        {
            GameObject obju = (GameObject)Instantiate(boxu);
            poligon_count = poligon_count + obju.GetComponent<MeshFilter>().mesh.triangles.Length / 3;
            obju.SetActive(false);
            pooled_objects_BoxU.Add(obju);
        }
        for (int i=0;i<character_amount;++i)
        {
            GameObject obj3 = (GameObject)Instantiate(character);
            //Debug.Log(obj3.GetComponent<Mesh>().triangles.Length / 3);
            //poligon_count = poligon_count + obj3.GetComponent<Mesh>().triangles.Length / 3;
            poligon_count = poligon_count + 2676;
            obj3.SetActive(false);
            pooled_objects_Chr.Add(obj3);
        }
        for (int i = 0; i < enemy_amount; ++i)
        {
            GameObject obje = (GameObject)Instantiate(enemy01);
            poligon_count = poligon_count + obje.GetComponent<MeshFilter>().mesh.triangles.Length / 3;
            obje.SetActive(false);
            pooled_objects_BoxU.Add(obje);
        }
    }

    void OnAble()
    {
        Invoke("SetableBomb", 1.0f);
    }
    void SetAbleBomb()
    {
        for(int i=0; i < pooled_objects.Count; ++i)
        {
            if (!pooled_objects[i].activeInHierarchy)
            {
                pooled_objects[i].transform.position = character.transform.position;
                pooled_objects[i].transform.rotation = character.transform.rotation;

                pooled_objects[i].SetActive(true);
                return;
            }
        }
    }
    
    
    public GameObject GetPoolObject_Bomb()
    {
        for(int i =0;i<pooled_objects.Count;++i)
        {
            if(!pooled_objects[i].activeInHierarchy)
            {
                return pooled_objects[i];
            }
        }
        return null;
    }
    public GameObject GetPoolObject_Enemy01()
    {
        for (int i = 0; i < pooled_objects.Count; ++i)
        {
            if (!pooled_objects_Enemy[i].activeInHierarchy)
            {
                return pooled_objects_Enemy[i];
            }
        }
        return null;
    }
    public GameObject GetPoolObject_Box()
    {
        for (int i = 0; i < pooled_objects_Box.Count; ++i)
        {
            if (!pooled_objects_Box[i].activeInHierarchy)
            {
                return pooled_objects_Box[i];
            }
        }
        
        return null;
    }
    public GameObject GetPoolObject_BoxU()
    {
        for (int i = 0; i < pooled_objects_BoxU.Count; ++i)
        {
            if (!pooled_objects_BoxU[i].activeInHierarchy)
            {
                return pooled_objects_BoxU[i];
            }
        }
        return null;
    }
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(w*3/4, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        
      
        string text = string.Format("poligon count : {0} ", poligon_count);
        GUI.Label(rect, text, style);
    }
}
