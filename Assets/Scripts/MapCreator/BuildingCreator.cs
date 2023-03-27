using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [SerializeField] Tilemap preview, defaultMap;
    PlayerInput playerInput;

    Camera _camera;

    Vector2 mousePos;
    Vector3Int currentGridPos;
    Vector3Int lastGridPos;

    TileBase tileBase;

    BuildingObjectBase selectedObject;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
        _camera = Camera.main;
    }
    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.Gameplay.MousePos.performed += OnMouseMove;
        playerInput.Gameplay.MouseLeftClick.performed += OnLeftClick;
        playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        SelectedObject = null;
    }

    private void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if(selectedObject != null && !EventSystem.current.IsPointerOverGameObject())
            HandleDraw();
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.Gameplay.MousePos.performed -= OnMouseMove;
        playerInput.Gameplay.MouseLeftClick.performed -= OnLeftClick;
        playerInput.Gameplay.MouseRightClick.performed -= OnRightClick;
    }
    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }
    private BuildingObjectBase SelectedObject
    {
        set
        {
            selectedObject = value;
            tileBase = selectedObject != null ? selectedObject.TileBase : null;
            UpdatePreview();
        }
    }
    public void ObjectSelected(BuildingObjectBase obj)
    {
        SelectedObject = obj;
    }

    private void Update()
    {
        if (selectedObject != null)
        {
            Vector3 pos = _camera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = preview.WorldToCell(pos);

            if (gridPos != currentGridPos)
            {
                lastGridPos = currentGridPos;
                currentGridPos = gridPos;

                UpdatePreview();
            }
        }
    }

    private void UpdatePreview()
    {
        preview.SetTile(lastGridPos, null);
        preview.SetTile(currentGridPos, tileBase);
    }

    private void HandleDraw()
    {
        DrawItem();
    }
    private void DrawItem()
    {
        defaultMap.SetTile(currentGridPos, tileBase);
    }
}
