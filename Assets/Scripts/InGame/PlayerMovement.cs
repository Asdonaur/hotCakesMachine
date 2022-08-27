using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //          OBJETOS Y COMPONENTES
    public static PlayerMovement scr;
    public GameObject objPointC;
    public Animator animator;

    //          GENERALES
    public float flCaminSpeed = 1;
    public float flCaminWait = 0.25f,
        flCaminDist = 1f;
    [HideInInspector] public bool moviendose = false;

    Dictionary<KeyCode, string> dictionary = new Dictionary<KeyCode, string>();
    [HideInInspector]public string stateDir = "Dw";             // Up Dw Le Ri
    public string stateAct = "N";              // N = Normal, C = Cargando, H = Hit (Golpeado)
    bool canAct = true;
    public bool puedeMoverse = true;

    AudioClip sePick, seThrownt;

    //          VARIABLES FUNCION
    Vector3 v3Enfrente(bool sumar = true)
    {
        float vx = (stateDir == "Le") ? -2f : ((stateDir == "Ri") ? 2f : 0),
                vz = (stateDir == "Dw") ? -2f : ((stateDir == "Up") ? 2f : 0);
        return ((sumar) ? transform.position : new Vector3()) + new Vector3(vx, 0.5f, vz);
    }
    GameObject ObjEnfrente(int mult = 1)
    {
        Vector3 v3 = v3Enfrente() * mult;

        Debug.DrawLine(transform.position + new Vector3(0, 0.5f, 0), v3, Color.black, 5);
        RaycastHit hitFront;
        if (Physics.Linecast(transform.position + new Vector3(0, 0.5f, 0), v3, out hitFront))
        {
            return hitFront.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scr = this;
        animator = GetComponentInChildren<Animator>();
        sePick = Resources.Load<AudioClip>("Audio/se/sePlayerPick");
        seThrownt = Resources.Load<AudioClip>("Audio/se/sePlayerThrownt");

        dictionary.Add(KeyCode.LeftArrow, "Le");
        dictionary.Add(KeyCode.RightArrow, "Ri");
        dictionary.Add(KeyCode.UpArrow, "Up");
        dictionary.Add(KeyCode.DownArrow, "Dw");
        
    }

    // Update is called once per frame
    void Update()
    {
        string animToPlay = stateAct + stateDir;
        AnimPlay(animToPlay);
        if (puedeMoverse)
        {
            switch (stateAct)
            {
                case "N":
                case "C":
                    foreach (KeyCode keyCode in dictionary.Keys)
                    {
                        if (Input.GetKey(keyCode))
                        {
                            Moverse(dictionary[keyCode]);
                        }
                    }

                    if (!objPointC.GetComponentInChildren<Cargable>())
                    {
                        stateAct = "N";
                    }

                    animator.speed = 1f;

                    //      RAYCAST HACIA ARRIBA
                    RaycastHit hitArriba;
                    if (Physics.Linecast(transform.position, transform.position + new Vector3(0, 1.1f, 0), out hitArriba))
                    {
                        if (hitArriba.collider.gameObject.tag == "Cargable")
                        {
                            if ((hitArriba.collider.gameObject.GetComponent<Cargable>().estado == "F") || (hitArriba.collider.gameObject.GetComponent<Cargable>().estado == "G"))
                            {
                                vHurt();
                                hitArriba.collider.gameObject.GetComponent<Cargable>().Destruirse();
                            }
                        }
                    }
                    Habilidades();
                    break;

                case "H":
                default:
                    animator.speed = 1f;
                    break;
            }

            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "HotCake":
                HotCakeBehaviour scripthc = other.gameObject.GetComponent<HotCakeBehaviour>();
                if (!scripthc.devorando)
                {
                    scripthc.SerDevorado();
                    LevelManager.scr.ActualizarVidas(1);
                    if (LevelManager.scr.estado != LevelManager.Estado.Jugando)
                    {
                        LevelManager.scr.aviso = true;
                    }
                }
                
                break;

            case "Hurting":
                vHurt();
                break;

            default:
                break;
        }
    }

    void Habilidades()
    {
        if (!moviendose)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                GameObject obj = ObjEnfrente();
                switch (stateAct)
                {
                    case "N":
                        if ((obj != null) && (obj.tag == "Cargable"))
                        {
                            Cargable cargable0 = obj.GetComponent<Cargable>();
                            if (cargable0.estado == "S")
                            {
                                stateAct = "C";
                                obj.transform.parent = objPointC.transform;
                                obj.GetComponent<Cargable>().estado = "C";
                                GameManager.scr.PlaySE(sePick);
                                vDontAct();
                            }
                        }
                        
                        break;

                    case "C":
                        if (obj == null)
                        {
                            GameObject objCargado = objPointC.GetComponentInChildren<Cargable>().gameObject;
                            Cargable cargable = objCargado.GetComponent<Cargable>();

                            stateAct = "N";
                            objCargado.transform.parent = null;
                            objCargado.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
                            cargable.Lanzarse(v3Enfrente(false));
                            GameManager.scr.PlaySE(sePick);
                            vDontAct();
                        }
                        else
                        {
                            GameManager.scr.PlaySE(seThrownt);
                        }
                        break;
                }
            }
        }
        
    }

    public void Moverse(string dir)
    {
        if (!moviendose)
        {
            moviendose = true;
            stateDir = dir;
            StartCoroutine(ienMoverse(dir));
        }
    }

    void Centrar()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
    }

    void AnimPlay(string anim)
    {
        if (!animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).IsName(anim))
        {
            animator.Play(anim, 0);
        }
    }

    void vHurt()
    {
        if (stateAct != "H")
        {
            StartCoroutine(ienHurt());
        }
    }

    void vDontAct(float seg = 0.1f)
    {
        if (canAct)
        {
            canAct = false;
            StartCoroutine(ienDontAct(seg));
        }
    }

    IEnumerator ienMoverse(string direct)
    {
        Vector3 dir2 = new Vector3();
        switch (direct)
        {
            case "Dw":
                dir2 = new Vector3(0, 0, -flCaminDist);
                break;

            case "Up":
                dir2 = new Vector3(0, 0, flCaminDist);
                break;

            case "Le":
                dir2 = new Vector3(-flCaminDist, 0, 0);
                break;

            case "Ri":
                dir2 = new Vector3(flCaminDist, 0, 0);
                break;

            default:
                break;
        }
        Vector3 punto = transform.position + dir2;
        yield return new WaitForSeconds(0.05f);

        RaycastHit hit, hit2;
        Debug.DrawLine(transform.position + new Vector3(0, 1f, 0), punto + new Vector3(0, -0.5f, 0), Color.red, 5);
        if (Physics.Linecast(transform.position + new Vector3(0, 1f, 0), punto + new Vector3(0, -0.5f, 0), out hit))
        {
            if (hit.collider.gameObject.tag != "Suelo")
            {
                moviendose = false;
                StopCoroutine(ienMoverse(direct));
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
                if (!Physics.Linecast(transform.position + new Vector3(0, 0.5f, 0), punto + new Vector3(0, 0.5f, 0), out hit2))
                {
                    while ((Vector3.Distance(transform.position, punto) > 0.25f) && (moviendose))
                    {
                        transform.position = Vector3.MoveTowards(transform.position, punto, flCaminSpeed * Time.deltaTime);
                        yield return null;
                    }
                    transform.position = punto;
                    Centrar();
                    yield return new WaitForSeconds(flCaminWait);
                    animator.Play(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).fullPathHash);
                }
            }
        }
        moviendose = false;
        Habilidades();
    }

    IEnumerator ienHurt()
    {
        string estadoAntes = stateAct;
        stateAct = "H";
        LevelManager.scr.vTemblor( (8 / LevelManager.scr.inVidas) );
        LevelManager.scr.ActualizarVidas(-1);
        yield return new WaitForSeconds(0.5f);
        if (LevelManager.scr.inVidas != 0)
        {
            stateAct = estadoAntes;
        }
        
    }

    IEnumerator ienDontAct(float segundos)
    {
        canAct = false;
        yield return null;
        canAct = true;
    }
}
