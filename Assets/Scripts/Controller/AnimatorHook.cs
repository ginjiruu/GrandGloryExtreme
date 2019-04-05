using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        StatesManager states;

        public void Init(StatesManager st)
        {
            states = st;
            anim = states.anim;
        }

        private void OnAnimatorMove()
        {
            states.inp.animDelta = anim.deltaPosition;
            transform.localPosition = Vector3.zero;
        }

        public void CloseParticles()
        {

        }

        public void InitiateThrowForProjectile()
        {
            states.CastSpellActual();
        }

        public void OpenDamageColliders()
        {
            states.SetDamageCollidersStatus(true);
        }

        public void CloseDamageColliders()
        {
            states.SetDamageCollidersStatus(false);
        }
    }
}
