using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System;
using Zenject;

public class CountUpManage : MonoBehaviour
{
    public float countSpeed = 0.1f;

    public ScreenSwipe screenSwipe;

    public TextMeshProUGUI[] countUpTextes;

    private int currentChalange = 0;

    [HideInInspector]
    public int[] values = {0, 0};

    private void Start()
    { 
        for (int i = 0; i < countUpTextes.Length; i++)
        {
            StartCoroutine(CountRewarUp(i));
        }

        screenSwipe.StartCoroutine(CountChalangeUp());
    }

    public IEnumerator CountRewarUp(int index) 
    {
        while (true)
        {
            if (int.Parse(countUpTextes[index].text) + 1 <= values[index])
                countUpTextes[index].text = (int.Parse(countUpTextes[index].text) + 1).ToString();

            yield return new WaitForSeconds(countSpeed);
        }
    }

    public IEnumerator CountChalangeUp()
    {
        yield return new WaitForSeconds(0.5f);

        Challenge chalange = screenSwipe.screens[currentChalange].gameObject.GetComponent<ChallengeItem>().chalange;

        int endProgress = chalange.currentProgress + chalange.currentIngameProgress;

        chalange.currentIngameProgress = 0;

        while (chalange.currentProgress < endProgress && endProgress != 0)
        {
            chalange.currentProgress += 1;

            yield return new WaitForSeconds(0.08f);
        }

        if (chalange.isCompleted)
        {
            values[0] += chalange.cashReward;
            values[1] += chalange.brainCellReward;
        }

        yield return new WaitForSeconds(2f);

        if (currentChalange == screenSwipe.CurrentScreen)
            screenSwipe.GoToNextScreen();

        if (currentChalange + 1 < screenSwipe.screens.Count)
        {
            currentChalange++;

            StartCoroutine(CountChalangeUp());
        }
    }

    public void EndGame()
    {
        GameInstance.instance.chalangeValues.chalanges.ForEach(chalange => chalange.currentIngameProgress = 0);

        screenSwipe.screens.ForEach(chalange => chalange.gameObject.GetComponent<ChallengeItem>().enabled = false);

        GameInstance.instance.inGameValues.GamePoints = 0;

        GameInstance.instance.chalangeValues.CompleteCompletedChalanges();

        SceneManager.LoadScene(0);
    }
}
