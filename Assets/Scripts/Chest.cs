using UnityEngine;

public class Chest : MonoBehaviour, IIteractable
{   
    public bool IsOpened { get; private set; }
    public string ChestID {get; private set; }
    public GameObject itemPrefab;
    public Sprite openedSprite;
    
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }

    public void Interact()
    {
        if(!CanInteract()) return;
        OpenChest();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void OpenChest()
    {
        SetOpened(true);

        if(itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            droppedItem.GetComponent<BounceEffect>().StartBounce();
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if(IsOpened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
