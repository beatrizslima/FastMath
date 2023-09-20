using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Image SoundOn;
    [SerializeField] private Image SoundOff;

    private bool muted;
   
    void Start()
    {
        if (!PlayerPrefs.HasKey("muted"))
        {
            PlayerPrefs.SetInt("muted", 0);
            Load();
        }else
        {
            Load();
        }
        UpdateIcon();
        AudioListener.pause = muted;
    }

    public void OnButtonPressed(){
        if (muted == false)
        {
            muted = true;
            AudioListener.pause = true;
        }else
        {
            muted = false;
            AudioListener.pause = false;
        }
        Save();
        UpdateIcon();
    }

    void UpdateIcon(){
        if (muted == false)
        {
            SoundOn.enabled = true;
            SoundOff.enabled = false;
        }else
        {
            SoundOn.enabled = false;
            SoundOff.enabled = true;
        }
    }

    void Load(){
        muted = PlayerPrefs.GetInt("muted") == 1;
    }

    void Save(){
        PlayerPrefs.SetInt("muted", muted ? 1 : 0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
