using UnityEngine;
using UnityEngine.Events;

namespace SA
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent targetEvent;
        public UnityEvent response;

        private void OnEnable()
        {
            targetEvent.Register(this);
        }

        private void OnDisable()
        {
            targetEvent.Unregister(this);
        }

        public virtual void Raise()
        {
            response.Invoke();
        }
    }

}
