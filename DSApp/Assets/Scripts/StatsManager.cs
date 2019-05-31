using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour {

    [SerializeField] GameObject cover;

    public static StatsManager instance;

    DBConnection db;
    GameManager gm;

    public GameObject Main;
    public GameObject StatsSection;
    public GameObject DetailedStatsSection;
    public GameObject TutorSection;
    public GameObject TutorMenu;
    public GameObject MainPanel;

    public InputField tutorPass;
    public InputField kidName;

    public InputField[] passwords;   // 1. new       2. confirm       3. old

    public Dropdown statsChoice;
    public Dropdown kidChoice;
    public Dropdown kidTMChoice;

    public Text MacroCat;
    public Text Error;
    public Text AlertMsg;
    public Text YesNo;
    public Text DetStatKidName;
    public Text alertDet;

    public Text[] MiniGameName;
    public Button[] MiniGameButton;

    public Button Next;
    public Button Prev;
    public Button BackStats;

    public Text[] GameStats1;
    public Text[] GameStats2;
    public Text[] GameStats3;
    public Text[] GameStats4;
    public Text[] GameStats5;
    public Text[] GameStats6;

    public Slider[] Bars;
    public Text[] TextBars;
    public Text[] Timer;
    public Text[] Date;
    public Text[] YAxis;
    public GameObject[] Sliders;
    public Button LegendButtonsAudio;

    int maxpage = 2;
    int page;
    int scene;
    int rows;
    int detPages;
    int detLastPage;
    
    float maxErr, maxCorrect, maxWrong, maxPlay;

    string sceneName;

    string[] kids;
    string[,] detailedStats;

    bool kidMod;
    bool statView = true;

    public void NextPage() {
        if (page == maxpage)
            page = 0;
        else
            page++;
    }
    public void PrevPage() {
        if (page == 0)
            page = maxpage;
        else
            page--;
    }
    
    public void EnableCover() { cover.SetActive(true); }
    public void DisableCover() { cover.SetActive(false); }

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        db = GameObject.FindGameObjectWithTag("DBManager").GetComponent<DBConnection>();

        if (instance == null)                                           //Singleton
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        page = 0;
        kidMod = false;

    }

    private void Start()
    {
        kids = gm.getKids();

        if (gm.GetComponent<GameManager>().getFS())
            { SceneManager.LoadScene("1. Opening"); }
        else
        {
            StartCoroutine(FadeIn());

            //activePlayers = db.getCount("Account");
        }

        passwords[0].inputType = InputField.InputType.Password;
        passwords[1].inputType = InputField.InputType.Password;
        passwords[2].inputType = InputField.InputType.Password;
        tutorPass.inputType = InputField.InputType.Password;

        gm.setCollect(false);

        setDropDown(kids, db.getCount("Account")-1 );
        BackStats.onClick.AddListener(CloseStats);
        Next.onClick.AddListener(NextPage);
        Prev.onClick.AddListener(PrevPage);
    }

    private void Update()
    {
        if (statView)
            PageStats();
        //else
        //    showDetStat();

        
    }

    /**********************************/
    public void ShowStats()
    {
            StatsSection.SetActive(true);
            MainPanel.SetActive(false);
            TutorMenu.SetActive(false);
    }

    public void CloseStats()
    {
        StatsSection.SetActive(false);
        TutorMenu.SetActive(true);
        MainPanel.SetActive(true);
        AlertMsg.text = "";

    }

    public void setDropDown(string[] s, int n )
    {
        statsChoice.ClearOptions();
        kidChoice.ClearOptions();
        kidTMChoice.ClearOptions();

        List<string> names = new List<string>();

        for (int i = 0; i < n; i++)
        {
            names.Add(s[i]);
        }

        statsChoice.AddOptions(names);
        kidChoice.AddOptions(names);
        kidTMChoice.AddOptions(names);
    }

    void PageStats()
    {

        switch (page)
        {
            case 0:
                {
                    MiniGameButton[0].gameObject.SetActive(true);
                    MiniGameButton[1].gameObject.SetActive(true);
                    MiniGameButton[2].gameObject.SetActive(true);
                    MiniGameButton[3].gameObject.SetActive(true);
                    MiniGameButton[4].gameObject.SetActive(true);
					MiniGameButton[5].gameObject.SetActive(true);

                    MiniGameButton[0].onClick.RemoveAllListeners();
                    MiniGameButton[1].onClick.RemoveAllListeners();
                    MiniGameButton[2].onClick.RemoveAllListeners();
                    MiniGameButton[3].onClick.RemoveAllListeners();
                    MiniGameButton[4].onClick.RemoveAllListeners();
					MiniGameButton[5].onClick.RemoveAllListeners();

                    MiniGameButton[0].onClick.AddListener(delegate { selectDetStat(0); });
					MiniGameButton[1].onClick.AddListener(delegate { selectDetStat(11);}); //Grapheme Clouds
                    MiniGameButton[2].onClick.AddListener(delegate { selectDetStat(2); });
                    MiniGameButton[3].onClick.AddListener(delegate { selectDetStat(3); });
                    MiniGameButton[4].onClick.AddListener(delegate { selectDetStat(4); });
					MiniGameButton[5].onClick.AddListener(delegate { selectDetStat(1); });
                    
                    MacroCat.text = "Fonologici";      

                    MiniGameName[0].text = "Omiss/Agg Grafema Immagini";
                    GameStats1[0].text = "Errori ultima sessione: " + db.getStat("GraphemePic", "LastError", kids[statsChoice.value]);
                    GameStats1[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("GraphemePic", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats1[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("GraphemePic", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats1[3].text = "";

					MiniGameName[1].text = "Omiss/Agg Grafema Lettere Sparse";
					GameStats2[0].text = "Errori ultima sessione: " + db.getStat("GraphemeClouds", "LastError", kids[statsChoice.value]);
					GameStats2[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("GraphemeClouds", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
					GameStats2[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("GraphemeClouds", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
					GameStats2[3].text = "";

                    MiniGameName[2].text = "Sostituzione Grafema D - T";
                    GameStats3[0].text = "Errori ultima sessione: " + db.getStat("DTGrapheme", "LastError", kids[statsChoice.value]);
                    GameStats3[1].text = "Tempo ultima sessione: "  + (float.Parse(db.getStat("DTGrapheme", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats3[2].text = "Tempo totale giocato: "  + (float.Parse(db.getStat("DTGrapheme", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats3[3].text = "";

                    MiniGameName[3].text = "Sostituzione Grafema F - V";
                    GameStats4[0].text = "Errori ultima sessione: " + db.getStat("FVGrapheme", "LastError", kids[statsChoice.value]);
                    GameStats4[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("FVGrapheme", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats4[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("FVGrapheme", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats4[3].text = "";

                    MiniGameName[4].text = "Sostituzione Grafema P - B";
                    GameStats5[0].text = "Errori ultima sessione: " + db.getStat("PBGrapheme", "LastError", kids[statsChoice.value]);
                    GameStats5[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("PBGrapheme", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats5[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("PBGrapheme", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats5[3].text = "";

					MiniGameName[5].text = "Inversione Grafemi";
					GameStats6[0].text = "Errori ultima sessione: " + db.getStat("InvertedGrapheme", "LastError", kids[statsChoice.value]);
					GameStats6[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("InvertedGrapheme", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
					GameStats6[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("InvertedGrapheme", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
					GameStats6[3].text = "";
						
                    break;
                }
            case 1:
                {

                    MiniGameButton[5].gameObject.SetActive(false);
                    MiniGameButton[4].gameObject.SetActive(false);
                    
                    MiniGameButton[0].gameObject.SetActive(true);
                    MiniGameButton[1].gameObject.SetActive(true);
                    MiniGameButton[2].gameObject.SetActive(true);
                    MiniGameButton[3].gameObject.SetActive(true);

                    MiniGameButton[0].onClick.RemoveAllListeners();
                    MiniGameButton[1].onClick.RemoveAllListeners();
                    MiniGameButton[2].onClick.RemoveAllListeners();
                    MiniGameButton[3].onClick.RemoveAllListeners();

                    MiniGameButton[0].onClick.AddListener(delegate { selectDetStat(5); });
                    MiniGameButton[1].onClick.AddListener(delegate { selectDetStat(6); });
                    MiniGameButton[2].onClick.AddListener(delegate { selectDetStat(7); });
                    MiniGameButton[3].onClick.AddListener(delegate { selectDetStat(8); });

                    MacroCat.text = "Non Fonologici";
                    
                    MiniGameName[0].text = "Separa le parole:";
                    GameStats1[0].text = "Errori ultima sessione: " + db.getStat("SentenceSplit", "LastError", kids[statsChoice.value]);
                    GameStats1[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("SentenceSplit", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats1[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("SentenceSplit", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats1[3].text = "";

                    MiniGameName[1].text = "Indovina le parole: ";
                    GameStats2[0].text = "Errori ultima sessione: " + db.getStat("AudioGame", "LastError", kids[statsChoice.value]);
                    GameStats2[1].text = "Riproduzioni ultima sessione: " + db.getStat("AudioGame", "Other", kids[statsChoice.value]);
                    GameStats2[2].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("AudioGame", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats2[3].text = "Tempo totale giocato: " + (float.Parse(db.getStat("AudioGame", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");

                    MiniGameName[2].text = "Scambio Grafema CU-QU: ";
                    GameStats3[0].text = "Errori ultima sessione: " + db.getStat("CQGame", "LastError", kids[statsChoice.value]);
                    GameStats3[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("CQGame", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats3[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("CQGame", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats3[3].text = "";

                    MiniGameName[3].text = "Omissione/Aggiunta 'H': ";
                    GameStats4[0].text = "Errori ultima sessione: " + db.getStat("HGame", "LastError", kids[statsChoice.value]);
                    GameStats4[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("HGame", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats4[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("HGame", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats4[3].text = "";

                    break;
                }
            case 2:
                {

                    MiniGameButton[2].gameObject.SetActive(false);
                    MiniGameButton[3].gameObject.SetActive(false);
                    MiniGameButton[4].gameObject.SetActive(false);
                    MiniGameButton[5].gameObject.SetActive(false);

                    MiniGameButton[0].gameObject.SetActive(true);
                    MiniGameButton[1].gameObject.SetActive(true);

                    MiniGameButton[0].onClick.RemoveAllListeners();
                    MiniGameButton[1].onClick.RemoveAllListeners();

                    MiniGameButton[0].onClick.AddListener(delegate { selectDetStat(9); });
                    MiniGameButton[1].onClick.AddListener(delegate { selectDetStat(10); });

                    MacroCat.text = "Misti";

                    MiniGameName[0].text = "Omissione/Aggiunta Doppie: ";
                    GameStats1[0].text = "Errori ultima sessione: " + db.getStat("DoubleLetters", "LastError", kids[statsChoice.value]);
                    GameStats1[1].text = "Riproduzioni ultima sessione: " + db.getStat("DoubleLetters", "Other", kids[statsChoice.value]);
                    GameStats1[2].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("DoubleLetters", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats1[3].text = "Tempo totale giocato: " + (float.Parse(db.getStat("DoubleLetters", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");

                    MiniGameName[1].text = "Omissione/Aggiunta Accenti: ";
                    GameStats2[0].text = "Errori ultima sessione: " + db.getStat("AccentGame", "LastError", kids[statsChoice.value]);
                    GameStats2[1].text = "Tempo ultima sessione: " + (float.Parse(db.getStat("AccentGame", "LastTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats2[2].text = "Tempo totale giocato: " + (float.Parse(db.getStat("AccentGame", "TotalTime", kids[statsChoice.value])).ToString("n2") + "s");
                    GameStats2[3].text = "";
                    
                    break;
                }

        }
    }

    public void BackToMenu()
    {
        Main.SetActive(true);
        MainPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(650f, MainPanel.GetComponent<RectTransform>().sizeDelta.y);
        TutorMenu.SetActive(false);
        AlertMsg.text = "";
    }

    public bool CheckPassword()
    {
        if (String.Equals(tutorPass.text, db.getPassword()))
            return true;

        return false;
    }

    public void TutorContinue(){

        if (CheckPassword())
        {
            TutorMenu.SetActive(true);
            TutorSection.SetActive(false);

            MainPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1100f, MainPanel.GetComponent<RectTransform>().sizeDelta.y);

            tutorPass.text = "";
        }
        else
        {

            Error.text = "PASSWORD ERRATA";
            tutorPass.text = "";
        }
    }

    public void ShowTutorMenu()
    {
        Main.SetActive(false);
        TutorSection.SetActive(true);

        tutorPass.text = "";
    }

    public void TutorBack()
    {
        Main.SetActive(true);
        TutorSection.SetActive(false);
        AlertMsg.text = "";
        tutorPass.text = "";
    }

    public void KidSelect()
    {
        sceneName = "2. GameMenu";
        gm.setActivePlayer(kidChoice.value);
        StartCoroutine(FadeOut());
    }

    public void addKid()
    {
        string s = kidName.text;
        AlertMsg.color = Color.red;

        if (s.Length < 3)
        {
            AlertMsg.text = "Il nome deve avere almeno 3 lettere.";
        }
        else if(db.getCount("Account") > 12)
        {
            AlertMsg.text = "Bambino non inserito, numero massimo di bambini superato.";
        }
        else if (db.findKid(s))
        {
            AlertMsg.text = "Nome già esistente.";
        }
        else
        {
            AlertMsg.color = new Color(0.05f, 0.73f, 0f, 1f);

            db.addKid(gm.nameFormatter(s));
            gm.updateKids();
            kids = gm.getKids();

            setDropDown(kids, db.getCount("Account") - 1);

            AlertMsg.text = "Nuovo Bambino aggiunto.";
            kidMod = true;
        }

        kidName.text = "";
    }

    public void checkRemove()
    {
        string s = kids[kidTMChoice.value];

        YesNo.text = "Sei sicuro di voler rimuovere " + s + "?";

        YesNo.gameObject.SetActive(true);
    }

    public void removeKidNo() { YesNo.gameObject.SetActive(false); }

    public void removeKidYes()
    {
        string s = kids[kidTMChoice.value];
        AlertMsg.color = Color.red;


        if (db.findKid(s))
        {
            if (db.getCount("Account") < 3)
            {
                AlertMsg.text = "Impossibile rimuovere i dati dell'unico bambino presente";

                return;
            }

            AlertMsg.color = new Color(0.05f, 0.73f, 0f, 1f);

            db.removeKid(s);
            gm.updateKids();
            kids = gm.getKids();

            setDropDown(kids, db.getCount("Account") - 1);

            AlertMsg.text = "Bambino rimosso.";

            kidMod = true;
        }
        else
        {
            AlertMsg.text = "Nome non trovato.";
        }

        YesNo.gameObject.SetActive(false);
    }

    void GoToScene(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void ChangePW()
    {
        AlertMsg.color = Color.red;

        if (String.IsNullOrEmpty(passwords[0].text) || String.IsNullOrEmpty(passwords[1].text) || String.IsNullOrEmpty(passwords[2].text))
        {
            AlertMsg.text = "Tutti i campi devono essere compilati correttamente.";
        }
        else if (passwords[0].text.Length < 8) 
        {
            AlertMsg.text = "La nuova password deve avere almeno 8 caratteri.";

        }
        else if (!String.Equals(passwords[2].text, db.getPassword()))
        {
            AlertMsg.text = "La vecchia password non è corretta.";

        }
        else if (String.Equals(passwords[0].text, passwords[2].text))
        {
            AlertMsg.text = "La nuova e la vecchia password devono essere diverse.";

        }
        else if (!String.Equals(passwords[0].text, passwords[1].text))
        {
            AlertMsg.text = "La password nuova non combacia con la conferma.";

        }
        else if (!String.Equals(passwords[0].text, passwords[1].text))
        {
            AlertMsg.text = "La password nuova non non combacia con la conferma.";

        }
        else
        {
            AlertMsg.color = new Color(0.05f, 0.73f, 0f, 1f);

            db.changePassword(passwords[0].text);
            AlertMsg.text = "Password cambiata con successo!";

        }

        passwords[0].text = "";
        passwords[1].text = "";
        passwords[2].text = "";
    }
    // DETAILED STATS
    void backDet()
    {
        statView = true;
        BackStats.onClick.RemoveAllListeners();
        BackStats.onClick.AddListener(CloseStats);

        DetStatKidName.gameObject.SetActive(false);
        statsChoice.gameObject.SetActive(true);

        Next.onClick.RemoveAllListeners();
        Prev.onClick.RemoveAllListeners();
        Next.onClick.AddListener(NextPage);
        Prev.onClick.AddListener(PrevPage);

        alertDet.gameObject.SetActive(false);
        DetailedStatsSection.SetActive(false);

        YAxis[5].text = "";
        YAxis[0].text = "max";
        YAxis[1].text = "4/5";
        YAxis[2].text = "3/5";
        YAxis[3].text = "2/5";
        YAxis[4].text = "1/5";

    }

    void nextStat()
    {
        if (detPages == 0) return;

        if (page == detPages)
            page = 0;
        else
            page++;

        setBars();
    }

    void prevStat()
    {
        if (detPages == 0) return;

        if (page == 0)
            page = detPages;
        else
            page--;

        setBars();
    }
    
    void setBars()
    {
        Sliders[1].SetActive(true);
        Sliders[2].SetActive(true);
        Sliders[3].SetActive(true);
        Sliders[4].SetActive(true);

        int i = 0;

        if (page != detPages)
        {

            for (int r = 0; r < 5; r++)
            {
                int row = r + (page * 5);
                
                Date[r].text = detailedStats[row, 0];
                Timer[r].text = float.Parse(detailedStats[row, 4]).ToString("n2") + " s";
                TextBars[i].text = detailedStats[row, 2];
                Bars[i++].value = getBarHeight(int.Parse(detailedStats[row, 2]), maxErr);
                TextBars[i].text = detailedStats[row, 1];
                Bars[i++].value = getBarHeight(int.Parse(detailedStats[row, 1]), maxWrong);
                TextBars[i].text = detailedStats[row, 3];
                Bars[i++].value = getBarHeight(int.Parse(detailedStats[row, 3]), maxCorrect);

                if (maxPlay > 0)
                {
                    Bars[i].gameObject.SetActive(true);
                    TextBars[i].text = detailedStats[row, 5];
                    Bars[i++].value = getBarHeight(int.Parse(detailedStats[row, 5]), maxPlay);
                }
                else
                {

                    TextBars[i].text = "";
                    Bars[i++].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int r = 0; r < 5; r++)
            {
                if (r >= detLastPage)
                {
                    Sliders[r].SetActive(false);
                    Date[r].text = "";
                    Timer[r].text = "";

                    continue;
                }

                int row = r + (page * 5);

                String s = detailedStats[row, 0];
                //s = s.Replace(" ", "" + Environment.NewLine);
                Date[r].text = s;
                
                Timer[r].text = float.Parse(detailedStats[row, 4]).ToString("n2") + " s";
                TextBars[i].text = detailedStats[row, 2];
                Bars[i++].value = getBarHeight(int.Parse(detailedStats[row, 2]), maxErr);
                TextBars[i].text = detailedStats[row, 1];
                Bars[i++].value = getBarHeight(int.Parse(detailedStats[row, 1]), maxWrong);
                TextBars[i].text = detailedStats[row, 3];
                Bars[i++].value = getBarHeight(int.Parse(detailedStats[row, 3]), maxCorrect);

                if (maxPlay > 0)
                {
                    Bars[i].gameObject.SetActive(true);
                    TextBars[i].text = detailedStats[row, 5];
                    Bars[i++].value = getBarHeight(int.Parse(detailedStats[row, 5]), maxPlay);
                }
                else
                {

                    TextBars[i].text = "";
                    Bars[i++].gameObject.SetActive(false);
                }
            }
        }
    }

    void selectDetStat(int choice)
    {
        statView = false;
        string kidN = kids[statsChoice.value];
        DetStatKidName.gameObject.SetActive(true);
        DetStatKidName.text = kids[statsChoice.value];
        statsChoice.gameObject.SetActive(false);

        BackStats.onClick.RemoveAllListeners();
        BackStats.onClick.AddListener(backDet);

        Next.onClick.RemoveAllListeners();
        Prev.onClick.RemoveAllListeners();
        Next.onClick.AddListener(nextStat);
        Prev.onClick.AddListener(prevStat);

        //nome: statsChoice.value


        switch (choice)
        {
            case 0:
                {
                    MacroCat.text = "Omissione/Aggiunta Grafema Immagini";
                    DetStat(kidN, "GraphemePic");
                    break;
                }
            case 1:
                {
                    MacroCat.text = "Inversione Grafemi";
                    DetStat(kidN, "InvertedGrapheme");
                    break;
                }
            case 2:
                {
                    MacroCat.text = "Sostituzione Grafema D - T";
                    DetStat(kidN, "DTGrapheme");
                    break;
                }
            case 3:
                {
                    MacroCat.text = "Sostituzione Grafema F - V";
                    DetStat(kidN, "FVGrapheme");
                    break;
                }
            case 4:
                {
                    MacroCat.text = "Sostituzione Grafema P - B";
                    DetStat(kidN, "PBGrapheme");
                    break;
                }
            case 5:
                {
                    MacroCat.text = "Separa le parole";
                    DetStat(kidN, "SentenceSplit");
                    break;
                }
            case 6:
                {
                    MacroCat.text = "Indovina le parole";
                    DetStat(kidN, "AudioGame");
                    break;
                }
            case 7:
                {
                    MacroCat.text = "Scambio Grafema CU-QU";
                    DetStat(kidN, "CQGame");
                    break;
                }
            case 8:
                {
                    MacroCat.text = "Omissione/Aggiunta 'H'";
                    DetStat(kidN, "HGame");
                    break;
                }
            case 9:
                {
                    MacroCat.text = "Omissione/Aggiunta Doppie";
                    DetStat(kidN, "DoubleLetters");
                    break;
                }
            case 10:
                {
                    MacroCat.text = "Omissione/Aggiunta Accenti";
                    DetStat(kidN, "AccentGame");
                    break;
                }
			case 11:
				{
					MacroCat.text = "Omissione/Aggiunta Grafema Lettere Sparse";
					DetStat(kidN, "GraphemeClouds");
					break;
				}
        }
        //setBars();
    }

    void DetStat(string username, string game)
    {
        MiniGameButton[0].gameObject.SetActive(false);
        MiniGameButton[1].gameObject.SetActive(false);
        MiniGameButton[2].gameObject.SetActive(false);
        MiniGameButton[5].gameObject.SetActive(false);
        MiniGameButton[3].gameObject.SetActive(false);
        MiniGameButton[4].gameObject.SetActive(false);

        rows = db.countDetStat(username, game);

        if (rows < 1)
        {
            alertDet.gameObject.SetActive(true);
            return;
        }

        DetailedStatsSection.SetActive(true);

        detPages = (rows/5);
        detLastPage = (rows%5);
        if(detLastPage == 0) --detPages;
        page = 0;
        
        detailedStats = new string[rows, 6];
        detailedStats = db.getDetStat(username, game);

        maxWrong =   db.getMaxDet(username, game, "WrongAnswers");
        maxErr =     db.getMaxDet(username, game, "Errors");
        maxCorrect = db.getMaxDet(username, game, "CorrectAnswers");
        maxPlay =    db.getMaxDet(username, game, "Played");

        Timer[5].text = "MEDIE       Tempo: " + db.getAvgDet(username, game, "TotalTime").ToString("n2") + " s " + "   |   Errori : " + db.getAvgDet(username, game, "Errors").ToString("n2") + 
                        "   |   Risposte Sbagliate: " + db.getAvgDet(username, game, "WrongAnswers").ToString("n2") + "   |   Risposte Corrette: " + db.getAvgDet(username, game, "CorrectAnswers").ToString("n2");

        if (maxPlay > 0)
            Timer[5].text += "   |   Riproduzioni: " + db.getAvgDet(username, game, "Played").ToString("n2");

        setBars();
        checkPlay();
    }

    float getBarHeight(float stat, float max) { return (max == 0) ? 1 : stat / max;  }

    void checkPlay()
    {
        if (maxPlay < 1)
            LegendButtonsAudio.interactable = false;
        else
            LegendButtonsAudio.interactable = true;
    }

    public void changeYAxis(int change)
    {
        
        switch (change)
        {
            case 1:
                {
                    YAxis[5].text = "Corrette";
                    YAxis[0].text = "" +       maxCorrect.ToString("n2");
                    YAxis[1].text = "" + (maxCorrect*4/5).ToString("n2");
                    YAxis[2].text = "" + (maxCorrect*3/5).ToString("n2");
                    YAxis[3].text = "" + (maxCorrect*2/5).ToString("n2");
                    YAxis[4].text = "" + (maxCorrect*1/5).ToString("n2");
                    break;
                }
            case 2:
                {
                    YAxis[5].text = "Sbagliate";
                    YAxis[0].text = "" +       maxWrong.ToString("n2");
                    YAxis[1].text = "" + (maxWrong*4/5).ToString("n2");
                    YAxis[2].text = "" + (maxWrong*3/5).ToString("n2");
                    YAxis[3].text = "" + (maxWrong*2/5).ToString("n2");
                    YAxis[4].text = "" + (maxWrong*1/5).ToString("n2");
                    break;
                }
            case 3:
                {
                    YAxis[5].text = "Errori";
                    YAxis[0].text = "" +           maxErr.ToString("n2");
                    YAxis[1].text = "" + (maxErr * 4 / 5).ToString("n2");
                    YAxis[2].text = "" + (maxErr * 3 / 5).ToString("n2");
                    YAxis[3].text = "" + (maxErr * 2 / 5).ToString("n2");
                    YAxis[4].text = "" + (maxErr * 1 / 5).ToString("n2");
                    break;
                }
            case 4:
                {
                    YAxis[5].text = "Riprod.";
                    YAxis[0].text = "" +           maxPlay.ToString("n2");
                    YAxis[1].text = "" + (maxPlay * 4 / 5).ToString("n2");
                    YAxis[2].text = "" + (maxPlay * 3 / 5).ToString("n2");
                    YAxis[3].text = "" + (maxPlay * 2 / 5).ToString("n2");
                    YAxis[4].text = "" + (maxPlay * 1 / 5).ToString("n2");
                    break;
                }
        }
    }

    IEnumerator FadeIn()
    {
        float f = 1f;

        Color tmp = cover.GetComponent<Image>().color;

        do
        {
            f -= 0.02f;
            tmp.a = f;
            cover.GetComponent<Image>().color = tmp;
            yield return new WaitForSeconds(0.0001f);

        } while (f > 0f);

        cover.SetActive(false);

        yield return null;
    }

    IEnumerator FadeOut()
    {
        enabled = false;

        cover.SetActive(true);

        float f = 0f;

        Color tmp = cover.GetComponent<Image>().color;

        do
        {
            f += 0.02f;
            tmp.a = f;
            cover.GetComponent<Image>().color = tmp;
            yield return new WaitForSeconds(0.0001f);

        } while (f < 1f);

        yield return new WaitForSeconds(0.2f);

        GoToScene(sceneName);
        yield return null;
    }

    public void quit() { Application.Quit(); }

}
