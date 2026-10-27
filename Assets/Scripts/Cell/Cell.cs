
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private Board board;
    private int row;
    private int column;

    private bool isMine;
    private int adjacentMines;
    private bool isRevealed;
    private bool isFlaged;

    [Header("Visual de la celda")]
    [SerializeField] private SpriteRenderer cellRenderer;
    [SerializeField] private TextMeshPro cellText;

    [Header("Iconos")]
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Sprite flagSprite;

    [Header("Colores")]
    [SerializeField]
    private Color hiddenColor = new Color(0.65f, 0.65f, 0.65f);

    [SerializeField]
    private Color revealedColor = new Color(0.9f, 0.9f, 0.9f);

    // Inicialización compatible con Board.cs: cuatro argumentos.
    public void Initialize(Board board, int row, int column, GameObject boardObject)
    {
        this.board = board;
        this.row = row;
        this.column = column;

        isMine = false;
        adjacentMines = 0;
        isRevealed = false;
        isFlaged = false;

        // Obtener los componentes visuales si no están asignados.
        if (cellRenderer == null)
            cellRenderer = GetComponent<SpriteRenderer>();

        if (cellText == null)
            cellText = GetComponentInChildren<TextMeshPro>();

        // Conservamos GetComponent para acceder al componente Board.
        if (boardObject != null)
        {
            Board boardComponent = boardObject.GetComponent<Board>();

            if (boardComponent != null)
                this.board = boardComponent;
        }

        UpdateVisual();
    }

    public void Interact()
    {
        if (board != null)
            board.RevealCell(row, column);
    }

    public void Flag()
    {
        if (board != null)
            board.FlagCell(row, column);
    }

    public void UpdateVisual()
    {
        // Actualizar el color del fondo.
        if (cellRenderer != null)
        {
            cellRenderer.color = isRevealed
                ? revealedColor
                : hiddenColor;
        }

        // Mostrar el número de minas cercanas.
        if (cellText != null)
        {
            cellText.text = "";

            if (isRevealed && !isMine && adjacentMines > 0)
            {
                cellText.text = adjacentMines.ToString();
                cellText.color = GetNumberColor(adjacentMines);
            }
        }

        // Mostrar las imágenes de bombas y banderas.
        if (iconRenderer != null)
        {
            iconRenderer.sprite = null;
            iconRenderer.enabled = false;

            if (isRevealed && isMine)
            {
                iconRenderer.sprite = bombSprite;
                iconRenderer.enabled = bombSprite != null;
            }
            else if (isFlaged && !isRevealed)
            {
                iconRenderer.sprite = flagSprite;
                iconRenderer.enabled = flagSprite != null;
            }
        }
    }

    private Color GetNumberColor(int number)
    {
        switch (number)
        {
            case 1: return Color.blue;
            case 2: return new Color(0f, 0.5f, 0f);
            case 3: return Color.red;
            case 4: return new Color(0f, 0f, 0.5f);
            case 5: return new Color(0.5f, 0f, 0f);
            case 6: return Color.cyan;
            case 7: return Color.black;
            default: return Color.gray;
        }
    }

    #region Setters

    public void SetRow(int row)
    {
        this.row = row;
    }

    public void SetColumn(int column)
    {
        this.column = column;
    }

    public void SetBoard(Board board)
    {
        this.board = board;
    }

    public void SetMine(bool isMine)
    {
        this.isMine = isMine;
    }

    public void SetFlag(bool flag)
    {
        isFlaged = flag;
    }

    public void SetIsRevealed(bool isRevealed)
    {
        this.isRevealed = isRevealed;
    }

    public void SetAdjacentMines(int adjacentMines)
    {
        this.adjacentMines = adjacentMines;
    }

    #endregion

    #region Getters

    public int GetRow()
    {
        return row;
    }

    public int GetColumn()
    {
        return column;
    }

    public Board GetBoard()
    {
        return board;
    }

    public bool IsMine()
    {
        return isMine;
    }

    public bool IsFlaged()
    {
        return isFlaged;
    }

    public bool IsRevealed()
    {
        return isRevealed;
    }

    public int GetAdjacentMines()
    {
        return adjacentMines;
    }

    #endregion
}
