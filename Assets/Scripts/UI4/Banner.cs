using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Banner : MonoBehaviour
{
    public GameObject obj;
    public ArrayList inputFieldList = new ArrayList();
    public GameObject inputfield;
    

    
    //加载banner长度；在下一图片的时候移除所有输入框；重新生成输入框；
    //并将输入框添加到inputList中
    public void loadInputField (string question)
    {
        if (inputFieldList.Count!=0)
        {
            for (int i = 0; i < inputFieldList.Count; i++)
            {
                GameObject.Destroy(GameObject.Find((string)inputFieldList[i]));
            }
        }
        int length = question.Length;
        int withd = 80 + 80 * length;//60 +(length-1)*5 + length * 50;
        RectTransform transform = obj.transform.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(withd, 400);

        for (int i = 0; i < length; i++)
        {
            string path = "Prefabs/Text";
            GameObject prefab = (GameObject)Resources.Load(path);
            GameObject instance = Instantiate(prefab);
            instance.GetComponentInChildren<Text>().text = "";
            instance.transform.SetParent(obj.transform);
            //instance.transform.localScale = new Vector3(1,1,1);
            instance.transform.name = "Text" + i.ToString();
            inputFieldList.Add(instance.transform.name);
            //Debug.Log(inputFieldList[i]);
        }
    }

    
    public void showAnwser(string word)
    {
        char[] chars = word.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            inputfield = GameObject.Find((string) inputFieldList[i]);
            //Debug.Log((string) inputFieldList[i]);
            Text text = inputfield.transform.GetComponent<Text>();
            text.text = chars[i].ToString();
        }
    }
    public void showAnwser(string word,int index)
    {
        try
        {
            inputfield = GameObject.Find((string) inputFieldList[index]).gameObject.transform.Find("Char").gameObject;
            inputfield.GetComponent<Animator>().Play("CharShowUp");

            //Debug.Log((string) inputFieldList[i]);
            Text text = inputfield.transform.GetComponent<Text>();
            text.text = word;
            if(Application.isMobilePlatform)
                ASR.text.text = word;
        }
        catch (Exception o)
        {
            if (Application.isMobilePlatform)
                ASR.text.text = $" {o.GetType().ToString()}| |{o.Message}";
            else
                print($" {o.GetType().ToString()}| |{o.Message}");
            throw;
        }

    }
    

    public void set(string word)
    {
        inputfield = GameObject.Find("Text");
        Text text = inputfield.transform.GetComponent<Text>();
        text.text =word;
    }
    
    void Start()
    {
        obj = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
