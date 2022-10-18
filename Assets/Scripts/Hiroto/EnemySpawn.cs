using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
	//　出現させる敵を入れておく
	[SerializeField] GameObject[] enemys;
	//　次に敵が出現するまでの時間
	[SerializeField] float appearNextTime;
	//　この場所から出現する敵の数
	[SerializeField] int maxNumOfEnemys;
	//　今何人の敵を出現させたか（総数）
	private int numberOfEnemys;
	//　待ち時間計測フィールド
	private float elapsedTime;

	// Use this for initialization
	void Start()
	{
		numberOfEnemys = 0;
		elapsedTime = 0f;
	}

	// Update is called once per frame
	void Update()
	{
		//　この場所から出現する最大数を超えてたら何もしない
		if (numberOfEnemys >= maxNumOfEnemys)
		{
			return;
		}
		//　経過時間を足す
		elapsedTime += Time.deltaTime;

		//　経過時間が経ったら
		if (elapsedTime > appearNextTime)
		{
			elapsedTime = 0f;

			AppearEnemy();
		}
	}

	void AppearEnemy()
	{
		//　出現させる敵をランダムに選ぶ
		var randomValue = Random.Range(0, enemys.Length);
		//　敵の向きをランダムに決定
		//var randomRotationY = Random.value * 360f;

		GameObject.Instantiate(enemys[randomValue], transform.position, Quaternion.Euler(0f, 0f, 0f));

		numberOfEnemys++;
		elapsedTime = 0f;
	}
}
