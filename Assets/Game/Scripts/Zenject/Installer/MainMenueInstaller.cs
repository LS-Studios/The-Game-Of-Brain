using UnityEngine;
using Zenject;

public class MainMenueInstaller : MonoInstaller
{
    public EquipmentSetHandler equipmentSetHandler;
    public override void InstallBindings()
    {
        Container.Bind<EquipmentSetHandler>().FromInstance(equipmentSetHandler).AsSingle();
        Container.Bind<MainMenueHandler>().FromComponentOn(gameObject).AsSingle();
    }
}