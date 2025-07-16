using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mos : MonoBehaviour
{
    public Transform player; //추적 대상
    public float detectionRange = 10f; //탐지 거리
    public float moveSpeed = 2f; //추적 속도
    public float floatAmplitude = 0.5f; //위아래 두둥실 흔들리는 높이
    public float floatFrequency = 1f; //두둥실 흔들리는 속도

    private Vector3 initialOffset; //초기 위치 오프셋 (흔들림 기준)
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        //중력 제거
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        initialOffset = transform.position;
    }
    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {
            //부드러운 추적
            Vector3 targetPos = player.position + new Vector3(0, 1.5f, 0); //플레이어 머리 위
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / moveSpeed);
        }
        //정면 바라보기
        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0; //위아래 회전 제거 (no 아래로 내려다 보는게 자연스러운듯)
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        FloatMotion();
    }

    void FloatMotion()
    {
        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position += new Vector3(0, offsetY * Time.deltaTime, 0);
    }
}
