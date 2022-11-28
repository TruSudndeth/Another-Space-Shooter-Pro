using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOneShotAnimation : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DisableAfterAnimation());
    }

    private IEnumerator DisableAfterAnimation()
    {
        yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}
