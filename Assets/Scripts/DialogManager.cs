using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox = null;
    [SerializeField] Text dialogText = null;

    [SerializeField] GameObject dummyTurt = null;
    [SerializeField] GameObject dummyTort = null;

    [SerializeField] float printTime = 0.03f;
    [SerializeField] float pauseTime = 0.5f;
    float timer = 0;
    bool waitForEvent = false;

    List<string> dialogLines = new List<string>();
    string currentLine = null;
    int currentLineID = 0;
    int currentCharID = 0;
    int currentEventID = 1;

    enum DialogState
    {
        StartLine,
        Printing,
        DecidingNextState,
        WaitForNext,
        WaitForClose,
        WaitForContinue,
        WaitForEvent,
        Pause,
        Event01,
        Event02,
        Event03,
        Event04,
        Event05,
        Event06,
        Event07,
        Event08,
        Closed
    }

    DialogState currentDialogState = DialogState.Closed;

    private void Start()
    {
        LoadDialog();
    }

    private void Update()
    {
        switch (currentDialogState)
        {
            case DialogState.StartLine:
                {
                    ProcessNextDialogLine(true);
                    currentDialogState = DialogState.Printing;
                }
                break;

            case DialogState.Printing:
                {
                    timer += Time.deltaTime;

                    if (timer >= printTime)
                    {
                        ProcessNextDialogLine();
                        timer = 0;
                    }
                }
                break;

            case DialogState.DecidingNextState:
                {
                    if (currentLineID <= dialogLines.Count - 2)
                        currentDialogState = DialogState.WaitForNext;
                    else
                        currentDialogState = DialogState.WaitForClose;
                }
                break;

            case DialogState.WaitForNext:
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    ++currentLineID;
                    currentDialogState = DialogState.StartLine;
                }
                break;

            case DialogState.WaitForClose:
                timer += Time.deltaTime;

                if (timer >= 1.0f)
                {
                    EndDialog();
                    timer = 0;
                }
                break;

            case DialogState.WaitForContinue:
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
                {
                    currentDialogState = DialogState.Printing;
                }
                break;

            case DialogState.Pause:
                if (timer >= pauseTime)
                {
                    currentDialogState = DialogState.Printing;
                    timer = 0;
                    break;
                }

                timer += Time.deltaTime;
                break;

            case DialogState.Event01:
                if (dummyTurt.transform.position.x <= 0)
                {
                    currentCharID = 0;
                    ++currentLineID;
                    currentDialogState = DialogState.StartLine;
                }

                dummyTurt.transform.position -= new Vector3(Time.deltaTime * GameManager.current.Ctrl.gameSpeed, 0, 0);
                break;

            case DialogState.Event02:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    dummyTurt.GetComponent<Animator>().SetBool("InputHit", true);
                    dummyTurt.GetComponent<Animator>().SetBool("FailedInput", false);
                    currentCharID = 0;
                    ++currentLineID;
                    currentDialogState = DialogState.StartLine;
                }
                break;

            case DialogState.Event03:
                dummyTurt.transform.position -= new Vector3(Time.deltaTime * GameManager.current.Ctrl.gameSpeed, 0, 0);

                timer += Time.deltaTime;

                if (timer >= printTime)
                {
                    ProcessNextDialogLine();
                    timer = 0;
                }

                if (dummyTurt.transform.position.x <= -10.0f)
                    waitForEvent = false;
                break;

            case DialogState.Event04:
                dummyTort.GetComponent<Animator>().SetInteger("IdleLV", 2);

                if (dummyTort.transform.position.x <= 0)
                {
                    currentCharID = 0;
                    ++currentLineID;
                    currentDialogState = DialogState.StartLine;
                }

                dummyTort.transform.position -= new Vector3(Time.deltaTime * GameManager.current.Ctrl.gameSpeed, 0, 0);
                break;

            case DialogState.Event05:
                timer += Time.deltaTime;

                if (timer >= 5.0f)
                {
                    timer = 0;
                    currentCharID = 0;
                    ++currentLineID;
                    currentDialogState = DialogState.StartLine;
                }
                break;

            case DialogState.Event06:
                timer += Time.deltaTime;

                if (timer >= 2.0f)
                {
                    timer = 0;
                    currentCharID = 0;
                    ++currentLineID;
                    currentDialogState = DialogState.StartLine;
                }
                break;

            case DialogState.Event07:
                break;

            case DialogState.Event08:
                break;

            case DialogState.Closed:
                break;
        }
    }

    void ProcessNextDialogLine(bool begin = false)
    {
        if (begin) // prepare line
        {
            dialogText.text = "";
            currentLine = dialogLines[currentLineID];
            return;
        }

        if (currentCharID >= currentLine.Length) // check for if end of line printed, probably should actually hit because of n control code
        {
            if (waitForEvent == false)
            {
                // exit print state, reset ready for next line
                currentDialogState = DialogState.DecidingNextState;
                currentCharID = 0;
                return;
            }
            else
            {
                return; // wait for event to switch bool before doing more dialog
            }    
        }

        if (currentLine[currentCharID] != '[') // print dialog char to screen
        {
            dialogText.text += currentLine[currentCharID];
            ++currentCharID;
        }
        else // not dialog char, do control code functions
        {
            char attributeCode = currentLine[currentCharID + 1];

            switch (attributeCode)
            {
                case 'p': // pause printing briefly for emphasis
                    currentDialogState = DialogState.Pause;
                    currentCharID += 3;
                    break;

                case 'e':                    
                    switch (currentEventID) // init event data and switch update state
                    {
                        case 1:
                            // scroll single turtle until reaches player
                            currentDialogState = DialogState.Event01;
                            break;

                        case 2:
                            // await E input
                            currentDialogState = DialogState.Event02;
                            break;

                        case 3:
                            // scroll single turtle until offscreen
                            waitForEvent = true;
                            currentDialogState = DialogState.Event03;
                            break;

                        case 4:
                            // scroll tortoise until reaches player
                            currentDialogState = DialogState.Event04;
                            break;

                        case 5:
                            // tortoise reveal
                            dummyTort.GetComponent<Animator>().SetBool("Event", true);
                            dummyTort.GetComponent<Animator>().SetBool("InputHit", true);
                            dummyTort.GetComponent<Animator>().SetBool("FailedInput", true);
                            currentDialogState = DialogState.Event05;
                            break;

                        case 6:
                            // play tortoise rejection animation
                            dummyTort.GetComponent<Animator>().SetBool("Event", false);
                            currentDialogState = DialogState.Event06;
                            break;

                        case 7:
                            // initiate fresh terts ready for game start
                            GameManager.current.Ctrl.state = GameController.GameState.INTRO;
                            currentDialogState = DialogState.Printing;
                            break;

                        case 8:
                            // start game
                            currentDialogState = DialogState.Printing;
                            break;
                    }

                    currentCharID += 4;
                    ++currentEventID;
                    break;

                default: // not a valid control code, just print i guess
                    ++currentCharID;
                    break;
            }
        }
    }

    public void BeginDialog()
    {
        dialogBox.SetActive(true);
        currentDialogState = DialogState.StartLine;
    }

    public void EndDialog()
    {
        dialogBox.SetActive(false);
        currentDialogState = DialogState.Closed;
    }

    void LoadDialog()
    {
        dialogLines.Add("* Welcome to the factory!");
        dialogLines.Add("* Your job here will be to get these turtles back in working order.");
        dialogLines.Add("* How?");
        dialogLines.Add("* Why,[p] with heartfelt words of encouragement of course!");
        dialogLines.Add("* Didn’t you...[p] know what you signed up for?");
        dialogLines.Add("* ...");
        dialogLines.Add("* Anyway,[p] this is your job now.[p] It’s not so bad...");
        dialogLines.Add("* Especially when you get to see so many precious happy turtle faces the moment their confidence has been restored anew.");
        dialogLines.Add("* OK,[p] let’s give it a try.[p] Here comes one now![e1]");
        dialogLines.Add("* Just do your best[p] (press E to do your best)[e2]");
        dialogLines.Add("* Alright![p] Great job![p] Amazing!");
        dialogLines.Add("* [e3]Just look at the little guy go.");
        dialogLines.Add("* Since it’s your first day,[p] I’ll supervise for now,[p] just to make sure you get the hang of things.");
        dialogLines.Add("* Let’s get started![e4]");
        dialogLines.Add("* Oh no![p] Look at this poor sweet baby.[p] He can’t even bear to show his face!");
        dialogLines.Add("* Let’s give him our best--[p] WAIT A SECOND[e5]");
        dialogLines.Add("* Oh no you don’t!![e6]");
        dialogLines.Add("* /SIGH/...");
        dialogLines.Add("* Watch out for tortoises...[p] (press F to dispose of tortoises)");
        dialogLines.Add("* You do[p] /NOT/[p] want to accidentally give[p] /THEM/[p] any kind of encouragement.");
        dialogLines.Add("* One kind word and...[p] we’ll never have peace again.");
        dialogLines.Add("* ...");
        dialogLines.Add("* ...At least they’re easy enough to tell apart.");
        dialogLines.Add("* [e7]Go on, now.[p] You’ve got this![e8]");
    }
}