using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_respawn_point : MonoBehaviour
{
 [Header("모기")]
    public GameObject mosquitoPrefab;
    public Transform spawnAreaCenter;
    public Vector3 spawnAreaSize = new Vector3(10f, 5f, 10f);

    [Header("생성 제한")]
    public float spawnInterval = 3f;
    public int maxMosquitoCount = 10;
    private int currentMosquitoCount = 0;

    void Start()
    {
        StartCoroutine(SpawnMosquitoRoutine());
    }

    IEnumerator SpawnMosquitoRoutine()
    {
        while (true)
        {
            if (currentMosquitoCount < maxMosquitoCount)
            {
                SpawnMosquito();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    void SpawnMosquito()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        Vector3 spawnPos = spawnAreaCenter.position + randomOffset;
        GameObject mosquito = Instantiate(mosquitoPrefab, spawnPos, Quaternion.identity);
        currentMosquitoCount++;

        //모기가 죽으면 카운트 줄이기
        mosquito.GetComponent<EnemyHealth>().OnDeath += () => currentMosquitoCount--;
    }

    //디버그용 영역 표시
    void OnDrawGizmosSelected()
    {
        if (spawnAreaCenter != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(spawnAreaCenter.position, spawnAreaSize);
        }
    }
}
