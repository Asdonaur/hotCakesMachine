using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transicion : MonoBehaviour
{
    public string strEscena = "a";
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(ienTrnasicion());
    }

    IEnumerator ienTrnasicion()
    {
        yield return null;
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene(strEscena);
    }
}
