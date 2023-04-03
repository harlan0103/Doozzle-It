using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoodleGenerator : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private int gridWidth;

    [SerializeField]
    private int gridHeight;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private Material doodleMat;

    [SerializeField]
    private int drawSize = 1;

    [Space()]
    // Texture that to be generated
    private Texture2D gridTexture;

    // Buffer that stores canvas color
    private Color[] colorArray;

    private Color drawColor;
    private Vector2 bottomLeftCorner;
    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    // Start is called before the first frame update
    void Start()
    {
        // Create a new texture
        gridTexture = new Texture2D(gridWidth, gridHeight);
        gridTexture.filterMode = FilterMode.Point;

        // Initialize the colorArray
        colorArray = new Color[gridWidth * gridHeight];
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                colorArray[y * gridWidth + x] = Color.white;
            }
        }

        // Apply color array to the texture
        gridTexture.SetPixels(colorArray);
        gridTexture.Apply();
        doodleMat.mainTexture = gridTexture;

        // Graphics raycaster
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();

        // Calculate bottom left corner position as origin point
        Vector3[] corners = new Vector3[4];
        canvas.GetComponent<RectTransform>().GetWorldCorners(corners);
        bottomLeftCorner = new Vector2(corners[0].x, corners[0].y);

        // Initial draw color is red
        drawColor = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            // Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            // Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            // Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            // For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult objectHit in results)
            {
                if (objectHit.gameObject.name.Equals("Square")) {
                    float canvasWidth = objectHit.gameObject.GetComponent<RectTransform>().rect.width;
                    float canvasHeight = objectHit.gameObject.GetComponent<RectTransform>().rect.height;

                    float unit_xPos = Mathf.Abs(Input.mousePosition.x - bottomLeftCorner.x) / canvasWidth;
                    float unit_yPos = Mathf.Abs(Input.mousePosition.y - bottomLeftCorner.y) / canvasHeight;

                    // Now try to draw something
                    DrawDoodleOnCanvas(unit_xPos, unit_yPos);
                }
            }
        }
    }

    private void DrawDoodleOnCanvas(float x, float y) {
        // Get corresponding position using unit position
        int gridX = (int)(gridTexture.width * x);
        int gridY = (int)(gridTexture.height * y);

        // Get rid of out of index
        gridX = Mathf.Clamp(gridX, 0, gridTexture.width - 1);
        gridY = Mathf.Clamp(gridY, 0, gridTexture.height - 1);

        UpdateColorArray(gridX, gridY, colorArray);
    }

    private void UpdateColorArray(int x, int y, Color[] colorArray) {
        // Update color with different draw size
        for (int x_offset = x - drawSize; x_offset < x + drawSize; x_offset++) {
            for (int y_offset = y - drawSize; y_offset < y + drawSize; y_offset++) {
                if (x_offset >= 0 && x_offset < gridWidth && y_offset >= 0 && y_offset < gridWidth) {
                    colorArray[y_offset * gridWidth + x_offset] = drawColor;
                }
            }
        }

        gridTexture.SetPixels(colorArray);
        gridTexture.Apply();
    }

    public void SetColor(GameObject obj) {
        drawColor = obj.gameObject.GetComponent<Image>().color;
    }

    public void GeneratePuzzleTiles() {
        PuzzleGenerator.Instance.GeneratePuzzleTexture(gridTexture);
    }
}
