using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteIngrediente : MonoBehaviour
{
    SpriteRenderer sprRen;
    public Sprite sprOn, sprOff;
    bool prendido = false;
    public bool esLeche;
    AudioClip seIngred;

    // Start is called before the first frame update
    void Start()
    {
        sprRen = GetComponent<SpriteRenderer>();
        seIngred = Resources.Load<AudioClip>("Audio/se/seIngredient");
    }

    // Update is called once per frame
    void Update()
    {
        prendido = (esLeche) ? LevelManager.scr.blLeche : LevelManager.scr.blHarina;

        Prenderse(prendido);
    }

    void Prenderse(bool a)
    {
        Sprite sprAPoner = (a) ? sprOn : sprOff;
        bool mismoSpr = sprRen.sprite == sprAPoner;

        if (!mismoSpr)
        {
            sprRen.sprite = sprAPoner;
            if (a)
            {
                GameManager.scr.PlaySE(seIngred);
            }
        }
    }
}
