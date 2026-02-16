using DG.Tweening;
using UnityEngine;

/**
* PowerToggleClickedView クラス
* - PowerToggleInteractor のトグルがクリックされたときのエフェクトを再生するクラス
* - DOJump を用いて、トグルがクリックされたときに、トグルのビジュアルがジャンプするエフェクトを実装する
*/
public sealed class PowerToggleClickedView
{
    private readonly Transform _target;
    private readonly Vector3 _jumpEndPosition;
    private readonly float _jumpPower;
    private readonly int _numJumps;
    private readonly float _duration;

    public PowerToggleClickedView(
        Transform target,
        Vector3 jumpEndPosition,
        float jumpPower,
        int numJumps,
        float duration)
    {
        _target = target;
        _jumpEndPosition = jumpEndPosition;
        _jumpPower = jumpPower;
        _numJumps = numJumps;
        _duration = duration;
    }

    public void Play()
    {
        if (_target == null) return;

        _target.DOKill();
        _target.DOJump(_jumpEndPosition, _jumpPower, _numJumps, _duration);
    }
}

