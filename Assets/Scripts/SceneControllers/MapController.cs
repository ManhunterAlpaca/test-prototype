using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    private enum BuildModeType {
        None,
        Build,
        Demolish,
    }

    // Unity objects
    public Tilemap BaseTileMap;
    public Tilemap LowerHighlightMap;
    public Tilemap BuildingMap;
    public Tilemap HighlightMap;

    public Tile BaseTile;
    public Tile BuildingTile;
    public Tile LowerHighlightTile;
    public Tile HighlightTile;
    public Tile DisabledTile;

    // Unity input system
    private PlayerControls PlayerControls;

    // Controller state
    private Vector3Int TilePosition;
    private GameState GameState;
    private BuildModeType BuildMode = BuildModeType.None;

    public void Awake() {
        PlayerControls = new PlayerControls();
    }

    private void OnEnable() {
        PlayerControls.Enable();
    }

    private void OnDisable() {
        PlayerControls.Disable();
    }

    public void Start() {
        GameState = SaveManager.Load();

        BuildMode = BuildModeType.None;
        ResetTiles();
        PlayerControls.Mouse.MouseClick.performed += _ => OnMouseClick();
        PlayerControls.Mouse.MousePos.performed += _ => OnMousePos();
    }

    public void OnDestroy() {
        SaveManager.Save(GameState);
    }

    void OnMousePos() {
        if (BuildMode == BuildModeType.None) {
            return;
        }

        Vector2 mousePosition = PlayerControls.Mouse.MousePos.ReadValue<Vector2>();
        // Check if mouse position is in play area and not in the UI panel
        if (mousePosition.x <= 300) {
            HighlightMap.ClearAllTiles();
            return;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int coordinate = HighlightMap.WorldToCell(worldPos);
        if (coordinate != TilePosition) {
            TilePosition = coordinate;
            RedrawHighlightedTile(coordinate);
        }
    }

    void OnMouseClick() {
        if (BuildMode == BuildModeType.None) {
            return;
        }

        Vector2 mousePosition = PlayerControls.Mouse.MousePos.ReadValue<Vector2>();
        // Check if mouse position is in play area and not in the UI panel
        if (mousePosition.x <= 300) {
            return;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int coordinate = HighlightMap.WorldToCell(worldPos);
        BuildAtCoordinate(coordinate);
    }

    public void OnBuild() {
        if (BuildMode == BuildModeType.Build) {
            ExitAllModes();
        } else {
            EnterBuildMode();
        }
    }

    public void OnDemolish() {
        if (BuildMode == BuildModeType.Demolish) {
            ExitAllModes();
        } else {
            EnterDemolishMode();
        }
    }

    public void OnConvo() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ConvoScene");
    }

    private void EnterBuildMode() {
        BuildMode = BuildModeType.Build;
        HighlightMap.ClearAllTiles();
        LowerHighlightMap.ClearAllTiles();
        for (int x = 0; x < GameState.GridSize; x++) {
            for (int y = 0; y < GameState.GridSize; y++) {
                if (!GameState.Grid[x][y]) {
                    LowerHighlightMap.SetTile(new Vector3Int(x, y, 0), LowerHighlightTile);
                }
            }
        }
    }

    private void EnterDemolishMode() {
        BuildMode = BuildModeType.Demolish;
        HighlightMap.ClearAllTiles();
        LowerHighlightMap.ClearAllTiles();
        for (int x = 0; x < GameState.GridSize; x++) {
            for (int y = 0; y < GameState.GridSize; y++) {
                if (GameState.Grid[x][y]) {
                    LowerHighlightMap.SetTile(new Vector3Int(x, y, 0), LowerHighlightTile);
                }
            }
        }
    }

    private void ExitAllModes() {
        BuildMode = BuildModeType.None;
        HighlightMap.ClearAllTiles();
        LowerHighlightMap.ClearAllTiles();
    }

    private void ResetTiles() {
        BaseTileMap.ClearAllTiles();
        BuildingMap.ClearAllTiles();

        for (int x = 0; x < GameState.GridSize; x++) {
            for (int y = 0; y < GameState.GridSize; y++) {
                BaseTileMap.SetTile(new Vector3Int(x, y, 0), BaseTile);
                if (GameState.Grid[x][y]) {
                    BuildingMap.SetTile(new Vector3Int(x, y, 0), BuildingTile);
                }
            }
        }
    }

    private void RedrawHighlightedTile(Vector3Int coordinate) {
        HighlightMap.ClearAllTiles();
        if (coordinate.x >= 0 && coordinate.x < GameState.GridSize
                && coordinate.y >= 0 && coordinate.y < GameState.GridSize) {
            Tile tile;
            if (BuildMode == BuildModeType.Build) {
                tile = GameState.Grid[coordinate.x][coordinate.y] ? DisabledTile : HighlightTile;
            } else {
                tile = GameState.Grid[coordinate.x][coordinate.y] ? HighlightTile : DisabledTile;
            }
            HighlightMap.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), tile);
        }
    }

    private void BuildAtCoordinate(Vector3Int coordinate) {
        if (coordinate.x >= 0 && coordinate.x < GameState.GridSize
                && coordinate.y >= 0 && coordinate.y < GameState.GridSize) {
            if (BuildMode == BuildModeType.Build && !GameState.Grid[coordinate.x][coordinate.y]) {
                GameState.Grid[coordinate.x][coordinate.y] = true;
                BuildingMap.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), BuildingTile);
                ExitAllModes();
            } else if (BuildMode == BuildModeType.Demolish && GameState.Grid[coordinate.x][coordinate.y]) {
                GameState.Grid[coordinate.x][coordinate.y] = false;
                BuildingMap.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), null);
                ExitAllModes();
            }
        }
    }
}