using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WindowsManager : MonoBehaviour
{
    public static WindowsManager instance;
    public AudioSource audioSource;

    [Header("GoodImage")]
    public Sprite [] praiseImage;
    [Header("TryImage")]
    public Sprite [] tryImage;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
    }

    public GameObject pumpUpWindows;
    public GameObject conclusionWindows;
    public GameObject pauseWindow;
    public void initWindows()
    {
        //print("initWindows");
        isPlaying = false;
        audioSource = GetComponent<AudioSource>();
        string windowsBgPath = "prefabs/Windows/WindowsBg";
        GameObject windowsBg = Resources.Load<GameObject>(windowsBgPath);
        conclusionWindows = Instantiate(windowsBg, GameObject.Find("Canvas").transform);
        conclusionWindows.SetActive(false);

        string pumpUpWindowsPath = "prefabs/Windows/PumpUpWindows";
        pumpUpWindows = Instantiate(Resources.Load<GameObject>(pumpUpWindowsPath), GameObject.Find("Canvas").transform);
        pumpUpWindows.gameObject.SetActive(false);
        //conclusionWindows.transform.SetParent();

        #region init Conclusion win
        Button replayBtn = conclusionWindows.transform.Find("ReplayBtn").GetComponent<Button>();
        replayBtn.onClick.AddListener(replay);
        Button returnBtn = conclusionWindows.transform.Find("ReturnBtn").GetComponent<Button>();
        returnBtn.onClick.AddListener(returnFunc);
        Button playBtn = conclusionWindows.transform.Find("PlayBtn").GetComponent<Button>();
        playBtn.onClick.AddListener(delegate { playWav(configManager.instance.mySetting.lang); });
        wavSlider = conclusionWindows.GetComponentInChildren<Slider>();
        scoreText = conclusionWindows.transform.Find("Text").GetComponent<Text>();
        #endregion


        string PauseWindowPath = "prefabs/Windows/PauseWindow";
        pauseWindow = Instantiate(Resources.Load<GameObject>(PauseWindowPath), GameObject.Find("Canvas").transform);
        pauseWindow.gameObject.SetActive(false);
        pauseWindow.transform.Find("ReturnBtn").GetComponent<Button>().onClick.AddListener(returnFunc);
        pauseWindow.transform.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(shutOffPauseWindows);

        Button pauseBtn = GameObject.Find("PauseBtn").GetComponent<Button>();
        pauseBtn.onClick.AddListener(showPauseWindows);
    }

    public void showPauseWindows()
    {
        if (conclusionWindows.activeInHierarchy) return;
        pauseWindow.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void shutOffPauseWindows()
    {
        pauseWindow.gameObject.SetActive(false);
        Time.timeScale = 1;
    }


    public void showPumpUpWindows(bool isGood)
    {
        if (isGood)
        {
            pumpUpWindows.GetComponent<Image>().sprite = praiseImage [Random.Range(0, praiseImage.Length)];
        }
        else
        {
            pumpUpWindows.GetComponent<Image>().sprite = tryImage [Random.Range(0, tryImage.Length)];
        }
        StartCoroutine(showPumpUpWindows());
    }

    IEnumerator showPumpUpWindows()
    {
        pumpUpWindows.gameObject.SetActive(true);
        pumpUpWindows.gameObject.GetComponent<Animator>().Play("PumpUpUp");
        yield return new WaitForSeconds(1f);
        pumpUpWindows.gameObject.SetActive(false);
        yield return null;
    }

    
        Text scoreText;
    public void showConclusionWindows(int score)
    {
        //print("show conclusion windows");
        conclusionWindows.SetActive(true);
        scoreText.text = $"You have won \n{score}\n scores!!";
        
    }

    public void replay()
    {
        audioSource.Stop();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void returnFunc(){
        audioSource.Stop();
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(1);
    }

    bool isPlaying = false;
    AudioClip ac;
    public void playWav(int lang)
    {
        ASR.text.text = "wav will be turned";
        PCMConvertor.PCM2WAV($"{Application.persistentDataPath}/{configManager.instance.directories [lang]}/replay.pcm", $"{Application.persistentDataPath}/{configManager.instance.directories [lang]}/replay.wav", 16000, 1, 8);
        ASR.text.text = "wav has been turned";
        StartCoroutine(loadWAV($"{Application.persistentDataPath}/{configManager.instance.directories [lang]}/replay.wav"));
    }

    public IEnumerator loadWAV(string path)
    {
        WWW www = new WWW($"file://{path}");
        yield return www;
        ASR.text.text = "wav has loaded";


        
        ac = www.GetAudioClip();
        audioSource.clip = ac;
        
        audioSource.Play();

        
    }

    public Slider wavSlider;

    private void Update()
    {
        if (audioSource!=null && audioSource.isPlaying)
        {
            wavSlider.value =
            audioSource.time / audioSource.clip.length;

        }
    }

}
