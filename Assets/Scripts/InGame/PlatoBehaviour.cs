using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatoBehaviour : MonoBehaviour
{
    public float flVeloc = 1f;
    int dir = 1;
    bool act = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(ienStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (act)
        {
            transform.position = transform.position + new Vector3(dir * flVeloc * Time.deltaTime, 0, 0);
        }

        if (Mathf.Abs(transform.position.x) > 50f)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator ienStart()
    {
        yield return null;
        dir = (transform.position.x < PlayerMovement.scr.transform.position.x) ? 1 : -1;
        act = true;
    }
}
