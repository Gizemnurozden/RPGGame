
using System.Collections.Generic;
using UnityEngine;


public delegate void HealthChanged(float health);

public delegate void CharacterRemoved();

public class Enemy : Character, IInteractable

{
    public event HealthChanged healthChanged;

    public event CharacterRemoved characterRemoved;

    [SerializeField]
    private CanvasGroup healthGroup;


    private IState currentState;

    [SerializeField]
    private LootTable lootTable;

    [SerializeField]
    private Astar astar;

    [SerializeField]
    private LayerMask losMask;

    [SerializeField]
    protected int damage;

    private bool canDoDamage = true;

    [SerializeField]
    private float attackRange;
  

    public float  MyAttackTime { get; set; }

    [SerializeField]
    private float initAggroRange;

    public float MyAggroRange { get; set; }

    public Vector3 MyStartPosition { get; set; }


    [SerializeField]
    private Sprite portrait;

    public Sprite MyPortrait { get => portrait; }

    public bool InRange
    {
        get
        {
            return Vector2.Distance(transform.position, MyTarget.transform.position) < MyAggroRange;
        }
    }

    public float MyAttackRange { get => attackRange; set => attackRange = value; }

    public Astar MyAstar { get => astar; }

    protected void Awake()
    {
        health.Initialized(initHealth, initHealth);
        SpriteRenderer sr;
        sr = GetComponent<SpriteRenderer>();
        MyStartPosition = transform.position;
        MyAggroRange = initAggroRange;
        
        ChangeState(new IdleState());
    }

    protected override void Start()
    {
        base.Start();
        MyAnimator.SetFloat("y", -1);
    }

    protected override void Update()
    {
        if (IsAlive)
        {
            if (!IsAttacking)
            {
                MyAttackTime += Time.deltaTime;
            }

            currentState.Update();

            if (MyTarget != null && !Player.MyInstance.IsAlive)
            {
                ChangeState(new EvadeState());
            }
          
        }
        base.Update();
    }

    public Character Select()
    {
        healthGroup.alpha = 1;

        return this;
    }

    public void DeSelect()
    {

        healthGroup.alpha = 0;

        healthChanged -= new HealthChanged(UIManager.MyInstance.UpdateTargetFrame); //diğer hedefi seçtiğmde framei göster.
        characterRemoved -= new CharacterRemoved(UIManager.MyInstance.HideTargetFrame); //karakter kalkınca framei gizle
    }

    public override void TakeDamage(float damage, Character source) //frame barı tetikliyorum.
    {
        if (!(currentState is EvadeState))
        {
            if (IsAlive)
            {
                SetTarget(source);

                base.TakeDamage(damage, source);

                OnHealthChanged(health.MyCurrentValue); //sağlığın değiştiği kısım

                if (!IsAlive)
                {
                    Player.MyInstance.MyAttackers.Remove(this);
                    Player.MyInstance.GainXP(XPManager.CalculateXP((this as Enemy)));

                }
            }
            

        }
       
    }

    public void DoDamage()
    {
        if (canDoDamage)
        {
            MyTarget.TakeDamage(damage, this);
            
            canDoDamage = false;
        }
   
    }

    public void CanDoDamage()
    {
        canDoDamage = true;
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }



        currentState = newState;
        currentState.Enter(this);

        
    }

    public void SetTarget(Character target) //düşmanla mesafe ve seçip ayarlama
    {
        if (MyTarget == null && !(currentState is EvadeState))
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            MyAggroRange = initAggroRange;
            MyAggroRange += distance;
            MyTarget = target;
        }
       
    }
    public void Reset()
    {
        this.MyTarget = null;
        this.MyAggroRange = initAggroRange;
        this.MyHealth.MyCurrentValue = this.MyHealth.MyMaxValue;
        OnHealthChanged(health.MyCurrentValue);
    }

    public  void Interact() //düşman öldüyse loot pencersini aç.
    {
        if (!IsAlive)
        {
            List<Drops> drops = new List<Drops>();

            foreach (IInteractable interactable in Player.MyInstance.MyInteractables)
            {
                if (interactable is Enemy && !(interactable as Enemy).IsAlive)
                {
                    drops.AddRange((interactable as Enemy).lootTable.GetLoot());
                }
            }
            LootWindow.MyInstance.SetCurrentEnemy(this);
            LootWindow.MyInstance.CreatePages(drops);

        }
        
    }

    public void StopInteract() //düşmanla etkileişmi kesince loot penceresi kapansın
    {
        LootWindow.MyInstance.Close();
    }

    public void OnHealthChanged(float health) //olayı tetiklemeden önce dinleip dinlrmediğini anlamak içni
    {
        if (healthChanged != null)
        {
            healthChanged(health); //tetiklemek için enemy scriptinde hasarda yazıyorum.
        }

    }

    public void OnCharacterRemoved()
    {
        if (characterRemoved != null)
        {
            characterRemoved();
        }
        Destroy(gameObject);
    }

    public bool CanSeePlayer()
    {
        Vector3 targetDirection = (MyTarget.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, Vector2.Distance(transform.position, MyTarget.transform.position), losMask);

        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    public void OnLootWindowClosed()
    {
        Destroy(gameObject); // Düşmanı yok et
    }

}

