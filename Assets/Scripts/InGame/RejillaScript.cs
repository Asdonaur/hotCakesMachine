using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RejillaScript : MonoBehaviour
{
    GameObject humo;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(ienActuar());
    }

    private void FixedUpdate()
    {
        if (LevelManager.scr.bakeDerrotada)
        {
            if (humo)
                Destroy(humo);

            StopCoroutine(ienActuar());
            StartCoroutine(ienDestroy());
        }
    }

    IEnumerator ienActuar()
    {
        yield return new WaitForSeconds(3f);
        humo = Instantiate( Resources.Load<GameObject>("Prefabs/Nivel/obRejillaHumo"), transform.position, new Quaternion() );
        yield return new WaitForSeconds(3f);
        StartCoroutine(ienDestroy());
    }

    IEnumerator ienDestroy()
    {
        animator.Play("Close");
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
