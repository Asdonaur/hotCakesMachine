using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuOptGroup : MonoBehaviour
{
    // Start is called before the first frame update
    MenuOption[] opciones;

    int selected = 0;
    int max = 0;
    Dictionary<KeyCode, int> dict = new Dictionary<KeyCode, int>();

    string strOriginal;
    public bool activo = true;

    void Start()
    {
        List<MenuOption> lista = new List<MenuOption>();
        foreach (var item in GetComponentsInChildren<MenuOption>())
        {
            lista.Add(item);
        }
        opciones = lista.ToArray();
        max = opciones.Length - 1;

        dict.Add(KeyCode.UpArrow, -1);
        dict.Add(KeyCode.DownArrow, 1);

        StartCoroutine(ienAnimOption(selected));
    }

    // Update is called once per frame
    void Update()
    {
        if (activo)
        {
            foreach (KeyCode keyCode in dict.Keys)
            {
                if (Input.GetKeyUp(keyCode))
                {
                    ChangeOption(dict[keyCode]);
                }
            }

            if (Input.GetKeyUp(KeyCode.Z))
            {
                opciones[selected].Pressed();
            }
        }
    }

    void ChangeOption(int factor)
    {
        if (opciones.Length > 1)
        {
            StopAllCoroutines();
            opciones[selected].GetComponent<TextMeshProUGUI>().text = strOriginal;

            selected += factor;
            if (selected < 0)
            {
                selected = max;
            }
            if (selected > max)
            {
                selected = 0;
            }
            GameManager.scr.PlaySE(MenuManager.scr.seMove);
            StartCoroutine(ienAnimOption(selected));
        }
    }

    public IEnumerator ActualizarOriginal()
    {
        StopAllCoroutines();
        strOriginal = opciones[selected].txtBeh.GetText();
        opciones[selected].GetComponent<TextMeshProUGUI>().text = strOriginal;
        StartCoroutine(ienAnimOption(selected));
        yield return null;
    }

    IEnumerator ienAnimOption(int a = 0)
    {
        TextMeshProUGUI tmp = opciones[a].GetComponent<TextMeshProUGUI>();
        strOriginal = tmp.text;
        float tiempoWait = 0.4f;

        while (true)
        {
            tmp.text = ">> " + strOriginal;
            yield return new WaitForSeconds(tiempoWait);
            tmp.text = ">>  " + strOriginal;
            yield return new WaitForSeconds(tiempoWait);
        }
    }
}
