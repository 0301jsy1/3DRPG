using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShortcut : MonoBehaviour
{
    [SerializeField]
    GameObject inventory;

    [SerializeField]
    GameObject Skills;

    void Awake()
    {
        inventory.SetActive(false);
        Skills.SetActive(false);
    }
    public void On_Inventory()
    {
        inventory.SetActive(!inventory.activeSelf);
    }
    public void On_Skills()
    {
       Skills.SetActive(!Skills.activeSelf);
    }
}

