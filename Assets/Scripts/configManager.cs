using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class configManager : MonoBehaviour
{

    public static configManager instance;
    string requestDomainName = "https://www.triplebug.com/";
    string settingFileName = "setting.dat";
    public string [] directories = { "game_chinese", "game_english" };
    string bufferString;
    public setting mySetting;
    byte [] bufferByte;
    [SerializeField]
    public Dictionary<string, int> test;
    [Header("UI element")]
    public Text status;
    public Text percentage;
    public Slider progessBar;

    // Use this for initialization
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
        StartCoroutine(onStartSetup());
    }

    IEnumerator onStartSetup()
    {
        requestPermission();
        loadSetting();
        yield return StartCoroutine(serverRequest());
        yield return StartCoroutine(loadScene(1));
    }

    void requestPermission()
    {
        string DEFAULT_DEVICE = Microphone.devices [0];
        if (!Microphone.IsRecording(DEFAULT_DEVICE))
        {
            Microphone.Start(DEFAULT_DEVICE, false, 1, 44100);
        }
        Microphone.End(DEFAULT_DEVICE);
    }

    IEnumerator serverRequest()
    {
        foreach (var item in directories)
        {
            yield return StartCoroutine(requestFileBuffer($"{item}/checksum"));
            yield return StartCoroutine(checkMD5sum(item));
        }
    }

    IEnumerator loadScene(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        status.text = "Loading...";
        while (!operation.isDone)
        {
            percentage.text = (Mathf.Clamp01(operation.progress)).ToString("#.0%");
            progessBar.value = Mathf.Clamp01(operation.progress);
            yield return null;
        }
    }
    void loadSetting()
    {
        mySetting.ranking = new Dictionary<string, int>();
        if (File.Exists(Application.persistentDataPath + $"/{settingFileName}"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + $"/{settingFileName}", FileMode.Open);
            mySetting = (setting)bf.Deserialize(file);
            file.Close();
        }
        if (mySetting.ranking.Count < 5)
        {
            int size = mySetting.ranking.Count;
            for (int i = 0 ; i < (5 - size) ; i++)
            {
                mySetting.ranking.Add($"{System.DateTime.Now.ToString()}/{i.ToString()}", 0);
            }
        }
    }

    void changeModel()
    {
    }

    public void saveSetting()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + $"/{settingFileName}");

        bf.Serialize(file, mySetting);
        file.Close();
    }

    IEnumerator requestFileBuffer(string serverPath)
    {
        UnityWebRequest www = new UnityWebRequest($"{requestDomainName}{serverPath}");
        www.downloadHandler = new DownloadHandlerBuffer();
        status.text = "Downloading...";
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            bufferString = www.downloadHandler.text;
            // Or retrieve results as binary data
            bufferByte = www.downloadHandler.data;
        }


    }

    IEnumerator requestFile(string serverPath, string targetPath)
    {
        var myWr = new UnityWebRequest($"{requestDomainName}{serverPath}", UnityWebRequest.kHttpVerbGET);
        string path = Path.Combine(Application.persistentDataPath, targetPath);
        myWr.downloadHandler = new DownloadHandlerFile(path);
        // status.text = ($"downloading {serverPath}");
        status.text = "Downloading...";
        myWr.SendWebRequest();
        while (!myWr.isDone)
        {
            percentage.text = (Mathf.Clamp01(myWr.downloadProgress)).ToString("#.0%");
            progessBar.value = Mathf.Clamp01(myWr.downloadProgress);
            yield return null;
        }

        if (myWr.isNetworkError || myWr.isHttpError)
        {
            // status.text=($"myWr erro {myWr.error} ");
        }
        else
        {
            // status.text=($"file save to ${path}");
        }
    }

    IEnumerator checkMD5sum(string dir)
    {
        string [] words = bufferString.Split(new [] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in words)
        {
            string [] delimiter = { " " };
            string [] innerWords = item.Split(delimiter, System.StringSplitOptions.RemoveEmptyEntries);
            string fileName = $"{Application.persistentDataPath}/{dir}/{innerWords [1]}";
            if (System.IO.File.Exists(fileName))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        var hash = md5.ComputeHash(stream);
                        string currentHash = System.BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        if (currentHash.CompareTo(innerWords [0]) != 0)
                        {// hash not equal
                            #region NEW ADDED
                            stream.Close();
                            print($"{fileName} has been deleted");
                            File.Delete(fileName);
                            #endregion
                            yield return StartCoroutine(requestFile($"{dir}/{innerWords [1]}", $"{dir}/{innerWords [1]}"));
                        }
                    }
                }
            }
            else
            {// file not exist
                yield return StartCoroutine(requestFile($"{dir}/{innerWords [1]}", $"{dir}/{innerWords [1]}"));
            }
        }
        yield return null;
    }

}

[Serializable]
public class setting
{
    public int lang; //0 chinese 1 englsih
    public bool music;
    public bool sfx;
    public Dictionary<string, int> ranking;
}