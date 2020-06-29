using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public void Use(float timeActive)
    {
        StartCoroutine(CheckTimeActive(timeActive));
    }

    private IEnumerator CheckTimeActive(float timeActive)
    {
        yield return new WaitForSeconds(timeActive);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ResetShield()
    {
        gameObject.SetActive(false);
    }
}
