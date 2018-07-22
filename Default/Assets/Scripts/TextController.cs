using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour {

    public GameObject dialoguePanel;
    public Text dialoguePanelText;

    TextAsset textFile;
    string[] textList;

    string textLine;

    public int currentLine = 0;

    public bool isInDialogueProximity;

    bool isTyping;

    public int currentLetter;

    void Start() {
        //textList = textFile.text.Split('\n');
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (textList != null)
        {
            dialoguePanelText.text = textLine;
            if (isInDialogueProximity)
            {

                if (isTyping) {
                    textLine += textList[currentLine][currentLetter];
                    currentLetter++;
                    if (currentLetter >= textList[currentLine].Length) { isTyping = false; currentLetter = 0; }
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    if (currentLine == textList.Length - 1)
                    {
                        currentLine = 0;
                        isInDialogueProximity = false;
                        dialoguePanel.SetActive(false);
                    }
                    else
                    {
                        textLine = "";
                        currentLine++;
                        currentLetter = 0;
                        isTyping = true;
                    }
                }
            }
            else if (!isInDialogueProximity)
            {
                currentLine = 0;
                currentLetter = 0;
                dialoguePanel.SetActive(false);
            }
        }
    }

    public void startDialogue(string[] textlist) {
        textLine = "";
        currentLetter = 0;
        textList = textlist;
        dialoguePanel.SetActive(true);
        isTyping = true;
    }
}
