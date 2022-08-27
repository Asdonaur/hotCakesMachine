using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBehaviour : MonoBehaviour
{
    public int ID = 0; 

    public string GetText()
    {
        return MenuManager.scr.palabras[ID];
    }

    public void ChangeText()
    {
        GetComponent<TextMeshProUGUI>().text = GetText();
    }
}
