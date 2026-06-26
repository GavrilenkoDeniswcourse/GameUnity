using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    public List<string> phrases = new List<string>();
    private Trader trader;
    private TRadeUI tradeUI;
    private DialogueUI dialogueUI;
    public DialogueUI dialogueUIReference;
    private int indexPhrace;
    private float radius;
    private NPCMovementWaypoints npcMovement;

    void Start()
    {
        trader = GameObject.FindFirstObjectByType<Trader>();
        dialogueUIReference = FindFirstObjectByType<DialogueUI>();
        tradeUI = FindFirstObjectByType<TRadeUI>();
        npcMovement = GetComponent<NPCMovementWaypoints>();

        Debug.Log("DialogueNPC стартовал!");

        if (tradeUI == null)
        {
            Debug.LogWarning("tradeUI = null, NPC не имеет магазина");
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Update работает");
            Debug.Log("E нажата");
            if (trader != null && trader.IsPlayerInRange())
            {
                Debug.Log("Игрок в радиусе, открываем диалог");
                Interact();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (tradeUI.tradePanel.activeSelf)
            {
                return;
            }
            NextPhrace();
        }
    }

    private void Interact()
    {
        Debug.Log("Interact() вызван");

        if (npcMovement != null)
            npcMovement.SetInteracting(true);

        if (dialogueUIReference == null)
        {
            Debug.Log("dialogueUIReference = null");
            return;
        }

        Debug.Log($"dialogueUIReference.gameObject.activeSelf = {dialogueUIReference.gameObject.activeSelf}");

        if (!dialogueUIReference.panelDialogue.activeSelf)  // проверяем панель, а не весь объект
        {
            indexPhrace = 0;
            dialogueUIReference.StartDialogue(this);
        }
        else
        {
            Debug.Log("Диалог уже активен");
        }
    }

    private void NextPhrace()
    {
        if (dialogueUIReference.gameObject.activeSelf != true)
            return;

        indexPhrace++;

        if (indexPhrace < phrases.Count)
        {
            // Показываем следующую фразу (NPC продолжает стоять)
            dialogueUIReference.ShowLine(phrases[indexPhrace]);
            // НЕ вызываем SetInteracting(false) здесь!
        }
        else
        {
            // Диалог закончен
            dialogueUIReference.CloseDialogue();

            // Возобновляем движение NPC
            if (npcMovement != null)
                npcMovement.SetInteracting(false);

            // Открываем магазин, если есть
            if (trader != null)
                trader.Interact();
        }
    }
}
