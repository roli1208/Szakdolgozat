using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;
using System.Threading;
using UnityEngine.SceneManagement;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [SerializeField] Tilemap preview, defaultMap;
    [SerializeField] GameObject handler;
    PlayerInput playerInput;
    public Dictionary<Vector3Int, int> checkpointID = new Dictionary<Vector3Int, int>();
    int currentId = 0;
    public 
    Camera _camera;
    [SerializeField] CarController carController;
    bool isFirst = true;
    public Vector2 Spawnpoint;
    [SerializeField] BuildingObjectBase finish;
    [SerializeField] BuildingObjectBase grass;
    [SerializeField] BuildingObjectBase sand;
    [SerializeField] TextMeshProUGUI background;

    Vector2 mousePos;
    Vector3Int currentGridPos;
    Vector3Int lastGridPos;

    LineRenderer lineRenderer;

    TileBase tileBase;
    List<Vector3> waypointPositions = new List<Vector3>();
    [SerializeField] public List<Waypoint> waypoints = new List<Waypoint>();

    [SerializeField] public GameObject waypoint;

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
        carController = GameObject.FindGameObjectWithTag("Car").GetComponent<CarController>();
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
            isFirst = false;
        }
       
    }
    
    public void changeBackground()
    {
        TileBase current;
        if(background.text != "Grass")
        {
            current = grass.TileBase;
            background.text = "Grass";
        }
        else
        {
            current = sand.TileBase;
            background.text = "Sand";
        }
        for (int x = -11; x <= 11; x++)
        {
            for (int y = 6; y >= -6; y--)
            {
                grass.Category.Tilemap.SetTile(new Vector3Int(x, y, 0), current);
            }
        }
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        background.text = "None";
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
    GameObject wp;
    private void DrawItem()
    {
        List<string> corners = new List<string> { "Road", "Road 2", "Road 4", "Road 7" };
        if (selectedObject.Category.name == "Checkpoint")
        {
            checkpointID.Add(currentGridPos, currentId++);
        }
        tilemap.SetTile(currentGridPos, tileBase);
        if (isFirst)
        {
            finish.Category.Tilemap.SetTile(currentGridPos, finish.TileBase);
            Spawnpoint = new Vector2(currentGridPos.x - 0.5f, currentGridPos.y + 0.5f);
        }
        if (selectedObject.Category.name != "Checkpoint")
        {
            Vector3 pos = new Vector3(currentGridPos.x + 0.5f, currentGridPos.y + 0.5f, currentGridPos.z);
            wp = Instantiate(waypoint, pos, Quaternion.identity);
            Waypoint wps = wp.GetComponent<Waypoint>();
            if (corners.Contains(selectedObject.name))
            {
                wps.maxSpeed = carController.maxSpeed * 0.35f;
            }
            else
            {
                wps.maxSpeed = carController.maxSpeed;
            }
            waypointPositions.Add(pos);
            waypoints.Add(wps);
            DrawLine();
        }
    }
    private void DrawLine()
    {
        if(waypointPositions.Count > 1)
        {
            int count = waypointPositions.Count;
            lineRenderer.positionCount = count;
            for (int i = 0; i < count; i++)
            {
                lineRenderer.SetPosition(i, waypointPositions[i]);
            }
        }
    }
}
