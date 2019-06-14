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
    public int activePlayer;
    bool firstStart;

    string[] games;

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

        //firstStart

        string conn = "URI=file:" + Application.dataPath + "/Resources/Question.db";            //Path to database.
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
    }

    void Start()
    {
        games = new string[19];

        games[0]	=	"AccentGame";
        games[1]	=	"AudioGame";
        games[2]	=	"CQGame";
        games[3]	=	"DoubleLetters";
        games[4]	=	"DTGrapheme"; 
        games[6]	=	"FVGrapheme";
        games[7]	=	"GraphemePic";
        games[5]	=	"HGame";
        games[8]	=	"InvertedGrapheme";
        games[9]	=	"PBGrapheme";
        games[10]	=	"SentenceSplit";
		games[11]	=	"GraphemeClouds";
		games[12]	=	"InvertedGraphemeSearch";
		games[13]	=	"SoundImages";
		games[14]	=	"CountWords";
		games[15]	=	"AccentWords";
		games[16]	=	"DoubleSentences";
		games[17]	=	"HSentences";
		games[18]	=	"CQSentences";
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

    // STATS SECTION

    public void updateStat(string gameName, string column , string toUpdate, string player)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "UPDATE Stats " + " SET " + column + " = " + toUpdate + " WHERE GameName = '" + gameName + "' AND Player = '" + player + "'";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;
    }

    public string getStat(string gameName, string attribute, string player)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT " + attribute + " FROM Stats WHERE GameName = '" + gameName + "' AND Player = '" + player + "'";   

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

    // ACCOUT CREATION

    public void addTutor(String s, String t, String q, String a)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "INSERT INTO Account ( Name, Password, is_Admin, SecretQuestion, SecretAnswer ) VALUES ( '" + s + "', '" + t + "', '1', '" + q + "', '" + a + "' )";

        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

    }

    public void addKid(String s)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "INSERT INTO Account ( Name, is_Admin ) VALUES ( '" + s + "', '0' )";

        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        initializeKidStats(s);
    }

    void initializeKidStats(string s)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery;

		for(int i = 0; i < games.Length; i++)
        {
            sqlQuery = "INSERT INTO Stats ( GameName, Player) VALUES ( '" + games[i] + "', '" + s + "' )";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
        }

        dbcmd.Dispose();
        dbcmd = null;
    }

    public void removeKid(string s)
    {
        dbcmd = dbconn.CreateCommand();

        string sqlQuery = "DELETE FROM Account WHERE is_Admin = 0 AND Name = '" + s + "'";

        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        removeStats(s);
    }

    void removeStats(string s)
    {
        dbcmd = dbconn.CreateCommand();

        string sqlQuery = "DELETE FROM Stats WHERE Player = '" + s + "'";

        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        removeDetStats(s);
    }
        
    public string[] getKids()
    {
        int kids = (getKidsNumber());

        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT Name FROM Account WHERE is_Admin = 0 ORDER BY Name";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        string[] s = new string[kids];

        int i = 0;

        do
        {
            while (reader.Read())
            {
                s[i++] = reader.GetString(0);
            }
        } while (reader.NextResult());

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }

    public int getKidsNumber()
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT COUNT(*) FROM Account WHERE is_Admin = 0";

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

    public bool findKid(string s)
    {
        bool aux = false;

        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT Name FROM Account WHERE is_Admin = 0 AND Name = '" + s + "'";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        if(reader.Read())
            aux = true;

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;
        
        return aux;
    }

    public string getPassword()
    {
        string s = "";
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT Password FROM Account WHERE is_Admin = 1";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            s = reader.GetString(0);
        }

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }
    
    public void changePassword(string s)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "UPDATE Account SET Password = '" + s + "' WHERE is_Admin = 1";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;
    }

    public void addStat(String Userame, String Game, String Date, String WrongAnsw, String Errors, String CorrectA, String Time, String Played)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery;

        sqlQuery = "INSERT INTO DetailedStats ( Username, Game, Date, WrongAnswers, Errors, CorrectAnswers, TotalTime, Played) VALUES ( '" + Userame + "', '" + Game + "', '" + Date + "', '" + WrongAnsw + "', '" + Errors + "', '" + CorrectA + "', '" + Time + "', '" + Played + "'  )";

        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

    }

    public void removeDetStats(string s)
    {
        dbcmd = dbconn.CreateCommand();

        string sqlQuery = "DELETE FROM DetailedStats WHERE Username = '" + s + "'";

        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;
    }

    public string[,] getDetStat(string User, string Game)
    {
        int rows = (countDetStat(User, Game));

        if (rows < 1)
        {
            Debug.Log("DB - Non ho niente");
            string[,] stmp = new string[1, 1];
            stmp[0, 0] = "empty";
            return stmp;
        }

        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT Date, WrongAnswers, Errors, CorrectAnswers, TotalTime, Played FROM DetailedStats WHERE Username = '" + User + "' AND Game = '" + Game + "'";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        string[,] s = new string[rows, 6];
        //Debug.Log("rows: "+ rows);

        int r = 0;
        do
        { 
            while (reader.Read())
            {
                s[r, 0] = reader.GetString(0);
                s[r, 1] = reader.GetString(1);
                s[r, 2] = reader.GetString(2);
                s[r, 3] = reader.GetString(3);
                s[r, 4] = reader.GetString(4);
                s[r, 5] = reader.GetString(5);
                r++;
            }
        } while (reader.NextResult());

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }

    public int countDetStat(string User, string Game)
    {
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT COUNT(*) FROM  DetailedStats WHERE Username = '" + User + "' AND Game = '" + Game +"'";
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

    public float getMaxDet(string User, string Game, string stat)
    {
        float s = 0;
        float tmp = 0;
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT " + stat + " FROM DetailedStats WHERE Username = '" + User + "' AND Game = '" + Game + "'";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            tmp = float.Parse(reader.GetString(0));
            if (s < tmp)
                s = tmp;
        }

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s;
    }

    public float getAvgDet(string User, string Game, string stat)
    {
        float s = 0;
        int count = countDetStat(User, Game);
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT " + stat + " FROM DetailedStats WHERE Username = '" + User + "' AND Game = '" + Game + "'";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            s += float.Parse(reader.GetString(0));
        }

        dbcmd.Dispose();
        dbcmd = null;
        reader.Close();
        reader = null;

        return s/count;
    }

}
