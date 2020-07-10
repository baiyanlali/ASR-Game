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

    int wordIndex = 0;
    

    public GameFlowManager gameFlowManager;
    
    //load next question's word
    public string getWords()
    {
        if (WordList.Count == 0)
        {
            return "end";
        }
        string result = (string)WordList[0];
        WordList.Remove(result);
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
        foreach (string str in Words.wordsToChoose)
        {
            WordList.Add(str);
        }
    }
    
    void Start()
    {
        wordIndex = 0;
        gameFlowManager = GameObject.Find("GameFlowManager").GetComponent<GameFlowManager>();
       initialWordList();
       StartCoroutine( nextPicture());
    }

   
    void Update()
    {
        //banner.showAnwser(userAnswer);
        //if (wordNow.Equals(userAnswer))
        //{
        //    nextPicture();
        //}

        

    }

    public void recieveAsrResult(string strResult)
    {
        strResult = strResult.Trim().ToUpper();
        if (strResult.Equals("pass"))
        {
            StartCoroutine(nextPicture());
            
            return;
        }
        else if (strResult.Length > 2)
        {
            if(Application.isMobilePlatform)
                ASR.text.text = $"{strResult} is to much";
            else print($"{strResult} is to much");
            return;
        }
        //print($"you need {wordNow.ToCharArray() [wordIndex]}, you have {strResult}, so {strResult.Equals(wordNow.ToCharArray() [wordIndex].ToString())} ");
        if (strResult.Equals(wordNow.ToCharArray()[wordIndex].ToString()))
        {

            Debug.Log("equals");
            banner.showAnwser(strResult,wordIndex);
            wordIndex++;
        }


        if (wordIndex == wordNow.Length) 
        {
            StartCoroutine(nextPicture());
            
        }
        userAnswer = strResult;
        //throw new NotImplementedException();
    }
}
