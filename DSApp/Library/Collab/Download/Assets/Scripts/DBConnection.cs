using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class DBConnection : MonoBehaviour {

    public static DBConnection instance;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader;

    public string choice;

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

        string conn = "URI=file:" + Application.dataPath + "/Resources/Question.db";            //Path to database.
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
    }
    
    public string ss_getQuestion(int n)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT text " + "FROM SentenceSplit WHERE id = " + n ;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        string s = "";

        while (reader.Read())
            s = reader.GetString(0);

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }

    public string getAnswer(string game, int n)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT answer " + "FROM " + game + " WHERE id = " + n;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        string s = "";

        while (reader.Read())
            s = reader.GetString(0);

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }

    public string[] getQuestion(int n, string game, bool elements)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT text, answer " + " FROM " + game + " WHERE id = " + n;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        string[] s = new string[2];

        while (reader.Read())
        {
            s[0] = reader.GetString(0);
            s[1] = reader.GetString(1);
        }

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }

    public string getQuestion(int n, string game)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT text " + "FROM " + choice + " WHERE id = " + n;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        string s = "";

        while (reader.Read())
            s = reader.GetString(0);

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }

    public int getCount(string s)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT COUNT(*) " + "FROM "+ s;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        int n = 0;

        while (reader.Read())
            n = reader.GetInt32(0);

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return n;
    }

    //STATS SECTION

    public void updateLastError(string gameName, int lastError)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "UPDATE Stats " + " SET LastError = " + lastError + " WHERE GameName = " + gameName;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;
    }

    public string getStat(string gameName, string attribute)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT " + attribute + " FROM Stats WHERE GameName = " + gameName;

        Debug.Log(sqlQuery);

        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        string s = "";

        while (reader.Read())
            s = reader.GetString(0);

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }


 }
