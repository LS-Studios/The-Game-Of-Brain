using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ItemSortHandler : MonoBehaviour
{
    public TMP_Dropdown sortTypSelect;
    public TMP_Dropdown sortDirectionSelect;

    public InventoryManage.SortTyp sortTyp;
    public InventoryManage.SortDirection sortDirection;

    public ItemSpawner itemSpawner;

    public bool sortEquipment = false;

    [Header("Name of list in equipmentset")]
    public string variableToSort;

    void Start()
    {
        List<TMP_Dropdown.OptionData> typOption = new List<TMP_Dropdown.OptionData>();
        foreach (string sortTyp in System.Enum.GetNames(typeof(InventoryManage.SortTyp)))
        { 
            typOption.Add(new TMP_Dropdown.OptionData(LSUtils.SeperateCompundString(sortTyp), null));
        }
        sortTypSelect.options = typOption;

        List<TMP_Dropdown.OptionData> dirOption = new List<TMP_Dropdown.OptionData>();
        foreach (string sortDir in System.Enum.GetNames(typeof(InventoryManage.SortDirection)))
        {
            dirOption.Add(new TMP_Dropdown.OptionData(LSUtils.SeperateCompundString(sortDir), null));
        }
        sortDirectionSelect.options = dirOption;
    }

    public void SortBySelectedParameters()
    {
        itemSpawner.SortByCategory(new(sortTyp, sortDirection), sortEquipment, variableToSort);
    }

    public void SetSortTyp(int val)
    {
        sortTyp = (InventoryManage.SortTyp)val;
    }

    public void SetSortDirection(int val)
    {
        sortDirection = (InventoryManage.SortDirection)val;
    }
}
