using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [SerializeField] Tilemap preview, defaultMap;
    PlayerInput playerInput;
    public Dictionary<Vector3Int, int> checkpointID = new Dictionary<Vector3Int, int>();
    int currentId = 0;

    Camera _camera;

    Vector2 mousePos;
    Vector3Int currentGridPos;
    Vector3Int lastGridPos;

    TileBase tileBase;

    BuildingObjectBase selectedObject;

    bool holdAction;
    Vector3Int holdStartPos;
    BoundsInt bounds;

    private Tilemap tilemap
    {
        get
        {
            if(selectedObject != null && selectedObject.Category != null && selectedObject.Category.Tilemap != null)
            {
                return selectedObject.Category.Tilemap;
            }

            return defaultMap;
        }
    }

    public Dictionary<Vector3Int,int> CheckpointID
    {
        get
        {
            return checkpointID;
        }
    }
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
        playerInput.Gameplay.MouseLeftClick.started += OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.canceled += OnLeftClick;

        playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        SelectedObject = null;
    }

    private void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if (selectedObject != null && !EventSystem.current.IsPointerOverGameObject())
        {
            if(ctx.phase == InputActionPhase.Started)
            {
                holdAction = true;
                if(ctx.interaction is TapInteraction)
                {
                    holdStartPos = currentGridPos;
                }
                HandleDraw();
            }
            else
            {
                if (ctx.interaction is SlowTapInteraction || ctx.interaction is TapInteraction && ctx.phase == InputActionPhase.Performed)
                {
                    holdAction = false;
                    HandleDrawRelease();
                }
            }
        }
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.Gameplay.MousePos.performed -= OnMouseMove;

        playerInput.Gameplay.MouseLeftClick.performed -= OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.started -= OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.canceled -= OnLeftClick;

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

                if (holdAction)
                {
                    HandleDraw();
                }
            }
        }
    }

    private void UpdatePreview()
    {
        preview.SetTile(lastGridPos, null);
        preview.SetTile(currentGridPos, tileBase);
    }


   private void RectanglePreview()
    {
        preview.ClearAllTiles();

        bounds.xMin = currentGridPos.x < holdStartPos.x ? currentGridPos.x : holdStartPos.x;
        bounds.xMax = currentGridPos.x > holdStartPos.x ? currentGridPos.x : holdStartPos.x;
        bounds.yMin = currentGridPos.y < holdStartPos.y ? currentGridPos.y : holdStartPos.y;
        bounds.yMax = currentGridPos.y > holdStartPos.y ? currentGridPos.y : holdStartPos.y;

        DrawBounds(preview);
    }

    private void DrawBounds(Tilemap map)
    {
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                map.SetTile(new Vector3Int(x, y, 0), tileBase);
            }
        }
    }

    private void HandleDraw()
    {
        if(selectedObject != null)
        {
            switch (selectedObject.PlaceType)
            {
                case PlaceType.Single: default:
                    DrawItem();
                    break;
                case PlaceType.Rectangle:
                    RectanglePreview();
                    break;
                
            }
        }

    }
    private void HandleDrawRelease()
    {

        if (selectedObject != null)
        {
            switch (selectedObject.PlaceType)
            {
                case PlaceType.Rectangle:
                    DrawBounds(tilemap);
                    preview.ClearAllTiles();
                    break;

            }
        }
    }
    private void DrawItem()
    {

        if(selectedObject.Category.name == "Checkpoint")
        {
            checkpointID.Add(currentGridPos, currentId++);   
        }
            tilemap.SetTile(currentGridPos, tileBase);
    }
}
