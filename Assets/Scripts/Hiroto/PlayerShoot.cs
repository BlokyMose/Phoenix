using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

    [SerializeField]
    private float shootTimer, shootTimeDelay = 0.2f;

    [SerializeField]
    private Transform bulletSpawnPos;
    private PlayerBulletManager playerBulletManager;
    // Start is called before the first frame update

    private void Awake()
    {
        playerBulletManager = GetComponent<PlayerBulletManager>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shooting();
    }

    void Shooting()
    {
        if (Input.GetMouseButton(0))
        {
            if (Time.time > shootTimer)
            {
                shootTimer = Time.time + shootTimeDelay;

                playerBulletManager.Shoot(bulletSpawnPos.position);
            }
            
        }
    }

}
