using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField]
    private int maxHealth = 5;
    private int health;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsAllive()
    {
        return health > 0 ? true : false;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damageAmount)
    {
        health = health - damageAmount;

        if (health <= 0)
        {

            animator.SetTrigger("Death");

        }

        //GetComponent<BoxCollider>().enabled = false;

    }
}
