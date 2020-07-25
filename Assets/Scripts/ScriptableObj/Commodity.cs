using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Creat Commodity")]
public class Commodity : ScriptableObject
{
    public string title;
    public string description;
    public int coinCost;
    public Sprite image;

}
