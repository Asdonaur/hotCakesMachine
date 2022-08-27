using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextBehaviour))]
public class MenuOption : MonoBehaviour
{
    public string kind = "log";
    public TextBehaviour txtBeh;

    private void Start()
    {
        txtBeh = GetComponent<TextBehaviour>();
        switch (txtBeh.ID)
        {
            case 4:
                if (PlayerPrefs.GetFloat("VSND", 0) == 0)
                {
                    GameManager.scr.audSrc.volume = 0f;
                    txtBeh.ID = 5;
                }
                break;

            case 6:
                if (PlayerPrefs.GetFloat("VMSC", 0) == 0)
                {
                    GameManager.scr.audSrcBGM.volume = 0f;
                    txtBeh.ID = 7;
                }
                break;
        }
    }

    public void Pressed()
    {
        if (!MenuManager.scr.actuando)
        {
            MenuManager.scr.actuando = true;
            StartCoroutine(ienPress());
        }
    }

    IEnumerator ienPress()
    {
        MenuManager.scr.actuando = true;
        if (kind != "no")
        {
            GameManager.scr.PlaySE(MenuManager.scr.seSelect);
            yield return new WaitForSeconds(0.25f);
            switch (kind)
            {
                case "start":
                    GameManager.scr.StopBGM(false);
                    GameManager.scr.StopSE();
                    GameManager.scr.CargarEscena("SampleScene");
                    yield return new WaitForSeconds(1f);
                    break;

                case "options":
                    MenuManager.scr.CambiarMenu("menuOptions");
                    if (MenuManager.scr.animator)
                    {
                        MenuManager.scr.animator.SetInteger("est", 1);
                    }
                    break;

                case "optSnd":
                    if (GameManager.scr.audSrc.volume == GameManager.scr.volSound)
                    {
                        GameManager.scr.audSrc.volume = 0f;
                        txtBeh.ID = 5;
                    }
                    else
                    {
                        GameManager.scr.audSrc.volume = GameManager.scr.volSound;
                        txtBeh.ID = 4;
                    }
                    PlayerPrefs.SetFloat("VSND", GameManager.scr.audSrc.volume);
                    StartCoroutine(GetComponentInParent<MenuOptGroup>().ActualizarOriginal());
                    yield return null;
                    break;

                case "optMsc":
                    if (GameManager.scr.audSrcBGM.volume == GameManager.scr.volMusic)
                    {
                        GameManager.scr.audSrcBGM.volume = 0f;
                        txtBeh.ID = 7;
                    }
                    else
                    {
                        GameManager.scr.audSrcBGM.volume = GameManager.scr.volMusic;
                        txtBeh.ID = 6;
                    }
                    PlayerPrefs.SetFloat("VMSC", GameManager.scr.audSrcBGM.volume);
                    StartCoroutine(GetComponentInParent<MenuOptGroup>().ActualizarOriginal());
                    break;

                case "optBack":
                    MenuManager.scr.CambiarMenu("menuStart");
                    if (MenuManager.scr.animator)
                    {
                        MenuManager.scr.animator.SetInteger("est", 0);
                    }
                    break;

                case "optCred":
                    MenuManager.scr.CambiarMenu("menuCred");
                    if (MenuManager.scr.animator)
                    {
                        MenuManager.scr.animator.SetInteger("est", 2);
                    }
                    break;

                case "lang":
                    string idi = (PlayerPrefs.GetString("LANG", "eng") == "eng") ? "esp" : "eng";
                    StartCoroutine(MenuManager.scr.ienCambiarIdioma(idi));
                    break;

                case "credBack":
                    MenuManager.scr.CambiarMenu("menuOptions");
                    if (MenuManager.scr.animator)
                    {
                        MenuManager.scr.animator.SetInteger("est", 1);
                    }
                    break;

                case "quit":
                    Application.Quit();
                    break;

                case "menu":
                    GameManager.scr.CargarEscena("MainMenu");
                    GameManager.scr.StopSE();
                    yield return new WaitForSeconds(1f);
                    break;

                case "endAct":
                    MenuManager.scr.GetComponent<Animator>().Play("FIN");
                    MenuManager.scr.CambiarMenu("menuCred");
                    break;

                default:
                    GameObject menuNuevo = GameObject.Find(kind);
                    if (menuNuevo)
                    {
                        MenuManager.scr.CambiarMenu(kind);
                    }
                    else
                    {
                        Debug.Log("Presionaste el boton!");
                    }
                    break;
            }
        }
        yield return null;
        yield return null;
        MenuManager.scr.actuando = false;
    }
}
