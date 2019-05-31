using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


public class InitializeAccount : MonoBehaviour {


    [SerializeField]
    GameObject cover;

    DBConnection db;
    GameManager gm;

    public InputField[] Input;      // 0: TutorName     1: Password     2: re-Pwd       3: kid

    public Text GenericError;
    public Text KidError;
    public Text KidShow;

    int counter;

    public Dropdown rmKidList;

    //bool flag;
    String[] kidList;

    void Awake()
    {
        Input[1].inputType = InputField.InputType.Password;
        Input[2].inputType = InputField.InputType.Password;

        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        db = GameObject.FindGameObjectWithTag("DBManager").GetComponent<DBConnection>();
        StartCoroutine(fadeIn());
    }

    void Start()
    {
        rmKidList.ClearOptions();

        counter = 0;
        kidList = new string[12];               
        for (int i = 0; i < 12; i++) { kidList[i] = ""; }

        KidShow.text = "";

        //flag = false;
    }
    
    public void checkInfos()
    {
        KidError.color = Color.red;
        string s = Input[3].text;
        int max = db.getKidsNumber();

        if (max > 11)
        {
            KidError.text = "Numero massimo di bambini raggiunto. ";
            return;
        }

        //if (Input[3].text.Contains("1") || Input[3].text.Contains("2") || Input[3].text.Contains("3") || Input[3].text.Contains("4") || Input[3].text.Contains("5") || Input[3].text.Contains("6") || Input[3].text.Contains("7") || Input[3].text.Contains("0") || Input[3].text.Contains("8") || Input[3].text.Contains("9"))
        if (String.IsNullOrEmpty(s) || s.Length < 2 )
        {
            KidError.text = "Il nome deve avere almeno 2 lettere. ";
            return;
        }
        
        if (!Regex.IsMatch(s, @"^[a-zA-Z]+$"))
        {
            KidError.text = "Il nome NON deve contenere numeri o caratteri speciali (ad esempio: '!','_','+', eccetera). ";
            return;
        }

        s = gm.nameFormatter(Input[3].text);

        if (db.findKid(s))
        {
            KidError.text = "Nome già esistente";
            return;
        }

        KidError.color = new Color(0.05f, 0.73f, 0f, 1f); 
        KidError.text = "Bambino inserito!";

        Input[3].text = "";

        db.addKid(s);
        max++;
        gm.updateKids();
        kidList = gm.getKids();
        counter++;
        setDropDown(max);
        refreshList(max);

    }

    public void removeKids()
    {
        int max = db.getKidsNumber();
        string s = kidList[rmKidList.value];
        
        if(max < 1)
        {
            KidError.color = Color.red;
            KidError.text = "Lista vuota.";
        }
        else if (db.findKid(s))
        {
            KidError.color = new Color(0.05f, 0.73f, 0f, 1f);
            KidError.text = "Bambino rimosso.";

            if(max < 2)
            {
                kidList[0] = "";
                gm.updateKids();
                db.removeKid(s);
                KidShow.text = "";
                rmKidList.ClearOptions();
                counter = 0;
                return;
            }

            db.removeKid(s);
            max--;
            gm.updateKids();
            kidList = gm.getKids();

            gm.updateKids();
            refreshList(max);
            setDropDown(max);
            counter--;
        }
    }

    void refreshList(int n)
    {
        KidShow.text = "";
        for (int i = 0; i < n ; i++)
        {
            KidShow.text += kidList[i] + "   ";
        }
    }

    public void sendInfos()
    {
        GenericError.color = Color.red;

        if (String.IsNullOrEmpty(Input[0].text) || String.IsNullOrEmpty(Input[1].text) || String.IsNullOrEmpty(Input[2].text))
        {
            GenericError.text = "Compilare correttamente i campi relativi al Tutor. ";
            Input[0].text = "";
            Input[1].text = "";
            Input[2].text = "";

            return;
        }

        if(db.getKidsNumber() < 1)
        {
            GenericError.text = "Inserire almeno un bambino.";

            return;
        }

        if(Input[0].text.Length < 2)
        {
            GenericError.text = "Il nome del Tutor deve avere almeno 2 lettere. ";

            Input[0].text = "";

            return;
        }

        if (Input[1].text.Length < 8)
        {
            GenericError.text = "La password deve avere almeno 8 caratteri ";
            Input[1].text = "";
            Input[2].text = "";

            return;
        }

        if (!String.Equals(Input[1].text, Input[2].text))
        {
            GenericError.text = "Le password non combaciano.";

            Input[1].text = "";
            Input[2].text = "";

            return;
        }

        if ( String.IsNullOrEmpty(Input[4].text) || String.IsNullOrEmpty(Input[5].text))
        {
            GenericError.text = "Completare correttamentei campi 'Domanda e Risposta Segreta'";
            return;
        }


        if (Input[4].text.Length < 10)
        {
            GenericError.text = "Il campo 'Domanda Segreta' deve contenere almeno 10 caratteri!";
            return;
        }

        db.addTutor(gm.nameFormatter(Input[0].text), Input[1].text, Input[4].text, Input[5].text);

        gm.updateKids();
        StartCoroutine(closing());
    }

    void setDropDown(int n)
    {
        rmKidList.ClearOptions();

        List<string> names = new List<string>();

        for (int i = 0; i < n; i++)
        {
            names.Add(kidList[i]);
        }
        
        rmKidList.AddOptions(names);
    }

    IEnumerator closing()
    {

        GenericError.color = new Color(0.05f, 0.73f, 0f, 1f);

        cover.SetActive(true);

        gm.GetComponent<GameManager>().setFS(false);

        KidError.text = "";
        GenericError.text = "Inserimento avvenuto con successo!";
        GenericError.gameObject.SetActive(true);

        //Error.color = new Color(15, 200, 0);
        float f = 0f;

        Color tmp = cover.GetComponent<Image>().color;

        yield return new WaitForSeconds(3f);

        do
        {
            f += 0.02f;
            tmp.a = f;
            cover.GetComponent<Image>().color = tmp;
            yield return new WaitForSeconds(0.00001f);

        } while (f < 1.1f);

        SceneManager.LoadScene(0);

        yield return null;
    }

    IEnumerator fadeIn()
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
}
