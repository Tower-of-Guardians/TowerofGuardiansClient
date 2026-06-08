using UnityEngine;

namespace Jongmin
{
    public class ActionManualSystem : MonoBehaviour
    {
        private readonly ActionManualView _view;

        public ActionManualSystem(ActionManualView view)
        {
            _view = view;
        }

        public void UpdateView(ActionData actionData, bool canAction)
        {
            _view.UpdateUI(actionData, canAction);
        }
    }
}
