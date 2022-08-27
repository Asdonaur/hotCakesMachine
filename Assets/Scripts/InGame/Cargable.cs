using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargable : MonoBehaviour
{
    GameObject sombra;
    [HideInInspector]public Rigidbody rb;

    AudioClip seFall,
        seGround;

    public enum Tipo
    {
        Caja, Leche, Harina
    }
    public Tipo tipo = Tipo.Caja;
    public float flVelocLanz = 2f;
    public string estado = "S";

    Vector3 lugar;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sombra = Instantiate( Resources.Load<GameObject>("Prefabs/Nivel/sombra") );
        sombra.transform.parent = this.transform;

        seFall = Resources.Load<AudioClip>("Audio/se/seBoxFall");
        seGround = Resources.Load<AudioClip>("Audio/se/seBoxGround");

        estado = "S";
    }

    // Update is called once per frame
    void Update()
    {
        switch (estado)
        {
            case "C":
                rb.useGravity = false;
                transform.localPosition = new Vector3();
                gameObject.layer = 2;
                break;

            case "S":
                rb.useGravity = true;
                gameObject.layer = 0;
                Centrar();
                break;

            case "L":
                rb.useGravity = true;
                gameObject.layer = 0;
                break;

            case "F":
                Centrar();
                rb.useGravity = true;
                RaycastHit hitDown;
                if (Physics.Linecast(transform.position + new Vector3(0, -0.6f, 0), transform.position + new Vector3(0, -1f, 0), out hitDown))
                {
                    if (hitDown.collider.tag == "Suelo")
                    {
                        estado = "S";
                        LevelManager.scr.vTemblor(4);
                        GameManager.scr.PlaySE(seGround);
                    }
                }
                gameObject.layer = 0;
                break;

            default: // "G" = Generada,
                rb.useGravity = false;
                gameObject.layer = 0;
                break;
        }

        if (transform.position.y < -50)
            Destroy(this.gameObject);

        if (Mathf.Round(transform.position.y) == 15)
        {
            GameManager.scr.PlaySE(seFall);
        }

        if (rb.useGravity)
        {
            RaycastHit hitDown;

            if (Physics.Linecast(transform.position, transform.position + (new Vector3(0, -4, 0) * 20), out hitDown))
            {
                float dist = Vector3.Distance(transform.position, hitDown.point);
                if ((dist > 1.75f) && (hitDown.point.y > -1))
                {
                    sombra.transform.position = new Vector3(hitDown.point.x, -0.05f, hitDown.point.z);
                    Debug.DrawLine(transform.position, hitDown.point, Color.green, 5);
                    print("DISTANCIA: " + dist + "htswpy: " + hitDown.point.y);
                }
                else
                {
                    sombra.transform.position = new Vector3(0, 2000, 0);
                }
            }
            else
            {
                sombra.transform.position = new Vector3(0, 2000, 0);
            }
        }
    }

    private void FixedUpdate()
    {
        if (LevelManager.scr.bakeDerrotada)
        {
            Destruirse();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.collider.tag)
        {
            case "Cargable":
                collision.collider.GetComponent<Cargable>().Destruirse();
                Destruirse();
                break;

            case "HotCake":
                Destruirse();
                break;

            case "Receptor":
                switch (tipo)
                {
                    case Tipo.Caja:
                        break;
                    case Tipo.Leche:
                        LevelManager.scr.blLeche = true;
                        break;
                    case Tipo.Harina:
                        LevelManager.scr.blHarina = true;
                        break;
                    default:
                        break;
                }
                Destroy(this.gameObject);
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlatoBehaviour>())
        {
            LevelManager.scr.vTemblor(2f);
            GameManager.scr.PlaySE(Resources.Load<AudioClip>("Audio/se/seBoxBreak"));
            Instantiate(Resources.Load<GameObject>("Prefabs/Particles/prtExplosion"), other.transform.position, new Quaternion());
            Destroy(other.gameObject);
        }
        else
        {
            Destruirse();
        }
    }

    public void Centrar()
    {
        float posx = Mathf.Round( transform.position.x / 2) * 2,
            posz = Mathf.Round(transform.position.z / 2) * 2;
        transform.position = new Vector3(posx, transform.position.y, posz);
    }

    public void Lanzarse(Vector3 sit)
    {
        if (estado != "L")
        {
            estado = "L";
            StartCoroutine(ienLanzarse(sit));
        }
    }

    public void Destruirse()
    {
        LevelManager.scr.vTemblor(4.5f);
        GameManager.scr.PlaySE( Resources.Load<AudioClip>("Audio/se/seBoxBreak") );
        Instantiate(Resources.Load<GameObject>("Prefabs/Particles/prtExplosion"), transform.position, new Quaternion());
        Destroy(this.gameObject);
    }

    IEnumerator ienLanzarse(Vector3 sitio)
    {
        lugar = transform.position + sitio + new Vector3(0, -1.5f, 0);
        yield return null;
        while (Vector3.Distance(transform.position, lugar) > 0.2f)
        {
            Debug.DrawLine(transform.position, lugar, Color.blue, 5);
            transform.position = Vector3.MoveTowards(transform.position, lugar, flVelocLanz * Time.deltaTime);
            yield return null;
        }
        yield return null;
        estado = "S";
    }
}
