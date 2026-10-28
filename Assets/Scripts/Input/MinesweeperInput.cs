
using UnityEngine;
using UnityEngine.InputSystem;

public class MinesweeperInput : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        // Obtener la cámara principal si no se ha asignado.
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (Mouse.current == null || mainCamera == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Buscar la celda en el objeto impactado.
            Cell cell = hit.collider.GetComponent<Cell>();

            // Si el Collider está en un objeto hijo,
            // buscar también en sus padres.
            if (cell == null)
                cell = hit.collider.GetComponentInParent<Cell>();

            if (cell != null)
            {
                cell.Interact();
            }
        }
    }

    public void OnFlag(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (Mouse.current == null || mainCamera == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Cell cell = hit.collider.GetComponent<Cell>();

            if (cell == null)
                cell = hit.collider.GetComponentInParent<Cell>();

            if (cell != null)
            {
                cell.Flag();
            }
        }
    }
}
