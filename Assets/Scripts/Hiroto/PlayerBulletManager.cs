using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletManager : MonoBehaviour
{
    [SerializeField]
    private BuletPointManager[] playerBulletPoints;
    private int bulletPointIndex;

    [SerializeField]
    private GameObject[] bullets;
    private Vector2 tagetPos, direction, bulletSpawnPos;
    private Camera cam;
    private Quaternion bulletRotation;
    private void Awake()
    {
        bulletPointIndex = 0;
        playerBulletPoints[bulletPointIndex].gameObject.SetActive(true);

        cam = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeBullet();
    }

    private void ChangeBullet()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            playerBulletPoints[bulletPointIndex].gameObject.SetActive(false);

            bulletPointIndex++;

            if (bulletPointIndex >= playerBulletPoints.Length)
            {
                bulletPointIndex = 0;
            }

            playerBulletPoints[bulletPointIndex].gameObject.SetActive(true);
        }

        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            playerBulletPoints[bulletPointIndex].gameObject.SetActive(false);

            bulletPointIndex--;

            if (bulletPointIndex < 0)
            {
                bulletPointIndex = playerBulletPoints.Length - 1;
            }

            playerBulletPoints[bulletPointIndex].gameObject.SetActive(true);
        }

        for (int i = 0; i < playerBulletPoints.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                playerBulletPoints[bulletPointIndex].gameObject.SetActive(false);

                bulletPointIndex = i;

                playerBulletPoints[bulletPointIndex].gameObject.SetActive(true);

                break;
            }
        }

    }

    public void Shoot(Vector2 spawnPos)
    {
        tagetPos = cam.ScreenToWorldPoint(Input.mousePosition);

        bulletSpawnPos = spawnPos;

        direction = (tagetPos - bulletSpawnPos).normalized;

        bulletRotation = Quaternion.Euler(0,0,Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg);

        GameObject newBullet = Instantiate(bullets[bulletPointIndex],spawnPos,bulletRotation);

        newBullet.GetComponent<Bullet>().MoveDirection(direction);
    }
}
