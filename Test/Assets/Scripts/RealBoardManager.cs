using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
//timescale로 인해 타임이 멈춰도 애니매이션 출력하는 방법 - If you are using "time.timeScale = 0" for pause, then this must help;
//Choose button/canvas/etc that you need -> Animator -> Update Mode -> Unscaled Time
public class RealBoardManager : MonoBehaviour {

    public GameObject enemyprefab;
    public GameObject enemyprefab2;
    public GameObject enemyprefab3;
    public GameObject enemyprefab3_1;
    public GameObject enemyprefabp;

    public Canvas c1;  // 메인
    public Canvas c2;  // 클리어
    public Canvas c3; // 게임오버
    public Canvas c4; //일시정지
    public Canvas c5; //스테이지 준비 땅 텍스트
   

    public GameObject objprefab;
    public GameObject enemy;
    public GameObject enemy2;
    public GameObject enemy3;

    public GameObject boss0;
    public GameObject boss1;
    public GameObject tile1;
    public GameObject tiletri1;
    public GameObject tiletri2;
    public GameObject tiletrire;
    public GameObject tiletrire2;
    public GameObject item;
    public GameObject itemprefab;
    

    public AudioSource bgm;
    public AudioSource bosssound;
    public AudioSource bosssound0;

    public AudioSource clearbgm;
    public AudioSource gameoverbgm;


    public GameObject player;
    public GameObject wallHor;
    public GameObject wallVer;
    public GameObject toiletwall;
    public GameObject toiletwall2;
    public GameObject toiletwallhor;
    public GameObject toiletwallhor2;
    public GameObject toiletwallver;
    public GameObject toiletwallver2;


    public Text obj_Left_Text;
    public GameObject obj;
    public Transform tr;
    public static int stage;
    public static int sizeofmap=3;
    Vector2 playert;
    public static int watercase=1; //  0 - 컵 1- 변기 2 - 어항
    public static int objclear=0;
    bool paused;
    // Use this for initialization
    private void Awake () {
        DontDestroyOnLoad(this);
        
        StartCoroutine("Docheck2");
        if(c1!=null)
            c1.enabled = true;
        if (c2 != null)
            c2.enabled = false;
        if (c3 != null)
            c3.enabled = false;
        if (c4 != null)
            c4.enabled = false;
        if (c5 != null)
            c5.enabled = true;
        paused = false;
        if (watercase == 0 && wallHor != null && wallVer != null)
        {
            CreateMap();

        }
        else if (watercase == 1 && toiletwall != null && toiletwall2 != null && toiletwallhor != null && toiletwallhor2 != null && toiletwallver != null && toiletwallver2 != null)
            CreateMap2();
        else if (watercase == 2 && toiletwall != null && toiletwall2 != null && toiletwallhor != null && toiletwallhor2 != null && toiletwallver != null && toiletwallver2 != null)
            CreateMap3();
        if (stage == 5 && watercase == 0)
        {
            /*
            if (plmove != null)
            {
                plmove.MakePlayerInvisible();
                
            }
            */
            if (boss0 != null && bosssound != null && bgm != null && clearbgm != null && gameoverbgm != null)
            {
                objclear = 1;
               
                bosssound0.enabled = true;
                bgm.enabled = false;
                bosssound.enabled = false;
                clearbgm.enabled = false;
                gameoverbgm.enabled = false;
                CreateBoss0();
                StartCoroutine(Docheck(8f));
                
            }
        }
        else if (stage == 5&&watercase==1)
        {
            if (boss1 != null&&bosssound!=null&&bgm!=null&&clearbgm!=null&&gameoverbgm!=null)
            {
                c5.enabled = false;
                objclear = 1;

                bosssound.enabled = true;
                bosssound0.enabled = false;
                bgm.enabled = false;
                clearbgm.enabled = false;
                gameoverbgm.enabled = false;
                CreateBoss();
                StartCoroutine(Docheck(8f));
            }
            
        }
        else
        {
            if (bosssound != null && bgm != null && clearbgm != null && gameoverbgm != null)
            {
                StartCoroutine(Docheck(0));

                bosssound0.enabled = false;

                bosssound.enabled = false;
                bgm.enabled = true;
                clearbgm.enabled = false;
                gameoverbgm.enabled = false;
            }
        }
        if(obj_Left_Text!=null)
            obj_Left_Text.text = "Object : " + objclear;

    }

    IEnumerator Docheck(float a)
    {
        if (a != 0)
        {
            yield return new WaitForSeconds(a);
        }
        for (;;)
        {
           
            CreateEnemy();
            if (stage >= 3)
            {
                CreateEnemy2();
            }
            if (stage >= 4)
            {
                CreateEnemy3();
            }
            if(watercase>=1&&stage>=2)
                CreateEnemy_Missile();
            if (stage==1)
                yield return new WaitForSeconds(5f);
            else if(stage==2)
                yield return new WaitForSeconds(4f);
            else if (stage == 3)
                yield return new WaitForSeconds(3f);
            else if (stage == 4)
                yield return new WaitForSeconds(2.5f);
            else
                yield return new WaitForSeconds(2.3f);
        }
    }
    IEnumerator Docheck2()
    {
        for (;;)
        {

            CreateLifeItem();
            yield return new WaitForSeconds(20f);
        }
    }
    
    void CreateMap2()
    {
        int ran = Random.Range(2, sizeofmap * 3 - 1);
        //변기 앞 통쪽
        for (int y = 0; y < sizeofmap*6; ++y)
        {
            for (int x = 0-y; x < sizeofmap*6; ++x)
            {
                if (x == sizeofmap * 6 - 1)
                {
                    if (y <ran ||y>=ran+(sizeofmap*2)) {
                        Vector2 pipewall2 = new Vector2(x * 2.56f, y * 2.56f);
                        Instantiate(toiletwallver, pipewall2, UnityEngine.Quaternion.identity);
                    }
                }
                if (y == 0)
                {
                    Vector2 wall = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(toiletwallhor, wall, UnityEngine.Quaternion.identity);
                }
                if (x == 0 - y)
                {
                    Vector2 wall = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(toiletwall2, wall, UnityEngine.Quaternion.identity);
                    Vector2 a = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(tiletri1, a, UnityEngine.Quaternion.identity);

                }
                else
                {
                    Vector2 a = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(tile1, a, tr.rotation);
                }
            }
        }
        
        for (int y = 0; y < sizeofmap*2; ++y)
        {
            for(int x = 0; x < sizeofmap*4; ++x)
            {
                //파이프 통로쪽
                
                if (y == 0)
                {
                    Vector2 pipewall = new Vector2(((sizeofmap * 6) + x) * 2.56f, ran * 2.56f + (y * 2.56f));
                    Instantiate(toiletwallhor, pipewall, UnityEngine.Quaternion.identity);
                }
                else if (y==sizeofmap*2-1)
                {

                    Vector2 pipewall = new Vector2(((sizeofmap * 6) + x) * 2.56f, ran * 2.56f + (y * 2.56f));
                    Instantiate(toiletwallhor2, pipewall, UnityEngine.Quaternion.identity);
                }
                Vector2 pipewater = new Vector2(((sizeofmap*6)+x)*2.56f, ran*2.56f+(y*2.56f));
                Instantiate(tile1, pipewater, UnityEngine.Quaternion.identity);
                //오르막 파이프
                if (y == 0)
                {
                    
                    Vector2 towall = new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + ((y + x) * 2.56f));
                    Instantiate(toiletwall, towall, UnityEngine.Quaternion.identity);
                    Vector2 pipewater2 = new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + ((y + x) * 2.56f));
                    Instantiate(tiletri2, pipewater2, UnityEngine.Quaternion.identity);
                }
                else if (y == sizeofmap * 2 - 1)
                {
                    Vector2 towall = new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + ((y + x+1) * 2.56f));
                    Instantiate(toiletwall, towall, UnityEngine.Quaternion.identity);
                    Vector2 pipewater3 = new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + ((y + x + 1) * 2.56f));
                    Instantiate(tiletrire2, pipewater3, UnityEngine.Quaternion.identity);
                    Vector2 pipewater2 = new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + ((y + x) * 2.56f));
                    Instantiate(tile1, pipewater2, UnityEngine.Quaternion.identity);
                }
                else
                {
                    Vector2 pipewater2 = new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + ((y + x) * 2.56f));
                    Instantiate(tile1, pipewater2, UnityEngine.Quaternion.identity);
                }
                if (x == sizeofmap * 4 - 1)
                {
                    //오브젝트 제작
                    Vector2 objj =new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + ((3*y) * 0.91f)+(x*2.56f));
                    Instantiate(objprefab, objj, UnityEngine.Quaternion.identity);
                    Vector2 objj2 = new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + (((3*y)+1) * 0.91f) + (x * 2.56f));
                    Instantiate(objprefab, objj2, UnityEngine.Quaternion.identity);
                    Vector2 objj3 = new Vector2(((sizeofmap * 10) + x) * 2.56f, ran * 2.56f + (((3*y)+2) * 0.91f) + (x * 2.56f));
                    Instantiate(objprefab, objj3, UnityEngine.Quaternion.identity);
                    Vector2 objj4 = new Vector2(((sizeofmap * 10) + x) * 2.56f + 1.01f, ran * 2.56f + (((3 * y)+1) * 0.91f) + (x * 2.56f));
                    Instantiate(objprefab, objj4, UnityEngine.Quaternion.identity);
                    Vector2 objj5 = new Vector2(((sizeofmap * 10) + x) * 2.56f + 1.01f, ran * 2.56f + (((3 * y) + 2) * 0.91f) + (x * 2.56f));
                    Instantiate(objprefab, objj5, UnityEngine.Quaternion.identity);
                    Vector2 objj6 = new Vector2(((sizeofmap * 10) + x) * 2.56f+1.01f, ran * 2.56f + (((3 * y) + 3) * 0.91f) + (x * 2.56f));
                    Instantiate(objprefab, objj6, UnityEngine.Quaternion.identity);
                    objclear = objclear + 6;
                }
                //내리막파이프
                Vector2 pipewater4 = new Vector2((sizeofmap*14)*2.56f+(x*1.28f),ran*2.56f+(sizeofmap*4+y-x)*2.56f);
                Instantiate(tile1, pipewater4, UnityEngine.Quaternion.identity);
            }
        }

    }
    void CreateMap3()
    {
        if (stage != 5)
            CreateObj_FishBowl();
        int ran = Random.Range(2, sizeofmap * 3 - 1);
        //어항 하단+상단
        for (int y = 0; y < sizeofmap * 8; ++y)
        {
            for (int x = 0 - y; x < sizeofmap * 8+y; ++x)
            {
                
                
                if (y == 0)
                {
                    if (x % (sizeofmap * 2)==0&&x!=0)
                    {
                        Vector2 a = new Vector2(x * 2.56f, y * 2.56f);
                        Instantiate(enemyprefabp, a, UnityEngine.Quaternion.identity);
                    }
                    Vector2 wall = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(toiletwallhor, wall, UnityEngine.Quaternion.identity);
                }
                if (x == sizeofmap * 8 - 1 + y)
                {

                    Vector2 wall = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(toiletwall, wall, UnityEngine.Quaternion.identity);
                    Vector2 a = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(tiletri2, a, UnityEngine.Quaternion.identity);
                    Vector2 wall2 = new Vector2(x * 2.56f, (2.56f * (sizeofmap * 20 - 1)) - y * 2.56f);
                    Instantiate(toiletwall2, wall2, UnityEngine.Quaternion.identity);
                    Vector2 a2 = new Vector2(x * 2.56f, (2.56f * (sizeofmap * 20 - 1)) - y * 2.56f);
                    Instantiate(tiletrire, a2, UnityEngine.Quaternion.identity);


                }
                else if (x == 0 - y)
                {
                    Vector2 wall = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(toiletwall2, wall, UnityEngine.Quaternion.identity);
                    Vector2 a = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(tiletri1, a, UnityEngine.Quaternion.identity);
                    Vector2 wall2 = new Vector2(x * 2.56f, (2.56f * (sizeofmap * 20 - 1)) - y * 2.56f);
                    Instantiate(toiletwall, wall2, UnityEngine.Quaternion.identity);
                    Vector2 a2 = new Vector2(x * 2.56f, (2.56f * (sizeofmap * 20 - 1)) - y * 2.56f);
                    Instantiate(tiletrire2, a2, UnityEngine.Quaternion.identity);

                }
                else
                {
                    Vector2 b = new Vector2(x * 2.56f, (2.56f * (sizeofmap * 20 - 1)) - y * 2.56f);
                    Instantiate(tile1, b, tr.rotation);
                    Vector2 a = new Vector2(x * 2.56f, y * 2.56f);
                    Instantiate(tile1, a, tr.rotation);
                }
            }

        }
        for (int y = sizeofmap*8; y < sizeofmap * 12; ++y)
        {
            for (int x = 1-sizeofmap*8; x < sizeofmap * 16-1; ++x)
            {
                Vector2 a = new Vector2(x * 2.56f, y * 2.56f);
                Instantiate(tile1, a, tr.rotation);
            }
        }

    }
    void CreateObj()
    {

        for (int x=-0; x < sizeofmap*2-2; ++x)
        {

            int ran = Random.Range(0, sizeofmap*2 - 2);
            int ran2 = Random.Range(1, sizeofmap * 3 - 1);
            Vector2 objj = new Vector2(ran*2.56f,ran2*2.56f);
            Instantiate(objprefab, objj, UnityEngine.Quaternion.identity);
            objclear = objclear + 1;
        }
    }
    void CreateObj_FishBowl()
    {

        for (int x = -0; x < sizeofmap * 8 - 2; ++x)
        {

            int ran = Random.Range(0, sizeofmap * 8 - 2);
            int ran2 = Random.Range(1, sizeofmap * 20 - 1);
            Vector2 objj = new Vector2(ran * 2.56f, ran2 * 2.56f);
            Instantiate(objprefab, objj, UnityEngine.Quaternion.identity);
            objclear = objclear + 1;
        }
    }
    void CreateMap()
    {
        if(stage!=5)
            CreateObj();
        for(int x = -1; x < sizeofmap*2-1; ++x)
        {
            Vector2 b = new Vector2(x*2.56f,-1.28f);
            Instantiate(wallHor, b, UnityEngine.Quaternion.identity);
            for (int y = 0; y < sizeofmap *3; ++y)
            {
                
                Vector2 a = new Vector2(x*2.56f, y*2.56f);
                Instantiate(tile1, a, UnityEngine.Quaternion.identity);
                
            }
        }
        for (int y = 0; y < sizeofmap *3; ++y)
        {
            
            Vector2 d = new Vector2((((sizeofmap*2)-1)*2.56f)-1.28f, y * 2.56f);
            Instantiate(wallVer, d, UnityEngine.Quaternion.identity);
            Vector2 c = new Vector2(-(1*2.56f)-1.5f, y * 2.56f);
            Instantiate(wallVer, c, UnityEngine.Quaternion.identity);
        }

    }
    void CreateBoss()
    {
        if (player != null)
        {
            Vector2 a = new Vector2(player.transform.position.x + 15f, 75f);
            enemy = (GameObject)Instantiate(boss1, a, UnityEngine.Quaternion.identity);
        }
    }
    void CreateBoss0()
    {
        
            Vector2 a = new Vector2(player.transform.position.x + 10f, 10f);
            enemy2 = (GameObject)Instantiate(boss0, a, UnityEngine.Quaternion.identity);
        
    }
    void CreateEnemy()
    {
        if (player != null)
        {
            Vector2 a = new Vector2(player.transform.position.x + Random.Range(10, 15), player.transform.position.y + Random.Range(-6, 6));
            enemy = (GameObject)Instantiate(enemyprefab, a, UnityEngine.Quaternion.identity);
            //enemy.GetComponent<Rigidbody2D>().velocity = firePoint.transform.forward * 20;
            Destroy(enemy, 20.0f);
        }
    }
    void CreateEnemy2()
    {
        if (player != null)
        {
            Vector2 a = new Vector2(player.transform.position.x + Random.Range(10, 15), player.transform.position.y + Random.Range(-6, 6));
            enemy2 = (GameObject)Instantiate(enemyprefab2, a, UnityEngine.Quaternion.identity);
            //enemy.GetComponent<Rigidbody2D>().velocity = firePoint.transform.forward * 20;
            Destroy(enemy2, 20.0f);
        }

    }
    void CreateEnemy3()
    {
        if (player != null)
        {
            Vector2 a = new Vector2(player.transform.position.x + Random.Range(13, 19), player.transform.position.y + Random.Range(-6, 6));
            enemy3 = (GameObject)Instantiate(enemyprefab3_1, a, UnityEngine.Quaternion.identity);
            //enemy.GetComponent<Rigidbody2D>().velocity = firePoint.transform.forward * 20;
            Destroy(enemy3, 20.0f);
        }
    }
    void CreateEnemy_Missile()
    {
        if (player != null)
        {
            Vector2 a = new Vector2(player.transform.position.x + Random.Range(20, 35), player.transform.position.y + Random.Range(-7, 7));
            enemy2 = (GameObject)Instantiate(enemyprefab3, a, UnityEngine.Quaternion.identity);
            //enemy.GetComponent<Rigidbody2D>().velocity = firePoint.transform.forward * 20;
            Destroy(enemy2, 20.0f);
        }
    }
    void CreateLifeItem()
    {
        if (player != null)
        {
            Vector2 b = new Vector2(player.transform.position.x + Random.Range(10, 15), player.transform.position.y + Random.Range(3, 5));
            item = (GameObject)Instantiate(itemprefab, b, UnityEngine.Quaternion.identity);
            //enemy.GetComponent<Rigidbody2D>().velocity = firePoint.transform.forward * 20;
            Destroy(item, 10.0f);
        }
    }
    public void showStageInfoT()
    {
        stage = 0;
    }
    public void showStageInfo1()
    {
        stage = 1;

    }
    public void showStageInfo2()
    {
        stage = 2;
    }
    public void showStageInfo3()
    {
        stage = 3;
    }
    public void showStageInfo4()
    {
        stage = 4;
    }
    public void showStageInfo5()
    {
        stage = 5;

    }
    public void waterCaseCup()
    {
        watercase = 0;
        sizeofmap = stage * 3;
    }
    public void waterCaseToilet()
    {
        watercase = 1;
        sizeofmap = stage * 1;
    }
    public void waterCaseFishbowl()
    {
        watercase = 2;
        sizeofmap = stage * 1;
    }
   
    public void BossDamaged()
    {
        objclear = objclear - 1;
        obj_Left_Text.text = "Boss HP : " + objclear;
    }
    public void swallowObj()
    {
        objclear = objclear - 1;
        obj_Left_Text.text = "Object : " + objclear;
    }

    public void clearSwap()
    {
        /*
        if(plmove!=null)
            plmove.MakePlayerInvisible();
            */
        //Time.timeScale = 0;
        if (bgm != null&&c2!=null)
        {
            bgm.Stop();
            bosssound.Stop();
            bosssound0.Stop();
            clearbgm.enabled = true;

            c1.enabled = false;
            c2.enabled = true;
        }
    }
    public void Stage1load()
    {

        Invoke("Stage1load2", 2f);
    }


    public void Startload()
    {
        Time.timeScale = 1;
        bgm.Stop();
        bosssound.Stop();
        bosssound0.Stop();
        SceneManager.LoadScene(0);
    }
    public void Selectload()
    {
        
        Time.timeScale = 1;
        
        if (bosssound != null)
        {
            bgm.Stop();
            bosssound.Stop();
            bosssound0.Stop();
        }
        SceneManager.LoadScene(1);
    }
    public void Stage1load2()
    {
        if(stage!=0)
            SceneManager.LoadScene(2);
        else
            SceneManager.LoadScene(3);
    }
    public void Gameover()
    {
        
        bgm.Stop();
        bosssound.Stop();
        bosssound0.Stop();
        gameoverbgm.enabled = true;
        c1.enabled = false;
        c3.enabled = true;
    }
    public void GameTimeAlways1()
    {
        Time.timeScale = 1;
    }
    public void pauseGameOrReturn()
    {
        if (paused)
        {
            c4.enabled = false;
            c1.enabled = true;
            Time.timeScale = 1;
            paused = false;
        }
        else {
            c4.enabled = true;
            c1.enabled = false;
            Time.timeScale = 0;
            paused = true;
        }
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.End))
        {
           
            CreateEnemy();
        }
       
        
        
        
        if (objclear <= 0)
        {
            clearSwap();
        }
    }
}
