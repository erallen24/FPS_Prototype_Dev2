using UnityEngine;

public class ItemIterator : MonoBehaviour
{
    public int selectedItem = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectItem();
    }

    // Update is called once per frame
    void Update()
    {
        int previousItem = selectedItem;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedItem >= transform.childCount - 1)
                selectedItem = 0;
            else
                selectedItem++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedItem <= 0)
                selectedItem = transform.childCount - 1;
            else
                selectedItem--;
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

}
