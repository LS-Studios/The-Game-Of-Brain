using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class TheGameOfBrainScreenHandler : MonoBehaviour
{
    public TextMeshProUGUI missionPreviewText;
    public Image missionPreviewIcon;

    public EquipmentSetObject equipmentSetObject;

    void Update()
    {
        if (GameInstance.instance.missionValues.selectedMission != null)
        {
            missionPreviewText.text = GameInstance.instance.missionValues.selectedMission.GetComponent<MissionHandler>().missionName;
            missionPreviewIcon.sprite = GameInstance.instance.missionValues.selectedMission.GetComponent<MissionHandler>().missionMapIcon;
        }
        else
        {
            missionPreviewText.text = GameInstance.instance.missionValues.missions[0].GetComponent<MissionHandler>().missionName;
            missionPreviewIcon.sprite = GameInstance.instance.missionValues.missions[0].GetComponent<MissionHandler>().missionMapIcon;
        }

        equipmentSetObject.RefreshEquipmentObject();
    }

    public void ChangeDifficulty(float index)
    {
        GameInstance.instance.inGameValues.difficulty = (GameInstance.InGameValues.Difficulty)index;
    }
}
