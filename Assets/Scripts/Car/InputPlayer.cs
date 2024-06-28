using Car;
using Domain;

namespace Kart
{
    public class InputPlayer : InputCreator
    {
        private InputActions _input = null;
        
        public override float Horizontal => _input.Kart.Steer.ReadValue<float>();
        public override float Forward => _input.Kart.Forward.ReadValue<float>();
        public override bool IsDrifting => _input.Kart.Drift.IsPressed();

        public override void Initialize(CarController controller)
        {
            _input = new InputActions();
            _input.Enable();
        }

        public override void UnInitialize()
        {
            _input.Disable();
        }
    }
}