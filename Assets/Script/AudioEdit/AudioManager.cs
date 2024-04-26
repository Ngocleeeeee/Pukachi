using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    //public GameObject audioObject;
    public static AudioManager instance;
    public AudioSource sound;

    public AudioClip soundClip;
    public Slider slider;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }
    private void Start()
    {
        sound.clip = soundClip;
        sound.Play();
    }
}
