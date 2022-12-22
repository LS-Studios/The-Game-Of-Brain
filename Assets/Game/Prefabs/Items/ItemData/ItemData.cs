using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new Itemdata", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;

    public float damage;

    public int currentAmmount = 1;
    public int maxAmmount = 1;

    public bool canGetLeveled = true;

    public int currentLevel;

    public int maxLevel = 20;

    public List<LevelImprovement> levelImprovements = new List<LevelImprovement>();

    public List<UpgradeImprovement> upgradeImprovements = new List<UpgradeImprovement>();

    public int gamePointPrice = 5;
    public int cashPrice = 0;
    public int braincellPrice = 0;

    public int UpgradePrice 
    { 
        get 
        {
            return Mathf.RoundToInt((
                ((currentLevel * gamePointPrice + ((int)Mathf.Pow(currentLevel, 2))) +
                gamePointPrice)) * upgradeMultiplier);
        } 
    }

    private float upgradeMultiplier = 1;

    public DataTyp dataTyp = DataTyp.Weapondata;
    public enum DataTyp { Weapondata, Grenadedata, TrapData, PerkData, ExtraData }

    public enum ItemCategory { PrimaryWeapon, SecondaryWeapon, Grenade, Trap, Perk, Health, Ammo }
    public ItemCategory itemCategory;

    public WeaponData weaponData;

    public GrenadeData grenadeData;

    public LMGData lMGData;

    public ExtraData extraData;

    public PerkData perkData;

    public GeneralData generalData;

    public SlotData slotData;

    public GlobalData globalData;

    [Serializable]
    public struct UpgradeImprovement
    {
        public string upgradeName;

        public ChangeVariable changeVariable;
    }

    [Serializable]
    public struct LevelImprovement
    {
        public string name;
        public int level;

        public ChangeVariable changeVariable;
    }

    [Serializable]
    public struct ChangeVariable
    {
        public bool changeDataClass;

        public enum DataClass { WeapondataClass, GrenadedataClass, TrapdataClass,
                                ExtradataClass, PerkdataClass, GeneraldataClass,
                                SlotdataClass }

        [ConditionalHide("changeDataClass")]
        public DataClass dataTyp;

        [ConditionalHide("changeDataClass")]
        public ItemData dataClass;

        [ConditionalHide("changeDataClass", false)]
        public SlotData.Stet.StetValues variablesToChange;

        [ConditionalHide("changeDataClass", false)]
        public ChangeValues changeValues;

        [Serializable]
        public struct ChangeValues
        {
            public string valToUpgradeVar1;
            public string valToUpgradeVar2;
        }
    }

    private void Awake()
    {
        weaponData.currentAmmo = weaponData.maxAmmo;
        weaponData.currentClipAmmo = weaponData.maxClipAmmo;

        if (currentLevel > 0)
        {
            for (int i = 1; i < currentLevel; i++)
            {
                UpgradeItem();
            }
        }

        if (GameInstance.instance != null)
        {
            globalData = GameInstance.instance.referenceValues.globalData;

            switch (GameInstance.instance.inGameValues.difficulty)
            {
                case GameInstance.InGameValues.Difficulty.Easy:
                    break;

                case GameInstance.InGameValues.Difficulty.Medium:
                    gamePointPrice = Mathf.RoundToInt(gamePointPrice * 1.1f);
                    upgradeMultiplier = 1.1f;
                    break;

                case GameInstance.InGameValues.Difficulty.Hard:
                    gamePointPrice = Mathf.RoundToInt(gamePointPrice * 1.2f);
                    upgradeMultiplier = 1.2f;
                    break;

                case GameInstance.InGameValues.Difficulty.Insane:
                    gamePointPrice = Mathf.RoundToInt(gamePointPrice * 1.3f);
                    upgradeMultiplier = 1.3f;
                    break;
            }
        }
    }

    //Returns the itemDataType and the field as well as the field value as a turple
    ValueTuple<object, FieldInfo, object> GetItemDataVariablePropertys(string variableName)
    {
        List<object> itemDataTypes = new List<object>();
        itemDataTypes.Add(this);
        itemDataTypes.Add(weaponData);
        itemDataTypes.Add(grenadeData);
        itemDataTypes.Add(lMGData);
        itemDataTypes.Add(generalData);
        itemDataTypes.Add(slotData);

        return LSUtils.GetClassFieldProperties(itemDataTypes, variableName);
    }

    #region Stets
    //Creates a string with the given stet data 
    public string CreateStetsString()
    {
        string stets = "";

        foreach (SlotData.Stet stet in slotData.stets)
        {
            if (stets != "")
                stets += "\n";

            object value1 = GetItemDataVariablePropertys(stet.stetValues.variable1Name).Item3;

            string appending = stet.stetValues.appending;

            object value2 = GetItemDataVariablePropertys(stet.stetValues.variable2Name).Item3;

            if (value1 == null)
                stets += stet.name + ":                              " + appending;
            else
                stets += stet.name + ":                              " + value1 + appending + value2;
        }

        return stets;
    }

    //Creates a string with the given stet data and the upgrade preview
    public string CreateUpgradeStetsString()
    {
        string stets = "";

        foreach (UpgradeImprovement upgrade in upgradeImprovements)
        {
            if (stets != "")
                stets += "\n";

            var value1 = GetItemDataVariablePropertys(upgrade.changeVariable.variablesToChange.variable1Name);
            var appending = upgrade.changeVariable.variablesToChange.appending;
            var value2 = GetItemDataVariablePropertys(upgrade.changeVariable.variablesToChange.variable2Name);

            bool hasLeftSide = false;
            bool hasRightSide = false;

            string leftSideUpgradePreview = "";
            string rightSideUpgradePreview = "";

            stets += upgrade.upgradeName + ": \n" + value1.Item3 + appending + value2.Item3;

            //Two variable stet
            if (value1.Item1 != null && value2.Item1 != null)
            {
                if (upgrade.changeVariable.changeValues.valToUpgradeVar1 != "")
                {
                    if (value1.Item3.GetType() == typeof(int))
                        leftSideUpgradePreview += " -> " + ((int)value1.Item3 + int.Parse(upgrade.changeVariable.changeValues.valToUpgradeVar1)) + appending;
                    else if (value1.Item3.GetType() == typeof(float))
                        leftSideUpgradePreview += " -> " + ((float)value1.Item3 + float.Parse(upgrade.changeVariable.changeValues.valToUpgradeVar1)) + appending;
                    else
                        leftSideUpgradePreview += " -> " + upgrade.changeVariable.changeValues.valToUpgradeVar1 + appending;

                    hasLeftSide = true;
                }

                if (upgrade.changeVariable.changeValues.valToUpgradeVar2 != "")
                {
                    if (value1.Item3.GetType() == typeof(int))
                        rightSideUpgradePreview += ((int)value2.Item3 + int.Parse(upgrade.changeVariable.changeValues.valToUpgradeVar2));
                    else if (value1.Item3.GetType() == typeof(float))
                        rightSideUpgradePreview += ((float)value2.Item3 + float.Parse(upgrade.changeVariable.changeValues.valToUpgradeVar2));
                    else
                        rightSideUpgradePreview += upgrade.changeVariable.changeValues.valToUpgradeVar2;

                    hasRightSide = true;
                }
            }
            //Just the first value
            else
            {
                if (upgrade.changeVariable.changeValues.valToUpgradeVar1 != "")
                {
                    if (value1.Item3.GetType() == typeof(int))
                        leftSideUpgradePreview += " -> " + ((int)value1.Item3 + int.Parse(upgrade.changeVariable.changeValues.valToUpgradeVar1)) + appending;
                    else if (value1.Item3.GetType() == typeof(float))
                        leftSideUpgradePreview += " -> " + ((float)value1.Item3 + float.Parse(upgrade.changeVariable.changeValues.valToUpgradeVar1)) + appending;
                    else
                        leftSideUpgradePreview += " -> " + upgrade.changeVariable.changeValues.valToUpgradeVar1 + appending;
                }
            }

            //Just left side get upgradet
            if (hasLeftSide && !hasRightSide)
            {
                rightSideUpgradePreview += value2.Item3;
            }
            //Just right side gets upgradet
            else if (!hasLeftSide && hasRightSide)
            {
                leftSideUpgradePreview += " -> " + value1.Item3 + appending;
            }

            stets += leftSideUpgradePreview + rightSideUpgradePreview;
        }

        foreach (LevelImprovement levelImprove in levelImprovements)
        {
            if (!levelImprove.changeVariable.changeDataClass &&
                levelImprove.level == currentLevel + 1)
            {
                if (stets != "")
                    stets += "\n";

                var value1 = GetItemDataVariablePropertys(levelImprove.changeVariable.variablesToChange.variable1Name);
                var appending = levelImprove.changeVariable.variablesToChange.appending;
                var value2 = GetItemDataVariablePropertys(levelImprove.changeVariable.variablesToChange.variable2Name);

                bool hasLeftSide = false;
                bool hasRightSide = false;

                string leftSideUpgradePreview = "";
                string rightSideUpgradePreview = "";

                stets += levelImprove.name + ": \n" + value1.Item3 + appending + value2.Item3;

                //Two variable stet
                if (value1.Item1 != null && value2.Item1 != null)
                {
                    if (levelImprove.changeVariable.changeValues.valToUpgradeVar1 != "")
                    {
                        if (value1.Item3.GetType() == typeof(int))
                            leftSideUpgradePreview += " -> " + ((int)value1.Item3 + int.Parse(levelImprove.changeVariable.changeValues.valToUpgradeVar1)) + appending;
                        else if (value1.Item3.GetType() == typeof(float))
                            leftSideUpgradePreview += " -> " + ((float)value1.Item3 + float.Parse(levelImprove.changeVariable.changeValues.valToUpgradeVar1)) + appending;
                        else
                            leftSideUpgradePreview += " -> " + levelImprove.changeVariable.changeValues.valToUpgradeVar1 + appending;

                        hasLeftSide = true;
                    }

                    if (levelImprove.changeVariable.changeValues.valToUpgradeVar2 != "")
                    {
                        if (value1.Item3.GetType() == typeof(int))
                            rightSideUpgradePreview += ((int)value2.Item3 + int.Parse(levelImprove.changeVariable.changeValues.valToUpgradeVar2));
                        else if (value1.Item3.GetType() == typeof(float))
                            rightSideUpgradePreview += ((float)value2.Item3 + float.Parse(levelImprove.changeVariable.changeValues.valToUpgradeVar2));
                        else
                            rightSideUpgradePreview += levelImprove.changeVariable.changeValues.valToUpgradeVar2;

                        hasRightSide = true;
                    }
                }
                //Just the first value
                else
                {
                    if (levelImprove.changeVariable.changeValues.valToUpgradeVar1 != "")
                    {
                        if (value1.Item3.GetType() == typeof(int))
                            leftSideUpgradePreview += " -> " + ((int)value1.Item3 + int.Parse(levelImprove.changeVariable.changeValues.valToUpgradeVar1)) + appending;
                        else if (value1.Item3.GetType() == typeof(float))
                            leftSideUpgradePreview += " -> " + ((float)value1.Item3 + float.Parse(levelImprove.changeVariable.changeValues.valToUpgradeVar1)) + appending;
                        else if (value1.Item2.FieldType.IsEnum)
                            leftSideUpgradePreview += " -> " + value1.Item2.FieldType.GetEnumValues().GetValue(int.Parse(levelImprove.changeVariable.changeValues.valToUpgradeVar1));
                        else
                            leftSideUpgradePreview += " -> " + levelImprove.changeVariable.changeValues.valToUpgradeVar1 + appending;
                    }
                }

                //Just left side get upgradet
                if (hasLeftSide && !hasRightSide)
                {
                    rightSideUpgradePreview += value2.Item3;
                }
                //Just right side gets upgradet
                else if (!hasLeftSide && hasRightSide)
                {
                    leftSideUpgradePreview += " -> " + value1.Item3 + appending;
                }

                stets += leftSideUpgradePreview + rightSideUpgradePreview;
            }
        }

        return stets;
    }

    public ValueTuple<ValueTuple<object, FieldInfo, object>, ValueTuple<object, FieldInfo, object>> GetStetVariabels(string stetName)
    {
        foreach (SlotData.Stet stet in slotData.stets)
        {
            if (stet.name.Equals(stetName))
            {
                ValueTuple<object, FieldInfo, object> value1 = GetItemDataVariablePropertys(stet.stetValues.variable1Name);

                string appending = stet.stetValues.appending;

                ValueTuple<object, FieldInfo, object> value2 = GetItemDataVariablePropertys(stet.stetValues.variable2Name);

                return new(value1,value2);
            }
        }

        return new (new (null, null, null), new(null, null, null));
    }
    #endregion

    public void AddData(string valueData, string valueToAdd)
    {
        var value = GetItemDataVariablePropertys(valueData);

        var valueToAddValue = GetItemDataVariablePropertys(valueToAdd);

        //Value to add is a normal value
        if (valueToAddValue.Item1 == null)
        {
            if (value.Item3 != null && valueToAdd != "")
            {
                if (value.Item3.GetType() == typeof(int))
                    value.Item2.SetValue(value.Item1, (int)value.Item3 + int.Parse(valueToAdd));

                else if (value.Item3.GetType() == typeof(float))
                    value.Item2.SetValue(value.Item1, ((float)value.Item3 + float.Parse(valueToAdd)));

                else if (value.Item2.FieldType.IsEnum)
                    value.Item2.SetValue(value.Item1, int.Parse(valueToAdd));

                else
                    value.Item2.SetValue(value.Item1, valueToAdd);
            }
        } 
        //Value to add is an existing value
        else
        {
            if (value.Item3.GetType() == valueToAddValue.Item3.GetType())
                value.Item2.SetValue(value.Item1, valueToAddValue.Item3);
            else if (valueToAddValue.Item3 != null)
                Debug.LogError("Value " + valueToAddValue.Item3 + " of typ " + valueToAddValue.Item3.GetType() + 
                               " is not valid for typ " + value.Item3.GetType() + " of " + value.Item2.Name +  " in " + itemName);
        }
    }

    //Upgrades the scriptable object item
    public void UpgradeItem()
    {
        if (currentLevel + 1 <= maxLevel)
        {
            GameInstance.instance.inGameValues.GamePoints -= UpgradePrice;

            currentLevel++;

            foreach (LevelImprovement levelImprovement in levelImprovements)
            {
                if (currentLevel == levelImprovement.level)
                {
                    ChangeVariable changeVariable = levelImprovement.changeVariable;

                    if (changeVariable.changeDataClass)
                    {
                        switch (changeVariable.dataTyp)
                        {
                            case ChangeVariable.DataClass.WeapondataClass:
                                weaponData = changeVariable.dataClass.weaponData;
                                break;

                            case ChangeVariable.DataClass.GrenadedataClass:
                                grenadeData = changeVariable.dataClass.grenadeData;
                                break;

                            case ChangeVariable.DataClass.TrapdataClass:
                                lMGData = changeVariable.dataClass.lMGData;
                                break;

                            case ChangeVariable.DataClass.ExtradataClass:
                                extraData = changeVariable.dataClass.extraData;
                                break;

                            case ChangeVariable.DataClass.PerkdataClass:
                                perkData = changeVariable.dataClass.perkData;
                                break;

                            case ChangeVariable.DataClass.GeneraldataClass:
                                generalData = changeVariable.dataClass.generalData;
                                break;

                            case ChangeVariable.DataClass.SlotdataClass:
                                slotData = changeVariable.dataClass.slotData;
                                break;
                        }
                    }
                    else
                    {
                        AddData(levelImprovement.changeVariable.variablesToChange.variable1Name,
                                levelImprovement.changeVariable.changeValues.valToUpgradeVar1);
                        AddData(levelImprovement.changeVariable.variablesToChange.variable2Name,
                                levelImprovement.changeVariable.changeValues.valToUpgradeVar2);
                    }
                }
            }

            foreach (UpgradeImprovement upgrade in upgradeImprovements)
            {
                AddData(upgrade.changeVariable.variablesToChange.variable1Name, upgrade.changeVariable.changeValues.valToUpgradeVar1);
                AddData(upgrade.changeVariable.variablesToChange.variable2Name, upgrade.changeVariable.changeValues.valToUpgradeVar2);
            }
        }
    }

    [Serializable]
    public class WeaponData
    {
        [HideInInspector] public float currentAmmo;
        [HideInInspector] public float currentClipAmmo;

        public float maxAmmo;
        public float maxClipAmmo;
        public float firerate;
        public bool isTriggerWeapon;

        [Space]

        public bool isSingleReloader = false;

        [HideInInspector]
        public string currentReloadSound = "";
        [Tooltip("Following is only necceary if weapon is reloaded by each bullet. \n" +
                 "- First add the sound for each single bullet reload \n" +
                 "- Secondly add the sound which completes the reload")]
        public string[] reloadSounds;

        [ConditionalHide("isSingleReloader")]
        public float singleReloadTime = 0.5f;

        [Tooltip("Following is only necceary if weapon is reloaded by each bullet. \n" +
              "- First add the animation for each single bullet reload \n" +
              "- Secondly add the animation which completes the reload")]
        public string[] reloadAnimations;

        public float reloadTimeMultiply = 1f;

        [Space]

        [HideInInspector]
        public string currentAttackSound = "";
        public string[] attackSounds;

        public enum WeaponTyp { Projectile, Particel, NoAmmu };

        //Is used to define the ammu display
        public WeaponTyp weaponTyp = WeaponTyp.Projectile;

        [ConditionalHide("weaponTyp", WeaponTyp.Projectile)]
        public Prokectile.ProjectileTyp projectileTyp;

        [ConditionalHide("weaponTyp", WeaponTyp.NoAmmu, true)]
        public Prokectile.ProjectileDamageTyp damageTyp;

        [Tooltip("Number of enemys the bullets can go trough")]
        public int penetratingPower = 1;
    }

    [Serializable]
    public class GrenadeData
    {
        public enum GrenadeTyp { Normal, Flashbang }
        public GrenadeTyp grenadeTyp = GrenadeTyp.Normal;
        public float effectValue;
        public Sprite sprite;
        public string throwSound;
    }

    [Serializable]
    public class LMGData
    {
        public float maxHealth = 100f;

        [Header("Destroy State Info [Should start with MaxLife and end with 0]")]
        public float[] destructiveStateSteps;

        public Sprite[] tops;
        public Sprite[] bases;

        public float aimRadius = 5f;
    }

    [Serializable]
    public class ExtraData
    {
        public enum ExtraTyp { AllAmmo, CurrentAmmo, AllHealth, MiddleHealth, SmallHealth, NewGrenades, NewTraps }
        public ExtraTyp extraTyp;
    }

    [Serializable]
    public class PerkData
    {
        public enum PerkTyp { ExtraHealth, ExtraLife, ExtraAmmo, FasterReload, 
                              ExtraGP, ExtraReward, ExtraGrenade, ExtraTrap, 
                              ExtraDamage, ExtraSpeed }
        public PerkTyp perkTyp;
    }

    [Serializable]
    public class GeneralData
    {
        [Header("Sight")]
        [ConditionalHide("sightTyp", SightTyp.Place)]
        public float placeSightSize = 7;
        public enum SightTyp { Spreat, Streight, Place, NoSight }
        public SightTyp sightTyp = SightTyp.Streight;

        [Header("Holdng")]
        public HoldingType holdingType = HoldingType.HoldingMeeleWeapon;
        public enum HoldingType { HoldingMeeleWeapon, HoldingOneHandedWeapon,
                                  HoldingTwoHandedWeapon, HoldinObjectInHands };

        public enum MeeleWeaponType { Knife, Katana, Shield }
        [ConditionalHide("holdingType", HoldingType.HoldingMeeleWeapon)]
        public MeeleWeaponType meeleWeaponType;

        public enum OneHandetWeaponTyp { Grenade, Pistol, Reolver }
        [ConditionalHide("holdingType", HoldingType.HoldingOneHandedWeapon)]
        public OneHandetWeaponTyp oneHandetWeaponTyp;

        public enum TwoHandetWeaponTyp { Default, Flamethrower, RocketLauncher, Minigun, AutoShotGun, SemiShotGun,
                                         Sniper, GrenadeLaucher, Crossbow }
        [ConditionalHide("holdingType", HoldingType.HoldingTwoHandedWeapon)]
        public TwoHandetWeaponTyp twoHandetWeaponTyp;

        public enum ObjectTyp { LMG }
        [ConditionalHide("holdingType", HoldingType.HoldinObjectInHands)]
        public ObjectTyp objectTyp;
    }

    [Serializable]
    public class SlotData
    {
        [Header("Preview")]
        public Sprite itemImageSide;
        public Vector2 slotImageScaleSide = new Vector2(3, 3);
        public float slotImageRotationSide = 0;

        [Space]

        public Sprite itemImageTop;
        public Vector2 slotImageScaleTop = new Vector2(3, 3);
        public float slotImageRotationTop = 0;

        [Space]

        [Header("Text")]
        public Stet[] stets;

        [Serializable]
        public struct Stet
        {
            public string name;

            public StetValues stetValues;

            [Serializable]
            public struct StetValues
            {
                public string variable1Name;
                public string appending;
                public string variable2Name;
            }
        }

        [TextArea(1, 12)]
        public string decription;
    }
}


