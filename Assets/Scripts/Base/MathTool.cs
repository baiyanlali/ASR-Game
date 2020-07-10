using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MathTool
{
    /// <summary>
    /// 输出一个从数字a（包括）到b（包括）的乱序数组
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static public int[] DisorderSeq(int a, int b)
    {
        List<int> seq = new List<int>();
        List<int> disorderSeq = new List<int>();
        for (int i = a ; i <= b ; i++)
        {
            seq.Add(i);
        }
        for (int i = a ; i <= b ; i++)
        {
            int randomIndex = Random.Range(0, seq.Count);
            disorderSeq.Add(seq [randomIndex]);
            seq.RemoveAt(randomIndex);
        }
        return disorderSeq.ToArray();
    }
}


public class StringTool
{
    public static int TurnWordsIntoInt(string word)
    {
        word = word.Trim();
        int a = 999;

        switch (word)
        {
            case "zero":a = 0;break;
            case "one":a = 1;break;
            case "two":
            case "to":
                a = 2;break;
            case "three":
                a = 3;break;
            case "four":
            case "for":
                a = 4;break;
            case "five":
                a = 5;break;
            case "six":
                a = 6;break;
            case "seven":
                a = 7;break;
            case "eight":
                a = 8;break;
            case "nine":
                a = 9;break;
            case "ten":
                a = 10;break;
            case "eleven":
                a = 11;break;
            case "twelve":
                a = 12;break;
            case "thirteen":
                a = 13;break;
            case "fourteen":
                a = 14;break;
            case "fifteen":
                a = 15;break;
            case "sixteen":
                a = 16;break;
            case "seventeen":
                a = 17;break;
            case "eighteen":
                a = 18;break;
            case "nineteen":
                a = 19;break;
            case "twenty":
                a = 20;break;
            default:
                a = 999;
                break;
        }

        return a;
    }
    /// <summary>
    /// return a value of string owned by s2 but not s1
    /// </summary>
    /// <param name="s2"></param>
    /// <param name="s1"></param>
    /// <returns></returns>
    public static string minus(string s2,string s1)
    {
        if (!s2.Contains(s1)) return "!@#$%";//throw new System.Exception("The s1 is not a substring of s2");
        char [] c0 = s2.ToCharArray();
        char [] c1 = new char [s2.Length - s1.Length];
        for (int i = 0 ; i < c1.Length ; i++)
        {
            c1 [i] = c0 [s1.Length+i];
        }
        return new string(c1);
    }

    public static string minusInCommon(string s2,string s1)
    {


        char [] c0 = s2.ToCharArray();
        char [] c1 = new char [s2.Length - s1.Length];
        for (int i = 0 ; i < c1.Length ; i++)
        {
            c1 [i] = c0 [s1.Length + i];
        }
        return new string(c1);
    }
}
