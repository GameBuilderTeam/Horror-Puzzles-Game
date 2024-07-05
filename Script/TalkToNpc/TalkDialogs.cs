using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkDialogs : MonoBehaviour
{
    [Header("Dialogs")]
    public TMP_Text dialog;
    public Image npcImage;
    public TMP_Text nameOfNpc;

    [Header("TextFile")]
    public TextAsset diglogText;
    public int index;

    [Header("Npc")]
    public Sprite[] face;
    public string[] npcName;

    private bool isTextFinished;

    List<string> list = new List<string>();
    // Start is called before the first frame update
    private void Awake()
    {
        GetTextFromFile(diglogText);
        isTextFinished = true;
    }
    void OnEnable()
    {
        index = 0;
        StartCoroutine(ShowTextUI(0.01f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isTextFinished)
        {
            if (list.Count > index)
            {
                StartCoroutine(ShowTextUI(0.01f));
            }
            else
            {
                gameObject.SetActive(false);
                index = 0;
            }
        }
    }

    private void GetTextFromFile(TextAsset textFile)
    {
        list.Clear();
        index = 0;
        var lineData = textFile.text.Split('\n');
        foreach (var line in lineData)
        {
            list.Add(line);
        }
    }

    IEnumerator ShowTextUI(float waitTime)
    {
        dialog.text = "";
        isTextFinished = false;
        switch (list[index].Trim())
        {
            case "A":
                nameOfNpc.text = npcName[0];
                npcImage.sprite = face[0];
                index += 1;
                break;
            case "B":
                nameOfNpc.text = npcName[1];
                npcImage.sprite = face[1];
                index += 1;
                break;
        }
        foreach (var text in list[index])
        {
            dialog.text += text;
            yield return new WaitForSeconds(waitTime);
        }
        index += 1;
        isTextFinished = true;
    }
}
