using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject[] btnGroup;       //  0:MAIN      1:MACRO       2:PHONO      3:NOPHONO      4:MIX      5:ILLFUS       6:GRAPHEX      7:ADDOMH       8:FV-DT-PB

    public GameObject tutorPassword;
    public GameObject cover;

    public Button back;

    public Text playerName;
    public Text mode;
    public Text Error;
    public InputField tutorPass;

    DBConnection db;
    GameManager gm;

    int player;

    int next, curr, iter;
    int[] prevs;

    string sceneName;

    void Awake () {
        
        prevs = new int[4];
        for(int i = 0; i < 4; ++i) { prevs[i] = 0; }

        curr = next = 0;
        iter = 0;

    }

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        db = GameObject.FindGameObjectWithTag("DBManager").GetComponent<DBConnection>();

        player = gm.getActivePlayerIndex();

        sceneName = "";
        mode.text = "Menù";

        gm.setCollect(false);


        playerName.text = gm.getKids()[player];
        StartCoroutine(FadeIn());
    }

    void GoToScene(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void blockMover(int next)
    {
        back.gameObject.SetActive(true);

        if(gm.getCollect())
            mode.text = "Allenamento";
        else
            mode.text = "Gioco Libero";

        if (next != -1)
        {
            btnGroup[curr].gameObject.SetActive(false);
            btnGroup[next].gameObject.SetActive(true);

            prevs[iter++] = curr;
            curr = next;

        }
        else
        {
            btnGroup[curr].gameObject.SetActive(false);
            btnGroup[prevs[--iter]].gameObject.SetActive(true);

            curr = prevs[iter];

            if (curr == 0)
            {
                mode.text = "Menù";
                gm.setCollect(false);
                back.gameObject.SetActive(false);
            }
        }

    }

    public void ss_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "SentenceSplit");
        db.choice = "SentenceSplit";
    }
    public void h_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "HGame");
        db.choice = "HGame";

    }
    public void cq_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "CQGame");
        db.choice = "CQGame";
    }
    public void ag_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "AudioGame");
        db.choice = "AudioGame";
    }
    public void dl_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "DoubleLetters");
        db.choice = "DoubleLetters";
    }
    public void acc_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "AccentGame");
        db.choice = "AccentGame";
    }
    public void DTGrapheme_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "DTGrapheme");
        db.choice = "DTGrapheme";
    }
    public void FVGrapheme_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "FVGrapheme");
        db.choice = "FVGrapheme";
    }
    public void PBGrapheme_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "PBGrapheme");
        db.choice = "PBGrapheme";
    }
    public void InvertedGraph_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "InvertedGrapheme");
        db.choice = "InvertedGrapheme";
    }
    public void GraphemePic_load()
    {
        SceneManager.LoadScene("3. QuestionTab");
        PlayerPrefs.SetString("choice", "GraphemePic");
        db.choice = "GraphemePic";
    }

	public void GraphemeClouds_load()
	{
		SceneManager.LoadScene("3. QuestionTab");
		PlayerPrefs.SetString ("choice", "GraphemeClouds");
		db.choice = "GraphemeClouds";
	}
	public void InvertedGraphSearch_load()
	{
		SceneManager.LoadScene("3. QuestionTab");
		PlayerPrefs.SetString("choice", "InvertedGraphemeSearch");
		db.choice = "InvertedGraphemeSearch";
	}
	public void SoundImages_load()
	{
		SceneManager.LoadScene("3. QuestionTab");
		PlayerPrefs.SetString("choice", "SoundImages");
		db.choice = "SoundImages";
	}
	public void CountWords_load()
	{
		SceneManager.LoadScene("3. QuestionTab");
		PlayerPrefs.SetString("choice", "CountWords");
		db.choice = "CountWords";
	}
	public void AccentWords_load()
	{
		SceneManager.LoadScene("3. QuestionTab");
		PlayerPrefs.SetString("choice", "AccentWords");
		db.choice = "AccentWords";
	}
	public void DoubleSentences_load()
	{
		SceneManager.LoadScene("3. QuestionTab");
		PlayerPrefs.SetString("choice", "DoubleSentences");
		db.choice = "DoubleSentences";
	}
	public void HSentences_load()
	{
		SceneManager.LoadScene("3. QuestionTab");
		PlayerPrefs.SetString("choice", "HSentences");
		db.choice = "HSentences";
	}
	public void CQSentences_load()
	{
		SceneManager.LoadScene("3. QuestionTab");
		PlayerPrefs.SetString("choice", "CQSentences");
		db.choice = "CQSentences";
	}

	//public void menuSelection(int moveTo)
    //{

    //    if(moveTo > 0 && moveTo != 500)
    //    {
    //        nextBlock = true;
    //        moveBlock = true;
    //        next = moveTo;

    //        prevs[iter] = curr;

    //    } else {

    //        prevBlock = true; 
    //        moveBlock = true;
    //        iter--;
    //        next = prevs[iter];

    //    }
    //}

    public void exitGame()
    {
        sceneName = "0. AccountMenu";
        StartCoroutine(FadeOut());
    }

    public void passwordTutor()
    {
        tutorPass.inputType = InputField.InputType.Password;

        btnGroup[0].gameObject.SetActive(false);
        tutorPassword.gameObject.SetActive(true);

        tutorPass.text = "";

    }

    public void TutorContinue()
    {

        if (CheckPassword())
        {
            btnGroup[0].SetActive(true);
            tutorPassword.SetActive(false);

            gm.setCollect(true);
            blockMover(1);

            tutorPass.text = "";
        }
        else
        {

            Error.text = "PASSWORD ERRATA";
            tutorPass.text = "";

        }
        //Debug.Log(gm.getCollect());

    }

    public bool CheckPassword()
    {
        if ( String.Equals(tutorPass.text, db.getPassword()))
            return true;

        return false;
    }

    public void TutorBack()
    {
        tutorPassword.SetActive(false);
        btnGroup[0].SetActive(true);

        tutorPass.text = "";
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

    IEnumerator Refresh(Text t, Color tmp)
    {
        float f = 1.0f;
        t.color = tmp;

        t.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.5f);

        do
        {
            f -= 0.02f;
            tmp.a = f;
            t.color = tmp;
            yield return new WaitForSeconds(0.00001f);
        } while (f > 0f);

        t.gameObject.SetActive(false);
        tmp.a = 1.0f;
        t.color = tmp;

        yield return null;
    }
}