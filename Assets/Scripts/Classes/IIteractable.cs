using UnityEngine;

public interface IIteractable
{
    void Interact();
    bool CanInteract();
    Transform GetTransform();
}
