using DG.Tweening;
using Ikigai.DontTouchTheSpikes.Utils;
using TMPro;
using UnityEngine;

namespace Ikigai.DontTouchTheSpikes.Animations
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PlusOneAnimation : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private TextMeshProUGUI _label;

        [Header("Animation")]
        [SerializeField] private float _animationDuration = 1f;
        [Space]
        [SerializeField] private Color _color;
        [SerializeField] private AnimationCurve _fadeCurve;
        [Space]
        [SerializeField] private float _verticalDisplacement = 1f;
        [SerializeField] private AnimationCurve _positionYCurve;

        private Sequence _fadeSequence;

        public void PlaceInWorld(Vector3 worldPosition)
        {
            _label.color = _color;

            RectTransform canvasRect = (RectTransform)_label.rectTransform.root;
            _label.rectTransform.anchoredPosition = worldPosition.GetScreenPosFromWorldPos(canvasRect, _camera);

            gameObject.SetActive(true);

            Vector3 endWorldPos = worldPosition + Vector3.up * _verticalDisplacement;
            Vector2 endPos = endWorldPos.GetScreenPosFromWorldPos(canvasRect, _camera);

            if (_fadeSequence.IsActive())
                _fadeSequence.Kill();

            _fadeSequence = DOTween.Sequence();
            _fadeSequence.Append(
                _label.DOFade(0, _animationDuration)
                    .SetEase(_fadeCurve));
            _fadeSequence.Insert(0,
                _label.rectTransform.DOAnchorPosY(endPos.y, _animationDuration)
                    .SetEase(_positionYCurve));
            _fadeSequence.OnComplete(() => gameObject.SetActive(false));
            _fadeSequence.SetAutoKill();
            _fadeSequence.Play();
        }

        // Automatically set label when adding the script in editor
        private void Reset()
        {
            _label = GetComponent<TextMeshProUGUI>();
        }
    }
}
