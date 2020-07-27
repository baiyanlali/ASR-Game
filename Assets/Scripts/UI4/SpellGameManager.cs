using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
public class SpellGameManager : MonoBehaviour,IGameManager
{
    //a list that all questions are in it
    public ArrayList WordList = new ArrayList();
    public Banner banner;
    public Image question;
    //the answer(word) of the picture now
    public string wordNow;
    //the input of user
    public string userAnswer;

    int wordTotal;

    Sprite [] images;

    int wordIndex = 0;
    

    public GameFlowManager gameFlowManager;
    
    //load next question's word
    public string getWords()
    {
        if (WordList.Count == 0)
        {
            return "end";
        }

            //print($"{wordOrder [0]} is {WordList[wordOrder [0]]}");

        
        
        string result = (string)WordList[wordOrder[0]];
        wordOrder.RemoveAt(0);
        //WordList.Remove(result);
        wordNow = result;
        return result;
    }
    //load next question's picture 
    public void getImage(string name)
    {
        string path = "Pictures/" + name;
        //Debug.Log(path);
        question.sprite = Resources.Load<Sprite>(path);
        
        //Debug.Log("加载成功");
    }
    //load next question
    IEnumerator nextPicture()
    {
        //print("next picture!");
        if (wordTotal==images.Length)
        {
            gameFlowManager.gameOver();
        }
        wordTotal++;
        //If it's played more than one turn
        if (wordIndex != 0)
            yield return new WaitForSeconds(1f);
        wordIndex = 0;
        //Debug.Log("Next question");
        string pictureName = getWords();
        if (pictureName.Equals("end"))
        {

            yield return null;
        }
        //Debug.Log(pictureName);
        getImage(pictureName);
        banner.loadInputField(pictureName);
        //Debug.Log("banner改变了");
    }
    
    //initial the WordList
    private void initialWordList()
    {
        string path = "Pictures" ;
        images = Resources.LoadAll<Sprite>(path);

        foreach (Sprite sprite in images)
        {
            WordList.Add(sprite.name);
        }

    }

    List<int> wordOrder;

    void Start()
    {
        wordIndex = 0;
        gameFlowManager = GameObject.Find("GameFlowManager").GetComponent<GameFlowManager>();
       initialWordList();
        wordOrder = new List<int>();
        wordOrder.AddRange( MathTool.DisorderSeq(0, WordList.Count - 1));
        //foreach (var item in wordOrder)
        //{
        //    print(item);
        //}
       StartCoroutine( nextPicture());
    }

   
    void Update()
    {
        CD += Time.deltaTime;
        

    }
    float CD;

    public void recieveAsrResult(string strResult)
    {
        //if (CD<=1f)
        //{
        //    //print("CD has not reach!");
        //    return;
        //}
        strResult = strResult.Trim().ToUpper();
        if (strResult.Equals("PASS"))
        {
            StartCoroutine(nextPicture());
            
            return;
        }
        else if (strResult.Length >= 2)
        {
            if (Application.isMobilePlatform)
                ASR.text.text = $"{strResult} is to much";
            //else print($"{strResult} is to much");
            return;
        }
        //print($"you need {wordNow.ToCharArray() [wordIndex]}, you have {strResult}, so {strResult.Equals(wordNow.ToCharArray() [wordIndex].ToString())} ");
        if (strResult.Equals(wordNow.ToCharArray()[wordIndex].ToString()))
        {
            CD = 0;
            gameFlowManager.changeScore(+1);
            //Debug.Log("equals");
            //print(wordIndex);
            banner.showAnwser(strResult,wordIndex);
            wordIndex++;
            //WindowsManager.instance.showPumpUpWindows("GOOD");
        }


        if (wordIndex == wordNow.Length) 
        {
            WindowsManager.instance.showPumpUpWindows(true);
            //WindowsManager.instance.showPumpUpWindows("EXCELLENT");
            StartCoroutine(nextPicture());
            
        }
        userAnswer = strResult;
        //throw new NotImplementedException();
    }
}
