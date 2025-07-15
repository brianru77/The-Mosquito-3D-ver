using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("무기 소켓 위치 (손뼈의 자식 오브젝트)")]
    public Transform weaponSocket;
    [Header("장착할 무기 프리팹들")]
    public List<GameObject> weaponPrefabs;
    private int currentWeaponIndex = -1;
    private GameObject currentWeapon;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);
    }
    public void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weaponPrefabs.Count)
            return;

        //같은 무기를 다시 선택하면 해제
        if (currentWeaponIndex == index)
        {
            UnequipWeapon();
            currentWeaponIndex = -1;
            return;
        }

        //장착
        UnequipWeapon();
        EquipWeapon(weaponPrefabs[index]);
        currentWeaponIndex = index;
    }
    //무기 장착
    public void EquipWeapon(GameObject prefab)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(prefab, weaponSocket);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
    }
    //무기 해제
    public void UnequipWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }
    }
}