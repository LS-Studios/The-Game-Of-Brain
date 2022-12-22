using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GoToButton : MonoBehaviour
{
    [Inject]
    private MainMenueHandler menueHandler;

    public Vector2 direction = new Vector2(0, 0);

    [ContextMenu("Go to")]
    public void GoTo()
    {
        menueHandler.GoTo(direction);
    }

    public void GoBackTo()
    {
        menueHandler.GoBackTo();
    }
}
