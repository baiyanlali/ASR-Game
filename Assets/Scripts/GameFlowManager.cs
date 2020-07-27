using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using System;

interface IGameManager {
    void recieveAsrResult(string strResult);
}

public class GameFlowManager : MonoBehaviour
{

    public GameObject gameManager;
    public RectTransform guoChang;
    public Text reverseCount;
    public Text scoreText, timeText;

    private StringBuilder str;
    private string str_before;
    private string str_single;
    private bool isOver;
    private int score;
    float timeForCalling;
    float time;
    public int totalTime;
    public float timePerCalling = 0.2f;
    public bool ASRIsOn;
    IGameManager gameManagerInterface;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager.SetActive(false);
        gameManagerInterface = gameManager.GetComponent<IGameManager>();
        str = new StringBuilder(1024);
        ASRIsOn = false;
        if (Application.isMobilePlatform)
        {
            ASR.recorderSetUp(0);
            ASR.text.text = "Asr has set up";
        }
        guoChang = GameObject.Find("GuoChang").GetComponent<RectTransform>();
        reverseCount.gameObject.SetActive(false);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        LeanTween.alpha(guoChang, 0, 0.5f);

        if (WindowsManager.instance!=null)
            WindowsManager.instance.initWindows();

        //pretend it's over to hold the time
        isOver = true;
         
        yield return new WaitForSeconds(1f);
        reverseCount.gameObject.SetActive(true);
        reverseCount.text = "3";
        yield return new WaitForSeconds(1f);
        reverseCount.text = "2";
        yield return new WaitForSeconds(1f);
        reverseCount.text = "1";
        yield return new WaitForSeconds(1f);
        
        reverseCount.gameObject.SetActive(false);
        gameManager.SetActive(true);
        isOver = false;
        if (Application.isMobilePlatform)
        {
            ASR.startPlay();
            ASRIsOn = true;
        }
    }

    private void FixedUpdate()
    {
        if (Application.platform==RuntimePlatform.WindowsEditor)
        {
            if (Input.anyKeyDown)
           {
               foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
               {
                   if (Input.GetKeyDown(keyCode))
                   {
                       //Debug.Log("Current Key is : " + keyCode.ToString());
                        gameManagerInterface.recieveAsrResult(keyCode.ToString());
                   }
               }
           }
        }
        if (ASRIsOn==false) return;
        try
        {
            timeForCalling += Time.fixedDeltaTime;
            if (timeForCalling >= timePerCalling)
            {
                str.Capacity = 1024;
                timeForCalling %= timePerCalling;
                str_before = str.ToString();

                ASR.catchPlay(1, str);

                //ASR.text.text =  str.ToString()+"| |"+str_before;
                //ASR.text.text = "";

                #region recognizeRe
                if (!str_before.Equals(str.ToString()))
                {
                    string [] strs = str.ToString().Trim().Split(' ');
                    ASR.text.text = strs [strs.Length - 1];
                    gameManagerInterface.recieveAsrResult(strs[strs.Length-1]);
                }
                #endregion
              
            }
        }
        catch (System.Exception o)
        {
            ASR.text.text = o.GetType().ToString() + "| |" + o.Message;
            throw;
        }

    }

    void Update()
    {

        if (!isOver)
        {
            ShowUI();

            time += Time.deltaTime;
        }
    }

    public void ShowUI()
    {
        scoreText.text = score.ToString();
        #region showTime
        int time = totalTime - (int)this.time;
        timeText.text = time < 60 ? time.ToString() : time >= 3600 ? time / 3600 + ":" + time % 3600 / 60 + ":" + time % 3600 % 60 : time / 60 + ":" + time % 60;
        if (time <= 10) timeText.color = Color.red;
        if (time <= 0) gameOver();
        #endregion
    }


    IEnumerator EndGame()
    {
        ASRIsOn = false;
        if (Application.isMobilePlatform)
        {
            str.Capacity = 1024;
            ASR.stopPlay(1, str);

        }

        WindowsManager.instance.showConclusionWindows(score);
        yield return null;
    }

    public void gameOver()
    {
        isOver = true;
        configManager.instance.mySetting.coin += score;
        configManager.instance.saveSetting();
        StartCoroutine(EndGame());
    }

    public string getWords()
    {
        string s = str.ToString();
        str.Clear();
        return s;
    }
    public string getWord()
    {
        
        return str_single;
    }

    public void changeScore(int offset)
    {
        score += offset;
    }

}
