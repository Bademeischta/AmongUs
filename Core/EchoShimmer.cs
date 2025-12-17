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
            if (_player?.cosmetics != null)
            {
                _originalColor = _player.cosmetics.BodyColor;
            }
            else
            {
                ModPlugin.Logger.LogError("[EchoShimmer] Failed to get PlayerControl or cosmetics component!");
                Destroy(this); // Destroy the component if it can't initialize properly.
            }
        }

        void Update()
        {
            if (_shimmerTimer > 0)
            {
                _shimmerTimer -= Time.deltaTime;
                var color = Color.Lerp(_originalColor, Color.cyan, _shimmerTimer / ShimmerDuration);
                if (_player?.cosmetics != null)
                {
                    _player.cosmetics.SetBodyColor(color);
                }
            }
            else
            {
                if (_player?.cosmetics != null)
                {
                    _player.cosmetics.SetBodyColor(_originalColor);
                }
                Destroy(this);
            }
        }

        public void StartShimmer()
        {
            _shimmerTimer = ShimmerDuration;
        }
    }
}
