using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class QuestionManager : MonoBehaviour {

    GameManager gm;
    DBConnection db;

    public GameObject cover;

    //generic
    public Text questionTxt;
    public Text result;
    public Text counter;

    public Button btnMenu;

    GameObject GO;

    int timecalled;

    int q_id;
    int other;
    int total;
    int index;
    int errorTmp;
    int errorCount;
    int wrongAnsw;

    float mainTimer;
    float singleTimer;
    float[] timerArray;

    bool[] qstDone;
    bool collect;

    string player;
    string sec;
    string q_type;
    string question;
    string answer;
    string[] str;
    string sceneName;

//    DateTime today;

    //Separate Sentence
    public InputField ss_answerTxt;
    public Button ss_btnSend;
    public Button ss_btnNext;
    string[] words;

    //HGame
    public Button h_btnYes;
    public Button h_btnNo;
    public Button h_btnNext;

    //CQGame    
    public Button cq_CU;
    public Button cq_QU;
    public Button cq_CQU;
    public Button cq_CCU;
    public Button cq_next;

    //AudioGame
    public InputField ag_answerTxt;
    public Button ag_btnSend;
    public Button ag_btnNext;
    public Button ag_play;
    public AudioClip[] ag_clips;
    int ag_answer;

    //DoubleLetter
    public InputField dl_answerTxt;
    public Button dl_btnSend;
    public Button dl_btnNext;
    public Button dl_play;
    public AudioClip[] dl_clips;
    int dl_answer;

    //AccentGame
    public Button acc_btnYes;
    public Button acc_btnNo;
    public Button acc_btnNext;

    //GraphemePic
    public InputField gp_answer;
    public Button gp_send;
    public Button gp_next;
    public Image[] gp_images;
    public Text gp_letters;
    int lenght;

	//GraphemeClouds
	public InputField gc_answer;
	public Button gc_send;
	public Button gc_next;
	public Image gc_cloud;
	public Text[] gc_letters;

    //InvertedGrapheme
    public Button ig_same;
    public Button ig_different;
    public Button ig_next;

	//InvertedGraphemeSearch
	public Button sg_next;
	public Text sg_question;
	public GameObject sg_matrix;
	public Button[] sg_buttons;
	private int sg_correctAnswers;
	private int sg_counter;

    //GraphemeExchg
    public Button ge_answerA;
    public Button ge_answerB;
    public Button ge_next;
    string ge_type;

	//SoundImages
	public Button si_next;
	public Button si_audio;
	public Text si_question;
	public Image[] si_images;
	public Sprite[] si_sprites;
	public Sprite si_correct;
	public Sprite si_wrong;
	public AudioClip[] si_clips;
	private string si_qstType;
	private string si_sound;
	private int si_correctAnswers;
	private int si_counter;

	//CountWords
	public Button cw_next;
	public Button cw_send;
	public InputField cw_answerText;

	//AccentWords
	public Button aw_next;
	public Button[] aw_buttons;
	private int aw_correctAnswers;
	private int aw_counter;

	//DoubleSentences
	public Button ds_next;
	public Button ds_answ1;
	public Button ds_answ2;

    //flags
    bool correct;
    bool toNext;
    bool active;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        db = GameObject.FindGameObjectWithTag("DBManager").GetComponent<DBConnection>();

        result.text = "";
        q_type = PlayerPrefs.GetString("choice");

        if ((String.Equals(q_type, "DTGrapheme")) || (String.Equals(q_type, "FVGrapheme")) || (String.Equals(q_type, "PBGrapheme")))
        {
            GO = GameObject.FindWithTag("GraphemeExchange");
        }
        else
            GO = GameObject.FindWithTag(q_type);

        GameObject.FindWithTag("AccentGame").SetActive(false);
        GameObject.FindWithTag("AudioGame").SetActive(false);
        GameObject.FindWithTag("CQGame").SetActive(false);
        GameObject.FindWithTag("DoubleLetters").SetActive(false);
        GameObject.FindWithTag("GraphemePic").SetActive(false);
		GameObject.FindWithTag("GraphemeClouds").SetActive (false);
		GameObject.FindWithTag("HGame").SetActive(false);
        GameObject.FindWithTag("InvertedGrapheme").SetActive(false);
		GameObject.FindWithTag("InvertedGraphemeSearch").SetActive(false);
		GameObject.FindWithTag("SentenceSplit").SetActive(false);
        GameObject.FindWithTag("GraphemeExchange").SetActive(false);
		GameObject.FindWithTag("SoundImages").SetActive(false);
		GameObject.FindWithTag("CountWords").SetActive(false);
		GameObject.FindWithTag("AccentWords").SetActive(false);
		GameObject.FindWithTag("DoubleSentences").SetActive(false);

        timecalled = 0;
    }

    void Start () {

        StartCoroutine(FadeIn());

        player = gm.getActivePlayerName();

        timerArray = new float[5];

        index = 1;

        questionTxt.text = "";
        result.text = "";
        btnMenu.onClick.AddListener(backToMenu);

        correct = false;
        toNext = false;
        active = false;

        collect = gm.getCollect();

        errorCount = errorTmp = wrongAnsw = other = 0;
    }
    
    void Update () {
        if (index < 6)
            updateTimer(active);
    }

    void GoToScene(string s)
    {
        SceneManager.LoadScene(s);
    }

    void loadScenario()
    {

        total = db.getCount(q_type);

        if ((String.Equals(q_type, "DTGrapheme")) || (String.Equals(q_type, "FVGrapheme")) || (String.Equals(q_type, "PBGrapheme")))
        {
            ge_type = q_type;
            q_type = "GraphemeExchange";
        }
        
        qstDone = new bool[total];

        switch (q_type)
        {
            case "SentenceSplit":
                {
                    GO.SetActive(true);                    

                    ss_btnSend.onClick.AddListener(ss_checkResult);
                    ss_btnNext.onClick.AddListener(ss_nextQuestion);

                    q_id = getNewQuestion();

                    question = db.getAnswer(q_type, q_id);
                    questionTxt.text += ss_toShow(question);
                    answer = ss_getCorrectAnswer(question);

                    //ss_answerTxt.text = answer;

                    break;
                }
            case "HGame":
                {
                    GO.SetActive(true);

                    q_id = getNewQuestion();
                    h_btnNext.onClick.AddListener(h_nextQuestion);

                    str = db.getQuestion(q_id, q_type, true);

                    questionTxt.text += str[0];
                    answer = str[1];

                    h_setButtons(answer);

                    break;
                }
            case "CQGame":
                {
                    GO.SetActive(true);

                    q_id = getNewQuestion();
                    str = db.getQuestion(q_id, q_type, true);

                    questionTxt.text += str[0];
                    answer = str[1];

                    cq_setButtons(answer);

                    cq_next.onClick.AddListener(cq_nextQuestion);

                    break;
                }
            case "AudioGame":
                {
                    GO.SetActive(true);

                    questionTxt.text = "";

                    q_id = getNewQuestion();

                    answer = db.getAnswer(q_type, q_id);

                    ag_play.GetComponent<AudioSource>().clip = ag_clips[q_id-1];
                    ag_play.onClick.AddListener(ag_playSound);
                    ag_btnSend.onClick.AddListener(ag_compare);
                    ag_btnNext.onClick.AddListener(ag_nextQuestion);

                    break;
                }
            case "DoubleLetters":
                {
                    GO.SetActive(true);

                    questionTxt.text = "";

                    q_id = getNewQuestion();

                    answer = db.getAnswer(q_type, q_id);

                    dl_play.GetComponent<AudioSource>().clip = dl_clips[q_id - 1];
                    dl_play.onClick.AddListener(dl_playSound);
                    dl_btnSend.onClick.AddListener(dl_compare);
                    dl_btnNext.onClick.AddListener(dl_nextQuestion);

                    break;
                }
            case "AccentGame":
                {
                    GO.SetActive(true);

                    q_id = getNewQuestion();
                    acc_btnNext.onClick.AddListener(acc_nextQuestion);

                    str = db.getQuestion(q_id, q_type, true);

                    questionTxt.text += str[0];
                    answer = str[1];

                    acc_setButtons(answer);

                    break;
                }
            case "GraphemeExchange":
                {
                    GO.SetActive(true);

                    q_type = ge_type;

                    q_id = getNewQuestion();

                    str = db.getQuestion(q_id, ge_type, true);

                    questionTxt.text += str[0];

                    ge_setButtons(str[1]);
                    ge_next.onClick.AddListener(ge_nextQuestion);

                    q_id = getNewQuestion();

                    break;
                }
            case "GraphemePic":
                {
                    GO.SetActive(true);

                    questionTxt.text = "";

                    q_id = getNewQuestion();

                    answer = db.getAnswer(q_type, q_id);

                    gp_images[q_id - 1].gameObject.SetActive(true);
                    
                    gp_send.onClick.AddListener(gp_checkResult);
                    gp_next.onClick.AddListener(gp_nextQuestion);

                    underscores(answer);

                    break;
                }

			case "GraphemeClouds":
				{
					GO.SetActive(true);
					questionTxt.text = "";

					q_id = getNewQuestion();

					str = db.getQuestion(q_id, q_type, true);
					gc_fillCloud(str[0]);
					answer = str[1];

					gc_send.onClick.AddListener(gc_checkResult);
					gc_next.onClick.AddListener(gc_nextQuestion);

					break;
				}

            case "InvertedGrapheme":
                {
                    GO.SetActive(true);
                    questionTxt.text = "";

                    q_id = getNewQuestion();
                    ig_next.onClick.AddListener(ig_nextQuestion);

                    str = db.getQuestion(q_id, q_type, true);

                    questionTxt.text += str[0];
                    answer = str[1];

                    ig_setButtons();

                    break;
                }

			case "InvertedGraphemeSearch":
				{
					GO.SetActive(true);
					questionTxt.text = "";

					q_id = getNewQuestion();
					sg_next.onClick.AddListener(sg_nextQuestion);

					str = db.getQuestion(q_id, q_type, true);
					answer = str[1];
					sg_question.text += answer;
					sg_setButtons(str[0]);
					
					break;
				}

			case "SoundImages":
				{
					GO.SetActive(true);
					questionTxt.text = "";
					
					q_id = getNewQuestion();
					si_next.onClick.AddListener(si_nextQuestion);

					str = db.getQuestion(q_id, q_type, true);
					answer = str[1]; //request+sound
					si_generateQuestion();
					si_fillArray(str[0]);

					break;
				}
			case "CountWords":
				{
					GO.SetActive(true);                    

					cw_send.onClick.AddListener(cw_checkResult);
					cw_next.onClick.AddListener(cw_nextQuestion);

					q_id = getNewQuestion();

					str = db.getQuestion(q_id, q_type, true);
					questionTxt.text += str[0];
					answer = str[1];

					break;
				}
			case "AccentWords":
				{
					GO.SetActive(true);                    

					aw_next.onClick.AddListener(aw_nextQuestion);

					q_id = getNewQuestion();

					str = db.getQuestion(q_id, q_type, true);
					answer = str[1];
					aw_generateButtons (str[0]);

					break;
				}
			case "DoubleSentences":
				{
					GO.SetActive(true);

					q_id = getNewQuestion();
					ds_next.onClick.AddListener(ds_nextQuestion);

					str = db.getQuestion(q_id, q_type, true);

					questionTxt.text += str[0];
					answer = str[1];

					ds_setButtons(answer);

					break;
				}
            default:
                {
                    break;
                }

        }


    }

    //Separate Sentence
    string ss_getCorrectAnswer(string s) { return s.Replace('+', ' '); }

    string ss_toShow(string s) { return s.Replace('+', '\b'); }

    string checkSpace(string s)
    {
        RegexOptions options = RegexOptions.None;
        Regex regex = new Regex("[ ]{2,}", options);
        s = regex.Replace(s, " ");
        return s;
    }

    public void ss_checkResult()
    {
        //ss_answerTxt.text = checkSpace(ss_answerTxt.text);

        if (String.Equals(ss_answerTxt.text, answer, StringComparison.OrdinalIgnoreCase))
        {
            result.text = "Corretto!";
            errorTmp = 0;

            questionTxt.GetComponent<AudioSource>().Play();
            //recordTime();

            correct = true;
            ss_btnSend.gameObject.SetActive(false);
            ss_btnNext.gameObject.SetActive(true);

            return;
        }

        if (errorTmp < 2)
		{
            result.text = "Riprova!";
            result.GetComponent<AudioSource>().Play();

            errorCount++;
            errorTmp++;
            wrongAnsw++;


            return;
        }

        result.text = "La frase corretta era:\n" + answer;
        recordTime();
        errorCount++;
        wrongAnsw++;

        ss_btnSend.gameObject.SetActive(false);
        ss_btnNext.gameObject.SetActive(true);

        result.GetComponent<AudioSource>().Play();

        ss_btnSend.interactable = false;

        correct = true;
        errorTmp = 0;

    }

    public void ss_nextQuestion()
    {
        checkCorrect();

        if (toNext == true && index < 5)
        {
            toNext = false;
            index++;

            q_id = getNewQuestion();
            question = db.getAnswer(q_type, q_id);
            questionTxt.text = ss_toShow(question);

            answer = ss_getCorrectAnswer(question);
            ss_answerTxt.text = "";
            result.text = "";
            ss_btnSend.interactable = true;
            active = true;

            ss_btnSend.gameObject.SetActive(true);
            ss_btnNext.gameObject.SetActive(false);


        }
        else if (index < 5 || !toNext)
        {
            result.text = "Devi prima rispondere correttamente alla domanda!";
        }
        else if (toNext)
        {
            GO.SetActive(false);

            questionTxt.text = "Hai completato le domande! Torna al menù!";

            result.text = "";
            if (collect)
            { 
                db.updateStat("SentenceSplit", "LastError", errorCount.ToString(), player);
                db.updateStat("SentenceSplit", "LastTime", mainTimer.ToString(), player);
                db.updateStat("SentenceSplit", "TotalTime", (float.Parse(db.getStat("SentenceSplit", "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, "SentenceSplit", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");
            }

            btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);

        }
    }

    //HGame
    void h_setButtons(string s)
    {
        h_btnNext.gameObject.SetActive(false);
        h_btnYes.gameObject.SetActive(true);
        h_btnNo.gameObject.SetActive(true);

        h_buttonsName(s);
        if (s.ToLower().Contains("h"))
        {
            h_btnYes.onClick.RemoveAllListeners();
            h_btnNo.onClick.RemoveAllListeners();
            h_btnYes.onClick.AddListener(h_correct);
            h_btnNo.onClick.AddListener(h_wrong);
        }
        else
        {
            h_btnYes.onClick.RemoveAllListeners();
            h_btnNo.onClick.RemoveAllListeners();
            h_btnYes.onClick.AddListener(h_wrong);
            h_btnNo.onClick.AddListener(h_correct);
        }
    }

    void h_buttonsName(string s)
    {
        
        Text yesTxt = h_btnYes.transform.Find("Text").gameObject.GetComponent<Text>();
        Text noTxt = h_btnNo.transform.Find("Text").gameObject.GetComponent<Text>();

        if (s.ToLower().Contains("h"))
        {
            yesTxt.text = s;
            noTxt.text = s.Substring(1);
        }
        else
        {
            yesTxt.text = "h" + s;
            noTxt.text = s;
        }
    }

    public void h_correct()
    {
        result.text = "Corretto!";
        questionTxt.text = questionTxt.text.Replace("_", answer);
        h_btnYes.onClick.RemoveAllListeners();
        h_btnNo.onClick.RemoveAllListeners();

        h_btnNext.gameObject.SetActive(true);
        h_btnYes.gameObject.SetActive(false);
        h_btnNo.gameObject.SetActive(false);

        recordTime();
        errorTmp = 0;

        questionTxt.GetComponent<AudioSource>().Play();

        correct = true;
    }

    public void h_wrong()
    {
        if (errorTmp == 2)
        {
            result.text = "La risposta è " + answer + "! Prova con la prossima!";

            questionTxt.text = questionTxt.text.Replace("_", answer);

            h_btnYes.onClick.RemoveAllListeners();
            h_btnNo.onClick.RemoveAllListeners();

            h_btnNext.gameObject.SetActive(true);
            h_btnYes.gameObject.SetActive(false);
            h_btnNo.gameObject.SetActive(false);

            errorCount++;
            wrongAnsw++;

            errorTmp = 0;
            recordTime();
            result.GetComponent<AudioSource>().Play();

            correct = true;
            return;
        }
        
        result.text = "Riprova!";
        result.GetComponent<AudioSource>().Play();
        errorCount++;
        errorTmp++;

    }

    public void h_nextQuestion()
    {
        checkCorrect(); 

        if (toNext == true && index < 5)
        {
            index++;

            toNext = false;

            q_id = getNewQuestion();
            str = db.getQuestion(q_id, q_type, true);

            questionTxt.text = index + ". " + str[0];
            answer = str[1];

            h_setButtons(answer);
            active = true;

            result.text = "";
        }
        else if (index < 5)
            result.text = "Devi prima rispondere correttamente alla domanda!";
        else
        {
            h_btnNext.onClick.RemoveAllListeners();
            GO.SetActive(false);

            questionTxt.text = "Hai completato le domande! Torna al menù!";

            result.text = "";

            if (collect)
            {
                db.updateStat("HGame", "LastError", errorCount.ToString(), player);
                db.updateStat("HGame", "LastTime", mainTimer.ToString(), player);
                db.updateStat("HGame", "TotalTime", (float.Parse(db.getStat("HGame", "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, "HGame", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");

            }


            btnMenu.transform.position = new Vector3(Screen.width/2, Screen.height/2 - 30f, 0f);
        }
    }

    //CQGame
    public void cq_correct()
    {
        result.text = "Corretto!";
        questionTxt.text = questionTxt.text.Replace("...", answer );
        cq_setButtons("rm");

        cq_next.gameObject.SetActive(true);
        cq_CU.gameObject.SetActive(false);
        cq_CCU.gameObject.SetActive(false);
        cq_QU.gameObject.SetActive(false);
        cq_CQU.gameObject.SetActive(false);

        questionTxt.GetComponent<AudioSource>().Play();

        recordTime();
        errorTmp = 0;

        correct = true;
    }

    public void cq_wrong()
    {
        if(errorTmp == 2)
        {
            cq_setButtons("rm");
            result.text = "La risposta è " + answer + "! Prova con la prossima!";

            questionTxt.text = questionTxt.text.Replace("...", answer);

            wrongAnsw++;
            errorCount++;

            cq_next.gameObject.SetActive(true);
            cq_CU.gameObject.SetActive(false);
            cq_CCU.gameObject.SetActive(false);
            cq_QU.gameObject.SetActive(false);
            cq_CQU.gameObject.SetActive(false);

            errorTmp = 0;
            recordTime();
            result.GetComponent<AudioSource>().Play();

            correct = true;
            return;
        }
        result.text = "Riprova!";
        result.GetComponent<AudioSource>().Play();
        errorCount++;
        errorTmp++;
    }

    void cq_setButtons(string s)
    {
        cq_next.gameObject.SetActive(false);
        cq_CU.gameObject.SetActive(true);
        cq_CCU.gameObject.SetActive(true);
        cq_QU.gameObject.SetActive(true);
        cq_CQU.gameObject.SetActive(true);

        switch (s.ToLower())
        {
            case "cu":
                {
                    cq_CU.onClick.AddListener(cq_correct);
                    cq_CCU.onClick.AddListener(cq_wrong);
                    cq_QU.onClick.AddListener(cq_wrong);
                    cq_CQU.onClick.AddListener(cq_wrong);

                    break;
                }
            case "qu":
                {
                    cq_QU.onClick.AddListener(cq_correct);
                    cq_CU.onClick.AddListener(cq_wrong);
                    cq_CCU.onClick.AddListener(cq_wrong);
                    cq_CQU.onClick.AddListener(cq_wrong);

                    break;
                }
            case "cqu":
                {
                    cq_CQU.onClick.AddListener(cq_correct);
                    cq_CU.onClick.AddListener(cq_wrong);
                    cq_QU.onClick.AddListener(cq_wrong);
                    cq_CCU.onClick.AddListener(cq_wrong);

                    break;
                }
            case "ccu":
                {
                    cq_CCU.onClick.AddListener(cq_correct);
                    cq_CU.onClick.AddListener(cq_wrong);
                    cq_QU.onClick.AddListener(cq_wrong);
                    cq_CQU.onClick.AddListener(cq_wrong);

                    break;
                }
            case "rm":
                {
                    cq_CU.onClick.RemoveAllListeners();
                    cq_QU.onClick.RemoveAllListeners();
                    cq_CQU.onClick.RemoveAllListeners();
                    cq_CCU.onClick.RemoveAllListeners();

                    break;
                }
        }
    }

    public void cq_nextQuestion()
    {
        checkCorrect();

        if (toNext == true && index < 5)
        {
            toNext = false;
            index++;

            q_id = getNewQuestion();
            str = db.getQuestion(q_id, q_type, true);

            questionTxt.text = str[0];
            answer = str[1];

            cq_setButtons(answer);
            active = true;
        
            result.text = "";

        }
        else if (index < 5 || !toNext)
            result.text = "Devi prima rispondere correttamente alla domanda!";
        else if(toNext)
        {
            cq_next.onClick.RemoveAllListeners();
            active = true;
            
            GO.SetActive(false);

            questionTxt.text = "Hai completato le domande! Torna al menù";

            result.text = "";

            if (collect)
            {
                db.updateStat("CQGame", "LastError", errorCount.ToString(), player);
                db.updateStat("CQGame", "LastTime", mainTimer.ToString(), player);
                db.updateStat("CQGame", "TotalTime", (float.Parse(db.getStat("CQGame", "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, "CQGame", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");

            }

            btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);

        }
    }

    //AudioGame
    public void ag_playSound()
    {
        if(ag_play.GetComponent<AudioSource>().isPlaying == false)
        {
            ag_play.GetComponent<AudioSource>().Play();
            other++;
        }
    }

    void ag_compare()
    {
        ag_answerTxt.text = checkSpace(ag_answerTxt.text);

        if (ag_answerTxt.text == answer)
        {
            recordTime();

            ag_btnSend.gameObject.SetActive(false);
            ag_btnNext.gameObject.SetActive(true);

            result.text = "Corretto!";
            questionTxt.GetComponent<AudioSource>().Play();
            errorTmp = 0;

            ag_btnSend.interactable = false;
            ag_play.interactable = false;

            correct = true;
            return;
        }

        if(errorTmp < 2)
        {
            result.text = "Riprova!";
            result.GetComponent<AudioSource>().Play();

            errorCount++;
            errorTmp++;

            return;
        }

        recordTime();

        result.text = "La risposta esatta è: " + answer;
        wrongAnsw++;
        errorCount++;
        errorTmp = 0;

        ag_btnSend.gameObject.SetActive(false);
        ag_btnNext.gameObject.SetActive(true);

        result.GetComponent<AudioSource>().Play();

        ag_btnSend.interactable = false;
        ag_play.interactable = false;

        correct = true;
    }

    public void ag_nextQuestion()
    {
        checkCorrect();

        if (toNext == true && index < 5)
        {
            toNext = false;
            index++;

            active = true;

            q_id = getNewQuestion();
            answer = db.getAnswer(q_type, q_id);

            ag_answerTxt.text = "";
            ag_btnSend.interactable = true;
            ag_play.interactable = true;
            ag_play.GetComponent<AudioSource>().clip = ag_clips[q_id - 1];

            errorTmp = 0;

            result.text = "";

            ag_btnSend.gameObject.SetActive(true);
            ag_btnNext.gameObject.SetActive(false);

        }
        else if (index < 5 || !toNext)
            result.text = "Devi prima rispondere correttamente alla domanda!";
        else if (toNext)
        {

            GO.SetActive(false);
            questionTxt.text = "Hai completato le domande! Torna al menù!";
            result.text = "";

            if (collect)
            {
                db.updateStat("AudioGame", "LastError", errorCount.ToString(), player);
                db.updateStat("AudioGame", "LastTime", mainTimer.ToString(), player);
                db.updateStat("AudioGame", "Other", other.ToString(), player);
                db.updateStat("AudioGame", "TotalTime", (float.Parse(db.getStat("AudioGame", "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, "AudioGame", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), other.ToString());

            }


            btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);
        }
    }

    //DoubleLetter
    public void dl_playSound()
    {
        if (dl_play.GetComponent<AudioSource>().isPlaying == false)
        {
            dl_play.GetComponent<AudioSource>().Play();
            other++;
        }
    }

    void dl_compare()
    {
        dl_answerTxt.text = checkSpace(dl_answerTxt.text);

        if (String.Equals(dl_answerTxt.text, answer, StringComparison.OrdinalIgnoreCase))
        {
            recordTime();

            result.text = "Corretto!";
            questionTxt.GetComponent<AudioSource>().Play();
            errorTmp = 0;

            dl_btnSend.gameObject.SetActive(false);
            dl_btnNext.gameObject.SetActive(true);
            dl_play.interactable = false;

            correct = true;
            return;
        }

        if (errorTmp < 2)
        {
            result.text = "Riprova!";
            result.GetComponent<AudioSource>().Play();

            errorCount++;
            errorTmp++;

            return;
        }

        recordTime();

        result.text = "La risposta esatta è: " + answer;
        errorCount++;
        errorTmp = 0;
        wrongAnsw++;

        dl_btnSend.gameObject.SetActive(false);
        dl_btnNext.gameObject.SetActive(true);

        result.GetComponent<AudioSource>().Play();

        dl_btnSend.interactable = false;
        dl_play.interactable = false;

        correct = true;
    }

    public void dl_nextQuestion()
    {
        checkCorrect();

        if (toNext == true && index < 5)
        {
            toNext = false;
            index++;

            active = true;

            q_id = getNewQuestion();
            answer = db.getAnswer(q_type, q_id);

            dl_answerTxt.text = "";
            dl_btnSend.interactable = true;
            dl_play.interactable = true;
            dl_play.GetComponent<AudioSource>().clip = dl_clips[q_id - 1];

            errorTmp = 0;

            result.text = "";

            dl_btnSend.gameObject.SetActive(true);
            dl_btnNext.gameObject.SetActive(false);

        }
        else if (index < 5 || !toNext)
            result.text = "Devi prima rispondere correttamente alla domanda!";
        else if (toNext)
        {

            GO.SetActive(false);
            questionTxt.text = "Hai completato le domande! Torna al menù!";
            result.text = "";

            if (collect)
            {
                db.updateStat("DoubleLetters", "LastError", errorCount.ToString(), player);
                db.updateStat("DoubleLetters", "LastTime", mainTimer.ToString(), player);
                db.updateStat("DoubleLetters", "Other", other.ToString(), player);
                db.updateStat("DoubleLetters", "TotalTime", (float.Parse(db.getStat("DoubleLetters", "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, "DoubleLetters", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), other.ToString());

            }

            btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);

        }
    }

    //AccentGame
    void acc_setButtons(string s)
    {

        acc_btnNext.gameObject.SetActive(false);
        acc_btnYes.gameObject.SetActive(true);
        acc_btnNo.gameObject.SetActive(true);

        acc_buttonsName(s);

        if (CheckAccent(s) < 5)
        {
            acc_btnYes.onClick.RemoveAllListeners();
            acc_btnNo.onClick.RemoveAllListeners();
            acc_btnYes.onClick.AddListener(acc_correct);
            acc_btnNo.onClick.AddListener(acc_wrong);
        }
        else
        {
            acc_btnYes.onClick.RemoveAllListeners();
            acc_btnNo.onClick.RemoveAllListeners();
            acc_btnYes.onClick.AddListener(acc_wrong);
            acc_btnNo.onClick.AddListener(acc_correct);
        }
    }

    int CheckAccent(string s)
    {

        switch (s.Substring(s.Length - 1))
        {
            case "à": { return 0; }
            case "è": { return 1; }
            case "ì": { return 2; }
            case "ò": { return 3; }
            case "ù": { return 4; }
            case "a": { return 5; }
            case "e": { return 6; }
            case "i": { return 7; }
            case "o": { return 8; }
            case "u": { return 9; }
            default: { return -1; }
        }
    }
    
    void acc_buttonsName(string s)
    {
        int tmp = CheckAccent(s);

        Text yesTxt = acc_btnYes.transform.Find("Text").gameObject.GetComponent<Text>();
        Text noTxt = acc_btnNo.transform.Find("Text").gameObject.GetComponent<Text>();

        switch (tmp)
        {
            case 0:
                {
                    yesTxt.text = s;                        
                    noTxt.text = (s.TrimEnd('à') + 'a');
                    break;
                }
            case 1:
                {
                    yesTxt.text = s;
                    noTxt.text = (s.TrimEnd('è') + 'e');
                    break;
                }
            case 2:
                {
                    yesTxt.text = s;
                    noTxt.text = (s.TrimEnd('ì') + 'i');
                    break;
                }
            case 3:
                {
                    yesTxt.text = s;
                    noTxt.text = (s.TrimEnd('ò') + 'o');
                    break;
                }
            case 4:
                {
                    yesTxt.text = s;
                    noTxt.text = (s.TrimEnd('ù') + 'u');
                    break;
                }
            case 5:
                {
                    yesTxt.text = (s.TrimEnd('a') + 'à');
                    noTxt.text = s;
                    break;
                }
            case 6:
                {
                    yesTxt.text = (s.TrimEnd('e') + 'è');
                    noTxt.text = s;
                    break;
                }
            case 7:
                {
                    yesTxt.text = (s.TrimEnd('i') + 'ì');
                    noTxt.text = s;
                    break;
                }
            case 8:
                {
                    yesTxt.text = (s.TrimEnd('o') + 'ò');
                    noTxt.text = s;
                    break;
                }
            case 9:
                {
                    yesTxt.text = (s.TrimEnd('u') + 'ù');
                    noTxt.text = s;
                    break;
                }
        }        
    }

    public void acc_correct()
    {
        result.text = "Corretto!";
        questionTxt.text = questionTxt.text.Replace("_", answer);
        acc_btnYes.onClick.RemoveAllListeners();
        acc_btnNo.onClick.RemoveAllListeners();

        recordTime();

        acc_btnNext.gameObject.SetActive(true);
        acc_btnYes.gameObject.SetActive(false);
        acc_btnNo.gameObject.SetActive(false);

        questionTxt.GetComponent<AudioSource>().Play();

        errorTmp = 0;
        correct = true;
    }

    public void acc_wrong()
    {
        if (errorTmp == 2)
        {
            result.text = "La risposta è " + answer + "! Prova con la prossima!";

            questionTxt.text = questionTxt.text.Replace("_", answer);

            wrongAnsw++;
            errorCount++;

            acc_btnNext.gameObject.SetActive(true);
            acc_btnYes.gameObject.SetActive(false);
            acc_btnNo.gameObject.SetActive(false);

            errorTmp = 0;
            recordTime();
            result.GetComponent<AudioSource>().Play();

            correct = true;
            acc_btnYes.onClick.RemoveAllListeners();
            acc_btnNo.onClick.RemoveAllListeners();
            return;
        }
        //if(err)
        result.text = "Riprova!";
        result.GetComponent<AudioSource>().Play();
        errorCount++;
        errorTmp++;

    }

    public void acc_nextQuestion()
    {
        checkCorrect();

        if (toNext == true && index < 5)
        {
            index++;

            toNext = false;

            q_id = getNewQuestion();
            str = db.getQuestion(q_id, q_type, true);

            questionTxt.text = str[0];
            answer = str[1];

            acc_setButtons(answer);
            active = true;

            result.text = "";
        }
        else if (index < 5)
            result.text = "Devi prima rispondere correttamente alla domanda!";
        else
        {
            acc_btnNext.onClick.RemoveAllListeners();
            GO.SetActive(false);

            questionTxt.text = "Hai completato le domande! Torna al menù!";

            result.text = "";

            if (collect)
            {
                db.updateStat("AccentGame", "LastError", errorCount.ToString(), player);
                db.updateStat("AccentGame", "LastTime", mainTimer.ToString(), player);
                db.updateStat("AccentGame", "TotalTime", (float.Parse(db.getStat("AccentGame", "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, "AccentGame", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");

            }

            btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);

        }
    }

    //GraphemeExchange
    void ge_setButtons(string s)
    {
        int len = s.Length / 2;

        ge_next.gameObject.SetActive(false);
        ge_answerA.gameObject.SetActive(true);
        ge_answerB.gameObject.SetActive(true);

        ge_answerA.onClick.RemoveAllListeners();
        ge_answerB.onClick.RemoveAllListeners();

        if ( UnityEngine.Random.Range(0, 100) < 50 )
        {
            answer = s.Substring(0, len);

            ge_answerA.transform.Find("Text").gameObject.GetComponent<Text>().text = answer;
            ge_answerA.onClick.AddListener(ge_correct);
            ge_answerB.transform.Find("Text").gameObject.GetComponent<Text>().text = s.Substring(len+1);
            ge_answerB.onClick.AddListener(ge_wrong);
        }
        else
        {
            answer = s.Substring(0, len);

            ge_answerB.transform.Find("Text").gameObject.GetComponent<Text>().text = answer;
            ge_answerB.onClick.AddListener(ge_correct);
            ge_answerA.transform.Find("Text").gameObject.GetComponent<Text>().text = s.Substring(len + 1);
            ge_answerA.onClick.AddListener(ge_wrong);
        }
    }

    public void ge_correct()
    {
        result.text = "Corretto!";
        questionTxt.text = questionTxt.text.Replace("_", answer);
        ge_answerA.onClick.RemoveAllListeners();
        ge_answerB.onClick.RemoveAllListeners();

        ge_answerA.gameObject.SetActive(false);
        ge_answerB.gameObject.SetActive(false);

        ge_next.gameObject.SetActive(true);

        recordTime();

        questionTxt.GetComponent<AudioSource>().Play();

        errorTmp = 0;
        correct = true;
    }

    public void ge_wrong()
    {
        if (errorTmp == 2)
        {
            result.text = "La risposta è " + answer + "! Prova con la prossima!";

            questionTxt.text = questionTxt.text.Replace("_", answer);

            ge_next.gameObject.SetActive(true);
            ge_answerA.gameObject.SetActive(false);
            ge_answerB.gameObject.SetActive(false);

            wrongAnsw++;
            errorCount++;

            errorTmp = 0;
            recordTime();
            result.GetComponent<AudioSource>().Play();

            correct = true;
            ge_answerA.onClick.RemoveAllListeners();
            ge_answerB.onClick.RemoveAllListeners();
            return;
        }
        
        result.text = "Riprova!";
        result.GetComponent<AudioSource>().Play();
        errorCount++;
        errorTmp++;

    }

    public void ge_nextQuestion()
    {
        checkCorrect();

        if (toNext == true && index < 5)
        {
            index++;

            toNext = false;

            q_id = getNewQuestion();
            str = db.getQuestion(q_id, q_type, true);

            questionTxt.text = str[0];

            ge_setButtons(str[1]);
            active = true;

            result.text = "";

            ge_next.gameObject.SetActive(false);
            ge_answerA.gameObject.SetActive(true);
            ge_answerB.gameObject.SetActive(true);
        }
        else if (index < 5)
            result.text = "Devi prima rispondere correttamente alla domanda!";
        else
        {
            ge_next.onClick.RemoveAllListeners();
            GO.SetActive(false);

            questionTxt.text = "Hai completato le domande! Torna al menù!";

            result.text = "";

            if (collect)
            {
                db.updateStat(q_type, "LastError", errorCount.ToString(), player);
                db.updateStat(q_type, "LastTime", mainTimer.ToString(), player);
                db.updateStat(q_type, "TotalTime", (float.Parse(db.getStat(q_type, "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, q_type, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");

            }

            btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);

        }
    }

    //GraphemePic
    public void gp_checkResult()
    {
        gp_answer.text = checkSpace(gp_answer.text);

        if (String.Equals(gp_answer.text, answer, StringComparison.OrdinalIgnoreCase))
        {
            result.text = "Corretto!";
            errorTmp = 0;

            gp_next.gameObject.SetActive(true);
            gp_send.gameObject.SetActive(false);

            questionTxt.GetComponent<AudioSource>().Play();

            correct = true;
            gp_send.interactable = false;

            return;
        }

        if (errorTmp < 2)
        {
            result.text = "Riprova!";
            result.GetComponent<AudioSource>().Play();

            errorCount++;
            errorTmp++;

            return;
        }

        result.text = "La frase corretta era:\n" + answer;
        recordTime();
        errorCount++;
        wrongAnsw++;

        gp_next.gameObject.SetActive(true);
        gp_send.gameObject.SetActive(false);

        result.GetComponent<AudioSource>().Play();

        gp_send.interactable = false;

        correct = true;
        errorTmp = 0;

    }

    public void gp_nextQuestion()
    {
        checkCorrect();

        if (toNext == true && index < 5)
        {
            toNext = false;
            index++;
            gp_images[q_id - 1].gameObject.SetActive(false);

            q_id = getNewQuestion();

            answer = db.getAnswer(q_type, q_id);
            gp_images[q_id - 1].gameObject.SetActive(true);
            result.text = "";
            gp_answer.text = "";
            gp_send.interactable = true;

            underscores(answer);

            gp_next.gameObject.SetActive(false);
            gp_send.gameObject.SetActive(true);
        }
        else if (index < 5 || !toNext)
        {
            result.text = "Devi prima rispondere correttamente alla domanda!";
        }
        else if (toNext)
        {
            GO.SetActive(false);
            gp_images[q_id - 1].gameObject.SetActive(false);
            questionTxt.text = "Hai completato le domande! Torna al menù!";

            result.text = "";

            if (collect)
            {
                db.updateStat("GraphemePic", "LastError", errorCount.ToString(), player);
                db.updateStat("GraphemePic", "LastTime", mainTimer.ToString(), player);
                db.updateStat("GraphemePic", "TotalTime", (float.Parse(db.getStat("GraphemePic", "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, "GraphemePic", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");

            }

            btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);
        }
    }
    
    void underscores(string s)
    {
        //gp_letters.transform.position = new Vector3(((Screen.width / 2) - (12.5f * s.Length)), gp_letters.transform.position.y, gp_letters.transform.position.z);
        int i = 0;
        for(; i < s.Length; )
        {
            i++;
        }

        gp_letters.text = i + " lettere";

    }

	//GraphemeClouds
	public void gc_fillCloud(string letters)
	{
		int rnd;
		for (int i = 0; i < letters.Length; i++) 
		{
			do {
				rnd = UnityEngine.Random.Range (0, gc_letters.Length);
			} while(gc_letters[rnd].text != "");
			gc_letters[rnd].text = letters[i].ToString();
		}
	}

	public void gc_emptyCloud()
	{
		for (int i = 0; i < gc_letters.Length; i++)
			gc_letters [i].text = "";
	}

	public void gc_checkResult()
	{
		gc_answer.text = checkSpace(gc_answer.text);

		if (String.Equals (gc_answer.text, answer, StringComparison.OrdinalIgnoreCase)) {
			result.text = "Corretto!";
			errorTmp = 0;

			gc_send.gameObject.SetActive (false);
			gc_next.gameObject.SetActive (true);

			questionTxt.GetComponent<AudioSource> ().Play ();

			correct = true;
			gc_send.interactable = false;

			return;
		} 

		if (errorTmp < 2)
		{
			result.text = "Riprova!";
			result.GetComponent<AudioSource>().Play();

			errorCount++;
			errorTmp++;

			return;
		}

		result.text = "La parola corretta era:\n" + answer;
		recordTime();
		errorCount++;
		wrongAnsw++;

		gc_next.gameObject.SetActive(true);
		gc_send.gameObject.SetActive(false);

		result.GetComponent<AudioSource>().Play();

		gc_send.interactable = false;

		correct = true;
		errorTmp = 0;
	}

	public void gc_nextQuestion()
	{
		gc_emptyCloud();
		checkCorrect();

		if (toNext == true && index < 5) 
		{
			toNext = false;
			index++;
			q_id = getNewQuestion ();

			str = db.getQuestion (q_id, q_type, true);
			answer = str[1];
			gc_fillCloud(str[0]);
			result.text = "";
			gc_answer.text = "";
			gc_send.interactable = true;

			gc_next.gameObject.SetActive (false);
			gc_send.gameObject.SetActive (true);
		} 
		else if (index < 5 || !toNext) 
		{
			result.text = "Devi prima rispondere correttamente alla domanda!";
		} 
		else if (toNext)
		{
			GO.SetActive (false);
			gc_cloud.gameObject.SetActive (false);
			questionTxt.text = "Hai completato le domande! Torna al menù!";

			result.text = "";

			if (collect) {
				db.updateStat ("GraphemeClouds", "LastError", errorCount.ToString (), player);
				db.updateStat ("GraphemeClouds", "LastTime", mainTimer.ToString (), player);
				db.updateStat ("GraphemeClouds", "TotalTime", (float.Parse (db.getStat ("GraphemeClouds", "TotalTime", player)) + mainTimer).ToString (), player);
				db.addStat (player, "GraphemeClouds", DateTime.Now.ToString ("dd/MM/yyyy HH:mm:ss").ToString (), wrongAnsw.ToString (), errorCount.ToString (), (5 - wrongAnsw).ToString (), mainTimer.ToString (), "0");
			}

			btnMenu.transform.position = new Vector3 (Screen.width / 2, Screen.height / 2 - 30f, 0f);
		}
	}

    //InvertedGrapheme
    public void ig_correct()
    {
        result.text = "Corretto!";
        ig_same.onClick.RemoveAllListeners();
        ig_different.onClick.RemoveAllListeners();

        ig_next.gameObject.SetActive(true);
        ig_same.gameObject.SetActive(false);
        ig_different.gameObject.SetActive(false);

        recordTime();

        questionTxt.GetComponent<AudioSource>().Play();

        correct = true;
    }

    public void ig_wrong()
    {
        if (errorTmp == 2)
        {
            if (answer == "same")
                result.text = "Le due sillabe sono uguali!";
            else
                result.text = "Le due sillabe sono diverse!";

            errorCount++;
            wrongAnsw++;

            errorTmp = 0;
            recordTime();
            result.GetComponent<AudioSource>().Play();

            correct = true;
            ig_same.onClick.RemoveAllListeners();
            ig_different.onClick.RemoveAllListeners();

            ig_next.gameObject.SetActive(true);
            ig_same.gameObject.SetActive(false);
            ig_different.gameObject.SetActive(false);

            return;
        }

        result.text = "Riprova!";
        result.GetComponent<AudioSource>().Play();
        errorCount++;
        errorTmp++;

    }

    void ig_setButtons()
    {
        ig_next.gameObject.SetActive(false);
        ig_same.gameObject.SetActive(true);
        ig_different.gameObject.SetActive(true);

        ig_same.onClick.RemoveAllListeners();
        ig_different.onClick.RemoveAllListeners();

        if (String.Equals(answer, "same"))
        {
            ig_same.onClick.AddListener(ig_correct);
            ig_different.onClick.AddListener(ig_wrong);
            
        }
        else
        {
            ig_same.onClick.AddListener(ig_wrong);
            ig_different.onClick.AddListener(ig_correct);

        }

    }

    public void ig_nextQuestion()
    {
        checkCorrect();

        if (toNext == true && index < 5)
        {
            index++;

            toNext = false;

            q_id = getNewQuestion();
            str = db.getQuestion(q_id, q_type, true);

            questionTxt.text = index + ". " + str[0];
            answer = str[1];
            errorTmp = 0;

            ig_setButtons();
            active = true;

            result.text = "";
        }
        else if (index < 5)
            result.text = "Devi prima rispondere correttamente alla domanda!";
        else
        {
            ig_next.onClick.RemoveAllListeners();
            GO.SetActive(false);

            questionTxt.text = "Hai completato le domande! Torna al menù!";

            result.text = "";

            if (collect)
            {
                db.updateStat("InvertedGrapheme", "LastError", errorCount.ToString(), player);
                db.updateStat("InvertedGrapheme", "LastTime", mainTimer.ToString(), player);
                db.updateStat("InvertedGrapheme", "TotalTime", (float.Parse(db.getStat("InvertedGrapheme", "TotalTime", player)) + mainTimer).ToString(), player);
                db.addStat(player, "InvertedGrapheme", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");

            }

            btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);

        }
    }

	//InvertedGraphemeSearch
	public void sg_setButtons(string syllables)
	{
		sg_matrix.gameObject.SetActive (true);
		sg_counter = 0;
		sg_correctAnswers = 0;
		for (int i = 0; i < sg_buttons.Length; i++) 
		{
			sg_buttons [i].GetComponentInChildren<Text>().text = syllables.Substring(3*i, 2);
			sg_buttons [i].onClick.RemoveAllListeners ();
			int _i = i;
			if (sg_buttons [i].GetComponentInChildren<Text> ().text == answer) 
			{
				sg_buttons [i].onClick.AddListener (delegate{sg_correctButton(_i);});
				sg_correctAnswers++;
			}
			else
				sg_buttons [i].onClick.AddListener (delegate{sg_wrongButton(_i);});
		}
	}

	public void sg_resetButtons()
	{
		for (int i = 0; i < sg_buttons.Length; i++) 
		{
			sg_buttons [i].GetComponent<Button> ().interactable = true;
			sg_buttons [i].GetComponentInChildren<Text> ().text = "";
			sg_buttons [i].GetComponent<Image> ().color = Color.white;
		}
	}

	public void sg_correctButton (int idx)
	{
		sg_buttons [idx].GetComponent<Button> ().interactable = false;
		sg_buttons [idx].GetComponent<Image> ().color = Color.green;
		questionTxt.GetComponent<AudioSource>().Play();
		sg_counter++;
		if (sg_counter == sg_correctAnswers)
			sg_checkResult ();
	}

	public void sg_wrongButton(int idx)
	{
		sg_buttons [idx].GetComponent<Button> ().interactable = false;
		sg_buttons [idx].GetComponent<Image> ().color = Color.red;
		result.GetComponent<AudioSource> ().Play ();
		errorTmp++;
	}

	public void sg_checkResult()
	{
		if (sg_counter != sg_correctAnswers) 
		{
			result.text = "Devi prima trovare tutte le corrispondenze!";
			return;
		}

		questionTxt.text = "Hai trovato tutte le corrispondenze!";
		recordTime();
		if(errorTmp > 0)
			wrongAnsw++;

		sg_question.text = "";
		sg_next.gameObject.SetActive(true);
		sg_matrix.gameObject.SetActive(false);

		correct = true;
		errorCount += errorTmp;
		errorTmp = 0;
	}

	public void sg_nextQuestion()
	{
		checkCorrect();
		sg_next.gameObject.SetActive (false);
		sg_resetButtons ();

		if (toNext == true && index < 5) 
		{
			index++;
			toNext = false;

			q_id = getNewQuestion ();
			str = db.getQuestion (q_id, q_type, true);
			answer = str [1];
			sg_question.text += answer;
			sg_setButtons (str [0]);

			errorTmp = 0;
			active = true;
			questionTxt.text = "";
		}
		else if (index < 5)
			result.text = "Devi prima rispondere correttamente alla domanda!";
		else
		{
			sg_next.onClick.RemoveAllListeners();
			GO.SetActive(false);

			questionTxt.text = "Hai completato le domande! Torna al menù!";
			result.text = "";

			if (collect)
			{
				db.updateStat("InvertedGraphemeSearch", "LastError", errorCount.ToString(), player);
				db.updateStat("InvertedGraphemeSearch", "LastTime", mainTimer.ToString(), player);
				db.updateStat("InvertedGraphemeSearch", "TotalTime", (float.Parse(db.getStat("InvertedGraphemeSearch", "TotalTime", player)) + mainTimer).ToString(), player);
				db.addStat(player, "InvertedGraphemeSearch", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");
			}

			btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);
		}
	}

	//SoundImages
	public void si_generateQuestion()
	{
		si_qstType = answer.Substring (0, answer.IndexOf ("+"));
		si_sound = answer.Substring(answer.IndexOf("+")+1, 1);
		si_correctAnswers = 0;
		si_counter = 0;

		if (si_qstType == "starts")
			si_question.text = "Seleziona le figure che <color=red>iniziano</color> con il suono <color=red>" + si_sound + "</color>";
		else if (si_qstType == "contains")
			si_question.text = "Seleziona le figure che <color=red>contengono</color> il suono <color=red>" + si_sound + "</color>";
		else
			si_question.text = "Seleziona le figure con il suono " + si_sound;

		si_audio.onClick.RemoveAllListeners ();
		si_audio.onClick.AddListener (delegate{si_textToSpeech(q_id);});
		si_audio.gameObject.SetActive (true);
	}

	public void si_fillArray(string imgNames)
	{
		char[] words = imgNames.ToCharArray();
		int c_idx = 0;
		int i;

		for (int iter = 0; iter < si_images.Length; iter++) 
		{
			do {
				i = UnityEngine.Random.Range(0, si_images.Length);
			} while(si_images[i].IsActive());

			while (words[c_idx] != '+')
				si_images [i].transform.GetChild(0).GetComponent<Text>().text += words[c_idx++];
			if (iter < si_images.Length - 2)
				c_idx++;
			if (iter == si_images.Length - 1)
				si_images [i].transform.GetChild(0).GetComponent<Text>().text = imgNames.Substring (++c_idx);
			si_attachSprite(i, si_images[i].transform.GetChild(0).GetComponent<Text>().text.ToLower());
			si_updateCounter(si_images[i].transform.GetChild(0).GetComponent<Text>().text);

			si_images [i].gameObject.SetActive (true);
			int _i = i;
			si_images [i].GetComponent<Button> ().onClick.AddListener (delegate {si_assignBehaviour(_i);});
		}
	}

	public void si_attachSprite(int idx, string img)
	{
		for(int i = 0; i < si_sprites.Length; i++)
			if (si_sprites [i].name == img) 
			{
				si_images [idx].GetComponent<Image> ().sprite = si_sprites [i];
				break;
			}
	}

	public void si_updateCounter(string img)
	{
		if (si_qstType == "starts") 
		{
			if (img [0] == si_sound [0])
				si_correctAnswers++;
		} 
		else if (si_qstType == "contains") 
		{
			if (img.ToUpper ().Contains (si_sound))
				si_correctAnswers++;	
		}
	}

	public void si_assignBehaviour(int i)
	{
		if (si_qstType == "starts") 
		{
			if (si_images [i].transform.GetChild (0).GetComponent<Text> ().text [0] == si_sound [0]) //correct
			{
				si_images [i].transform.GetChild (1).GetComponent<Image> ().sprite = si_correct;
				questionTxt.GetComponent<AudioSource> ().Play ();
				si_counter++;
				if (si_counter == si_correctAnswers)
					si_checkResult ();
			}
			else //wrong
			{
				si_images [i].transform.GetChild (1).GetComponent<Image> ().sprite = si_wrong;
				result.GetComponent<AudioSource> ().Play ();
				errorTmp++;
			}
		} 
		else if (si_qstType == "contains") 
		{
			if (si_images [i].transform.GetChild (0).GetComponent<Text> ().text.ToUpper ().Contains (si_sound)) //correct
			{
				si_images [i].transform.GetChild (1).GetComponent<Image> ().sprite = si_correct;
				questionTxt.GetComponent<AudioSource> ().Play ();
				si_counter++;
				if (si_counter == si_correctAnswers)
					si_checkResult ();
			}
			else //wrong
			{
				si_images [i].transform.GetChild (1).GetComponent<Image> ().sprite = si_wrong;
				result.GetComponent<AudioSource> ().Play ();
				errorTmp++;
			}
		}

		si_images [i].GetComponent<Button> ().interactable = false;
		si_images [i].transform.GetChild(0).gameObject.SetActive (true);
		si_images [i].transform.GetChild(1).gameObject.SetActive (true);
	}

	public void si_textToSpeech(int i)
	{
		Debug.Log ("Play track " + i);
		//si_audio.GetComponent<AudioSource> ().clip = si_clips [i];
		//si_audio.GetComponent<AudioSource> ().Play ();
	}

	public void si_checkResult()
	{
		if (si_counter != si_correctAnswers) 
		{
			result.text = "Devi prima trovare tutte le corrispondenze!";
			return;
		}

		questionTxt.text = "Hai trovato tutte le corrispondenze!";
		recordTime();
		if(errorTmp > 0)
			wrongAnsw++;

		si_question.text = "";
		si_next.gameObject.SetActive(true);
		si_audio.gameObject.SetActive (false);
		for (int i = 0; i < si_images.Length; i++)
			si_images [i].gameObject.SetActive (false);

		correct = true;
		errorCount += errorTmp;
		errorTmp = 0;
	}

	public void si_resetImages()
	{
		for (int i = 0; i < si_images.Length; i++) 
		{
			si_images [i].GetComponent<Image> ().sprite = null;
			si_images [i].GetComponent<Button> ().onClick.RemoveAllListeners ();
			si_images [i].transform.GetChild (0).GetComponent<Text> ().text = "";
			si_images [i].transform.GetChild (1).GetComponent<Image> ().sprite = null;
			si_images [i].transform.GetChild (0).gameObject.SetActive (false);
			si_images [i].transform.GetChild (1).gameObject.SetActive (false);
			si_images [i].GetComponent<Button> ().interactable = true;
		}
	}

	public void si_nextQuestion()
	{
		checkCorrect();
		si_next.gameObject.SetActive (false);
		si_resetImages ();

		if (toNext == true && index < 5) 
		{
			index++;
			toNext = false;

			q_id = getNewQuestion ();
			str = db.getQuestion (q_id, q_type, true);
			answer = str [1];
			si_generateQuestion();
			si_fillArray(str[0]);

			errorTmp = 0;
			active = true;
			questionTxt.text = "";
		}
		else if (index < 5)
			result.text = "Devi prima rispondere correttamente alla domanda!";
		else
		{
			si_next.onClick.RemoveAllListeners();
			GO.SetActive(false);

			questionTxt.text = "Hai completato le domande! Torna al menù!";
			result.text = "";

			if (collect)
			{
				db.updateStat("SoundImages", "LastError", errorCount.ToString(), player);
				db.updateStat("SoundImages", "LastTime", mainTimer.ToString(), player);
				db.updateStat("SoundImages", "TotalTime", (float.Parse(db.getStat("SoundImages", "TotalTime", player)) + mainTimer).ToString(), player);
				db.addStat(player, "SoundImages", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");
			}

			btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);
		}
	}

	//CountWords
	public void cw_checkResult()
	{
		if (String.Equals(cw_answerText.text, answer, StringComparison.OrdinalIgnoreCase))
		{
			result.text = "Corretto!";
			errorTmp = 0;

			questionTxt.GetComponent<AudioSource>().Play();
			//recordTime();

			correct = true;
			cw_send.gameObject.SetActive(false);
			cw_next.gameObject.SetActive(true);

			return;
		}

		if (errorTmp < 2)
		{
			result.text = "Riprova!";
			result.GetComponent<AudioSource>().Play();

			errorCount++;
			errorTmp++;

			return;
		}

		result.text = "La frase conteneva " + answer + " parole";
		recordTime();
		errorCount++;
		wrongAnsw++;

		cw_send.gameObject.SetActive(false);
		cw_next.gameObject.SetActive(true);

		result.GetComponent<AudioSource>().Play();

		cw_send.interactable = false;

		correct = true;
		errorTmp = 0;
	}

	public void cw_nextQuestion()
	{
		checkCorrect();

		if (toNext == true && index < 5)
		{
			toNext = false;
			index++;

			q_id = getNewQuestion();
			str = db.getQuestion(q_id, q_type, true);
			questionTxt.text = str[0];
			answer = str[1];

			cw_answerText.text = "";
			result.text = "";
			cw_send.interactable = true;
			active = true;

			cw_send.gameObject.SetActive(true);
			cw_next.gameObject.SetActive(false);
		}
		else if (index < 5 || !toNext)
		{
			result.text = "Devi prima rispondere correttamente alla domanda!";
		}
		else if (toNext)
		{
			GO.SetActive(false);

			questionTxt.text = "Hai completato le domande! Torna al menù!";

			result.text = "";
			if (collect)
			{ 
				db.updateStat("CountWords", "LastError", errorCount.ToString(), player);
				db.updateStat("CountWords", "LastTime", mainTimer.ToString(), player);
				db.updateStat("CountWords", "TotalTime", (float.Parse(db.getStat("CountWords", "TotalTime", player)) + mainTimer).ToString(), player);
				db.addStat(player, "CountWords", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");
			}

			btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);
		}
	}

	public void aw_generateButtons(string text)
	{
		aw_counter = 0;
		aw_correctAnswers = 0;
		char[] words = text.ToCharArray();
		int c_idx = 0;
		int i;

		for (int iter = 0; iter < aw_buttons.Length; iter++) 
		{
			do {
				i = UnityEngine.Random.Range (0, aw_buttons.Length);
			} while(aw_buttons [i].IsActive ());

			if (iter == aw_buttons.Length - 1)
				aw_buttons [i].transform.GetChild (0).GetComponent<Text> ().text = text.Substring (c_idx);
			else 
			{
				while (words [c_idx] != '+')
					aw_buttons [i].transform.GetChild (0).GetComponent<Text> ().text += words [c_idx++];
				c_idx++;
			}

			if (answer.Contains (aw_buttons [i].transform.GetChild (0).GetComponent<Text> ().text))
				aw_correctAnswers++;
			aw_buttons [i].gameObject.SetActive (true);
			int _i = i;
			aw_buttons[i].onClick.AddListener(delegate {aw_assignBehaviour(_i);});
		}
	}

	public void aw_assignBehaviour(int i)
	{
		if (answer.Contains (aw_buttons [i].transform.GetChild (0).GetComponent<Text> ().text)) {
			aw_buttons [i].GetComponent<Image> ().color = Color.green;
			questionTxt.GetComponent<AudioSource> ().Play ();
			aw_counter++;
			if (aw_counter == aw_correctAnswers)
				aw_checkResult ();
		} 
		else 
		{
			aw_buttons [i].GetComponent<Image> ().color = Color.red;
			result.GetComponent<AudioSource> ().Play ();
			errorTmp++;
		}

		aw_buttons [i].interactable = false;
	}

	public void aw_checkResult()
	{
		if (aw_counter != aw_correctAnswers) 
		{
			result.text = "Devi prima trovare tutte le corrispondenze!";
			return;
		}

		questionTxt.text = "Hai trovato tutte le corrispondenze!";
		recordTime();
		if(errorTmp > 0)
			wrongAnsw++;

		aw_next.gameObject.SetActive(true);
		for (int i = 0; i < aw_buttons.Length; i++)
			aw_buttons [i].gameObject.SetActive (false);

		correct = true;
		errorCount += errorTmp;
		errorTmp = 0;
	}

	public void aw_resetButtons()
	{
		for (int i = 0; i < aw_buttons.Length; i++) 
		{
			aw_buttons [i].GetComponent<Image> ().color = Color.white;
			aw_buttons [i].onClick.RemoveAllListeners ();
			aw_buttons [i].transform.GetChild (0).GetComponent<Text> ().text = "";
			aw_buttons [i].GetComponent<Button> ().interactable = true;
		}
	}

	public void aw_nextQuestion()
	{
		checkCorrect();
		aw_next.gameObject.SetActive (false);
		aw_resetButtons ();

		if (toNext == true && index < 5) 
		{
			index++;
			toNext = false;

			q_id = getNewQuestion ();
			str = db.getQuestion (q_id, q_type, true);
			answer = str [1];
			aw_generateButtons(str[0]);

			errorTmp = 0;
			active = true;
			questionTxt.text = "";
		}
		else if (index < 5)
			result.text = "Devi prima rispondere correttamente alla domanda!";
		else
		{
			aw_next.onClick.RemoveAllListeners();
			GO.SetActive(false);

			questionTxt.text = "Hai completato le domande! Torna al menù!";
			result.text = "";

			if (collect)
			{
				db.updateStat("AccentWords", "LastError", errorCount.ToString(), player);
				db.updateStat("AccentWords", "LastTime", mainTimer.ToString(), player);
				db.updateStat("AccentWords", "TotalTime", (float.Parse(db.getStat("AccentWords", "TotalTime", player)) + mainTimer).ToString(), player);
				db.addStat(player, "AccentWords", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");
			}

			btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);
		}
	}

	//DoubleSenteces
	public void ds_setButtons(string s)
	{
		string ds_correctAnswer = s.Substring(0, s.IndexOf('+'));
		string ds_wrongAnswer = s.Substring (s.IndexOf ('+') + 1);

		ds_next.gameObject.SetActive(false);
		ds_answ1.gameObject.SetActive(true);
		ds_answ1.gameObject.SetActive(true);
		ds_answ1.onClick.RemoveAllListeners ();
		ds_answ2.onClick.RemoveAllListeners ();

		if (UnityEngine.Random.Range (0, 100) < 50) 
		{
			ds_answ1.transform.GetChild (0).GetComponent<Text> ().text = ds_correctAnswer;
			ds_answ1.onClick.AddListener (ds_correct);
			ds_answ2.transform.GetChild (0).GetComponent<Text> ().text = ds_wrongAnswer;
			ds_answ2.onClick.AddListener (ds_wrong);
		} 
		else 
		{
			ds_answ2.transform.GetChild (0).GetComponent<Text> ().text = ds_correctAnswer;
			ds_answ2.onClick.AddListener (ds_correct);
			ds_answ1.transform.GetChild (0).GetComponent<Text> ().text = ds_wrongAnswer;
			ds_answ1.onClick.AddListener (ds_wrong);
		}
	}

	public void ds_correct()
	{
		result.text = "Corretto!";
		questionTxt.text = questionTxt.text.Replace("_", answer.Substring(0, answer.IndexOf('+')));

		ds_answ1.gameObject.SetActive(false);
		ds_answ2.gameObject.SetActive(false);
		ds_next.gameObject.SetActive(true);

		recordTime();
		questionTxt.GetComponent<AudioSource>().Play();

		errorTmp = 0;
		correct = true;
	}

	public void ds_wrong()
	{
		if (errorTmp == 2)
		{
			result.text = "La risposta corretta è " + answer.Substring(0, answer.IndexOf('+')) + "! Prova con la prossima!";

			questionTxt.text = questionTxt.text.Replace("_", answer.Substring(0, answer.IndexOf('+')));

			ds_next.gameObject.SetActive(true);
			ds_answ1.gameObject.SetActive(false);
			ds_answ2.gameObject.SetActive(false);

			wrongAnsw++;
			errorCount++;

			errorTmp = 0;
			recordTime();
			result.GetComponent<AudioSource>().Play();

			correct = true;
			return;
		}

		result.text = "Riprova!";
		result.GetComponent<AudioSource>().Play();
		errorCount++;
		errorTmp++;
	}

	public void ds_nextQuestion()
	{
		checkCorrect();

		if (toNext == true && index < 5)
		{
			index++;

			toNext = false;

			q_id = getNewQuestion();
			str = db.getQuestion(q_id, q_type, true);

			questionTxt.text = str[0];
			answer = str[1];
			ds_setButtons(answer);
			active = true;

			result.text = "";

			ds_next.gameObject.SetActive(false);
			ds_answ1.gameObject.SetActive(true);
			ds_answ2.gameObject.SetActive(true);
		}
		else if (index < 5)
			result.text = "Devi prima rispondere correttamente alla domanda!";
		else
		{
			ds_next.onClick.RemoveAllListeners();
			GO.SetActive(false);

			questionTxt.text = "Hai completato le domande! Torna al menù!";

			result.text = "";

			if (collect)
			{
				db.updateStat("DoubleSentences", "LastError", errorCount.ToString(), player);
				db.updateStat("DoubleSentences", "LastTime", mainTimer.ToString(), player);
				db.updateStat("DoubleSentences", "TotalTime", (float.Parse(db.getStat("DoubleSentences", "TotalTime", player)) + mainTimer).ToString(), player);
				db.addStat(player, q_type, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString(), wrongAnsw.ToString(), errorCount.ToString(), (5 - wrongAnsw).ToString(), mainTimer.ToString(), "0");
			}

			btnMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - 30f, 0f);
		}
	}

    //generic
	int getNewQuestion()
    {
        timecalled++;
        int rnd = UnityEngine.Random.Range(1, total + 1);

        while (true)
        {
            if (qstDone[rnd - 1] == false)
            {
                qstDone[rnd - 1] = true;
                return rnd;
            }
            rnd = UnityEngine.Random.Range(1, total + 1);
        }
    }

    void checkCorrect()
    {
        if (correct == true)
        {
            correct = false;
            toNext = true;
        }
    }

    public void backToMenu()
    {
        sceneName = "2. GameMenu";
        StartCoroutine(FadeOut());
    }

    void recordTime()
    {
        timerArray[index - 1] = singleTimer;
        singleTimer = 0f;
        active = false;

    }

    void updateTimer(bool flag)
    {
        if (flag)
        {
            mainTimer += Time.deltaTime;
            singleTimer += Time.deltaTime;
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
            yield return new WaitForSeconds(0.001f);
        } while (f > 0f);

        cover.SetActive(false);

        counter.text = "2";
        yield return new WaitForSeconds(1f);

        counter.text = "1";
        yield return new WaitForSeconds(1f);

        counter.text = "Via!";
        yield return new WaitForSeconds(1f);
        counter.gameObject.SetActive(false);

        loadScenario();
        active = true;
        mainTimer = singleTimer = 0f;

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

    }
