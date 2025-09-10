using UnityEngine;

public class ItemIterator : MonoBehaviour
{
    public int selectedItem = 0;
    public int selectedWeapon = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectItem();
        selectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousItem = selectedItem;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedItem = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedItem = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedItem = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedItem = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedItem = 4;
        }

        if (selectedItem != previousItem)
            selectItem();
    }
    public void selectItem()
    {
        int i = 0;
        foreach (Transform item in transform)
        {
            if (i == selectedItem)
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
            i++;
        }
    }
    public void selectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

}
