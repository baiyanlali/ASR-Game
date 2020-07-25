using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RewardManager : MonoBehaviour
{

    public Text coinText;

    // Start is called before the first frame update
    void Start()
    {
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
            go.GetComponent<Button>().onClick.AddListener(delegate { Cost(data.coinCost); });
            
            go.transform.Find("Image").GetComponent<Image>().sprite = data.image;
            
            go.transform.Find("Description").GetComponent<Text>().text = data.description;
            go.transform.Find("Title").GetComponent<Text>().text = data.title;
            go.transform.Find("StarImage").GetChild(0).GetComponent<Text>().text = data.coinCost.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Return2Menu()
    {
        SceneManager.LoadScene(1);
    }
    
    public void Cost(int coin)
    {
        if (configManager.instance.mySetting.coin >= coin)
        {
            configManager.instance.mySetting.coin -= coin;
            coinText.text = configManager.instance.mySetting.coin.ToString();
            //WindowsManager.instance.showPumpUpWindows(true);
            configManager.instance.saveSetting();
        }
    }

}
