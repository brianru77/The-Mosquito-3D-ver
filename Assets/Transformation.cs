using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformation : MonoBehaviour
{
    public int transform_level = 0; //0평소 1중간혈압 2고혈압
    public bool transform_level1 = false;
    public bool transform_level2 = false;
    private Animator anime;
    public GameObject Transform_Effect1;
    public GameObject Transform_Effect12;
    public GameObject Transform_final_level_Effect;

    void Start()
    {
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (transform_level <= 2)
            {
                transform_level++;
                StartCoroutine(TransformSequence(transform_level));
            }
        }
    }
    IEnumerator TransformSequence(int level)
    {
        if (level == 1)
        {
            transform_level1 = true;
            anime.SetBool("transform_level1", true); //애니메이터에 bool 값 전달
            Debug.Log("변신1단계!");

            if (Transform_Effect1 != null)
            {
                GameObject effect = Instantiate(Transform_Effect1, transform.position, Quaternion.identity);
                effect.transform.SetParent(transform);
                effect.transform.localPosition = Vector3.zero;
                effect.transform.localRotation = Quaternion.identity;
            }
            StartCoroutine(ResetTransformAfterDelay(1.3f)); //변신애니종료
        }
        else if (level == 2)
        {
            transform_level2 = true;
            anime.SetBool("transform_level2", true); //애니메이터에 bool 값 전달
            Debug.Log("변신2단계!");

            if (Transform_Effect12 != null)
            {
                GameObject effect1 = Instantiate(Transform_Effect12, transform.position, Quaternion.identity);
                effect1.transform.SetParent(transform);
                effect1.transform.localPosition = new Vector3(0, 0.7f, 0);
                effect1.transform.localRotation = Quaternion.identity;
            }

            if (Transform_final_level_Effect != null)
            {
                GameObject effect2 = Instantiate(Transform_final_level_Effect, transform.position, Quaternion.identity);
                effect2.transform.SetParent(transform);
                effect2.transform.localPosition = Vector3.zero;
                effect2.transform.localRotation = Quaternion.identity;

                Destroy(effect2, 2f);
            }
            StartCoroutine(ResetTransformAfterDelay(1.3f)); //변신애니종료
        }
        yield return new WaitForSeconds(1.5f);
    }
    IEnumerator ResetTransformAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform_level1 = false;
        transform_level2 = false;
        anime.SetBool("transform_level1", false);
        anime.SetBool("transform_level2", false);
        Debug.Log("변신 해제됨!");
    }
}

