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
    public GameObject guoChang;
    public Text reverseCount;
    public Text scoreText, timeText;
    public Animator anim_guoChang;
    public RuntimeAnimatorController animControllerUp;
    public RuntimeAnimatorController animControllerOut;

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
        
        anim_guoChang = guoChang.GetComponent<Animator>();
        reverseCount.gameObject.SetActive(false);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        if (WindowsManager.instance!=null)
            WindowsManager.instance.initWindows();
           isOver = true;
           anim_guoChang.runtimeAnimatorController = animControllerOut;
           yield return new WaitForSeconds(1f);
           guoChang.SetActive(false);
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
        if (!ASRIsOn) return;
        try
        {
            timeForCalling += Time.fixedDeltaTime;
            if (timeForCalling >= timePerCalling)
            {
                str.Capacity = 1024;
                timeForCalling %= timePerCalling;
                str_before = str.ToString();

                ASR.catchPlay(0, str);

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
                #region old code
                //#region First Saying
                //if (str_before.Equals("") && !str.ToString().Equals("")) 
                //{
                //    gameManagerInterface.recieveAsrResult(str.ToString());
                //    return;
                //}
                //#endregion

                //#region Continous Saying
                //str_single = StringTool.minus(str.ToString(), str_before);
                ////example "C " change to "six "
                //if (str_single.Equals("!@#$%"))
                //{


                //}
                //if (!str_single.Equals(str.ToString()))
                //{
                //    gameManagerInterface.recieveAsrResult(str_single);
                //}
                //#endregion
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
            ASR.stopPlay(0, str);
            //ASR.recorderSetDown(1);

        }
        //guoChang.SetActive(true);
        anim_guoChang.runtimeAnimatorController = animControllerUp;

        AsyncOperation asyn = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        asyn.allowSceneActivation = false;

        //Scene scene = SceneManager.GetSceneByName(SceneName);
        yield return new WaitForSeconds(1f);
        WindowsManager.instance.showConclusionWindows(score);
        //asyn.allowSceneActivation = true;
        yield return null;
    }

    public void gameOver()
    {
        isOver = true;
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
