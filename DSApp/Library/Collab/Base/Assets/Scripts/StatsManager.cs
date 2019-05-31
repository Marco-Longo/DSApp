using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour {

    [SerializeField]
    DBConnection db;

    public static StatsManager instance;

    private void Awake()
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
    }

    void Start () {
		
	}
	
	void Update () {
		
	}

}
