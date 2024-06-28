using Car;
using Domain;
using Kart;

namespace AI
{
    public class AIInput : InputCreator
    {
        private CarController _controller;
        public override float Horizontal => GenerateHorizontal();
        public override float Forward => GenerateForward();

        public override bool IsDrifting => GenerateIsDrifting();

        private float GenerateForward()
        {
            return 1;
        }

        public float GenerateHorizontal()
        {
            return 0;
        }

        private bool GenerateIsDrifting()
        {
            return false;
        }

        public override void Initialize(CarController controller)
        {
            _controller = controller;
        }

        public override void UnInitialize()
        {
            
        }
    }
}