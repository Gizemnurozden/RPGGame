using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class Player : Character
{
    private static Player instance;

    public static Player MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
            }

            return instance;
        }
    }

    private List<Enemy> attackers = new List<Enemy>();

    [SerializeField]
    private Stat mana;

    [SerializeField]
    private Stat xpStat;


    [SerializeField]
    private Text levelText;

    private float initMana = 50;

    private Vector2 initPos;

    [SerializeField]
    private Block[] blocks;

    [SerializeField]
    private Animator ding;

    [SerializeField]
    private Transform minimapIcon; //mini haritanın ikonu için referans

    [SerializeField]
    private Profession profession;

    
    private GameObject unusedSpell;

    private Spell aoeSpel;

    [SerializeField]
    private Transform[] exitPoint;

  
    private int exitIndex = 0;


    private List<IInteractable> interactables = new List<IInteractable>();

    //pathfinding come here.
    

    private Vector3 destination;

    private Vector3 goal;

    private Vector3 current;

    [SerializeField]
    private Astar astar;

    private Vector3 min, max;

    public Coroutine MyInitRoutine { get; set; }

    [SerializeField]
    private Camera mainCam;

    public int MyGold { get; set; }

    public List<IInteractable> MyInteractables { get => interactables; set => interactables = value; }

    public Stat MyXp { get => xpStat; set => xpStat = value; }

    public Stat MyMana { get => mana; set => mana = value; }

    public List<Enemy> MyAttackers { get => attackers; set => attackers = value; }
    

    protected override void Update()
    {
        GetInput();
        ClickToMove();

        //clamps the player inside the tilemap
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), Mathf.Clamp(transform.position.y, min.y, max.y),transform.position.z);

        if (unusedSpell != null) //fire yağmuru için.
        {
            Vector3 mouseScreenPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            unusedSpell.transform.position = new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0);

            float distance = Vector2.Distance(transform.position, mainCam.ScreenToWorldPoint(Input.mousePosition));
            if (distance >= aoeSpel.MyRange)
            {
                unusedSpell.GetComponent<AOESpell>().OutOfRange();
            }
            else
            {
                unusedSpell.GetComponent<AOESpell>().InRange();
            }

            if (Input.GetMouseButtonDown(0) && distance <= aoeSpel.MyRange)
            {
                AOESpell s = Instantiate(aoeSpel.MySpellPrefab, unusedSpell.transform.position, Quaternion.identity).GetComponent<AOESpell>();
                Destroy(unusedSpell);
                unusedSpell = null;
                s.Initialize(aoeSpel.MyDamage, aoeSpel.MyDuration);
            }
        }

        base.Update();
 
    }

    

    public void SetDefaultValues()
    {
        //sağlık ve mana nın güncellenmesi ve xp bar 
        MyGold = 1000;
        health.Initialized(initHealth, initHealth);
        MyMana.Initialized(initMana, initMana);
        MyXp.Initialized(0, Mathf.Floor(100 * MyLevel * Mathf.Pow(MyLevel, 0.5f))); // XP stati güncelleme için yapılan 
        levelText.text = MyLevel.ToString(); //oyuncunun level textini güncelleme
        initPos = transform.parent.position; //ölünce yeniden başlarkenki pozisyonu
    }
    

    private void GetInput() //klavyede oyuncunun hareketleri
    {
        Direction = Vector2.zero;


        // THİS İS USED FOR DEBUGGİNG ONLY -> I ve O tuşlarına basıldığında sağlığın azalıp artmaso 

        if (Input.GetKeyDown(KeyCode.I))
        {
            health.MyCurrentValue -= 10;
            MyMana.MyCurrentValue -= 10;
        }
        if (Input.GetKeyDown(KeyCode.X)) //gain artması için
        {
            GainXP(600);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            health.MyCurrentValue += 10;
            MyMana.MyCurrentValue += 10;
        }

        //buraya kadar
        //tuşlarla hareket 
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["UP"])) //MOVES UP
        {
            exitIndex = 2;
            Direction += Vector2.up;
            minimapIcon.eulerAngles = new Vector3(0, 0, 0);
           
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["LEFT"])) //MOVES LEFT
        {
           
            exitIndex = 3;
            Direction += Vector2.left;
            FlipCharacter(true);

            if (Direction.y == 0) //minimap ikonu için yön verme
            {
                minimapIcon.eulerAngles = new Vector3(0, 0, 90);
            }
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["DOWN"])) //MOVES DOWN
        {
            exitIndex = 0;
            Direction += Vector2.down;
            minimapIcon.eulerAngles = new Vector3(0, 0, 180);
            
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["RİGHT"])) //MOVES RİGHT
        {
            exitIndex = 1;
            Direction += Vector2.right;
            FlipCharacter(false);

            if (Direction.y == 0) //minimap ikonu için yön verme
            {
                minimapIcon.eulerAngles = new Vector3(0, 0, 270);
            }
        }
        if (IsMoving)
        { 
            StopAction();
            StopInit();
        }

        foreach (string action in KeybindManager.MyInstance.ActionBinds.Keys)
        {
            if (Input.GetKeyDown(KeybindManager.MyInstance.ActionBinds[action]))
            {
                UIManager.MyInstance.ClickActionButton(action);
            }
        }
       
    }

  
    private void  FlipCharacter(bool isLeft) //sağ sol yön hareket
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = isLeft;

    }

    public void SetLimits(Vector3 min, Vector3 max) //oyuncunun ekrandan çıkmaması için
    {
        this.min = min;
        this.max = max;
    }

    private IEnumerator AttackRoutine (ICastable castable)
    {

        Transform currentTarget = MyTarget.MyHitBox;


        yield return actionRoutine = StartCoroutine(ActionRoutine(castable));

        if (currentTarget !=  null && InLineOfSight())
        {

            Spell newSpell = SpellBook.MyInstance.GetSpell(castable.MyTitle);

            //hedefimi seçme
            SpellScript s = Instantiate(newSpell.MySpellPrefab, exitPoint[exitIndex].position, Quaternion.identity).GetComponent<SpellScript>();

            s.Initialize(currentTarget, newSpell.MyDamage, this,newSpell.MyDebuff); 
        }

        StopAction(); //atağı sonlandırır.

    }



    private IEnumerator GatherRotine(ICastable castable, List<Drops> items) //gather animasyonu
    {

        yield return actionRoutine = StartCoroutine(ActionRoutine(castable));

        LootWindow.MyInstance.CreatePages(items);
    }




    public IEnumerator CraftRoutine(ICastable castable) //Crafti etkinleştirip inventorymime almak için
    {
        yield return actionRoutine = StartCoroutine(ActionRoutine(castable));

        profession.AddItemsToInventory();
    }




    private IEnumerator ActionRoutine(ICastable castable) //attcak animasyonu
    {

        SpellBook.MyInstance.Cast(castable);

        IsAttacking = true;

        MyAnimator.SetBool("Attack", true); //atak animasyonunu başlatır

        yield return new WaitForSeconds(castable.MyCastTime);

        StopAction();


    }

    public void CastSpell(Spell spell)
    {

        Block();

        if (!spell.NeedsTarget)
        {
            unusedSpell = Instantiate(spell.MySpellPrefab, Camera.main.ScreenToWorldPoint
                (Input.mousePosition), Quaternion.identity);
            unusedSpell.transform.position = new Vector3(unusedSpell.transform.position.x,
                unusedSpell.transform.position.y, 0);
            aoeSpel = spell;
        }

        if (MyTarget != null &&MyTarget.GetComponentInParent<Character>().IsAlive && !IsAttacking && !IsMoving && InLineOfSight() && InRange((spell),MyTarget.transform.position))
        {

            MyInitRoutine = StartCoroutine(AttackRoutine(spell));
        }

    }

    private bool InRange(Spell spell,Vector2 targetPos)
    {
        if (Vector2.Distance(targetPos,transform.position) <= spell.MyRange)
        {
            return true;
        }
        MessageFeedManager.MyInstance.WriteMessage("OUT OF RANGE", Color.red);
        return false;
    }

    public void Gather(ICastable castable, List<Drops> items)
    {
        if (!IsAttacking)
        {
            MyInitRoutine = StartCoroutine(GatherRotine(castable, items));
        }
    }


    private bool InLineOfSight()
    {

        if (MyTarget != null)
        {
            Vector3 targetDirection = (MyTarget.transform.position - transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, Vector2.Distance(transform.position, MyTarget.transform.position), 64);

            if (hit.collider == null)
            {
                return true;
            }
        }

    
        return false;
    }

    private void Block() //bloklar
    {
        foreach (Block b in blocks)
        {
            b.Deactivate();
        }
        blocks[exitIndex].Activate();
    }

    public void StopAction()
    {

        SpellBook.MyInstance.StopCasting();

        IsAttacking = false;
        MyAnimator.SetBool("Attack", IsAttacking);

        if (actionRoutine  != null)
        {
            StopCoroutine(actionRoutine );

        }

    }

    private void StopInit()
    {
        if (MyInitRoutine != null)
        {
            StopCoroutine(MyInitRoutine);
        }
    }

    public void GainXP(int xp) //gain arttıkça efektli üstünden textin çıkması için
    {
        MyXp.MyCurrentValue += xp;
        CombatTextManager.MyInstance.CreateText(transform.position, xp.ToString(), SCTTYPE.XP,false);

        if (MyXp.MyCurrentValue >= MyXp.MyMaxValue)
        {
            StartCoroutine(Ding());
        }
    }

    public void AddAttacker(Enemy enemy)
    {
        if (!MyAttackers.Contains(enemy))
        {
            MyAttackers.Add(enemy);
        }
    }

    private IEnumerator Ding() //level arttırma fonksiyonu 
    {
        while (!MyXp.IsFull) //eğer level statim full ise 
        {
            yield return null;
        }

        MyLevel++; //leveli arttır.

        ding.SetTrigger("ding");

        levelText.text = MyLevel.ToString(); //leveltextini günceller

        MyXp.MyMaxValue = 100 * MyLevel * Mathf.Pow(MyLevel, 0.5f); //level dolunca ikinci level kısmına geçişi

        MyXp.MyMaxValue = Mathf.Floor(MyXp.MyMaxValue); // max value değerimi düzgün alsın diye

        MyXp.MyCurrentValue = MyXp.MyOverflow; //current value yenilenerek resetlesin diye 
        MyXp.Reset(); //stati resetlesin diye.

        Debug.Log(MyXp.MyCurrentValue);
        Debug.Log(MyXp.MyMaxValue);

        if (MyXp.MyCurrentValue >= MyXp.MyMaxValue)
        {
            
            StartCoroutine(Ding());
        }

    }

    public void UpdateLevel()
    {
        levelText.text = MyLevel.ToString();
    }

    public IEnumerator Respawn() //ölünce yeniden başlama.
    {
        MySpriteRenderer.enabled = false;
        yield return new WaitForSeconds(3f);
        health.Initialized(initHealth, initHealth);
        MyMana.Initialized(initMana, initMana);
        transform.parent.position = initPos;
        MySpriteRenderer.enabled = true;
        MyAnimator.SetTrigger("respawn");
    }

    
    public void GetPath(Vector3 goal)
    {
        MyPath = astar.Algorithm(transform.position, goal);
        current = MyPath.Pop();
        destination = MyPath.Pop();
        this.goal = goal;
    }

    public void ClickToMove()
    {
        if (MyPath != null)
        {
            transform.parent.position = Vector2.MoveTowards(transform.parent.position, destination, CurrentSpeed * Time.deltaTime);
            Vector3Int dest = astar.MyTilemap.WorldToCell(destination);
            Vector3Int cur = astar.MyTilemap.WorldToCell(current);

            float distance = Vector2.Distance(destination, transform.parent.position);

            if (cur.y > dest.y)
            {
                Direction = Vector2.down;
            }
             else if (cur.y < dest.y)
            {
                Direction = Vector2.up;
            }
            if (cur.y == dest.y)
            {
                if (cur.x > dest.x)
                {
                    Direction = Vector2.left;
                }
                else if (cur.x < dest.x)
                {
                    Direction = Vector2.right;
                }
            }
            if (distance <= 0f)
            {
                if (MyPath.Count >0)
                {
                    current = destination;
                    destination = MyPath.Pop();

                 }
                else
                {
                    MyPath = null;

                }
            }
        }
    }
   

    public void OnTriggerEnter2D(Collider2D collision) //düşmana yaklaşma tetikleme
    {
        if (collision.tag =="Enemy" || collision.tag == "Interactable") //chesti de triggerını tetikledim.ve game managerda da
        {

            IInteractable interactable = collision.GetComponent<IInteractable>();

            if (!MyInteractables.Contains(interactable))
            {
                MyInteractables.Add(interactable);
            }

          
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if (MyInteractables.Count > 0)
            {
                IInteractable interactable = MyInteractables.Find(x => x == collision.GetComponent<IInteractable>());

                if (interactable != null)
                {
                    interactable.StopInteract();
                }

                MyInteractables.Remove(interactable);
            }
            
        }
    }

}
