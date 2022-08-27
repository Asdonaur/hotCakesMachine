using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    void PlaySE(AudioClip CLIP)
    {
        GameManager.scr.PlaySE(CLIP);
    }

    void ApagarAnimator()
    {
        Animator anim = GetComponent<Animator>();
        HotCakeBehaviour hcb = GetComponent<HotCakeBehaviour>();
        anim.enabled = false;
        if (hcb)
        {
            hcb.devorando = false;
        }
    }

    void animDestruirse()
    {
        Destroy(this.gameObject);
    }

    void Particles(string objeto)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(string.Format("Prefabs/Particles/{0}", objeto)));
    }

    void CambiarMenu(string objec)
    {
        MenuManager.scr.CambiarMenu(objec);
    }
}
