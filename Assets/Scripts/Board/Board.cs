
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private int rows = 10;
    [SerializeField] private int columns = 10;
    [SerializeField] private int mines = 25;

    [Header("Cell")]
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private float cellSize = 1.0f;

    private Cell[,] cells;
    private int nFlags = 0;

    private void Start()
    {
        // Comprobar que los valores del tablero sean válidos.
        if (rows <= 0 || columns <= 0)
        {
            Debug.LogError("Las filas y columnas deben ser mayores que cero.");
            return;
        }

        if (mines < 0 || mines >= rows * columns)
        {
            Debug.LogError("El número de minas no es válido.");
            return;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("Debes asignar el prefab de Cell en el Inspector.");
            return;
        }

        GenerateBoard();
        PlaceMines();
        CalculateAdjacentMines();
    }

    private void GenerateBoard()
    {
        cells = new Cell[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = transform.position +
                    new Vector3(col * cellSize, row * cellSize, 0);

                Cell cell = Instantiate(
                    cellPrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                // Conservamos los cuatro argumentos de Initialize.
                cell.Initialize(this, row, col, gameObject);

                cells[row, col] = cell;
            }
        }
    }

    private void PlaceMines()
    {
        // Crear una lista con todas las celdas disponibles.
        List<Cell> availableCells = new List<Cell>();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                availableCells.Add(cells[row, col]);
            }
        }

        // Mezclar las celdas aleatoriamente.
        for (int i = availableCells.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            Cell temp = availableCells[i];
            availableCells[i] = availableCells[j];
            availableCells[j] = temp;
        }

        // Asignar una mina a las primeras celdas de la lista.
        for (int i = 0; i < mines; i++)
        {
            availableCells[i].SetMine(true);
        }
    }

    private void CalculateAdjacentMines()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (cells[row, col].IsMine())
                    continue;

                int adjacent = CountAdjacentMines(row, col);

                cells[row, col].SetAdjacentMines(adjacent);
            }
        }
    }

    private int CountAdjacentMines(int row, int col)
    {
        int count = 0;

        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {
                // No contar la propia celda.
                if (rowOffset == 0 && colOffset == 0)
                    continue;

                int neighbourRow = row + rowOffset;
                int neighbourCol = col + colOffset;

                // Ignorar las posiciones fuera del tablero.
                if (neighbourRow < 0 || neighbourRow >= rows ||
                    neighbourCol < 0 || neighbourCol >= columns)
                    continue;

                if (cells[neighbourRow, neighbourCol].IsMine())
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void RevealCell(int row, int column)
    {
        // Comprobar que las coordenadas sean válidas.
        if (cells == null ||
            row < 0 || row >= rows ||
            column < 0 || column >= columns)
            return;

        Cell cell = cells[row, column];

        // No revelar celdas ya reveladas ni celdas con bandera.
        if (cell.IsRevealed() || cell.IsFlaged())
            return;

        cell.SetIsRevealed(true);
        cell.UpdateVisual();

        Debug.Log(
            $"Board recibe: [{row}, {column}] " +
            $"| Mina: {cell.IsMine()} " +
            $"| Minas cercanas: {cell.GetAdjacentMines()}"
        );

        // Si contiene una mina, mostrarla y terminar esta revelación.
        if (cell.IsMine())
            return;

        // Si tiene minas alrededor, mostrar solamente su número.
        if (cell.GetAdjacentMines() > 0)
            return;

        // Si tiene cero minas alrededor, revelar los vecinos.
        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {
                if (rowOffset == 0 && colOffset == 0)
                    continue;

                int neighbourRow = row + rowOffset;
                int neighbourCol = column + colOffset;

                if (neighbourRow < 0 || neighbourRow >= rows ||
                    neighbourCol < 0 || neighbourCol >= columns)
                    continue;

                RevealCell(neighbourRow, neighbourCol);
            }
        }
    }

    public void FlagCell(int row, int column)
    {
        if (cells == null ||
            row < 0 || row >= rows ||
            column < 0 || column >= columns)
            return;

        Cell cell = cells[row, column];

        // No permitir banderas en celdas reveladas.
        if (cell.IsRevealed())
            return;

        if (!cell.IsFlaged())
        {
            // No superar el número total de minas.
            if (nFlags >= mines)
                return;

            cell.SetFlag(true);
            nFlags++;
        }
        else
        {
            // Retirar una bandera existente.
            cell.SetFlag(false);
            nFlags--;
        }

        // Actualizar el sprite de la bandera.
        cell.UpdateVisual();

        Debug.Log($"Banderas colocadas: {nFlags}/{mines}");
    }
}
