using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatsManager : MonoBehaviour {

    [SerializeField]
    DBConnection db;

    public static StatsManager instance;

    public Text CQGameLastSessionError;

    private void Awake()
    {
        if (instance == null)                                           //Singleton
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CQGameLastSessionError.text += db.getStat("CQGame", "LastError");
    }

}
