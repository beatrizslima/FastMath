using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource mainMusic, fail, success, late, winner, loser, gameFound, clockTicking, click;
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this);
    }

    public void MainMusic()
    {
        mainMusic.Play();
    }

    public void Fail()
    {
        fail.Play();
    }

    public void Success()
    {
        success.Play();
    }

    public void Late()
    {
        late.Play();
    }

    public void Winner()
    {
        winner.Play();
    }
    public void Loser()
    {
        loser.Play();
    }
    public void GameFound()
    {
        gameFound.Play();
    }
    public void ClockTicking()
    {
        clockTicking.Play();
    }
    public void PauseClockTicking()
    {
        clockTicking.Pause();
    }
    public void Click()
    {
        click.Play();
    }

}
