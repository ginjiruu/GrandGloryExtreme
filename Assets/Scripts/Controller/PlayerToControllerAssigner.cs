using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    public class PlayerToControllerAssigner : MonoBehaviour
    {
        private List<int> assignedControllers = new List<int>();
        public PlayerPannel[] playerPanels;

        // Update is called once per frame
        void Update()
        {
            for (int i = 1; i <= 6; i++)
            {
                if (hInput.GetButton(i + "Accept"))
                {
                    AddPlayerController(i);
                    break;
                }
            }
        }
        public InputHandler AddPlayerController(int controller)
        {
            if (assignedControllers.Contains(controller))
            {
                return null;
            }

            assignedControllers.Add(controller);
            for (int i = 0; i < playerPanels.Length; i++)
            {
                if (playerPanels[i].HasControllerAssigned() == false)
                    return playerPanels[i].AssignedController(controller);
            }
            return null;
        }
    }
}