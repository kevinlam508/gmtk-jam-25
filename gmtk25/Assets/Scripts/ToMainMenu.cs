using UnityEngine;
using System.Collections;

public class ToMainMenu : MonoBehaviour
{
    public float _delay = 2f;

    public void GoToMainMenu()
    {
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(_delay);

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
