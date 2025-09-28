using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InitialButtom : MonoBehaviour
{
    public Button firtsButton;
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firtsButton.gameObject);
    }


}
