using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Zenject;

public class DifficultiSlider : MonoBehaviour
{
    [Space]
    public Slider slider;
    public Image HandelBG;
    public TextMeshProUGUI text;

    [Space]
    public Color easyColor;
    public Color mediumColor;
    public Color hardColor;
    public Color insaneColor;

    [Space]
    public string baseText = "Schwierigkeit ...";
    public string easyName = "Easy";
    public string mediumName = "Medium";
    public string hardName = "Hard";
    public string insaneName = "Insane";

    private void Start()
    {
        slider.value = (int)GameInstance.instance.inGameValues.difficulty;
    }

    void Update()
    {
        switch (slider.value)
        {
            case 0:
                HandelBG.color = easyColor;
                text.text = baseText.Replace("...", easyName);
                text.color = easyColor;
                break;
            case 1:
                HandelBG.color = mediumColor;
                text.text = baseText.Replace("...", mediumName);
                text.color = mediumColor;
                break;
            case 2:
                HandelBG.color = hardColor;
                text.text = baseText.Replace("...", hardName);
                text.color = hardColor;
                break;
            case 3:
                HandelBG.color = insaneColor;
                text.text = baseText.Replace("...", insaneName);
                text.color = insaneColor;
                break;
        }
    }
}
