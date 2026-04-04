using UnityEngine.UI;
using UnityEngine;

public class PlateIconsSingleUI : MonoBehaviour
{

    [SerializeField] private Image image;
    
    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        image.sprite = kitchenObjectSO.sprite;
    }
}
