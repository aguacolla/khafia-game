using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CSVTest : MonoBehaviour
{
    private List<string> str;

    public string[] WordsArrays(string letter)
    {
        //List<string> words = new List<string>();
        var csv = Resources.Load<TextAsset>($"Words/{letter}");
        var words = csv.text.Split(",").ToArray();
        return words;
    }
    // Start is called before the first frame update
    void Start()
    {
        string[] ar = WordsArrays("baa2");
        print(ar.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            for (int i = 0; i < 5; i++)
            {
                print(str[i]);
            }
        }
    }
}
