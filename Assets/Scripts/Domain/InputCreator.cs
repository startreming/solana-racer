using Car;
using Kart;
using UnityEngine;

namespace Domain
{
    public abstract class InputCreator : MonoBehaviour
    {
        public abstract float Horizontal { get; }
        public abstract float Forward { get; }
        public abstract bool IsDrifting { get; }

        public abstract void Initialize(CarController controller);
        public abstract void UnInitialize();
    }
}