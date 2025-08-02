using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Mathematics;

public class MainMenuCharmSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> _charms;
    [SerializeField] GameObject _charmRigidbody;
    [SerializeField] private Transform _leftExtent;
    [SerializeField] private Transform _righttExtent;
    private Vector3 _leftPosition;
    private Vector3 _righttPosition;
    void Start()
    {
        _leftPosition = _leftExtent.position;
        _righttPosition = _righttExtent.position;
        StartCoroutine(CharmSpawning());

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator CharmSpawning()
    {

        while (true)
        {
            Vector3 positionToSpawn = new Vector3(UnityEngine.Random.Range(_leftPosition.x,_righttPosition.x),_leftPosition.y,_leftPosition.z);
            GameObject parent = Instantiate(_charmRigidbody,positionToSpawn,UnityEngine.Random.rotation) as GameObject;
            parent.transform.localScale *= UnityEngine.Random.Range(50f, 100f);
            GameObject.Instantiate(_charms[UnityEngine.Random.Range(0, _charms.Count)], parent.transform);
            parent.GetComponent<Rigidbody>().AddForce(0, -175f, 0f);
            parent.GetComponent<Rigidbody>().AddTorque(UnityEngine.Random.Range(5f, 20f),UnityEngine.Random.Range(5f, 20f),UnityEngine.Random.Range(5f, 20f));
            yield return new WaitForSeconds(UnityEngine.Random.Range(.25f, .75f));
        }
    }
}
