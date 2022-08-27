using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotCakeBehaviour : MonoBehaviour
{
    Animator animator;
    public bool devorando = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(ienStart());
    }

    public void SerDevorado()
    {
        if (!devorando)
        {
            devorando = true;
            StartCoroutine(ienDevorar());
        }
    }

    IEnumerator ienStart()
    {
        while (Vector3.Distance(transform.position, new Vector3(0, 0, -4)) > 1)
        {
            transform.position += new Vector3(0, -25 * Time.deltaTime, 0);
            yield return null;
        }
        transform.position = new Vector3(0, 0, -4);
        devorando = false;
    }

    IEnumerator ienDevorar()
    {
        animator.Play("Eaten");
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }
}
