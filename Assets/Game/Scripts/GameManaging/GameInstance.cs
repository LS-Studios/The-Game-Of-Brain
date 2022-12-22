using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;

public class GameInstance : MonoBehaviour
{
    [Serializable]
    public class InventoryValues
    {
        public List<GameObject> inventory = new List<GameObject>();

        public List<GameObject> packbackInventory = new List<GameObject>();
    }

    [Serializable]
    public class EquipmentValues
    {
        public List<EquipmentSet> equipmentSets = new List<EquipmentSet>();
        public EquipmentSet currentEquipmentSet = null;

        public bool CurrentSetContais(GameObject item)
        {
            bool isContained = false;

            for (int i = 0; i < currentEquipmentSet.setItems.Length; i++)
            {
                if (currentEquipmentSet.setItems[i].item != null && 
                    currentEquipmentSet.setItems[i].item.Equals(item))
                {
                    isContained = true;
                    break;
                }
            }

            return isContained;
        }

        public bool CurrentSetContaisPerk(ItemData.PerkData.PerkTyp perkTyp)
        {
            bool isContained = false;

            foreach (EquipmentSet.SetItem setItem in currentEquipmentSet.setItems)
            {
                if (setItem.item != null && setItem.category.Equals(ItemData.ItemCategory.Perk) &&
                    setItem.item.GetComponent<ItemInfo>().ItemData.perkData.perkTyp.Equals(perkTyp))
                {
                    isContained = true;
                    break;
                }
            }

            return isContained;
        }

        public void AddEquipmentSet()
        {
            EquipmentSet newSet = new EquipmentSet("Equipment Set " + (equipmentSets.Count + 1) + ".", new EquipmentSet.SetItem[6]);
            newSet.setItems[0].category = ItemData.ItemCategory.PrimaryWeapon;
            newSet.setItems[1].category = ItemData.ItemCategory.SecondaryWeapon;
            newSet.setItems[2].category = ItemData.ItemCategory.Grenade;
            newSet.setItems[3].category = ItemData.ItemCategory.Trap;
            newSet.setItems[4].category = ItemData.ItemCategory.Perk;
            newSet.setItems[5].category = ItemData.ItemCategory.Perk;
            equipmentSets.Add(newSet);
        }

        public void SetEquipmentSetValues(int index, GameObject item, ItemData.ItemCategory category)
        {
            currentEquipmentSet.setItems[index].item = item;
            currentEquipmentSet.setItems[index].category = category;
        }
    }

    [Serializable]
    public class MissionValues
    {
        public List<GameObject> missions = new List<GameObject>();
        public GameObject selectedMission;
        public string missionsSceneName;
    }

    [Serializable]
    public class ChalangeValues
    {
        public List<Challenge> chalanges = new List<Challenge>();

        public void CompleteCompletedChalanges()
        {
            for (int i = 0; i < chalanges.Count; i++)
            {
                if (chalanges[i].isCompleted)
                {
                    CompleteChalange(chalanges[i]);
                }
            }
        }

        public void CompleteChalange(Challenge chalange)
        {
            GameInstance.instance.playerValues.cash += chalange.cashReward;
            GameInstance.instance.playerValues.brainCells += chalange.brainCellReward;

            AddNewChalange(chalanges.IndexOf(chalange));

            chalanges.Remove(chalange);
        }

        public Challenge AddNewChalange(int index)
        {
            Challenge newChalange = new Challenge();
            
            Challenge.ChalangeTyp randomChalangeTyp = (Challenge.ChalangeTyp)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Challenge.ChalangeTyp)).Length);
            newChalange.chalangeTyp = randomChalangeTyp;

            int randomProgressToReachValue = 1;

            switch (randomChalangeTyp)
            {
                case Challenge.ChalangeTyp.Kill:
                    randomProgressToReachValue = UnityEngine.Random.Range(1, 100);

                    Enemy.EnemyTyp randomEnemyTyp = GameInstance.instance.referenceValues.allEnemys[
                                                    UnityEngine.Random.Range(0, GameInstance.instance.referenceValues.allEnemys.Count)].
                                                    GetComponent<Enemy>().enemyTyp;

                    newChalange.enemyTyp = randomEnemyTyp;

                    newChalange.description = "Kill " + randomProgressToReachValue + " " + 
                                        LSUtils.SeperateCompundString(randomEnemyTyp.ToString()) 
                                        + (randomProgressToReachValue > 1 ? "s" : "");

                    newChalange.cashReward = (int)(UnityEngine.Random.Range(2f, 3f) * randomProgressToReachValue);
                    newChalange.brainCellReward = (int)(UnityEngine.Random.Range(1f, 3f) * randomProgressToReachValue);
                    break;
                case Challenge.ChalangeTyp.Collect:
                    randomProgressToReachValue = UnityEngine.Random.Range(1, 50);

                    Collectable.CollectableTyp randomColelctableTyp = instance.referenceValues.allCollectible[
                                UnityEngine.Random.Range(0, instance.referenceValues.allCollectible.Count)].
                                GetComponent<Collectable>().collectableTyp;

                    newChalange.collectableTyp = randomColelctableTyp;

                    newChalange.description = "Collect " + randomProgressToReachValue + " " +
                                        LSUtils.SeperateCompundString(randomColelctableTyp.ToString())
                                        + (randomProgressToReachValue > 1 ? "s" : "");


                    newChalange.cashReward = (int)(UnityEngine.Random.Range(2f, 3f) * randomProgressToReachValue);
                    newChalange.brainCellReward = (int)(UnityEngine.Random.Range(1f, 3f) * randomProgressToReachValue);
                    break;
                case Challenge.ChalangeTyp.Survive:
                    randomProgressToReachValue = UnityEngine.Random.Range(1, 10);

                    newChalange.description = "Survive " + randomProgressToReachValue + " waves";

                    newChalange.cashReward = (int)(UnityEngine.Random.Range(2f, 3f) * randomProgressToReachValue);
                    newChalange.brainCellReward = (int)(UnityEngine.Random.Range(2f, 3f) * randomProgressToReachValue);
                    break;
            }

            newChalange.progressToReach = randomProgressToReachValue;

            chalanges.Insert(index, newChalange);
            return newChalange;
        }

        public void AddNewChalange()
        {
            AddNewChalange(chalanges.Count);
        }
    }

    [Serializable]
    public class PlayerValues
    {
        public Int64 cash = 0;
        public Int64 brainCells = 0;

        public List<Color> playerColors = new List<Color>();
    }

    [Serializable]
    public class ReferenceValues
    {
        public GlobalData globalData;

        public List<GameObject> allWeapons = new List<GameObject>();
        public List<GameObject> allGrenades = new List<GameObject>();
        public List<GameObject> allTraps = new List<GameObject>();
        public List<GameObject> allPerks = new List<GameObject>();

        [HideInInspector]
        public List<GameObject> allUseables = new List<GameObject>();

        public List<GameObject> allEnemys = new List<GameObject>();
        public List<GameObject> allCollectible = new List<GameObject>();
    }

    [Serializable]
    public class InGameValues
    {
        [SerializeField]
        private Int64 gamePoints = 0;
        public Int64 GamePoints {
            get { return gamePoints; }
            set {
                gamePoints = value;
            }
        }

        public enum Difficulty { Easy, Medium, Hard, Insane }
        public Difficulty difficulty;

        public List<Rarity> rarities = new List<Rarity>();

        [Serializable]
        public struct Rarity
        {
            public enum RarityTyp { Common, Rare, Epic, Legendary, MaxLevel }
            public RarityTyp rarityTyp;

            public int rarityStartLevel;
            public int rarityEndLevel;

            [ColorUsage(true, true)]
            public Color rarityColor;
        }

        public Rarity GetRarity(int level)
        {
            foreach (Rarity rarity in rarities)
            {
                if (level >= rarity.rarityStartLevel && level <= rarity.rarityEndLevel)
                {
                    return rarity;
                }
            }

            return rarities[0];
        }

        public Rarity GetRarity(Rarity.RarityTyp rarityTyp)
        {
            foreach (Rarity rarity in rarities)
            {
                if (rarityTyp == rarity.rarityTyp)
                {
                    return rarity;
                }
            }

            return rarities[0];
        }
    }

    [Serializable]
    public class DebugValues
    {
        public bool activateWaveSpawner = true;
        public bool isMobile = false;
    }


    [Serializable]
    public class SettingValues
    {
        public SettingsManager settings = new SettingsManager();
    }

    public static GameInstance instance;

    //Class references
    public InventoryValues inventoryValues;
    public EquipmentValues equipmentValues;
    public MissionValues missionValues;
    public PlayerValues playerValues;
    public ChalangeValues chalangeValues;
    public ReferenceValues referenceValues;
    public InGameValues inGameValues;
    public DebugValues debugValues;
    public SettingValues settingValues;

    //Infos
    [HideInInspector] public bool gameHasStartenAlready = false;
    [HideInInspector] public bool canDoItemAction = true;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        referenceValues.allUseables.AddRange(referenceValues.allWeapons);
        referenceValues.allUseables.AddRange(referenceValues.allGrenades);
        referenceValues.allUseables.AddRange(referenceValues.allTraps);

        if (equipmentValues.equipmentSets.Count <= 0)
            equipmentValues.AddEquipmentSet();

        equipmentValues.currentEquipmentSet = equipmentValues.equipmentSets[0];

        if (chalangeValues.chalanges.Count <= 0)
        {
            for (int i = 0; i < 4; i++)
                chalangeValues.AddNewChalange();
        }

        settingValues.settings.SetDefaultPositions();
    }

    [ContextMenu("Refresh chalanges")]
    public static void ShowEquipmentSlots()
    {
         instance.chalangeValues.chalanges.Clear();

        for (int i = 0; i < 4; i++)
            instance.chalangeValues.AddNewChalange();
    }
}
