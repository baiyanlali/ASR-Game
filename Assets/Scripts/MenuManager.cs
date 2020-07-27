using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Android;

public class MenuManager : MonoBehaviour
{

    RectTransform guoChang ;

    void Start()
    {
        guoChang = GameObject.Find("GuoChang").GetComponent<RectTransform>();
        LeanTween.alpha(guoChang, 0, 0.5f);
        SceneManager.activeSceneChanged += BackToMenu;
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone)==false)
        {
            Microphone.Start(Microphone.devices [0], false, 1, 16000);
        }
        WindowsManager.instance.initWindowsLite();
    }

    private void BackToMenu(Scene arg0, Scene arg1)
    {
        print($"From {arg0.name} to {arg1.name}");
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
        //Animator anim = guoChang.GetComponent<Animator>();
        //anim.Play("UIZoomUp");
        LeanTween.alpha(guoChang, 1, 0.5f);

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
