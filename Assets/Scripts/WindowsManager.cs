using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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

    [Header("Windows")]
    public GameObject pumpUpWindows;
    public GameObject conclusionWindows;
    public GameObject pauseWindow;
    public GameObject settingWindow;
    /// <summary>
    /// Load conclusion window,pause window,pump up window
    /// </summary>
    public void initWindows()
    {
        //print("initWindows");
        isPlaying = false;
        audioSource = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("RewardScene"))
        {
            Button pauseBtn = GameObject.Find("PauseBtn").GetComponent<Button>();
            pauseBtn.onClick.AddListener(showPauseWindows);

            Button settingBtn = GameObject.Find("SettingBtn").GetComponent<Button>();
            settingBtn.onClick.AddListener(showSettingWindow);
        }
        initConclusionWindow();

        initPumpUpWindow();

        initPauseWindow();

        initSettingWindow();



    }


    public void initWindowsLite()
    {
        initSettingWindow();

    }

    private void initSettingWindow()
    {
        string settingWindowPath = "prefabs/Windows/SettingWindow";
        GameObject windowsBg = Resources.Load<GameObject>(settingWindowPath);
        settingWindow = Instantiate(windowsBg, GameObject.Find("Canvas").transform);

        settingWindow.transform.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(shutOffWindows);

        settingWindow.transform.Find("Music").GetComponent<Toggle>().isOn = configManager.instance.mySetting.music;
        settingWindow.transform.Find("Music").GetComponent<Toggle>().onValueChanged.AddListener(delegate { settingMusic(); });

        settingWindow.transform.Find("SFX").GetComponent<Toggle>().isOn = configManager.instance.mySetting.sfx;
        settingWindow.transform.Find("SFX").GetComponent<Toggle>().onValueChanged.AddListener(delegate { settingSFX(); });

        Scrollbar scrollbar = settingWindow.transform.Find("Difficulty").Find("DifficultyScrollbar").GetComponent<Scrollbar>();
        scrollbar.value = configManager.instance.mySetting.difficulty * 0.5f;
        scrollbar.onValueChanged.AddListener(delegate { settingDifficulty(scrollbar.value); });



        settingWindow.SetActive(false);
    }

    private void initPauseWindow()
    {
        string PauseWindowPath = "prefabs/Windows/PauseWindow";
        pauseWindow = Instantiate(Resources.Load<GameObject>(PauseWindowPath), GameObject.Find("Canvas").transform);
        pauseWindow.gameObject.SetActive(false);
        pauseWindow.transform.Find("ReturnBtn").GetComponent<Button>().onClick.AddListener(returnFunc);
        pauseWindow.transform.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(shutOffWindows);

        

        pauseWindow.SetActive(false);
        
    }

    private void initPumpUpWindow()
    {
        string pumpUpWindowsPath = "prefabs/Windows/PumpUpWindows";
        pumpUpWindows = Instantiate(Resources.Load<GameObject>(pumpUpWindowsPath), GameObject.Find("Canvas").transform);
        pumpUpWindows.gameObject.SetActive(false);

        pumpUpWindows.SetActive(false);
    }



    private void initConclusionWindow()
    {
        string windowsBgPath = "prefabs/Windows/WindowsBg";
        GameObject windowsBg = Resources.Load<GameObject>(windowsBgPath);
        conclusionWindows = Instantiate(windowsBg, GameObject.Find("Canvas").transform);
        conclusionWindows.SetActive(false);

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

        conclusionWindows.SetActive(false);
    }

    public void showPauseWindows()
    {
        //if (conclusionWindows.activeInHierarchy||settingWindow.activeInHierarchy) return;
        pauseWindow.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void shutOffWindows()
    {
        if(pauseWindow!=null)
            pauseWindow.gameObject.SetActive(false);
        if(settingWindow!=null)
            settingWindow.gameObject.SetActive(false);
        configManager.instance.saveSetting();
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
        pumpUpWindows.GetComponent<Image>().SetNativeSize();
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

    public void showSettingWindow()
    {
        //if (conclusionWindows.activeInHierarchy || pauseWindow.activeInHierarchy) return;
        settingWindow.gameObject.SetActive(true);
        Time.timeScale = 0;
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
        //WWW www = new WWW($"file://{path}");
        //yield return www;

        var uwr = UnityWebRequestMultimedia.GetAudioClip($"file://{path}", AudioType.WAV);
        yield return uwr.SendWebRequest();
        ASR.text.text = "wav has loaded";

        ac = DownloadHandlerAudioClip.GetContent(uwr);


        //ac = www.GetAudioClip();
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

    public void settingMusic()
    {
        configManager.instance.mySetting.music = !configManager.instance.mySetting.music;
        //pauseWindow.transform.Find("Music").GetComponent<Toggle>().isOn = configManager.instance.mySetting.music;
    }

    public void settingSFX()
    {
        configManager.instance.mySetting.sfx = !configManager.instance.mySetting.sfx;
        //pauseWindow.transform.Find("SFX").GetComponent<Toggle>().isOn = configManager.instance.mySetting.sfx;
    }

    public void settingDifficulty(float difficulty)
    {
        if (difficulty<0.3)
        {
            configManager.instance.mySetting.difficulty = 0;

        }
        else if(difficulty<=0.7)
        {
            configManager.instance.mySetting.difficulty = 1;
        }
        else
        {
            configManager.instance.mySetting.difficulty = 2;

        }
    }

}
