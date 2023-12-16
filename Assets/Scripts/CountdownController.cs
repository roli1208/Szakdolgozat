using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    [SerializeField] GameObject countdown;
    [SerializeField] TMP_Text timer;
    [SerializeField] GameObject car;
    [SerializeField] GameObject aiCar;
    private void Awake()
    {
        car.GetComponent<CarInputHandler>().enabled = false;
        aiCar.GetComponent<CarAIHandler>().enabled = false;
        countdown.SetActive(true);
        StartCoroutine(countDown());
    }

    IEnumerator countDown()
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Timer: " + i);
            yield return new WaitForSeconds(1);
            car.GetComponent<LapCounter>().enabled = true;

            timer.text = (Convert.ToInt32(timer.text) - 1).ToString();
        }
        timer.text = "START";
        yield return new WaitForSeconds(0.5f);
        countdown.SetActive(false);
        car.GetComponent<CarInputHandler>().enabled = true;
        aiCar.GetComponent<CarAIHandler>().enabled = true;
    }
}
