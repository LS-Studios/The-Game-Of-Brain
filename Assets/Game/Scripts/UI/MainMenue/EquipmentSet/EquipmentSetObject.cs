using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Zenject;

public class EquipmentSetObject : MonoBehaviour
{
    public EquipmentSet equipmentSet;

    public ItemSpawner itemSpawner;

    public Button button;

    public TextMeshProUGUI equipmentNameText;

    public bool isCurrentSet;

    void Start()
    {
        gameObject.AddComponent<ZenAutoInjecter>().ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;

        RefreshEquipmentObject();
    }

    private void OnEnable()
    {
        if (isCurrentSet)
            equipmentSet = GameInstance.instance.equipmentValues.currentEquipmentSet;

        RefreshEquipmentObject();
    }

    private void Update()
    {
        if (isCurrentSet)
        {
            equipmentSet = GameInstance.instance.equipmentValues.currentEquipmentSet;

            RefreshEquipmentObject();
        }
    }

    public void RefreshEquipmentObject()
    {
        if (equipmentSet != null)
        {
            itemSpawner.ResetItemsToSpawn();

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    itemSpawner.information.itemsToSpawn.Add(
                        GameInstance.instance.equipmentValues.currentEquipmentSet.setItems[i].item
                        );
                } catch (Exception)
                {

                }
            }

            itemSpawner.RefreshInventory();

            equipmentNameText.text = equipmentSet.setName;
        }
    }

    public void ChangeEquipmentSet()
    {
        GameInstance.instance.equipmentValues.currentEquipmentSet = equipmentSet;
    }
}
