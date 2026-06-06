using UnityEngine;

namespace Jongmin
{
    [RequireComponent(typeof(Animator))]
    public class PreviewCard : MonoBehaviour
    {
        private Animator _animator;
        
        public RectTransform RectTransform { get; private set; } 

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            RectTransform = transform as RectTransform;
        }

        private void OnEnable()
        {
            _animator.SetTrigger("Enable");
        }
    }
}
