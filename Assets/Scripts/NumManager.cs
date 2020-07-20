using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

enum Sign
{
    Plus, Minus//,Multiply,Divide
}

public class NumManager : MonoBehaviour, IGameManager
{
    public Text numAText;
    public Text numBText;
    public Text signText;
    public Text conclusionText;
    public GameFlowManager gameFlowManager;

    public InputField answerInputField;
    Sign sign;
    int numA, numB;
    int conclusion;
    int mistake;


    // Start is called before the first frame update
    void Start()
    {
        mistake = 0;
        initMaths();
    }

    // Update is called once per frame


    public void initMaths()
    {
        string signal = "";
        numA = Random.Range(1, 10);
        numB = Random.Range(1, 10);
        Sign [] signs = Enum.GetValues(typeof(Sign)) as Sign [];
        sign = signs [Random.Range(0, 1)];
        switch (sign)
        {
            case Sign.Plus:
                conclusion = numA + numB;
                signal = "+";
                break;
            case Sign.Minus:
                if (numA < numB)
                {
                    int t = numA;
                    numA = numB;
                    numB = t;
                }
                conclusion = numA - numB;

                signal = "-";
                break;
            #region Multiply and divide
            //case Sign.Multiply:
            //    conclusion = numA * numB;
            //    signal = "*";
            //    break;
            //case Sign.Divide:
            //    conclusion = numA / numB;
            //    signal = "/";
            //    if (numA%numB!=0)
            //    {
            //        initMaths();
            //    }
            //    break;

            #endregion
            default:
                break;
        }
        numAText.text = numA.ToString();
        numBText.text = numB.ToString();
        signText.text = signal;
        conclusionText.text = "?";
    }

    IEnumerator nextTurn()
    {
        yield return new WaitForSeconds(0.3f);
        initMaths();
    }

    public void checkAnswer(int answer)
    {
        conclusionText.text = answer.ToString();
        if (answer == conclusion)
        {
            //WindowsManager.instance.showPumpUpWindows("GREAT");
            WindowsManager.instance.showPumpUpWindows(true);
            ASR.text.text = "You win!";//Debug.Log("You win!");
            gameFlowManager.changeScore(+2);
        }
        else
        {
            mistake++;
            if (mistake < 3)
            {
                //WindowsManager.instance.showPumpUpWindows("TRY");
                WindowsManager.instance.showPumpUpWindows(false);
                return;
            }
            ASR.text.text = "You lose...";//Debug.Log("You lose...");
        }
        mistake = 0;
        StartCoroutine(nextTurn());
        //return answer == conclusion;
    }

    public void pressButton()
    {
        //if (checkAnswer(int.Parse(answerInputField.text))) ASR.text.text = "You win!";//Debug.Log("You win!");
        //else ASR.text.text = "You lose...";//Debug.Log("You lose...");
        StartCoroutine(nextTurn());
    }

    public void recieveAsrResult(string strResult)
    {
        if (strResult.Equals("pass"))
        {
            StartCoroutine(nextTurn());
        }
        int a = StringTool.TurnWordsIntoInt(strResult);
        //ASR.text.text = "Num Manager has recieve a result:" + a;
        if (a == 999) return;
        checkAnswer(a);
    }
}
