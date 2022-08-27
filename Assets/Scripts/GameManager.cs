using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /*
     *           -------< GAME MANAGER >------
     * Aquí solamente van cosas que quiero se capaz de hacer
     * en todas las escenas.
     * 
     * PLAYER PREFS
     * FLOATS:
     * VSND = Volumen sonido
     * VMSC = Volumen musica
     * 
     * STRINGS:
     * LANG = Idioma. eng, esp
     * 
     */
    public static GameManager scr;
    public AudioSource audSrc,
        audSrcBGM;
    AudioClip bgmMenu;
    public float volSound = 0, volMusic = 0;

    public int dif;
    public LevelManager.Estado estad = LevelManager.Estado.Tutorial;
    bool repMenu = false;
    public string lang = "eng";

    void Start()
    {
        if (scr == null)
        {
            scr = this;
            DontDestroyOnLoad(this);
            Application.runInBackground = false;
            bgmMenu = Resources.Load<AudioClip>("Audio/bgm/bgmMainMenu");
            volSound = audSrc.volume;
            volMusic = audSrcBGM.volume;
            
            StartCoroutine(ienStart());
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (!audSrcBGM.isPlaying)
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "MainMenu":
                    if (!repMenu)
                    {
                        PlayBGM(bgmMenu);
                        repMenu = true;
                    }
                    break;

                default:
                    repMenu = false;
                    break;
            }
        }
    }

    IEnumerator ienStart()
    {
        yield return null;
        if (PlayerPrefs.HasKey("LANG"))
        {
            lang = PlayerPrefs.GetString("LANG", "eng");
            yield return null;
            audSrc.volume = PlayerPrefs.GetFloat("VSND", audSrc.volume);
            yield return null;
            audSrcBGM.volume = PlayerPrefs.GetFloat("VMSC", audSrcBGM.volume);
        }
        else
        {
            lang = "eng";
            PlayerPrefs.SetString("LANG", "eng");
            yield return null;
            PlayerPrefs.SetFloat("VSND", audSrc.volume);
            yield return null;
            PlayerPrefs.SetFloat("VMSC", audSrcBGM.volume);
        }
        
        yield return null;
        if (SceneManager.GetActiveScene().name == "Start")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    #region ESCENAS
    public void CargarEscena(string escena = "a")
    {
        string nombre = escena;
        GameObject transicion = Instantiate( Resources.Load<GameObject>("Prefabs/Canvas/canvTransition") );
        if (escena == "a")
        {
            nombre = SceneManager.GetActiveScene().name;
        }
        print(transicion != null);
        transicion.GetComponent<Transicion>().strEscena = nombre;
    }
    #endregion

    #region SONIDOS
    public void PlayBGM(AudioClip music)
    {
        audSrcBGM.Stop();
        audSrcBGM.clip = music;
        audSrcBGM.Play();
    }

    public void StopBGM(bool enSeco)
    {
        if (enSeco)
        {
            audSrcBGM.Stop();
        }
        else
        {
            StartCoroutine(ienStopMusic());
        }
    }

    IEnumerator ienStopMusic()
    {
        float volumenAntes = audSrcBGM.volume;
        while (audSrcBGM.volume > 0)
        {
            audSrcBGM.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        audSrcBGM.Stop();
        yield return null;
        audSrcBGM.volume = volumenAntes;
    }

    public void PlaySE(AudioClip clip)
    {
        audSrc.PlayOneShot(clip);
    }

    public void StopSE()
    {
        audSrc.Stop();
    }
    #endregion
}
