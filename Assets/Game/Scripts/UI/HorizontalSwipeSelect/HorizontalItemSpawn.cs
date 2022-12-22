using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HorizontalItemSpawn : MonoBehaviour
{
    public enum SpawnTyp { Mission, EquipmentSlot }
    public SpawnTyp spawnTyp;

    public GameObject spawnPrefab;

    [Inject]
    private MainMenueHandler menueHandler;

    private void OnEnable()
    {
        RefreshItemSpawn();
    }

    public void RefreshItemSpawn()
    {
        GameObject itemHolder = transform.GetChild(0).gameObject;

        foreach (Transform t in itemHolder.transform)
        {
            Destroy(t.gameObject);
        }

        switch (spawnTyp)
        {
            case SpawnTyp.Mission:
                foreach (GameObject g in GameInstance.instance.missionValues.missions)
                {
                    GameObject missionInst = Instantiate(g, itemHolder.transform);
                }
                break;
            case SpawnTyp.EquipmentSlot:
                foreach (EquipmentSet g in GameInstance.instance.equipmentValues.equipmentSets)
                {
                    GameObject equipmentInst = Instantiate(spawnPrefab, itemHolder.transform);
                    GoToButton goToButton = equipmentInst.GetComponentInChildren<GoToButton>();
                    equipmentInst.GetComponent<EquipmentSetObject>().equipmentSet = g;
                }
                break;
        }
    }
}
