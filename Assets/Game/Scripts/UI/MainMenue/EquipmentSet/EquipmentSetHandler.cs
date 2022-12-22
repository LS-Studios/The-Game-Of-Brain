using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
using UnityEngine.UI;
using System;

public class EquipmentSetHandler : MonoBehaviour
{
    public List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

    public EquipmentSet currentEquipmentSet;

    public ItemSpawner itemSpawner;

    public TMP_Dropdown dropdown;

    public TMP_InputField inputField;

    public RectTransform renameText;

    private void Awake()
    {
        inputField.characterLimit = 18;

        itemSpawner.transform.parent.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

        UpdateEquipmentSets();
    }

    private void Update()
    {
        renameText.anchoredPosition = new Vector2(-7.5f, renameText.anchoredPosition.y);
        renameText.sizeDelta = new Vector2(-15f, renameText.sizeDelta.y);

        RectTransform select = renameText.parent.GetChild(0).GetComponent<RectTransform>();
        select.anchoredPosition = new Vector2(-7.5f, renameText.anchoredPosition.y);
        select.sizeDelta = new Vector2(-15f, renameText.sizeDelta.y);
    }

    private void OnEnable()
    {
        UpdateEquipmentSets();
    }

    public void SetInputFieldText()
    {
        inputField.text = GameInstance.instance.equipmentValues.currentEquipmentSet.setName;
    }

    public void UpdateEquipmentSets()
    {
        options.Clear();
        
        foreach (EquipmentSet equipmentSet in GameInstance.instance.equipmentValues.equipmentSets)
        {
            options.Add(new TMP_Dropdown.OptionData(equipmentSet.setName, null));
        }

        dropdown.options = options;
    }

    public void ChangeNewEquipentSet(int val)
    {
        GameInstance.instance.equipmentValues.currentEquipmentSet = GameInstance.instance.equipmentValues.equipmentSets[val];
        itemSpawner.RefreshInventory();
    }

    public void RenameEquipentSet(string newName)
    {
        GameInstance.instance.equipmentValues.currentEquipmentSet.setName = newName;
        UpdateEquipmentSets();
    }
}
