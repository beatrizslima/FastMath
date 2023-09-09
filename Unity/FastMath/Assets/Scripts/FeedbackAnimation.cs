using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Button alternative;
    [SerializeField] private Button alternativeTwo;
    // Start is called before the first frame update
    void Start()
    {
       // anim = gameObject.GetComponent<Animator>();
        Animacao();
    }

    public void Animacao()
    {
        alternative.onClick.AddListener(() =>{
            anim.Play("Right");
        });

        alternativeTwo.onClick.AddListener(() =>{
            anim.Play("Left");
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
