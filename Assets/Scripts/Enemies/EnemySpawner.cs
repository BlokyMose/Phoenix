using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class EnemySpawner : MonoBehaviour
    {
		[SerializeField] 
		List<GameObject> enemies = new List<GameObject>();
		[SerializeField] 
		float appearNextTime;
		[SerializeField] 
		int maxNumOfEnemies;
		private int numberOfEnemies;
		private float elapsedTime;

		void Update()
		{
			if (numberOfEnemies >= maxNumOfEnemies)
				return;

			elapsedTime += Time.deltaTime;

			if (elapsedTime > appearNextTime)
			{
				elapsedTime = 0f;
				Spawn();
			}
		}

		void Spawn()
        {
			var randomValue = Random.Range(0, enemies.Count);

			var enemyGO = Instantiate(enemies[randomValue], transform.position, Quaternion.Euler(0f, 0f, 0f));
			enemyGO.SetActive(true);
			var enemy = enemyGO.GetComponent<Enemy>();
			if (enemy != null)
				enemy.Init(OnEnemyDies);

			numberOfEnemies++;
			elapsedTime = 0f;

			void OnEnemyDies()
            {
				numberOfEnemies--;
            }
		}
	}
}
