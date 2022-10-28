using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Hiroto/EnemySpawn - Hiroto")]
public class EnemySpawn : MonoBehaviour
{
	//�@�o��������G�����Ă���
	[SerializeField] GameObject[] enemys;
	//�@���ɓG���o������܂ł̎���
	[SerializeField] float appearNextTime;
	//�@���̏ꏊ����o������G�̐�
	[SerializeField] int maxNumOfEnemys;
	//�@�����l�̓G���o�����������i�����j
	private int numberOfEnemys;
	//�@�҂����Ԍv���t�B�[���h
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
		//�@���̏ꏊ����o������ő吔�𒴂��Ă��牽�����Ȃ�
		if (numberOfEnemys >= maxNumOfEnemys)
		{
			return;
		}
		//�@�o�ߎ��Ԃ𑫂�
		elapsedTime += Time.deltaTime;

		//�@�o�ߎ��Ԃ��o������
		if (elapsedTime > appearNextTime)
		{
			elapsedTime = 0f;

			AppearEnemy();
		}
	}

	void AppearEnemy()
	{
		//�@�o��������G�������_���ɑI��
		var randomValue = Random.Range(0, enemys.Length);
		//�@�G�̌����������_���Ɍ���
		//var randomRotationY = Random.value * 360f;

		GameObject.Instantiate(enemys[randomValue], transform.position, Quaternion.Euler(0f, 0f, 0f));

		numberOfEnemys++;
		elapsedTime = 0f;
	}
}