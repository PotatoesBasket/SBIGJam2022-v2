using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float gameSpeed = 1;

    [SerializeField] Text scoreDisplay = null;
    [SerializeField] GameObject[] spawners = null;

    [SerializeField] GameObject turtPrefab = null;
    [SerializeField] GameObject tortPrefab = null;

    enum GameState
    {
        TURTORIAL,
        INTRO,
        WAIT,
        GO,
        END
    }

    GameState state = GameState.INTRO;
    int score = 0;
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
                    state = GameState.WAIT;
                break;

            case GameState.WAIT:
                if (Input.GetKeyDown(KeyCode.E) && tertIDs[tertIDs.Count - 3] ||
                    Input.GetKeyDown(KeyCode.F) && !tertIDs[tertIDs.Count - 3])
                {
                    ++score;
                    scoreDisplay.text = score.ToString();

                    state = GameState.GO;
                    firstSpawn = false;
                    gameSpeed += 0.2f;
                    currentTertAnimator.SetBool("InputHit", true);
                    currentTertAnimator.SetBool("FailedInput", false);
                }
                if (Input.GetKeyDown(KeyCode.E) && !tertIDs[tertIDs.Count - 3] ||
                    Input.GetKeyDown(KeyCode.F) && tertIDs[tertIDs.Count - 3])
                {
                    scoreDisplay.text = "boooo";
                    currentTertAnimator.SetBool("InputHit", true);
                    currentTertAnimator.SetBool("FailedInput", true);
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

        if (num < 33)
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