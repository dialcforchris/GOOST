using UnityEngine;
using System.Collections;

//Might as well use this script to do the text wrapping for 3D text mesh since it's alreayd on the object
public class Push3DtoFront : MonoBehaviour
{
    public int layerToPushTo;
    [Range(16,140)]
    public int maxLineChars = 35;
    TextMesh thisText;
    void Start()
    {
        thisText = GetComponent<TextMesh>();
        GetComponent<Renderer>().sortingOrder = layerToPushTo;
        //GetComponent<Renderer>().sortingLayerName = layerToPushTo;
    }

    string oldtext;

    void Update()
    {
        if (thisText.text != oldtext)
        {
            thisText.text = FormatString(thisText.text, thisText);
        }
    }

    string FormatString(string text, TextMesh textObject)
    {
        int charCount = 0;
        var result = "";

        string[] words;
        words = text.Split(" "[0]); //Split the string into separate words

        for (var index = 0; index < words.Length; index++)
        {
            var word = words[index].Trim();

            if (word.StartsWith("http://") || word.StartsWith("https://"))
                word = "[LINK]";

            if (index == 0)
            {
                result = words[0];
                textObject.text = result;
            }

            if (index > 0)
            {
                charCount += word.Length + 1; //+1, because we assume, that there will be a space after every word
                if (charCount <= maxLineChars)
                {
                    result += " " + word;
                }
                else
                {
                    charCount = 0;
                    result += "\n " + word;
                }
                textObject.text = result;
            }
        }
        result = result.Replace("\\n", "\n ");
        return result;
    }
}