using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class MissionHandler : MonoBehaviour
{
    public string missionName;
    public string missionDiscription;
    public Sprite missionMapIcon;

    public Color textColor;

    private TextMeshProUGUI missionNameText;
    private TextMeshProUGUI missionDesriptionText;
    private Image missionImage;

    void Start()
    {
        gameObject.AddComponent<ZenAutoInjecter>().ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;

        missionNameText = Array.Find(GetComponentsInChildren<TextMeshProUGUI>(), text => text.name.Contains("Name"));
        missionDesriptionText = Array.Find(GetComponentsInChildren<TextMeshProUGUI>(), text => text.name.Contains("Discription"));
        missionImage = Array.Find(GetComponentsInChildren<Image>(), text => text.name.Contains("Map"));

        missionNameText.text = missionName;
        missionNameText.color = textColor;

        missionDesriptionText.text = missionDiscription;
        missionDesriptionText.color = textColor;

        missionImage.sprite = missionMapIcon;

        GetComponentInChildren<Button>().onClick.AddListener(ClickButton);
    }

    private void ClickButton()
    {
        GameInstance.instance.missionValues.missionsSceneName = missionName;

        GameInstance.instance.missionValues.selectedMission = gameObject;

        GetComponentInChildren<GoToButton>().GoBackTo();
    }
}
