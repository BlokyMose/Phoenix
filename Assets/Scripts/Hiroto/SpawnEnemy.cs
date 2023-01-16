using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class SpawnEnemy : MonoBehaviour
    {
        enum ShotType
        {
            NONE = 0,
            AIM,            // �v���C���[��_��
            THREE_WAY,      // �R����
        }

        [System.Serializable]
        struct ShotData
        {
            public int frame;
            public ShotType type;
            public ComingEnemy bullet;
        }

        // �V���b�g�f�[�^
        [SerializeField] ShotData shotData = new ShotData { frame = 60, type = ShotType.NONE, bullet = null };

        GameObject playerObj = null;    // �v���C���[�I�u�W�F�N�g
        int shotFrame = 0;              // �t���[��

        void Start()
        {
            // �v���C���[�I�u�W�F�N�g���擾����
            switch (shotData.type)
            {
                case ShotType.AIM:
                    playerObj = GameObject.Find("Player");
                    break;
            }
        }
        void Update()
        {
            Shot();
        }

        // �V���b�g�����i�����Update�ȂǂŌĂԁj
        void Shot()
        {
            ++shotFrame;
            if (shotFrame > shotData.frame)
            {
                switch (shotData.type)
                {
                    // �v���C���[��_��
                    case ShotType.AIM:
                        {
                            if (playerObj == null) { break; }
                            ComingEnemy bullet = (ComingEnemy)Instantiate(
                                shotData.bullet,
                                transform.position,
                                Quaternion.identity
                            );
                            bullet.SetMoveVec(playerObj.transform.position - transform.position);
                        }
                        break;

                    // �R����
                    case ShotType.THREE_WAY:
                        {
                            ComingEnemy bullet = (ComingEnemy)Instantiate(
                                shotData.bullet,
                                transform.position,
                                Quaternion.identity
                            );
                            bullet = (ComingEnemy)Instantiate(shotData.bullet, transform.position, Quaternion.identity);
                            bullet.SetMoveVec(Quaternion.AngleAxis(15, new Vector3(0, 0, 1)) * new Vector3(-1, 0, 0));
                            bullet = (ComingEnemy)Instantiate(shotData.bullet, transform.position, Quaternion.identity);
                            bullet.SetMoveVec(Quaternion.AngleAxis(-15, new Vector3(0, 0, 1)) * new Vector3(-1, 0, 0));
                        }
                        break;
                }

                shotFrame = 0;
            }
        }
    }
}
