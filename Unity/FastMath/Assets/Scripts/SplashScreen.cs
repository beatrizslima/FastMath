using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SplashScreen : MonoBehaviour
{
    float time, second;
    [SerializeField] private Image FillLoading;
    // Start is called before the first frame update
    void Start()
    {
        second = 5;
        Invoke("LoadHome", 5f);
    }

    public void LoadHome(){
        SceneManager.LoadScene("Home");
    }

    // Update is called once per frame
    void Update()
    {
        if (time < 5)
        {
            time += Time.deltaTime;
            FillLoading.fillAmount = time/second;
        }
    }
}
