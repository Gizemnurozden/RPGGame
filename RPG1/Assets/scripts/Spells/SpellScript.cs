using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Debuff;
using UnityEngine;

public class SpellScript : MonoBehaviour
{
    private Rigidbody2D myRigidBody;

    [SerializeField]
    protected float speed;

    
   

    public Transform MyTarget { get; protected set; }

    public Character Source { get; set; }

    protected float damage;

    private Debuff debuff;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    
    }
    public void Initialize(Transform target, float damage, Character source, Debuff debuff)
    {
        this.MyTarget = target;
        this.damage = damage;
        this.Source = source;
        this.debuff = debuff;
    }

    public void Initialize(Transform target, float damage, Character source)
    {
        this.MyTarget = target;
        this.damage = damage;
        this.Source = source;
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        if (MyTarget != null)
        {
            //büyü yönünü ayarlar
            Vector2 direction = MyTarget.position - transform.position;
            //büyü yer çekiminin hareketini ayarlar.
            myRigidBody.velocity = direction.normalized * speed;
            //açının rotasyonunu ayarlar.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //büyünün hedefe rotasyonunu ayarlar.
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
       
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision) //çarpışma sonucu etkinleştircek
    {
        if (collision.tag == "HitBox" && collision.transform == MyTarget)
        {
            Character c = collision.GetComponentInParent<Character>();
            speed = 0;
            c.TakeDamage(damage, Source);

            if (debuff != null)
            {
                Debuff clone = debuff.Clone();
                debuff.Apply(c);
            }
            GetComponent<Animator>().SetTrigger("impact");
            myRigidBody.velocity = Vector2.zero;
            MyTarget = null;
        }
    }
} 
