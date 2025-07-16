using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anime;
    [Header("Dash Settings")] //대시설정
    private float dashSpeed = 60f;
    private float maxDashDistance = 12f;
    private float punchDistance = 0.1f; //대시 후 펀치 거리
    private float targetSearchRadius = 10f; //대시 타겟 범위
    [SerializeField] private LayerMask dashTargetMask; //대시 타겟 레이어

    [Header("Attack Settings")] //공격설정
    [SerializeField] private float attackDuration = 0.8f; //공격 애니메이션 지속 시간

    private bool isDashing = false; //대시 중인지
    public bool isAttacking = false; //공격 중인지
    public bool Illusion_Sword_Dance = false;
    public bool Sword_Shoot = false;
    private GameObject dashTargetObject = null; //대시 타겟이 있을 경우 저장
    private bool Cooltime = true; //공격 가능 여부
    public float attackCooldown = 1.0f; //공격 쿨타임
    private Transformation get_transform_level;
    [SerializeField] private GameObject projectilePrefab; //투사체 프리팹
    [SerializeField] private Transform projectileSpawnPoint; //투사체가 발사될 위치

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
        get_transform_level = GetComponent<Transformation>();
    }
    void Update()
    {
        Debug.Log("현재 애니속도" + anime.speed);
        //슬래시 어택
        if (Input.GetMouseButtonDown(0) && Cooltime && !isAttacking)
        {
            if (get_transform_level.transform_level1 == true)
            {
                StartCoroutine(Illusion_Sword());
                StartCoroutine(AttackCool_Time());
            }
            else if (get_transform_level.transform_level2 == true)
            {
                StartCoroutine(Illusion_Sword());
                StartCoroutine(AttackCool_Time());
            }
            else
            {
                StartCoroutine(AttackRoutine()); //공격 애니메이션 처리
                StartCoroutine(AttackCool_Time()); //공격 쿨타임 처리
            }
        }
        //슬래시 날리기
        if (Input.GetMouseButtonDown(1) && Cooltime && !isAttacking)
        {
            if (get_transform_level.transform_level1 || get_transform_level.transform_level2)
            {
                StartCoroutine(Sword_Shooting());
                StartCoroutine(AttackCool_Time());
            }
            else
            {
                StartCoroutine(Sword_Shooting());
                StartCoroutine(AttackCool_Time());
            }
        }
        //대시
        if (Input.GetKeyDown(KeyCode.F) && !isDashing)
        {
            TryDash(); //자동 타겟
        }
    }
    IEnumerator Sword_Shooting() //투사체 발사
    {
        isAttacking = true;
        anime.speed = 3f;
        Sword_Shoot = true;
        anime.SetBool("Sword_Shoot", true); //애니메이션 파라미터 설정
        yield return new WaitForSeconds(attackDuration);

        //투사체 생성 및 발사
        LaunchProjectile();

        anime.SetBool("Sword_Shoot", false);
        Sword_Shoot = false;
        anime.speed = 1f;
        isAttacking = false;
    }
    //투사체 발사 함수
    void LaunchProjectile()
    {
        //투사체를 생성하고 발사
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        //캐릭터 전방으로 발사
        Vector3 direction = transform.forward; //전방 방향
        float speed = 25f; //발사 속도

        //Rigidbody를 통해 투사체 이동
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }
    IEnumerator Illusion_Sword()
    {
        anime.speed = 3f;
        Illusion_Sword_Dance = true;
        anime.SetBool("Illusion_Sword_Dance", true); //애니메이션 파라미터 설정
        yield return new WaitForSeconds(attackDuration); //애니메이션 재생 대기

        anime.SetBool("Illusion_Sword_Dance", false);
        Illusion_Sword_Dance = false;
        anime.speed = 1f;

    }
    //기본기 코루틴
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anime.speed = 1f;
        anime.SetTrigger("Slash");
        yield return new WaitForSeconds(attackDuration); //애니메이션 재생 대기

        anime.SetTrigger("Slash");
        isAttacking = false; //공격종료
    }

    //공격 쿨타임을 관리하는 코루틴
    IEnumerator AttackCool_Time()
    {
        Cooltime = false; //쿨타임 동안 공격 금지
        yield return new WaitForSeconds(attackCooldown);
        Cooltime = true; //쿨타임 종료 후 공격 가능
    }

    //대시 기능 (근처 적 자동 타겟팅)
    void TryDash()
    {
        GameObject nearest = FindClosestTargetInRange(); //주변 가장 가까운 적 탐색
        Vector3 target;
        Vector3 dir;

        if (nearest != null)
        {
            //자동 타겟팅 성공 시, 타겟 근처까지 이동
            dir = (nearest.transform.position - transform.position).normalized;
            target = nearest.transform.position - dir * punchDistance;
            dashTargetObject = nearest;
        }
        else
        {
            //타겟 없으면 마우스 클릭 위치로 대시
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, 100f, dashTargetMask)) return;

            dir = (hit.point - transform.position).normalized;
            target = hit.point;
            dashTargetObject = null;
        }

        //거리 계산 및 제한
        float distance = Vector3.Distance(transform.position, target);
        float dashDistance = Mathf.Min(distance, maxDashDistance);

        if (dashDistance <= 0.2f) return; //너무 가까우면 무시

        Vector3 dashTarget = transform.position + dir * dashDistance;

        //대시 애니메이션 트리거
        if (anime != null)
            anime.SetBool("isDashing", true);

        StartCoroutine(DashRoutine(dashTarget)); //대시 실행
    }

    //실제 대시 이동을 처리하는 코루틴
    IEnumerator DashRoutine(Vector3 targetPosition)
    {
        isDashing = true;

        float elapsed = 0f;
        float duration = Vector3.Distance(transform.position, targetPosition) / dashSpeed;
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(start, targetPosition, elapsed / duration);
            rb.MovePosition(newPos); //스근하게 이동

            //타겟이 있을 경우, 근접 시 멈춤
            if (dashTargetObject != null)
            {
                if (Vector3.Distance(newPos, dashTargetObject.transform.position) <= punchDistance)
                {
                    Vector3 stopDir = (dashTargetObject.transform.position - transform.position).normalized;
                    rb.MovePosition(dashTargetObject.transform.position - stopDir * punchDistance);
                    dashTargetObject = null;
                    break;
                }
            }

            yield return null;
        }
        //마지막 위치 보정
        rb.MovePosition(targetPosition);
        isDashing = false;

        //애니메이션 종료 처리
        if (anime != null)
        {
            anime.SetBool("isDashing", false);
            anime.speed = 1f;
        }
    }
    //가장 가까운 타겟 탐색하는 함수
    GameObject FindClosestTargetInRange()
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag("Mos");
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject target in allTargets)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < targetSearchRadius && dist < minDist)
            {
                closest = target;
                minDist = dist;
            }
        }
        return closest;
    }
}