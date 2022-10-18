using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_speed; // �ړ��̑���

    public static Player m_instance;

    private void Awake()
    {
        m_instance = this;
    }

    // ���t���[���Ăяo�����֐�
    private void Update()
    {
        // �Q�[���� 60 FPS �Œ�ɂ���
        Application.targetFrameRate = 60;

        // ���L�[�̓��͏����擾����
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        // ���L�[��������Ă�������Ƀv���C���[���ړ�����
        var velocity = new Vector3(h, v) * m_speed;
        transform.localPosition += velocity;

        transform.localPosition = Utils.ClampPosition(transform.localPosition);

        // �v���C���[�̃X�N���[�����W���v�Z����
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // �v���C���[���猩���}�E�X�J�[�\���̕������v�Z����
        var direction = Input.mousePosition - screenPos;

        // �}�E�X�J�[�\�������݂�������̊p�x���擾����
        var angle = Utils.GetAngle(Vector3.zero, direction);

        // �v���C���[���}�E�X�J�[�\���̕���������悤�ɂ���
        var angles = transform.localEulerAngles;
        angles.z = angle - 90;
        transform.localEulerAngles = angles;
    }
}