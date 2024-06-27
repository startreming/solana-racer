using UnityEngine;

namespace Effects
{
    [DisallowMultipleComponent]
    public class GroundSurface : MonoBehaviour
    {
        public GroundSurfacePreset props;
        protected Collider col;

        protected virtual void Awake() {
            col = GetComponent<Collider>();
        }
        
        public GroundSurfacePreset GetProps() {
            return props != null ? props : GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
        }
        
        public virtual float GetFriction() {
            if (props == null) {
                props = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
            }

            if (props.useColliderFriction && col != null) {
                return col.sharedMaterial != null ? col.sharedMaterial.dynamicFriction : 1.0f;
            }
            else {
                return props.friction;
            }
        }
        
        public virtual float GetFriction(Vector3 pos) {
            return GetFriction();
        }
        
        public virtual float GetSpeed() {
            if (props == null) {
                props = GroundSurfacePreset.CreateInstance<GroundSurfacePreset>();
            }

            return props.speed;
        }
        
        public virtual float GetSpeed(Vector3 pos) {
            return GetSpeed();
        }
    }
}
