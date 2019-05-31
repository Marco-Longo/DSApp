using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    bool firstStart;
    bool flag;
    bool collect;

    string[] kids;

    public Button Locker;
    public Image[] LockIcons;

    GameObject statMan;
    public GameObject LockPos;

    int activePlayer;
    int activeScene;

    [SerializeField]
    DBConnection db;

    SpriteRenderer spriteRenderer;
    float cameraHeight;
    Vector2 cameraSize;
    Vector2 spriteSize;
    public Vector2 scale;

    void Awake ()
    {
        if (instance == null)                                           //Singleton
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        setFS(FSInit());
        flag = false;

    }

    void Start()
    {
        if (!firstStart)
        {
            updateKids();
        }

        collect = false;
        //activeScene = SceneManager.GetActiveScene();

    }

    void Update () {
        if(SceneManager.GetActiveScene().buildIndex == 3)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    bool FSInit() {

        cameraHeight = Camera.main.orthographicSize * 2;
        cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        spriteSize = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;

        scale = transform.localScale;

        if (cameraSize.x >= cameraSize.y)
        { // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        }
        else
        { // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }

        return db.getCount("Account") < 1 ? true : false;
    }
    
    
    // FirstStart 
    public void setFS(bool set) { firstStart = set; }
    public bool getFS() { return firstStart; }
    // ActivePlayer
    public int getActivePlayerIndex() { return activePlayer; }
    public string getActivePlayerName() { return kids[activePlayer]; }
    public void setActivePlayer(int n) { activePlayer = n; }
    
    public void updateKids() { kids = db.getKids(); }
    public string[] getKids() { return kids; }

    public void setCollect(bool tmp){ collect = tmp; }
    public bool getCollect(){ return collect; }

    public void setLock() { flag = !flag; }
    public bool getFlag() { return flag; }

    public string nameFormatter(string s) { return s = (s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower()); }
}