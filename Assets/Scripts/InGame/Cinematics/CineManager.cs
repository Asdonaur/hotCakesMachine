using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CineManager : MonoBehaviour
{
    public static CineManager scr;
    public bool busy = false, playing = false;

    public Animator animBCara;
    public int expresion = 0;
    string animar = "anBAKEF00_0";
    bool generando = false;
    public CinemachineVirtualCamera vcam1;

    string IntADir(int index)
    {
        string dir = "Up";
        switch (index)
        {
            case 0:
                dir = "Up";
                break;
            case 1:
                dir = "Dw";
                break;
            case 2:
                dir = "Le";
                break;
            case 3:
                dir = "Ri";
                break;
        }
        return dir;
    }

    // Start is called before the first frame update
    void Start()
    {
        scr = this;
        //StartCoroutine(ienManageCinematics());
    }

    public void BAKETalk(bool hablando = true)
    {
        if (!generando)
        {
            string numStr =
            ((expresion < 10) ? "0" : "") + expresion;
            animar = string.Format("anBAKEF{0}_{1}", numStr, (hablando) ? "1" : "0");
            animBCara.Play(animar);
        }
    }

    public void vGenerar()
    {
        if (!generando)
        {
            generando = true;
            StartCoroutine(ienBAKEGenerate());
        }
    }

    IEnumerator ienBAKEGenerate()
    {
        string anim = "";
        float espera = 3f;
        switch (LevelManager.scr.estado)
        {
            case LevelManager.Estado.Tutorial:
                anim = "anBAKEG00";
                break;

            case LevelManager.Estado.Jugando:
                anim = "anBAKEG01";
                break;

            case LevelManager.Estado.Final:
                anim = "anBAKEG02";
                espera = 6.5f;
                GameManager.scr.StopBGM(false);
                break;
            default:
                break;
        }
        animBCara.Play(anim);
        yield return new WaitForSeconds(espera);
        generando = false;
    }

    public IEnumerator ienManageCinematics(int num = 0)
    {
        PlayerMovement.scr.puedeMoverse = false;
        playing = true;
        string path = string.Format("Texts/{0}/cinem{1}", GameManager.scr.lang, num);

        TextAsset texto = Resources.Load(path) as TextAsset;
        string guion = texto.text;
        yield return null;

        string[] lines = guion.Split('\n');

        while (generando)
        {
            yield return null;
        }

        foreach (var item in lines)
        {
            busy = true;
            LevelManager.scr.estado = LevelManager.Estado.Cinematica;
            LevelManager.scr.playing = false;
            //print(item);

            switch (item.Substring(0, 3))
            {
                case "txt":
                    GameObject objCnvs = Instantiate(Resources.Load<GameObject>("Prefabs/Canvas/canvText"));
                    CuadroTexto cnvsScr = objCnvs.GetComponent<CuadroTexto>();
                    cnvsScr.strTexto = item.Substring(4, item.Length - 4);
                    break;

                case "wai":
                    float tiempo = float.Parse(item.Substring(4, item.Length - 4));
                    yield return new WaitForSeconds(tiempo);
                    busy = false;
                    break;

                case "ani":
                    int numero = int.Parse(item.Substring(4, item.Length - 4));
                    expresion = numero;
                    yield return null;
                    BAKETalk(false);
                    yield return null;
                    busy = false;
                    break;

                case "cam":
                    vcam1.Priority = (vcam1.Priority == 11) ? 9 : 11;
                    yield return new WaitForSeconds(1f);
                    busy = false;
                    break;

                case "spw":
                    StartCoroutine(LevelManager.scr.ienEspaunearObjeto());
                    break;

                case "stm":
                    GameManager.scr.StopBGM(false);
                    busy = false;
                    break;

                case "plo": // 0=Up, 1=Dw, 2=Le, 3=Ri
                    int direc = int.Parse(item.Substring(4, item.Length - 4));
                    string dire = IntADir(direc);
                    PlayerMovement.scr.stateDir = dire;
                    busy = false;
                    break;

                case "pwa":
                    int direc2 = int.Parse(item.Substring(4, item.Length - 4));
                    string dire2 = IntADir(direc2);
                    PlayerMovement.scr.Moverse(dire2);
                    while (PlayerMovement.scr.moviendose)
                    {
                        yield return null;
                    }
                    busy = false;
                    break;

                case "pta":
                    Vector3 coord = PlayerMovement.scr.transform.position;
                    GameManager.scr.PlaySE( Resources.Load<AudioClip>("Audio/se/sePlayerPick") );
                    PlayerMovement.scr.transform.position = new Vector3(coord.x, 0.2f, coord.z);
                    yield return new WaitForSeconds(0.05f);
                    PlayerMovement.scr.transform.position = coord;
                    yield return new WaitForSeconds(0.05f);
                    busy = false;
                    break;

                case "end":
                    GameManager.scr.CargarEscena("Ending");
                    yield return new WaitForSeconds(0.25f);
                    GameManager.scr.estad = LevelManager.Estado.Tutorial;
                    GameManager.scr.dif = 0;
                    break;

                default:
                    busy = false;
                    break;
            }

            while (busy)
            {
                yield return null;
            }
        }
        playing = false;
        switch(num)
        {
            case 0:
            case 1:
                LevelManager.scr.estado = LevelManager.Estado.Tutorial;
                break;

            case 2:
                LevelManager.scr.estado = LevelManager.Estado.Jugando;
                Destroy(LevelManager.scr.paredes);
                break;

            case 3:
                LevelManager.scr.estado = LevelManager.Estado.Final;
                break;

            case 6:
                LevelManager.scr.estado = LevelManager.Estado.Final;
                break;

            default:
                LevelManager.scr.estado = LevelManager.Estado.Jugando;
                break;
        }
        PlayerMovement.scr.puedeMoverse = true;
    }
}
