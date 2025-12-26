using UnityEngine;
using DG.Tweening;

public class DotweenAliveTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"DOTween version: {DOTween.Version}, timeScale={Time.timeScale}");
        transform.DOMoveX(transform.position.x + 1f, 1f);
    }
}
