using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMusic : MonoBehaviour
{
    private static BgMusic bgMusic;
    void Awake() {
        if (bgMusic == null)
        {
            bgMusic = this;
            DontDestroyOnLoad(bgMusic);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
