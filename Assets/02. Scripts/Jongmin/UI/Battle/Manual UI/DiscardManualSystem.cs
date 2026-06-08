using UnityEngine;

namespace Jongmin
{
    public class DiscardManualSystem : MonoBehaviour
    {
        private readonly DiscardManualView _view;
        private readonly TurnManager _turnManager;

        public DiscardManualSystem(DiscardManualView view)
        {
            _view = view;
        }

        public void UpdateView(ActionData actionData, bool canDiscard)
        {
            _view.UpdateUI(actionData, canDiscard);
        }
    }
}
