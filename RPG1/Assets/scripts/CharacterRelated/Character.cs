
using System.Collections.Generic;

using Assets.Scripts.Debuff;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public abstract class Character : MonoBehaviour
{

    [SerializeField]
    private float speed;

    private float currentSpeed;

    [SerializeField]
    private string type;

    public Animator MyAnimator { get; set; }

    public Transform MyCurrentTile { get; set; }

    private Vector2 direction;

    [SerializeField]
    private Rigidbody2D myRigidbody;

    public bool IsAttacking { get; set; }

    protected Coroutine actionRoutine ;

    [SerializeField]
    private Transform hitBox;

    public Stack<Vector3> MyPath { get; set; }

    [SerializeField]
    protected Stat health;

    [SerializeField]
    private int level;

    public Character MyTarget { get; set; }

    private List<Debuff> debuffs = new List<Debuff>();

    private List<Debuff> newDebuffs = new List<Debuff>();

    private List<Debuff> expiredDebuffs = new List<Debuff>();

    public SpriteRenderer MySpriteRenderer { get; set; }

    //  public Stack<Vector3> MyPath { get; set; } //path için

    public Stat MyHealth //hedefin framedeki sağlığı
    {
        get { return health; }
    }

    [SerializeField]
    protected float initHealth;


    public bool IsMoving //hareket edip etmediğini kontrol eder.
    {
        get
        {
            return direction.x != 0 || direction.y != 0;
        }
    }

    public Vector2 Direction { get => direction; set => direction = value; }

    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }

    public bool IsAlive
    {
        get
        {
            return health.MyCurrentValue > 0;

        }
    }

    public string MyType { get => type; }
    public int MyLevel { get => level; set => level = value; }

    public Rigidbody2D MyRigidbody { get => MyRigidbody1;  }
    public  Transform MyHitBox { get => hitBox; set => hitBox = value; }
    public Rigidbody2D MyRigidbody1 { get => myRigidbody;  }
    public float Speed { get => speed; private set => speed = value; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentSpeed = Speed;

        MyAnimator = GetComponent<Animator>();
        MySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
   protected virtual void Update()
    {
        HandleLayers();
        HandleDebuffs();
    }

    public void FixedUpdate()
    {
        Move();
        
        if (MyTarget == null) return;
    }

    public void Move()
    {
        if (MyPath == null)
        {
            if (IsAlive)
            {
                //hareket ettirir
                MyRigidbody.velocity = Direction.normalized * CurrentSpeed; //normalized çapraz gittiğinde daha hızlı gitmesini engellemek vektörleri 1,1 yapmak için kullandık.

            }
        }



    }


    public void HandleLayers()
    {
        if (IsAlive)
        {
            //hareket etme ve durma animasyonlarını kontrol eder. eğer ediyosak yürüme animasyonu etmiyosak durma animasyonunun çalıştırır.
            if (IsMoving)
            {

                ActivateLayer("walkLayer");

                //myAnimator.SetFloat("x", direction.x);
                //myAnimator.SetFloat("y", direction.y);
            }
            else if (IsAttacking)
            {
                ActivateLayer("attackLayer");
            }
            else
            {
                //hareket yoksa sıfırla.
                ActivateLayer("idleLayer");
            }
        }
        else
        {
            ActivateLayer("DeathLayer");
           
        }

        
    }
    private void HandleDebuffs()
    {
        if (debuffs.Count > 0)
        {
            foreach (Debuff debuff in debuffs)
            {
                debuff.Update();
            }
        }

        if (newDebuffs.Count > 0)
        {
            debuffs.AddRange(newDebuffs);
            newDebuffs.Clear();
        }
        if (expiredDebuffs.Count > 0)
        {
            foreach (Debuff debuff in expiredDebuffs)
            {
                debuffs.Remove(debuff);
            }
            expiredDebuffs.Clear();
        }
    }

    public void ApplyDebuff(Debuff debuff)
    {
        //aynı isimle debuff var mı kontrol et.
        Debuff tmp = debuffs.Find(x => x.Name == debuff.Name);

        if (tmp != null) //eğer öyleyse 
        {
            expiredDebuffs.Add(tmp); //eski debuffı kaldır
        }

        this.newDebuffs.Add(debuff); //yeni debuffı ekle.
    }

    public void RemoveDebuff(Debuff debuff)
    {
        UIManager.MyInstance.RemoveDebuff(debuff);
        this.expiredDebuffs.Add(debuff);
    }

    //hangi layerin aktif olacağının döngüsü-layer ismine göre
    public void ActivateLayer(string layerName)
    {
        for (int i = 0; i < MyAnimator.layerCount; i++)
        {
            MyAnimator.SetLayerWeight(i, 0);

        }
        MyAnimator.SetLayerWeight(MyAnimator.GetLayerIndex(layerName), 1);
    }

    

    public virtual void TakeDamage(float damage, Character source)
    {
        health.MyCurrentValue -= damage;
        CombatTextManager.MyInstance.CreateText(transform.position, damage.ToString(), SCTTYPE.DAMAGE,false); //zarar aldıkça text çıksın diye 
        if (health.MyCurrentValue <= 0 )
        {
            Direction = Vector2.zero;
            MyRigidbody.velocity = Direction;
            GameManager.MyInstance.OnKillConfirmed(this);
            MyAnimator.SetTrigger("die");
            
            
        }
    }

    public void GetHealth(int health)
    {
        MyHealth.MyCurrentValue += health;
        CombatTextManager.MyInstance.CreateText(transform.position,health.ToString(),SCTTYPE.HEAL,true); //textin oluşması için sağlık arttıkça
    }
}
