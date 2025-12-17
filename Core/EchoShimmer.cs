using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class EchoShimmer : MonoBehaviour
    {
        public float ShimmerDuration = 2f;
        private float _shimmerTimer;
        private PlayerControl _player;
        private Color _originalColor;

        void Awake()
        {
            _player = GetComponent<PlayerControl>();
            _originalColor = _player.cosmetics.BodyColor;
        }

        void Update()
        {
            if (_shimmerTimer > 0)
            {
                _shimmerTimer -= Time.deltaTime;
                var color = Color.Lerp(_originalColor, Color.cyan, _shimmerTimer / ShimmerDuration);
                _player.cosmetics.SetBodyColor(color);
            }
            else
            {
                _player.cosmetics.SetBodyColor(_originalColor);
                Destroy(this);
            }
        }

        public void StartShimmer()
        {
            _shimmerTimer = ShimmerDuration;
        }
    }
}
