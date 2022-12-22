using System;
using System.Collections;
using UnityEngine;
public class SettingsManager
{
    public UIPlacement movementJoyStickPlacement;
    public UIPlacement aimJoyStickPlacement;
    public UIPlacement shootBtnPlacement;
    public UIPlacement reloadBtnPlacement;
    public UIPlacement punchBtnPlacement;
    public UIPlacement runBtnPlacement;
    public UIPlacement grenadeBtnPlacement;

    public class UIPlacement
    {
        public Vector2 pos;
        public ValueTuple<Vector2, Vector2> anchor;
        public UIPlacement(Vector2 pos, ValueTuple<Vector2, Vector2> anchor)
        {
            this.pos = pos;
            this.anchor = anchor;
        }
    }

    public void SetDefaultPositions()
    {
        movementJoyStickPlacement = new UIPlacement(new Vector2(260, 217), new(new Vector2(0, 0), new Vector2(0, 0)));
        aimJoyStickPlacement = new UIPlacement(new Vector2(-260, 217), new(new Vector2(1, 0), new Vector2(1, 0)));
        shootBtnPlacement = new UIPlacement(new Vector2(195, 545), new(new Vector2(0, 0), new Vector2(0, 0)));
        reloadBtnPlacement = new UIPlacement(new Vector2(-195, 545), new(new Vector2(1, 0), new Vector2(1, 0)));
        punchBtnPlacement = new UIPlacement(new Vector2(428, 470), new(new Vector2(0, 0), new Vector2(0, 0)));
        runBtnPlacement = new UIPlacement(new Vector2(-428, 470), new(new Vector2(1, 0), new Vector2(1, 0)));
        grenadeBtnPlacement = new UIPlacement(new Vector2(-566, 308), new(new Vector2(1, 0), new Vector2(1, 0)));
    }

    public void SetSwitchedPosition()
    {
        if (movementJoyStickPlacement == null)
            SetDefaultPositions();

        if (movementJoyStickPlacement.anchor.Item1.x == 0)
        {
            movementJoyStickPlacement.anchor.Item1 = new Vector2(1, 0);
            movementJoyStickPlacement.anchor.Item2 = new Vector2(1, 0);
        }
        else if (movementJoyStickPlacement.anchor.Item1.x == 1)
        {
            movementJoyStickPlacement.anchor.Item1 = new Vector2(0, 0);
            movementJoyStickPlacement.anchor.Item2 = new Vector2(0, 0);
        }
        movementJoyStickPlacement.pos.x *= -1;


        if (aimJoyStickPlacement.anchor.Item1.x == 0)
        {
            aimJoyStickPlacement.anchor.Item1 = new Vector2(1, 0);
            aimJoyStickPlacement.anchor.Item2 = new Vector2(1, 0);
        }
        else if (aimJoyStickPlacement.anchor.Item1.x == 1)
        {
            aimJoyStickPlacement.anchor.Item1 = new Vector2(0, 0);
            aimJoyStickPlacement.anchor.Item2 = new Vector2(0, 0);
        }
        aimJoyStickPlacement.pos.x *= -1;


        if (shootBtnPlacement.anchor.Item1.x == 0)
        {
            shootBtnPlacement.anchor.Item1 = new Vector2(1, 0);
            shootBtnPlacement.anchor.Item2 = new Vector2(1, 0);
        }
        else if (shootBtnPlacement.anchor.Item1.x == 1)
        {
            shootBtnPlacement.anchor.Item1 = new Vector2(0, 0);
            shootBtnPlacement.anchor.Item2 = new Vector2(0, 0);
        }
        shootBtnPlacement.pos.x *= -1;


        if (reloadBtnPlacement.anchor.Item1.x == 0)
        {
            reloadBtnPlacement.anchor.Item1 = new Vector2(1, 0);
            reloadBtnPlacement.anchor.Item2 = new Vector2(1, 0);
        }
        else if (reloadBtnPlacement.anchor.Item1.x == 1)
        {
            reloadBtnPlacement.anchor.Item1 = new Vector2(0, 0);
            reloadBtnPlacement.anchor.Item2 = new Vector2(0, 0);
        }
        reloadBtnPlacement.pos.x *= -1;


        if (punchBtnPlacement.anchor.Item1.x == 0)
        {
            punchBtnPlacement.anchor.Item1 = new Vector2(1, 0);
            punchBtnPlacement.anchor.Item2 = new Vector2(1, 0);
        }
        else if (punchBtnPlacement.anchor.Item1.x == 1)
        {
            punchBtnPlacement.anchor.Item1 = new Vector2(0, 0);
            punchBtnPlacement.anchor.Item2 = new Vector2(0, 0);
        }
        punchBtnPlacement.pos.x *= -1;


        if (runBtnPlacement.anchor.Item1.x == 0)
        {
            runBtnPlacement.anchor.Item1 = new Vector2(1, 0);
            runBtnPlacement.anchor.Item2 = new Vector2(1, 0);
        }
        else if (runBtnPlacement.anchor.Item1.x == 1)
        {
            runBtnPlacement.anchor.Item1 = new Vector2(0, 0);
            runBtnPlacement.anchor.Item2 = new Vector2(0, 0);
        }
        runBtnPlacement.pos.x *= -1;


        if (grenadeBtnPlacement.anchor.Item1.x == 0)
        {
            grenadeBtnPlacement.anchor.Item1 = new Vector2(1, 0);
            grenadeBtnPlacement.anchor.Item2 = new Vector2(1, 0);
        }
        else if (grenadeBtnPlacement.anchor.Item1.x == 1)
        {
            grenadeBtnPlacement.anchor.Item1 = new Vector2(0, 0);
            grenadeBtnPlacement.anchor.Item2 = new Vector2(0, 0);
        }
        grenadeBtnPlacement.pos.x *= -1;
    }
}