using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CountingAppleManager : MonoBehaviour,IGameManager
{
    //private int  score;
    //private float time;
    //private bool isOver;
    private bool isTransingTurn;
    private int mistakeToken;
    List<GameObject> applesValid = new List<GameObject>();
    List<GameObject> applesInValid = new List<GameObject>();

    //public Text  scoreText, timeText;
    public ParticleSystem winParticlePrefab;
    public ParticleSystem winParticle;
    public GameObject applePostions, applePrefab;
    public List<Transform> applePos;
    public GameObject applePool;
    public GameFlowManager gameFlowManager;

    [Header("")]
    public Sprite [] appleImages;
    [Space]
    [Header("Animations")]
    public AnimationClip [] appearAnimationClips; 
    public AnimationClip [] disappearAnimationClips;




    void Start()
    {
        isTransingTurn = false;
        mistakeToken = 0;
        //isOver = false;
        applesValid.Clear();
        applesInValid.Clear();
        //include applePosition, so remove 0 to remove itself;
        applePos.AddRange( applePostions.GetComponentsInChildren<Transform>());
        applePos.RemoveAt(0);
        ShowApples();
    }

    //void Update()
    //{
        
    //    if(!isOver)
    //        ShowUI();
    //    time += Time.deltaTime;
    //    //int a = StringTool.TurnWordsIntoInt(gameFlowManager.getWord());
    //    //if (a!=999)
    //    //{
    //    //    checkAnswer(a);
    //    //}
    //}

    void checkAnswer(int a)
    {
        ASR.text.text = "Check Answer" + a;
        if (isTransingTurn) return;
        if (a == applesValid.Count) Win();
        else Lose();
    }


    public void ShowApples()
    {
        int RandomAppearance = Random.Range(0, appleImages.Length);
        //print(RandomAppearance);
        //Debug.Log("showApples");
        int RandomAppleCount = Random.Range(1, 11);
        //print(RandomAppleCount);
        int[] applePosRandom = MathTool.DisorderSeq(0, 9);
        Transform pos;
        for (int i = 0 ; i < RandomAppleCount ; i++)
        {
            //print(applePosRandom [i]);
            pos = applePos [applePosRandom [i]];
            if (!PutOutOfPool(pos))
            {
                //Debug.Log("The apple runned out of use");
                GameObject apple = Instantiate(applePrefab, pos);
                apple.transform.SetParent(null);
                applesValid.Add(apple);
            }
        }

            float appearRandom = Random.Range(0f, 1f);
        foreach (GameObject apple in applesValid)
        {

            apple.GetComponent<Animator>().SetBool("IsAppear",true);
            apple.GetComponent<Animator>().SetFloat("ZoomUpBlend", appearRandom);
            
            
            apple.GetComponent<SpriteRenderer>().sprite = appleImages [RandomAppearance];
            apple.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }

    


    //public void ShowUI()
    //{
    //    scoreText.text = score.ToString();

    //    #region showTime
    //    int time = 60 - (int)this.time;
    //    timeText.text = time<60?time.ToString():time>=3600?time/3600+":"+time%3600/60+":"+time%3600%60:time/60+":"+time%60;
    //    if (time <= 10) timeText.color = Color.red;
    //    if (time <= 0) TurnOver();
    //    #endregion
    //}

    //private void TurnOver()
    //{
    //    isOver = true;
    //    //print("show Table");
    //    gameFlowManager.gameOver();
    //}

    public void Win()
    {
        //if (isOver) return;
        if (winParticle==null)
        { 
            //Debug.Log("win!");
            winParticle = Instantiate(winParticlePrefab);
        }
        //winParticle.Emit(5);
        winParticle.collision.SetPlane(1,GameObject.Find("Bound").transform);
        winParticle.Play(true);
        gameFlowManager.changeScore(+2);
        //score++;
        StartCoroutine(NextTurn());
    }

    public void Lose()
    {
        //ASR.text.text += " "+mistakeToken.ToString();
        //if (isOver) return;
        mistakeToken++;
        if (mistakeToken >= 3)
        {
            //ASR.text.text = "You Lose!";
            mistakeToken %= 3;
            foreach (GameObject apple in applesValid)
            {

                apple.GetComponent<Rigidbody2D>().gravityScale = 10;
            }
            StartCoroutine(NextTurn());
        }
        else
        {
            //ASR.text.text = "Try one more time!";
            foreach (GameObject apple in applesValid)
            {
                apple.GetComponent<Animator>().SetTrigger("ShowMistake");
            }
        }
    }


    private IEnumerator NextTurn()
    {
        isTransingTurn = true;
        mistakeToken = 0;
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject apple in applesValid)
        {
            //print("Disappear");
            apple.GetComponent<Animator>().SetBool("IsAppear", false);
            //apple.GetComponent<Animator>().SetBool(0,false);
        }
        yield return new WaitForSeconds(1f);
        foreach (var apple in applesValid)
        {
            PutInPool(apple);
        }
        applesValid.Clear();
        ShowApples();
        isTransingTurn = false;
    }


    void PutInPool(GameObject apple)
    {
        applesInValid.Add(apple);
        apple.transform.SetParent(applePool.transform);
        apple.SetActive(false);
        
    }
    bool PutOutOfPool(Transform trans)
    {
        if (applesInValid.Count==0)
        {
            return false;
        }
        else
        {
            applesInValid [0].SetActive(true);
            Transform appleTrans = applesInValid [0].transform;
            appleTrans.SetParent(null);
            appleTrans.position = trans.position;

            applesValid.Add(applesInValid [0]);
            applesInValid.RemoveAt(0);
            return true;
        }
    }

    public void recieveAsrResult(string strResult)
    {
        int a = StringTool.TurnWordsIntoInt(strResult);
        if (a == 999) return;
        checkAnswer(a);
    }

}
