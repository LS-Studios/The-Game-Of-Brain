using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveSpawnPointCategories : MonoBehaviour
{
    public Category[] spawnPointCategorys;
    public bool debugSpawnPoints = false;

    [Serializable]
    public class Category {
        public string name;
        public Transform spawnPointsHolder;
        public bool isActive;
        public delegate void ActivityChanged();
        public ActivityChanged activityChanged;

        public void SetArreaIsActive(bool isActive) {
            this.isActive = isActive;
            activityChanged?.Invoke();
        }
    }

    public Category GetCategory(string name) {
        return Array.Find(spawnPointCategorys, category => category.name == name);
    }

    public void ActivateArea(string name)
    {
        GetCategory(name).SetArreaIsActive(true);
    }
}
