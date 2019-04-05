using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Event")]
    public class GameEvent : ScriptableObject
    {
        List<GameEventListener> listerners = new List<GameEventListener>();

        public void Register(GameEventListener l )
        {
            if (!listerners.Contains(l))
                listerners.Add(l);
        }

        public void Unregister(GameEventListener l)
        {
            if (listerners.Contains(l))
                listerners.Remove(l);
        }

        public void Raise()
        {
            for (int i = 0; i < listerners.Count; i++)
            {
                listerners[i].Raise();
            }
        }
    }
}

