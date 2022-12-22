using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BGManage : MonoBehaviour
{
    public enum Background { TheGameOfBrain, Inventory, Chalange, Shop, Controlls, Credit, Info }
    public Background background = Background.Inventory;

    public enum Pos { Middle, Left, SubLeft, Right, SubRight, Down, Start, Credit }

    public BackgroundImage[] backgroundImages;
    public BackgroundObject[] backgroundObjects;

    private List<Tuple<Pos, List<GameObject>>> backgroundPositions = new List<Tuple<Pos, List<GameObject>>>();

    [Serializable]
    public struct BackgroundImage
    {
        public Image image;

        public Pos pos;

        public bool dontAssingBackgroundImage;
    }

    [Serializable]
    public struct BackgroundObject
    {
        public Background background;
        public Sprite sprite;
        public BackgroundButton[] backgroundButtons;

        [Serializable]
        public struct BackgroundButton
        {
            public GameObject buttonsObject;
            public Pos pos;
        }
    }

    private void Start()
    {
        EditBackground();

        SetScreenPos(new Vector2(0, 0));
    }

    public void SetBackground(int newBackground)
    {
        background = (Background)newBackground;

        UpdateBackground();
    }

    #region Edit methods
    [ContextMenu("Edit Start Position")]
    private void GameStartPos()
    {
        EditBackground();

        SetScreenPos(new Vector2(0, 0));
    }

    [ContextMenu("Edit Credit Position")]
    private void GameCreditPos()
    {
        EditBackground();

        SetScreenPos(new Vector2(-1, 0));
    }

    [ContextMenu("Edit Middle Position")]
    private void EditBackgroundPos()
    {
        EditBackground();

        SetScreenPos(new Vector2(0, -1));
    }

    [ContextMenu("Edit Left Screen Position")]
    private void EditLeftBackgroundPos()
    {
        EditBackground();

        SetScreenPos(new Vector2(-1, -1));
    }

    [ContextMenu("Edit Right Screen Position")]
    private void EditRightBackgroundPos()
    {
        EditBackground();

        SetScreenPos(new Vector2(1, -1));
    }

    [ContextMenu("Edit Down Screen Position")]
    private void EditDownBackgroundPos()
    {
        EditBackground();

        SetScreenPos(new Vector2(0, -2));
    }

    private void EditBackground()
    {
        CreatePostionList();

        UpdateBackground();

        //GetComponentInChildren<ScreenSwipe>().SetScreenPositionsAndContentWidth();
    }
    #endregion

    private void CreatePostionList()
    {
        foreach (Pos pos in Enum.GetValues(typeof(Pos)))
        {
            List<GameObject> objectsToPos = new List<GameObject>();

            foreach (BackgroundImage bgImg in backgroundImages)
            {
                if (bgImg.pos == pos)
                {
                    objectsToPos.Add(bgImg.image.gameObject);
                }
            }

            foreach (BackgroundObject bgObj in backgroundObjects)
            {
                foreach (BackgroundObject.BackgroundButton bgBtn in bgObj.backgroundButtons)
                {
                    if (bgBtn.pos == pos)
                    {
                        objectsToPos.Add(bgBtn.buttonsObject);
                    }
                }
            }

            backgroundPositions.Add(new Tuple<Pos, List<GameObject>>(pos, objectsToPos));
        }
    }

    private void UpdateBackground()
    {
        //Set right sprite
        Sprite currentSprite = Array.Find(backgroundObjects, bg => bg.background == background).sprite;

        foreach (BackgroundImage img in backgroundImages)
        {
            if (!img.dontAssingBackgroundImage)
                img.image.sprite = currentSprite;
        }

        ShowSceenContent();
    }

    private void SetScreenPos(Vector2 viewPos)
    {
        RectTransform screenRectTransform = GetComponent<RectTransform>();
        float with = screenRectTransform.rect.width;
        float height = screenRectTransform.rect.height;

        Vector2 BackgroundPos(Vector2 bgPos)
        {
            return new Vector2((with * bgPos.x) - (with * viewPos.x), (height * bgPos.y) - (height * viewPos.y));
        }

        Vector2 BackgroundSubPos(Vector2 bgPos)
        {
            return new Vector2((with * bgPos.x), 0);
        }

        foreach (Tuple<Pos, List<GameObject>> bgPos in backgroundPositions)
        {
            switch(bgPos.Item1)
            {
                case Pos.Left:
                    bgPos.Item2.ForEach(bgObj => 
                    bgObj.GetComponent<RectTransform>().anchoredPosition = 
                    BackgroundPos(new Vector2(-1, -1)));
                    break;

                case Pos.SubLeft:
                    bgPos.Item2.ForEach(bgObj =>
                    bgObj.GetComponent<RectTransform>().anchoredPosition =
                    BackgroundSubPos(new Vector2(-1, -1)));
                    break;

                case Pos.Middle:
                    bgPos.Item2.ForEach(bgObj =>
                    bgObj.GetComponent<RectTransform>().anchoredPosition =
                    BackgroundPos(new Vector2(0, -1)));
                    break;

                case Pos.Right:
                    bgPos.Item2.ForEach(bgObj =>
                    bgObj.GetComponent<RectTransform>().anchoredPosition =
                    BackgroundPos(new Vector2(1, -1)));
                    break;

                case Pos.SubRight:
                    bgPos.Item2.ForEach(bgObj =>
                    bgObj.GetComponent<RectTransform>().anchoredPosition =
                    BackgroundSubPos(new Vector2(1, -1)));
                    break;

                case Pos.Down:
                    bgPos.Item2.ForEach(bgObj =>
                    bgObj.GetComponent<RectTransform>().anchoredPosition =
                    BackgroundPos(new Vector2(0, -2)));
                    break;

                case Pos.Start:
                    bgPos.Item2.ForEach(bgObj =>
                    bgObj.GetComponent<RectTransform>().anchoredPosition =
                    BackgroundPos(new Vector2(0, 0)));
                    break;

                case Pos.Credit:
                    bgPos.Item2.ForEach(bgObj =>
                    bgObj.GetComponent<RectTransform>().anchoredPosition =
                    BackgroundPos(new Vector2(-1, 0)));
                    break;
            }
        }
    }

    private void ShowSceenContent()
    {
        foreach (BackgroundObject bgObj in backgroundObjects)
        {
            if (bgObj.background != Background.Credit && bgObj.background != Background.Info)
            {
                foreach (BackgroundObject.BackgroundButton btn in bgObj.backgroundButtons)
                {
                    btn.buttonsObject.SetActive(false);
                }
            }
        }

        foreach (BackgroundObject bgObj in backgroundObjects)
        {
            if (bgObj.background != Background.Credit && bgObj.background != Background.Info)
            {
                if (bgObj.background == background)
                {
                    foreach (BackgroundObject.BackgroundButton btn in bgObj.backgroundButtons)
                    {
                        btn.buttonsObject.SetActive(true);
                    }
                }
            }
        }
    }
}
