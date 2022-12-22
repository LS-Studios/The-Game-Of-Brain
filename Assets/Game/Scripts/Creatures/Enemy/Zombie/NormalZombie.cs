public class NormalZombie : ZombieEnemy
{
    protected override void Start()
    {
        switch (GameInstance.instance.inGameValues.difficulty)
        {
            case GameInstance.InGameValues.Difficulty.Easy:
                damage = 3;
                attackRate = 0.35f;
                healthComponent.startHealth = 40;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 1.5f;
                break;

            case GameInstance.InGameValues.Difficulty.Medium:
                damage = 3.2f;
                attackRate = 0.325f;
                healthComponent.startHealth = 50;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 1.5f;
                break;

            case GameInstance.InGameValues.Difficulty.Hard:
                damage = 4f;
                attackRate = 0.25f;
                healthComponent.startHealth = 80;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 1.8f;
                break;

            case GameInstance.InGameValues.Difficulty.Insane:
                damage = 6;
                attackRate = 0.2f;
                healthComponent.startHealth = 100;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 2.5f;
                break;
        }

        base.Start();
    }
}
