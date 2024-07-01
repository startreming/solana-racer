using Domain;

namespace Car
{
    public class InputPlayer : InputCreator
    {
        private InputActions _input = null;
        private CarController _controller;
        private bool _isEnabled;
        
        public override float Horizontal => _input.Kart.Steer.ReadValue<float>();
        public override float Forward => GetForward();

        private float GetForward()
        {
            if(_isEnabled)
                return _input.Kart.Forward.ReadValue<float>();
            
            while (_controller.CurrentSpeed > 0)
            {
                return -1f;
            }

            return 0;
        }

        public override bool IsDrifting => _input.Kart.Drift.IsPressed();

        public override void Initialize(CarController controller)
        {
            _controller = controller;
            _input = new InputActions();
            _input.Enable();
            _isEnabled = true;
        }

        public override void UnInitialize()
        {
            _input.Disable();
            _isEnabled = false;
        }

        public override void Stop()
        {
        }
    }
}