using UnityEngine;
using Zenject;

public class MainSceenSetUpInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<MainMenueHandler>().FromInstance(FindObjectOfType<MainMenueHandler>()).AsSingle();
        Container.Bind<CurrentItemHandler>().FromInstance(FindObjectOfType<CurrentItemHandler>()).AsSingle();
    }
}