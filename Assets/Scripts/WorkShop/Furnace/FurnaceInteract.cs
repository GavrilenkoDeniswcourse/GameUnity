using UnityEngine;

public class FurnaceInteract : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 2f;

    [Header("Links")]
    public Furnace furnace;     // КАКОЙ именно станок/печь
    public FurnaceUI ui;        // КАКОЙ UI открыть

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey) && IsPlayerInRange())
        {
            ui.Init(furnace);
            ui.Open();

            Debug.Log("Open UI: " + ui.name);
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= interactRange;
    }
}