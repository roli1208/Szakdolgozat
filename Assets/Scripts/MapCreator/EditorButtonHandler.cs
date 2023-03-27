using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorButtonHandler : MonoBehaviour
{
    [SerializeField] BuildingObjectBase item;
    Button btn;

    BuildingCreator buildingCreator;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonClicked);
        buildingCreator = BuildingCreator.GetInstance();
    }
    private void ButtonClicked()
    {
        Debug.Log("Button was clicked: " + item.name);
        buildingCreator.ObjectSelected(item);
    }
}
