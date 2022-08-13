using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public float gameSpeed = 1;
    public int score = 0;

    [SerializeField] Text scoreDisplay = null;
    [SerializeField] GameObject[] spawners = null;

    [SerializeField] GameObject turtPrefab = null;
    [SerializeField] GameObject tortPrefab = null;

    public enum GameState
    {
        TURTORIAL,
        INTRO,
        WAIT,
        GO,
        END
    }

    public GameState state = GameState.TURTORIAL;
    bool turtorialStarted = false;

    public List<GameObject> terts = new List<GameObject>();
    public List<bool> tertIDs = new List<bool>();
    bool firstSpawn = false;

    Animator currentTertAnimator = null;

    private void Start()
    {
        scoreDisplay.text = score.ToString();
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.TURTORIAL:
                if (turtorialStarted == false)
                {
                    GameManager.current.Dialog.BeginDialog();
                    turtorialStarted = true;
                }
                break;

            case GameState.INTRO:
                if (firstSpawn == false)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        terts.Add(Instantiate(RandomTert(), spawners[i].transform));
                    }

                    currentTertAnimator = terts[0].GetComponent<Animator>();
                    firstSpawn = true;
                }

                foreach (GameObject t in terts)
                {
                    t.transform.position += new Vector3(-gameSpeed * Time.deltaTime, 0, 0);
                }

                if (terts[0].transform.position.x <= 0.1f)
                {
                    state = GameState.WAIT;
                }
                break;

            case GameState.WAIT:
                if (Input.GetKeyDown(KeyCode.E) && tertIDs[tertIDs.Count - 3] ||
                    Input.GetKeyDown(KeyCode.F) && !tertIDs[tertIDs.Count - 3])
                {
                    ++score;
                    scoreDisplay.text = score.ToString();

                    state = GameState.GO;
                    firstSpawn = false;
                    gameSpeed += gameSpeed < 20.0f ? 0.2f : 0.0f;
                    currentTertAnimator.SetBool("InputHit", true);
                    currentTertAnimator.SetBool("FailedInput", false);
                }
                if (Input.GetKeyDown(KeyCode.E) && !tertIDs[tertIDs.Count - 3] ||
                    Input.GetKeyDown(KeyCode.F) && tertIDs[tertIDs.Count - 3])
                {
                    scoreDisplay.text = "boooo";
                    currentTertAnimator.SetBool("InputHit", true);
                    currentTertAnimator.SetBool("FailedInput", true);
                    state = GameState.END;
                }
                break;

            case GameState.GO:
                if (firstSpawn == false)
                {
                    terts.Add(Instantiate(RandomTert(), spawners[0].transform));
                    if (terts.Count > 7)
                    {
                        Destroy(terts[0]);
                        terts.RemoveAt(0);
                        tertIDs.RemoveAt(0);
                    }
                    currentTertAnimator = terts[tertIDs.Count - 3].GetComponent<Animator>();
                    terts[terts.Count - 1].GetComponent<Animator>().SetInteger("IdleLV", RandomIdleLV()); // i know
                    firstSpawn = true;
                }

                foreach (GameObject t in terts)
                {
                    t.transform.position += new Vector3(-gameSpeed * Time.deltaTime, 0, 0);
                }

                if (terts[terts.Count - 3].transform.position.x <= 0.1f)
                {
                    state = GameState.WAIT;
                }

                break;

            case GameState.END:
                break;

            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // explode factory (kill switch for the off chance you get softlock)
            state = GameState.END;
        }
    }

    GameObject RandomTert()
    {
        int num = Random.Range(0, 99);

        if (num < 80) // 0 to 79 (80%)
        {
            tertIDs.Add(true);
            return turtPrefab;
        }
        else // 80 to 99 (20%)
        {
            tertIDs.Add(false);
            return tortPrefab;
        }
    }

    int RandomIdleLV()
    {
        int num = Random.Range(0, 99);

        if (num < 33) // roughly 1/3 each
        {
            return 0;
        }
        else if (num < 66)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
}