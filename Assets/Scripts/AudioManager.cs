using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource BGMsource_default = null;
    [SerializeField] AudioSource SFXsource_default = null;

    [SerializeField] AudioClip BGM_tutorial = null;
    [SerializeField] AudioClip BGM_game = null;

    [SerializeField] AudioClip SFX_dialogboop = null;

    public AudioSource BGMSource_Default => BGMsource_default;
    public AudioSource SFXSource_Default => SFXsource_default;

    public AudioClip BGM_Tutorial => BGM_tutorial;
    public AudioClip BGM_Game => BGM_game;

    public AudioClip SFX_Dialogboop => SFX_dialogboop;
}