using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject panelDialogue;
    public Text txt;

    void Start()
    {
        Debug.Log("DialogueUI стартовал!");
        panelDialogue.SetActive(false);
    }

    void Update()
    {
        if (panelDialogue != null && panelDialogue.activeSelf)
        {
            Debug.Log("Панель диалога активна, но может быть не видна из-за Canvas или Sort Order");
        }
    }

    public void StartDialogue(DialogueNPC npc)
    {
        Debug.Log("StartDialogue: panelDialogue = " + (panelDialogue != null ? panelDialogue.name : "null"));

        if (panelDialogue != null)
        {
            panelDialogue.SetActive(true);
            Debug.Log("panelDialogue.activeSelf = " + panelDialogue.activeSelf);
        }
        else
        {
            Debug.LogError("panelDialogue = null! Назначь её в инспекторе.");
        }

        if (txt != null)
            txt.text = npc.phrases[0];
    }
    
    public void ShowLine(string line)
    {
        txt.text = line;
    }

    public void CloseDialogue()
    {
        panelDialogue.SetActive(false);

    }

}
