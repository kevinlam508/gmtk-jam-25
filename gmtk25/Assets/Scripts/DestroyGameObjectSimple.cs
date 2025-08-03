using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DestroyGameObjectSimple : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DestroyAfterDelay(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(gameObject);
    }
}
