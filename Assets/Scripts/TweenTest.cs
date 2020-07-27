using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenTest : MonoBehaviour
{
    public float to;
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        print("working");
        rect = GetComponent<RectTransform>();
        //LeanTween.value(rect.GetComponent<Image>().color.a, 0f, 1);
        //LeanTween.alpha(GetComponent<Image>().rectTransform, to, 1f);
        LeanTween.color(GetComponent<RectTransform>(), new Color(0, 0, 0, 0), 0.5f);
        //LeanTween.move(this.gameObject, new Vector2(2, 0), 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
