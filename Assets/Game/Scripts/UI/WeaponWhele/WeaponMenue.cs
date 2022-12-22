using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponMenue : MonoBehaviour
{
    public GameManager gameManager;

    private Vector2 normolizedMousePosition;
    private float currentAngle;
    private int selection;
    private int previosSelection;

    public GameObject[] menueItems;

    private MenueItemScript menueItemSelection;
    private MenueItemScript previousMenueItemSelection;

    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        normolizedMousePosition = new Vector2(mousePosition.x - Screen.width/2, mousePosition.y - Screen.height/2);
        currentAngle = Mathf.Atan2(normolizedMousePosition.y, normolizedMousePosition.x) * Mathf.Rad2Deg;

        currentAngle = (currentAngle + 360) % 360;

        selection = (int) currentAngle / 45;

        if (selection != previosSelection)
        {

            previousMenueItemSelection = menueItems[previosSelection].GetComponent<MenueItemScript>();
            previousMenueItemSelection.Deselect();

            previosSelection = selection;

            menueItemSelection = menueItems[selection].GetComponent<MenueItemScript>();
            menueItemSelection.Select();
        }
    }
}
