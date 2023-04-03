using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleTile : MonoBehaviour
{
    public int number;

    public PuzzleTile tile;

    private void Start()
    {
        gameObject.AddComponent<Button>();
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(delegate {
            PuzzleGenerator.Instance.OnTileClicked(tile);
        });
    }

    public void MoveToPosition(Vector2 position)
    {
        transform.localPosition = position;
    }
    

}
