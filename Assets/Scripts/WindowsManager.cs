using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WindowsManager : MonoBehaviour
{
    public static WindowsManager instance;
    public Text pumpUpWindows;
    public GameObject conclusionWindows;
    public AudioSource audioSource;

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

    public void initWindows()
    {
        //print("initWindows");
        audioSource = GetComponent<AudioSource>();
        string windowsBgPath = "prefabs/Windows/WindowsBg";
        GameObject windowsBg = Resources.Load<GameObject>(windowsBgPath);
        conclusionWindows = Instantiate(windowsBg, GameObject.Find("Canvas").transform);
        conclusionWindows.SetActive(false);

        string pumpUpWindowsPath = "prefabs/Windows/PumpUpWindows";
        pumpUpWindows = Instantiate(Resources.Load<GameObject>(pumpUpWindowsPath), GameObject.Find("Canvas").transform).GetComponent<Text>();
        pumpUpWindows.gameObject.SetActive(false);
        //conclusionWindows.transform.SetParent();

    }


    public void showPumpUpWindows(string title)
    {
        pumpUpWindows.text = title;
        StartCoroutine(showPumpUpWindows());
    }

    IEnumerator showPumpUpWindows()
    {
        pumpUpWindows.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        pumpUpWindows.gameObject.SetActive(false);
        yield return null;
    }

    public void showConclusionWindows(int score)
    {
        //print("show conclusion windows");
        conclusionWindows.SetActive(true);
        Text scoreText = conclusionWindows.transform.Find("Text").GetComponent<Text>();
        scoreText.text = $"You have won \n{score}\n scores!!";
        Button replayBtn = conclusionWindows.transform.Find("ReplayBtn").GetComponent<Button>();
        replayBtn.onClick.AddListener(replay);
        Button returnBtn = conclusionWindows.transform.Find("ReturnBtn").GetComponent<Button>();
        returnBtn.onClick.AddListener(returnFunc);
        Button playBtn = conclusionWindows.transform.Find("PlayBtn").GetComponent<Button>();
        playBtn.onClick.AddListener(delegate { playWav(configManager.instance.mySetting.lang); });
    }

    public void replay()
    {
        if(ASR.text!=null)ASR.text.text = "replay";
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void returnFunc(){
        if (ASR.text != null) ASR.text.text = "return";
        SceneManager.LoadScene(1);
         //SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

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
}
