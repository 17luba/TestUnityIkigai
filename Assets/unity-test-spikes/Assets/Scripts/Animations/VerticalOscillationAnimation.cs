using DG.Tweening;
using UnityEngine;

namespace Ikigai.DontTouchTheSpikes.Animations
{
    public class VerticalOscillationAnimation : MonoBehaviour
    {
        [SerializeField] private float _duration = 2f;
        [SerializeField] private float _amount = .25f;
        [SerializeField] private AnimationCurve _curve;

        private Tween _oscillationTween;

        private void OnEnable()
        {
            _oscillationTween = transform.DOMoveY(transform.position.y + _amount, _duration)
                .SetEase(_curve)
                .SetLoops(-1)
                .Play();
        }

        private void OnDisable()
        {
            _oscillationTween.Kill();
        }
    }
}