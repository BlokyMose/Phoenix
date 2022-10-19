using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Hiroto/Bullet - Hiroto")]
public class Bullet : MonoBehaviour
{

    private Rigidbody2D rb;
    [SerializeField]
    private float moveSpeed = 2.5f, deactiveTimer = 3f;
    [SerializeField]
    private int damageAmount = 25;
    private bool damage;
    [SerializeField]
    private bool destroyObj;
    [SerializeField]
    private bool getTrailRanderer;
    private TrailRenderer trail;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (getTrailRanderer)
        {
            trail = GetComponent<TrailRenderer>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        damage = false;

        Invoke("DeactiveBullet", deactiveTimer);
    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;

        if (getTrailRanderer)
        {
            trail.Clear();
        }
    }

    public void MoveDirection(Vector3 direction)
    {
        rb.velocity = direction * moveSpeed;
    }

    void DeactiveBullet()
    {

        if (destroyObj)
        {
            Destroy(gameObject);
        }

        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            rb.velocity = Vector2.zero;

            CancelInvoke("DeactiveBullet");

            if (!damage)
            {
                damage = true;

                collision.GetComponent<Health>().TakeDamage(damageAmount);
            }

            DeactiveBullet();
        }

        if (collision.CompareTag("Blocking"))
        {
            rb.velocity = Vector2.zero;

            CancelInvoke("DeactiveBullet");

           

            DeactiveBullet();
        }
    }


}
