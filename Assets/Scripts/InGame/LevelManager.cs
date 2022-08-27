using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    //          OBJETOS Y COMPONENTES
    public static LevelManager scr;
    public GameObject suelo, paredes;
    public Image[] corazones;
    public Sprite[] sprsCorazones;
    public DisparaRayos dispRayos;
    public CinemachineVirtualCamera virtualCamera;
    public Animator animCora;
    CinemachineBasicMultiChannelPerlin cbmcp;

    GameObject prefCaja, prefLeche, prefHarina;

    //          SONIDOS
    AudioClip seHumo;
    AudioClip bgm1, bgm2, bgm3;

    //          GENERALES
    public int inVidas = 4;
    int inVidasMax;

    int inDificultad = 0;
    int inHistoria = 0;
    public bool blLeche = false,
        blHarina = false,
        blGenerando = false;
    public bool busy = false;
    public bool playing = false;
    public bool aviso = false;
    bool temblando = false;
    public bool bakeDerrotada = false;

    bool musicafinal = false;

    public enum Estado
    {
        Tutorial, Jugando, Cinematica, Final
    }
    public Estado estado = Estado.Jugando;

    //          VARIABLES FUNCION
    Vector3 v3PosCaja(Vector3 inicio)
    {
        Vector3 resultado = new Vector3();
        resultado.x = (Mathf.Round(inicio.x) % 2 == 0) ? (Mathf.Round(inicio.x)) : (Mathf.Round(inicio.x) - 1);
        resultado.z = (Mathf.Round(inicio.z) % 2 == 0) ? (Mathf.Round(inicio.z)) : (Mathf.Round(inicio.z) - 1);
        return resultado;
    }

    bool EstaJugando()
    {
        return (estado == Estado.Jugando) || (estado == Estado.Final);
    }

    // Start is called before the first frame update
    void Start()
    {
        scr = this;
        inVidasMax = inVidas;
        seHumo = Resources.Load<AudioClip>("Audio/se/seHumo");

        StartCoroutine(ienStart());
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (estado)
        {
            case Estado.Tutorial:
            case Estado.Jugando:
            case Estado.Final:
                if (aviso)
                {
                    switch (estado)
                    {
                        case Estado.Tutorial:
                            inHistoria += 1;
                            int cinToShow = (estado == Estado.Final) ? 4 : inHistoria;
                            StartCoroutine(CineManager.scr.ienManageCinematics(cinToShow));
                            break;

                        case Estado.Final:
                            if (bakeDerrotada)
                            {
                                inHistoria += 1;
                                int cinToShow2 = (estado == Estado.Final) ? 4 : inHistoria;
                                StartCoroutine(CineManager.scr.ienManageCinematics(cinToShow2));
                                Invoke("EsconderVidas", 1f);
                            }
                            break;
                    }
                    aviso = false;
                }

                if (EstaJugando())
                {
                    if (!playing)
                    {
                        playing = true;
                        StartCoroutine(ienControlarAcciones());
                    }
                }

                if ((blLeche) && (blHarina))
                {
                    if (!blGenerando)
                    {
                        StopAllCoroutines();
                        blGenerando = true;
                        StartCoroutine(ienHotCakes());
                        if (EstaJugando())
                        {
                            StartCoroutine(ienDetenerAcciones());
                        }
                    }
                    
                }
                break;

            case Estado.Cinematica:
                break;

            default:
                break;
        }

        if (!GameManager.scr.audSrcBGM.isPlaying)
        {
            switch (estado)
            {
                case Estado.Tutorial:
                    GameManager.scr.PlayBGM(bgm1);
                    break;

                case Estado.Jugando:
                    GameManager.scr.PlayBGM(bgm2);
                    break;

                case Estado.Final:
                    if (!musicafinal)
                    {
                        musicafinal = true;
                        GameManager.scr.PlayBGM(bgm3);
                        
                    }

                    break;

            }
        }

    }

    IEnumerator ienStart()
    {
        yield return null;

        cbmcp =
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        bgm1 = Resources.Load<AudioClip>("Audio/bgm/bgmPreparingIngredients");
        bgm2 = Resources.Load<AudioClip>("Audio/bgm/bgmMixingUp");
        bgm3 = Resources.Load<AudioClip>("Audio/bgm/bgmBurningPan");
        prefCaja = Resources.Load<GameObject>("Prefabs/Nivel/obC_Caja");
        prefLeche = Resources.Load<GameObject>("Prefabs/Nivel/obC_Leche");
        prefHarina = Resources.Load<GameObject>("Prefabs/Nivel/obC_Harina");

        switch (GameManager.scr.estad)
        {
            case Estado.Jugando:
                estado = Estado.Jugando;
                inDificultad = GameManager.scr.dif;
                GameManager.scr.PlayBGM(bgm2);
                CineManager.scr.expresion = 4;
                CineManager.scr.BAKETalk(false);
                Destroy(paredes);
                break;

            case Estado.Final:
                estado = Estado.Final;
                inDificultad = 4;
                GameManager.scr.PlayBGM(bgm3);
                CineManager.scr.expresion = 5;
                CineManager.scr.BAKETalk(false);
                musicafinal = true;
                Destroy(paredes);
                break;

            default:
                yield return null;
                StartCoroutine(CineManager.scr.ienManageCinematics(0));
                break;
        }
    }

    public void ActualizarVidas(int valor)
    {
        if ((bakeDerrotada) && (valor < 0))
        {
            return;
        }

        inVidas = inVidas + valor;
        if (inVidas > inVidasMax)
        {
            inVidas = inVidasMax;
        }
        if (inVidas <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(ienPerder());
        }
        ActualizarVidasVisual(inVidas);
    }

    void ActualizarVidasVisual(int index)
    {
        if (estado != Estado.Tutorial)
        {
            if (animCora.GetBool("show"))
            {
                animCora.Play("Act");
            }
            else
            {
                animCora.SetBool("show", true);
            }
        }
        
        // Cada 2 es un corazon completo
        int num = 0; // Corazon actual
        foreach (var item in corazones)
        {
            num += 1;

            if (index >= num) // si la vida es mayor o igual al corazon actual
            {
                item.sprite = sprsCorazones[1];
            }
            else
            {
                item.sprite = sprsCorazones[0];
            }

        }
    }

    void EsconderVidas()
    {
        animCora.SetBool("show", false);
    }

    #region Generales
    public IEnumerator ienPerder()
    {
        GameManager.scr.estad = estado;
        GameManager.scr.dif = inDificultad;
        yield return null;
        estado = Estado.Cinematica;
        GameManager.scr.StopBGM(true);
        yield return null;
        PlayerMovement.scr.puedeMoverse = false;
        PlayerMovement.scr.animator.speed = 0f;
        Destroy(dispRayos);
        yield return null;
        GameObject menuPerder = Instantiate( Resources.Load<GameObject>("Prefabs/Canvas/canvLose") );
        foreach (var item in GameObject.FindGameObjectsWithTag("Hurting"))
        {
            if (item.GetComponent<PlatoBehaviour>())
            {
                Destroy(item);
            }
        }
        yield return null;
    }

    public void vTemblor(float i = 5f, float t = 0.25f)
    {
        if (!temblando)
        {
            temblando = true;
            StartCoroutine(ienTemblor(i, t));
        }
    }

    IEnumerator ienTemblor(float intensity = 5f, float time = 0.25f)
    {
        cbmcp.m_AmplitudeGain = intensity;
        float numActual = intensity;

        yield return new WaitForSeconds(time);
        cbmcp.m_AmplitudeGain = 0f;
        temblando = false;
    }
    #endregion

    #region Acciones
    IEnumerator ienControlarAcciones()
    {
        yield return new WaitForSeconds(1.5f);
        if (inDificultad >= 4)
        {
            float resta = 0f;
            float restaMax = 1f;
            while (true)
            {
                if (resta < restaMax)
                {
                    resta += 0.2f;
                }
                //                  --    RAYOS   --
                StartCoroutine(ienDispararRayos());
                yield return new WaitForSeconds(5f);

                //                  --    REJILLAS    --
                StartCoroutine(ienAparecerRejillas(10));
                yield return new WaitForSeconds(6f);

                //                  --    PLATOS    --
                StartCoroutine(ienLanzarPlatos());
                yield return new WaitForSeconds(4f);
                StartCoroutine(ienLanzarPlatos());
                yield return new WaitForSeconds(4f - (resta * 2));

                //                  --    CAJAS    --
                StartCoroutine(ienTirarCajas(3 + inDificultad));
                yield return new WaitForSeconds(10f - resta);
                if (resta == 0)
                {
                    StartCoroutine(ienLanzarPlatos());
                    yield return new WaitForSeconds(8f);
                }
                else {
                    StartCoroutine(ienLanzarPlatos());
                    yield return new WaitForSeconds(4f);
                    StartCoroutine(ienLanzarPlatos());
                    yield return new WaitForSeconds(4f);
                }

            }
        }
        else
        {
            while (true)
            {
                //                  --    RAYOS   --
                if (inDificultad >= 3)
                {
                    StartCoroutine(ienDispararRayos());
                    while (busy)
                    {
                        yield return null;
                    }
                }

                //                  --    REJILLAS    --
                if (inDificultad >= 2)
                {
                    StartCoroutine(ienAparecerRejillas( 30 - (inDificultad * 4) ));
                    while (busy)
                    {
                        yield return null;
                    }
                }
                
                yield return null;
                //                  --    PLATOS    --
                if (inDificultad >= 1)
                {
                    StartCoroutine(ienLanzarPlatos());
                    while (busy)
                    {
                        yield return null;
                    }
                }
                yield return null;

                //                  --    CAJAS    --
                StartCoroutine(ienTirarCajas(3 + inDificultad));
                while (busy)
                {
                    yield return null;
                }
                yield return null;
            }
        }
    }
    IEnumerator ienDetenerAcciones()
    {
        if (temblando)
        {
            cbmcp.m_AmplitudeGain = 0f;
            temblando = false;
        }

        foreach (var item in GameObject.FindGameObjectsWithTag("Cargable"))
        {
            if (transform.position.y >= 2)
            {
                item.GetComponent<Cargable>().estado = "F";
            }
        }

        yield return null;
        dispRayos.detener = true;
        switch (estado)
        {
            case Estado.Tutorial:
                break;
            case Estado.Jugando:
                if (inDificultad >= 3)
                {
                    inDificultad += 1;
                    print("La dificultad ahora es: " + inDificultad);
                    yield return new WaitForSeconds(7f);
                    StartCoroutine(CineManager.scr.ienManageCinematics(3));
                }
                else
                {
                    inDificultad += 1;
                    print("La dificultad ahora es: " + inDificultad);
                    yield return new WaitForSeconds(6f);
                    StartCoroutine(ienControlarAcciones());
                }
                break;

            case Estado.Final:
                bakeDerrotada = true;
                yield return new WaitForSeconds(4f);
                //StartCoroutine(ienControlarAcciones());
                break;

            default:
                break;
        }

        
    }
    IEnumerator ienHotCakes()
    {
        CineManager.scr.vGenerar();
        yield return new WaitForSeconds(1.8f);
        Instantiate(Resources.Load<GameObject>("Prefabs/Nivel/obHotCake"));
        blLeche = blHarina = false;
        yield return new WaitForSeconds(3f);
        blGenerando = false;
    }

    IEnumerator ienTirarCajas(int cajas = 3)
    {
        busy = true;
        float h = 28;
        int multPos = -1;

        for (int i = 0; i < cajas; i++)
        {
            multPos += 1;
            if (multPos > 1)
            {
                multPos = -1;
            }
            yield return new WaitForSeconds(2f + Random.Range(0, 1.5f) - (0.5f * inDificultad));

            Vector3 posJugador = PlayerMovement.scr.transform.position,
            posObjetivo = new Vector3(Mathf.Round(posJugador.x), Mathf.Round(posJugador.y) + h, Mathf.Round(posJugador.z));

            string pathObj = "Prefabs/Nivel/obC_Caja";
            if (i == cajas - 1)
            {
                if (!blLeche)
                {
                    pathObj = "Prefabs/Nivel/obC_Leche";
                }
                else
                {
                    pathObj = "Prefabs/Nivel/obC_Harina";
                }
            }

            GameObject caja = Instantiate(Resources.Load<GameObject>(pathObj), posObjetivo, new Quaternion());
            caja.GetComponent<Cargable>().estado = "G";

            float tim = 0f, timm = 2;
            while (tim < timm)
            {
                float flRange = 0.1f;
                posJugador = PlayerMovement.scr.transform.position + new Vector3(0, h + (4 * multPos), 0);
                posObjetivo = posJugador + new Vector3(Random.Range(-flRange, flRange), 0, Random.Range(-flRange, flRange));

                if (caja)
                {
                    caja.transform.position = posObjetivo;
                    tim += Time.deltaTime;
                    yield return null;
                }
                else
                {
                    tim += timm;
                    yield return null;
                }
                
            }
            if (caja)
            {
                posObjetivo = v3PosCaja(posJugador) + new Vector3(0, h - 8, 0);
                caja.transform.position = posObjetivo;
                caja.GetComponent<Cargable>().estado = "F";
            }
            
        }
        busy = false;
    }

    IEnumerator ienLanzarPlatos()
    {
        busy = true;
        int columns = 6;
        int xmult = Random.Range(-1, 1);
        xmult = (xmult == 0) ? 1 : -1;
        yield return null;

        for (int i = 0; i < columns; i++)
        {
            float changeVel = (inDificultad - 1) * Random.Range(0, 0.1f);
            xmult = -xmult;
            Vector3 posToSpawn = new Vector3(25 * xmult, 0.75f, 4 - (2 * i));
            GameObject plato = Instantiate(Resources.Load<GameObject>("Prefabs/Nivel/obPlato"), posToSpawn, new Quaternion());
            plato.GetComponent<PlatoBehaviour>().flVeloc += changeVel;
            yield return new WaitForSeconds((1f / (inDificultad + 1)) + Random.Range(0, 0.5f));
        }
        busy = false;
    }

    IEnumerator ienAparecerRejillas(int espaciosLibres = 12)
    {
        busy = true;
        Transform[] casillas = suelo.GetComponentsInChildren<Transform>();
        string pathRejilla = "Prefabs/Nivel/obRejilla";
        yield return null;

        int liberados = 0;
        int cuenta = 0;
        int espaciosOcupados = casillas.Length - espaciosLibres;
        foreach (var item in casillas)
        {
            cuenta += 1;
            int dado = Random.Range(0, 3);
            Vector3 coordenadas = item.position + new Vector3(0, 0.61f, 0);

            switch (dado)
            {
                case 0:
                    if (liberados > espaciosLibres)
                    {
                        Instantiate(Resources.Load<GameObject>(pathRejilla), coordenadas, new Quaternion());
                    }
                    else
                    {
                        liberados += 1;
                    }
                    break;

                case 1:
                default:
                    Instantiate(Resources.Load<GameObject>(pathRejilla), coordenadas, new Quaternion());
                    break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);
        GameManager.scr.PlaySE(seHumo);
        yield return new WaitForSeconds(3.5f);
        busy = false;
    }

    IEnumerator ienDispararRayos()
    {
        busy = true;
        dispRayos.Disparar();
        while (dispRayos.disparando)
        {
            yield return null;
        }
        busy = false;
    }
    #endregion

    #region CINEMATICAS
    public IEnumerator ienEspaunearObjeto()
    {
        GameObject obj = Instantiate(prefHarina);
        obj.GetComponent<Cargable>().estado = "F";
        obj.transform.position = new Vector3(4, 20, 0);
        yield return null;

        while (obj.GetComponent<Cargable>().estado == "F")
        {
            print(obj.name + " --- " + obj.transform.position);
            yield return null;
        }

        GameObject obj2 = Instantiate(prefLeche);
        obj2.GetComponent<Cargable>().estado = "F";
        obj2.transform.position = new Vector3(-4, 20, 0);
        yield return null;

        while (obj2.GetComponent<Cargable>().estado == "F")
        {
            print(obj2.name + " --- " + obj.transform.position);
            yield return null;
        }
        CineManager.scr.busy = false;
    }
    #endregion
}
