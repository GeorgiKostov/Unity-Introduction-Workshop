using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class MouseClickInteractor : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponents<IInteractable>() != null)
                    {
                        foreach (var interactive in hit.transform.GetComponents<IInteractable>())
                        {
                            interactive.Enter();
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponents<IInteractable>() != null)
                    {
                        foreach (var interactive in hit.transform.GetComponents<IInteractable>())
                        {
                            interactive.Exit();
                        }
                    }
                }
            }
        }
    }
}