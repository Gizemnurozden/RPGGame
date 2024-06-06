
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;



public delegate void KillConfirmed(Character character);

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public HashSet<Vector3Int> Blocked { get => blocked; set => blocked = value; }

    public event KillConfirmed killConfirmedEvent;

    private Camera mainCamera;

    [SerializeField]
    private Player player;

    private Enemy currentTarget;

 
    private int targetIndex;

    private HashSet<Vector3Int> blocked = new HashSet<Vector3Int>();


    [SerializeField]
    private LayerMask clickableLayer, groundLayer;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        ClickTarget();

        NextTarget();
        
    }

    private void ClickTarget() //düşmana tıkladığımda aktif olacak fonksiyon
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {

                RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 128);



                if (hit.collider != null && hit.collider.tag == "Enemy") //eğer bişeyi vurursak
                {

                    DeSelectTarget();

                    SelectTarget(hit.collider.GetComponent<Enemy>());   //yeni hedefi seç

                }
                else //deselect hedef
                {
                    UIManager.MyInstance.HideTargetFrame(); //hedef dışında bir yere tıkladığında framei kapat

                    DeSelectTarget(); //hedefi deselectyap.

                    //hedefin referansını kaldırdık.
                    currentTarget = null;

                    player.MyTarget = null;


                }
            }
            else if (Input.GetMouseButtonDown(1))
            {

                
                //mouse pozisyon ayarlaması
                if (!EventSystem.current.IsPointerOverGameObject())
                {
              

                    RaycastHit2D[] hits = Physics2D.RaycastAll(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, clickableLayer); //128
                    var hit = hits.FirstOrDefault(x => x.collider != null);
                    if (hit.collider != null)
                    {
                       
                        IInteractable entity = hit.collider.gameObject.GetComponent<IInteractable>();

                        if (hit.collider != null && (hit.collider.tag == "Enemy" || hit.collider.tag == "Interactable") && player.MyInteractables.Contains(entity))
                        {
                            entity.Interact();

                        }

                        else
                        {
                          
                            hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, groundLayer); //128
                            if (hit.collider != null)
                            {
                                player.GetPath(mainCamera.ScreenToWorldPoint(Input.mousePosition));
                            }
                        }
                    }

                }


            }



        }

    }

    private void NextTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DeSelectTarget();
            if (Player.MyInstance.MyAttackers.Count > 0 )
            {
                if (targetIndex < Player.MyInstance.MyAttackers.Count)
                {
                    SelectTarget(Player.MyInstance.MyAttackers[targetIndex]);
                    targetIndex++;

                    if (targetIndex >= Player.MyInstance.MyAttackers.Count)
                    {
                        targetIndex = 0;
                    }
                }
                else
                {
                    targetIndex = 0;

                }
                
            }
        }
    }

    private void DeSelectTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.DeSelect();
        }
    }

    private void SelectTarget(Enemy enemy )
    {
        currentTarget = enemy;
        player.MyTarget = currentTarget.Select(); //oyuncuya yeni hedefi ver
        UIManager.MyInstance.ShowTargetFrame(currentTarget); //hedefe tıklayınca hedefin frameini etkinleştirme

    }
    public void OnKillConfirmed(Character character)
    {
        if (killConfirmedEvent != null)
        {
            killConfirmedEvent(character);
        }
    }
}
