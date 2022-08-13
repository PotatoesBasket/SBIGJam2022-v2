using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current = null;

    [SerializeField] GameController gameController = null;
    [SerializeField] AnimationManager animationManager = null;
    [SerializeField] AudioManager audioManager = null;
    [SerializeField] DialogManager dialogManager = null;

    public GameController Ctrl => gameController;
    public AnimationManager Anim => animationManager;
    public AudioManager Audio => audioManager;
    public DialogManager Dialog => dialogManager;

    //--------------------------------------------------------

    private void Awake()
    {
        if (current == null)
        {
            current = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;
    }
}