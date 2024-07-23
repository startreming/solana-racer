using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(fileName = "Ground Surface Preset", menuName = "Powerslide Kart Physics/Ground Surface Preset")]
    public class GroundSurfacePreset : ScriptableObject
    {
        public float friction = 1.0f;
        public bool useColliderFriction = false;
        public float speed = 1.0f;
        public Material tireMarkMaterial;
        public bool offRoad = false;
    }
}
