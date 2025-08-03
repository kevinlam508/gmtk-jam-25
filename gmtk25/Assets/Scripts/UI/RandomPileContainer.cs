using UnityEngine;
using System.Collections.Generic;

public class RandomPileContainer : MonoBehaviour
{
    [SerializeField] private float _widthMargin;
    [SerializeField] private float _heightMargin;

    private HashSet<Transform> _alreadyPlaced = new HashSet<Transform>();

    private void OnTransformChildrenChanged()
    {
        RectTransform rt = (RectTransform)transform;
        float height = rt.rect.height - _heightMargin;
        float width = rt.rect.width - _widthMargin;

        HashSet<Transform> newAlreadyPlaced = new HashSet<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            newAlreadyPlaced.Add(child);
            if (_alreadyPlaced.Contains(child))
            {
                continue;
            }

            float randomX = Random.Range(-.5f, .5f) * width;
            float randomY = Random.Range(-.5f, .5f) * height;

            child.transform.localPosition = new Vector3(randomX, randomY, 0);
        }

        _alreadyPlaced = newAlreadyPlaced;
    }
}
