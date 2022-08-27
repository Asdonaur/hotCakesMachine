using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ending : MonoBehaviour
{
    public TextMeshProUGUI tmpFrase,
        tmpTheEnd;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ienEnding());
    }

    IEnumerator ienEnding()
    {
        yield return null;
        yield return null;
        yield return null;
        TextAsset texto = Resources.Load(string.Format("Texts/{0}/end", GameManager.scr.lang)) as TextAsset;
        string guion = texto.text;
        yield return null;

        string[] lines = guion.Split('\n');
        tmpTheEnd.text = lines[1];
        string frase = lines[0],
            recorte = "";
        int car = 0, carTotal = frase.Length - 1;

        while (car <= carTotal)
        {
            car += 1;
            recorte = frase.Substring(0, car);
            tmpFrase.text = recorte;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
