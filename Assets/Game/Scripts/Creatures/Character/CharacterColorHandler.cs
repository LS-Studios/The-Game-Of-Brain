using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterColorHandler : MonoBehaviour
{
    public enum ColorHandelTyp { Sprite, Image }
    public ColorHandelTyp colorHandelTyp = ColorHandelTyp.Sprite;

    public bool useUpdate = false;

    public bool usePlayerColors = false;

    public List<ColorGroup> colorGroups = new List<ColorGroup>();

    [Serializable]
    public struct ColorGroup
    {
        public string name;
        public List<GameObject> renderObjects;
        public List<Color> colors;
    }

    private void Start()
    {
        UpdateCharacterColors();
    }

    private void Update()
    {
        if (useUpdate)
            UpdateCharacterColors();
    }

    void UpdateCharacterColors()
    {
        if (usePlayerColors)
        {
            for (int i = 0; i < colorGroups.Count; i++)
            {
                if (colorHandelTyp == ColorHandelTyp.Sprite)
                {
                    foreach (GameObject rendererObject in colorGroups[i].renderObjects)
                    {
                        rendererObject.GetComponent<SpriteRenderer>().color = GameInstance.instance.playerValues.playerColors[i];
                    }
                } else
                {
                    foreach (GameObject rendererObject in colorGroups[i].renderObjects)
                    {
                        rendererObject.GetComponent<Image>().color = GameInstance.instance.playerValues.playerColors[i];
                    }
                }
            }
        }
        else
        {
            if (colorHandelTyp == ColorHandelTyp.Sprite)
            {
                foreach (ColorGroup colorGroup in colorGroups)
                {
                    Color randomColor = colorGroup.colors[UnityEngine.Random.Range(0, colorGroup.colors.Count)];
                    colorGroup.renderObjects.ForEach(rendererObject => rendererObject.GetComponent<SpriteRenderer>().color = randomColor);
                }
            }
            else
            {
                foreach (ColorGroup colorGroup in colorGroups)
                {
                    Color randomColor = colorGroup.colors[UnityEngine.Random.Range(0, colorGroup.colors.Count)];
                    colorGroup.renderObjects.ForEach(rendererObject => rendererObject.GetComponent<Image>().color = randomColor);
                }
            }
        }
    }
}
