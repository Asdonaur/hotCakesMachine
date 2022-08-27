using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparaRayos : MonoBehaviour
{
    //          OBJETOS Y COMPONENTES
    public GameObject obRayo;
    AudioClip seDisp;
    Animator animator;

    //          GENERALES
    float moveSpd = 20f;
    public bool disparando = false;
    public bool detener = false;
    bool detener2 = false;
    bool contando = false;
    Vector3 posFuera;

    private void Start()
    {
        seDisp = Resources.Load<AudioClip>("Audio/se/seRayo");
        animator = GetComponentInChildren<Animator>();
        posFuera = new Vector3(-50f, 0.83f, 5.5f);
    }

    private void Update()
    {
        if (detener)
        {
            detener = false;
            if (disparando)
            {
                detener2 = true;
                MostrarRayo(false);
            }
            else
            {
                detener2 = false;
            }
        }
    }

    public void Disparar()
    {
        if (!disparando)
        {
            disparando = true;
            StartCoroutine(ienDisparar());
        }
    }

    void MostrarRayo(bool mostrar)
    {
        obRayo.transform.localPosition = new Vector3(0, (mostrar) ? 0 : 3000, 0);
        if (mostrar)
        {
            GameManager.scr.PlaySE(seDisp);
        }
    }

    IEnumerator ienDisparar()
    {
        Vector3 posPlayer = new Vector3(PlayerMovement.scr.transform.position.x, 0.83f, 5.5f);
        while (Vector3.Distance(transform.position, posPlayer) > 2)
        {
            posPlayer = new Vector3(PlayerMovement.scr.transform.position.x, 0.83f, 5.5f);
            transform.position = Vector3.MoveTowards(transform.position, posPlayer, moveSpd * Time.deltaTime);
            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            if (!detener2)
            {
                yield return null;
                StartCoroutine(ienContar(2));
                while (contando)
                {
                    posPlayer = new Vector3(PlayerMovement.scr.transform.position.x, 0.83f, 5.5f);
                    transform.position = Vector3.Lerp(transform.position, posPlayer, 0.5f);
                    yield return null;
                }
                animator.Play("Prep");
                float posx = Mathf.Round( (transform.position.x / 2) ) * 2;
                Vector3 posEle = new Vector3(posx, 0.83f, 5.5f);
                while (Vector3.Distance(transform.position, posEle) > 0.05f)
                {
                    transform.position = Vector3.Lerp(transform.position, posEle, 0.75f);
                    yield return null;
                }
                yield return new WaitForSeconds(0.75f);
                MostrarRayo(true);
                animator.Play("Disp");
                yield return new WaitForSeconds(1f);
                MostrarRayo(false);
                animator.Play("Idle");
                yield return new WaitForSeconds(2f);
            }
        }
        disparando = false;
        while (Vector3.Distance(transform.position, posFuera) > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, posFuera, moveSpd * Time.deltaTime);
            yield return null;
        }
        detener2 = false;
    }

    IEnumerator ienContar(float tiempo)
    {
        contando = true;
        float c = 0f;
        while (c < tiempo)
        {
            c += Time.deltaTime;
            yield return null;
        }
        contando = false;
    }
}
