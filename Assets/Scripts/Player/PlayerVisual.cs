using UnityEngine;
using static UnityEngine.GridBrushBase;

public class PlayerVisual : MonoBehaviour
{
    private static readonly int Running = Animator.StringToHash("IsRunning");

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    


    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(Running, Player.Instance.IsRunning());
        Rotation();
       
    }

    private void Rotation()
    {
        Vector2 moveDir = Player.Instance.LastMoveDirection;
        
        if(Mathf.Abs(moveDir.x)  > Mathf.Abs(moveDir.y))
        {
            animator.SetFloat("MoveX", moveDir.x);
            animator.SetFloat("MoveY", 0);
            spriteRenderer.flipX = moveDir.x < 0;
        }
        else
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", moveDir.y);
            spriteRenderer.flipX = false ;
        }

    }

    public void FinishAttackFromInventory()
    {
        var inventory = FindFirstObjectByType<InventoriQuickAccess>();
        inventory.animator.SetBool("UseTool", false);
           
    }


}
