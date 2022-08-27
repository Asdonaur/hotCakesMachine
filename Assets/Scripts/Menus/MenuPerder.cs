using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPerder : MonoBehaviour
{
    public RectTransform rtPlayer,
        rtPointEnd;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ienAnimate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ienAnimate()
    {
        rtPlayer.position = Camera.main.WorldToScreenPoint(PlayerMovement.scr.transform.position);
        yield return new WaitForSeconds(2.5f);
        while (Vector3.Distance(rtPlayer.position, rtPointEnd.position) > 1)
        {
            rtPlayer.position = Vector3.Lerp(rtPlayer.position, rtPointEnd.position, 0.25f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
