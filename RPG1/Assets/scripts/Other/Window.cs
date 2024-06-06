
using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    private NPC npc;

    public virtual void Open(NPC npc) //sayfayı aç 
    {
        this.npc = npc;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void Close() //sayfayı kapa
    {
        npc.IsInteracting = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        npc = null;

    }
}
