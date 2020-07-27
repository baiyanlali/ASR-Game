using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RewardManager : MonoBehaviour
{

    public Text coinText;
    public RectTransform guoChang;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.alpha(guoChang, 0, 1f);
        
        //WindowsManager.instance.initWindows();
        coinText = GameObject.Find("CoinText").GetComponent<Text>();
        //print(configManager.instance.mySetting.coin);
        coinText.text = configManager.instance.mySetting.coin.ToString();

        initCommodity();
    }
    Commodity[] commodityData;
    GameObject commodityTemplate;
    public Transform commodityPanel;

    public void initCommodity()
    {
        commodityPanel = GameObject.Find("CommodityPanel").transform;
        string path = "RewardData";
        commodityData = Resources.LoadAll<Commodity>(path);
        commodityTemplate = Resources.Load<GameObject>("Prefabs/Reward/Commodity");
        foreach (var data in commodityData)
        {

            GameObject go = Instantiate(commodityTemplate, commodityPanel);
            Button goBtn = go.GetComponentInChildren<Button>();
            goBtn.onClick.AddListener(delegate { Cost( go, data); });
            
            go.transform.Find("CommodityBorder").Find("Image").GetComponent<Image>().sprite = data.image;
            go.transform.Find("CommodityBorder").Find("Locked").gameObject.SetActive(data.locked);

            //go.transform.Find("Description").GetComponent<Text>().text = data.description;
            //go.transform.Find("Title").GetComponent<Text>().text = data.title;
            go.transform.Find("StarImage").GetChild(0).GetComponent<Text>().text = data.coinCost.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Return2Menu()
    {
        StartCoroutine(returnToMenu());
    }

    IEnumerator returnToMenu()
    {
        LeanTween.alpha(guoChang, 1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }

    public void Cost(GameObject go, Commodity data)
    {
        print(data.locked);
        if (configManager.instance.mySetting.coin >= data.coinCost && data.locked) 
        {
            print("unlock!");
            configManager.instance.mySetting.coin -= data.coinCost;
            coinText.text = configManager.instance.mySetting.coin.ToString();
            //WindowsManager.instance.showPumpUpWindows(true);
            configManager.instance.saveSetting();
            data.locked = false;
            go.transform.Find("CommodityBorder").Find("Locked").gameObject.SetActive(data.locked);
        }
    }


}
