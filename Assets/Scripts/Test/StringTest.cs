using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("START");
        print("start");
        string str = "five six ";
        string [] strs = str.Trim().Split(' ');
        foreach (var item in strs)
        {
            print(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
