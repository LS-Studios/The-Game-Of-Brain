using UnityEngine;
using Zenject;

public class GameSceenSetUpInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameManager>().FromInstance(FindObjectOfType<GameManager>()).AsSingle();
        Container.Bind<GameMenueHandler>().FromInstance(FindObjectOfType<GameMenueHandler>()).AsSingle();
    }
}