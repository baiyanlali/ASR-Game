using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Android;

public class MenuManager : MonoBehaviour
{

    GameObject guoChang ;
    Animator anim_guoChang;

    void Start()
    {
        guoChang = GameObject.Find("GuoChang");
        anim_guoChang = guoChang.GetComponent<Animator>();
        guoChang.SetActive(false);
        SceneManager.activeSceneChanged += BackToMenu;
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone)==false)
        {
            Microphone.Start(Microphone.devices [0], false, 1, 16000);
        }
        WindowsManager.instance.initWindowsLite();
    }

    private void BackToMenu(Scene arg0, Scene arg1)
    {
        //guoChang = GameObject.Find("GuoChang");
        //anim_guoChang = guoChang.GetComponent<Animator>();
        if (arg1.buildIndex == 1)
        {
            //guoChang.SetActive(true);
        }

    }



    public void LoadScene(string SceneName)
    {
        StartCoroutine(LoadSceneAsyn(SceneName));
    }

    IEnumerator LoadSceneAsyn(string SceneName)
    {
        guoChang.SetActive(true);
        Animator anim = guoChang.GetComponent<Animator>();
        anim.Play("UIZoomUp");

        AsyncOperation asyn = SceneManager.LoadSceneAsync(SceneName,LoadSceneMode.Single);
        asyn.allowSceneActivation = false;
        
        //Scene scene = SceneManager.GetSceneByName(SceneName);
        yield return new WaitForSeconds(1f);

        asyn.allowSceneActivation = true;
        yield return null;
    }

    public void showSettingWindow()
    {
        WindowsManager.instance.showSettingWindow();
    }


}
