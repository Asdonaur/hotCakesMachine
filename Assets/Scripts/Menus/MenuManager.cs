using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager scr;
    public MenuOptGroup grupoAct;
    public Animator animator;
    public bool actuando = false;

    [HideInInspector] public string[] palabras;

    [HideInInspector]
    public AudioClip seMove,
        seSelect;

    void Start()
    {
        scr = this;
        animator = GetComponent<Animator>();
        seMove = Resources.Load<AudioClip>("Audio/se/seMenuMove");
        seSelect = Resources.Load<AudioClip>("Audio/se/seMenuSelect");

        foreach (var item in GetComponentsInChildren<MenuOptGroup>())
        {
            if (item != grupoAct)
            {
                item.activo = false;
            }
        }
        StartCoroutine(ienCambiarIdioma());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CambiarMenu(string name)
    {
        grupoAct.activo = false;
        GameObject obj = GameObject.Find(name);
        print(obj.name);
        grupoAct = obj.GetComponent<MenuOptGroup>();
        grupoAct.activo = true;
        
    }

    public IEnumerator ienCambiarIdioma(string which = "a")
    {
        if (which != "a")
        {
            PlayerPrefs.SetString("LANG", which);
            GameManager.scr.lang = which;
        }

        string path = string.Format("Texts/{0}/general", GameManager.scr.lang);

        TextAsset texto = Resources.Load(path) as TextAsset;
        string guion = texto.text;

        palabras = guion.Split('\n');

        yield return null;
        foreach (var item in GetComponentsInChildren<MenuOptGroup>())
        {
            StartCoroutine( item.ActualizarOriginal() );
            yield return null;
        }
        
        foreach (var item in GetComponentsInChildren<TextBehaviour>())
        {
            item.ChangeText();
        }
        
    }
}
