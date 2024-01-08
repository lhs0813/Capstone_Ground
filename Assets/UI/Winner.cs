using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Winner : MonoBehaviour
{
    public GameObject gameObject1;
    public GameObject gameObject2;
    public GameObject gameObject3;
    public GameObject gameObject4;
    private float activationInterval = 0.5f;

    void OnEnable()
    {
        StartCoroutine(ActivateGameObjects());
    }

    IEnumerator ActivateGameObjects()
    {
        yield return new WaitForSeconds(activationInterval);
        gameObject1.SetActive(true);

        yield return new WaitForSeconds(activationInterval);
        gameObject2.SetActive(true);

        yield return new WaitForSeconds(activationInterval);
        gameObject3.SetActive(true);

        yield return new WaitForSeconds(activationInterval);
        gameObject4.SetActive(true);
    }
}