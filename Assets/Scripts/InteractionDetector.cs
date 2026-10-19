using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IIteractable interactableInRange = null;

    [Header("World Interaction Icon")]
    public GameObject interactionIcon;

    [Header("Mobile Interaction Button")]
    public Button interactionButton;

    // This will be set by JoystickPanelManager
    [HideInInspector] public bool panelsOpen = false;

    void Start()
    {
        interactionIcon.SetActive(false);
        if (interactionButton != null)
        {
            interactionButton.gameObject.SetActive(false);
            interactionButton.onClick.AddListener(OnButtonInteract);
        }
    }

    void Update()
    {
        UpdateInteractionUI();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (interactableInRange != null && interactableInRange.CanInteract())
        {
            interactableInRange.Interact();

            if (!interactableInRange.CanInteract())
            {
                interactionIcon.SetActive(false);
                if (interactionButton != null)
                    interactionButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnButtonInteract()
    {
        TryInteract();
    }

    private void UpdateInteractionUI()
    {
        bool canShow = interactableInRange != null && interactableInRange.CanInteract() && !panelsOpen;

        if (interactionIcon != null)
            interactionIcon.SetActive(canShow);

        if (interactionButton != null)
            interactionButton.gameObject.SetActive(canShow);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IIteractable interactable))
            interactableInRange = interactable;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IIteractable interactable) && interactable == interactableInRange)
            interactableInRange = null;
    }
}