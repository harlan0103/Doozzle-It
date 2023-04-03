using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGenerator : MonoBehaviour
{

    public static PuzzleGenerator Instance;

    public int gridSize = 4;

    public Material tileMaterial;

    public GameObject tilePrefab;

    public GameObject puzzlePanel;

    public GameObject doodlePanel;

    private List<PuzzleTile> tiles;
    private PuzzleTile emptyTile;
    private int tileSize = 100;

    private System.Random rng;
    private Vector2 emptyPosition;
    private bool puzzleGenerated;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        tiles = new List<PuzzleTile>();
        rng = new System.Random();
        puzzleGenerated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (puzzleGenerated && IsSolved()) {
            Debug.Log("Solved!!");
        }
    }

    public void GeneratePuzzleTexture(Texture2D texture) {
        doodlePanel.SetActive(false);

        int tileTextureSize = texture.width / gridSize;
        Debug.Log("Tile size: " + tileTextureSize);

        // split the texture into tiles
        int cnt = 0;
        for (int y = 0; y < gridSize; y++) {
            for (int x = 0; x < gridSize; x++) {
                // Create a new texture for the tile
                Texture2D tileTexture = new Texture2D(tileTextureSize, tileTextureSize);

                // Get the pixels from the original texture for the current tile
                Color[] pixels = texture.GetPixels(x * tileTextureSize, y * tileTextureSize, tileTextureSize, tileTextureSize);

                // Set the pixels for the tile texture
                tileTexture.SetPixels(pixels);
                tileTexture.filterMode = FilterMode.Point;
                tileTexture.Apply();

                // Create a new tile object
                GameObject tileObject = Instantiate(tilePrefab);
                tileObject.transform.SetParent(puzzlePanel.transform);

                // Set the tile's material to use the new tile texture
                Material tileMat = new Material(tileMaterial);
                tileMat.mainTexture = tileTexture;
                tileObject.GetComponent<Image>().material = tileMat;

                // Add script to the tile
                tileObject.AddComponent<PuzzleTile>();
                PuzzleTile puzzleTile = tileObject.GetComponent<PuzzleTile>();
                puzzleTile.number = cnt;
                if (puzzleTile.number == 15) {
                    emptyTile = puzzleTile;
                    puzzleTile.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                }
                puzzleTile.tile = puzzleTile;

                tiles.Add(puzzleTile);
                cnt++;
            }
        }

        // Shuffle tiles
        Shuffle(tiles);

        // Set each tile's position within the grid
        for (int i = 0; i < tiles.Count; i++)
        {
            Vector2 position = new Vector2((i % 4) * tileSize, (i / 4) * tileSize);
            tiles[i].MoveToPosition(position);

            // Keep track of the empty tile's position
            if (tiles[i] == emptyTile)
            {
                emptyPosition = position;
            }
        }
    }

    bool IsSolved()
    {
        // Check if each tile is in its correct position
        for (int i = 0; i < tiles.Count; i++)
        {
            int idx = tiles[i].number;
            Vector3 position = new Vector3((idx % 4) * tileSize, (idx / 4) * tileSize);
            if (tiles[i].transform.localPosition != position)
            {
                return false;
            }
        }
        return true;
    }

    public void OnTileClicked(PuzzleTile tile)
    {
        // Check if the clicked tile is adjacent to the empty tile
        Vector2 position = tile.transform.localPosition;
        if (Vector2.Distance(position, emptyPosition) == tileSize)
        {
            // Swap the clicked tile and the empty tile
            tile.MoveToPosition(emptyPosition);
            emptyTile.MoveToPosition(position);

            // Update the empty position
            emptyPosition = position;
        }
    }

    private void Shuffle(List<PuzzleTile> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            // Swap
            var val = list[k];
            list[k] = list[n];
            list[n] = val;
        }
    }
}
