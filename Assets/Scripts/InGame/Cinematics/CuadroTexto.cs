using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CuadroTexto : MonoBehaviour
{
    public TextMeshProUGUI tmpTexto;
    public string strTexto;
    string strCorte;

    public float flVeloc;
    AudioClip seBlip;

    bool blTerminado = false;
    
    // Start is called before the first frame update
    void Start()
    {
        seBlip = Resources.Load<AudioClip>("Audio/se/seTalk");
        StartCoroutine(ienShowText());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (blTerminado)
            {
                CineManager.scr.busy = false;
                Destroy(this.gameObject);
            }
            else
            {
                blTerminado = true;
            }
        }
    }

    IEnumerator ienShowText()
    {
        yield return new WaitForSeconds(0.1f);
        CineManager.scr.BAKETalk(true);
        for (int i = 0; i <= strTexto.Length; i++)
        {
            if (blTerminado)
            {
                i = strTexto.Length;
            }

            float sumaTiempo = 0f;
            strCorte = strTexto.Substring(0, i);
            tmpTexto.text = strCorte;
            
            if (i % 2 == 0)
            {
                GameManager.scr.PlaySE(seBlip);
            }
            
            yield return new WaitForSeconds(flVeloc + sumaTiempo);
        }
        blTerminado = true;
        CineManager.scr.BAKETalk(false);
    }
}
