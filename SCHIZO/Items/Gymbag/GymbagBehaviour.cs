using System.Runtime.InteropServices;

namespace SCHIZO.Items.Gymbag;

public sealed class GymbagBehaviour : MonoBehaviour
{
    public static GymbagBehaviour Instance { get; private set; }

    public uGUI_ItemsContainer InventoryUGUI { get; set; }
    public InventoryItem CurrentOpenedRootGymbag { get; set; }
    public bool OpeningGymbag { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OnOpen(InventoryItem item)
    {
        Vector2int cursorPosition = GetCursorPosition();

        PDA pda = Player.main.GetPDA();

        OpeningGymbag = true;
        pda.Close();
        pda.isInUse = false;
        OpeningGymbag = false;

        StorageContainer container = item.item.gameObject.GetComponentInChildren<PickupableStorage>().storageContainer;
        container.Open(container.transform);
        // TODO: fix error
        // container.onUse.Invoke();

        if (PlayerInventoryContains(item))
        {
            if (CurrentOpenedRootGymbag != null)
            {
                CurrentOpenedRootGymbag.isEnabled = true;
                GetItemIcon(CurrentOpenedRootGymbag)?.SetChroma(1f);
            }

            item.isEnabled = false;
            GetItemIcon(item)?.SetChroma(0f);
            CurrentOpenedRootGymbag = item;
        }

        CoroutineHost.StartCoroutine(ResetCursor(cursorPosition));
    }

    public uGUI_ItemIcon GetItemIcon(InventoryItem item)
    {
        return InventoryUGUI.items.GetOrDefault(item, null);
    }

    private static bool PlayerInventoryContains(InventoryItem item)
    {
        IList<InventoryItem> matchingItems = Inventory.main.container.GetItems(item.item.GetTechType());
        return matchingItems != null && matchingItems.Contains(item);
    }

    #region Mouse Position

    private static IEnumerator ResetCursor(Vector2int position)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        SetCursorPosition(position);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }

    private static Vector2int GetCursorPosition()
    {
        GetCursorPos(out Point point);
        return new Vector2int(point.X, point.Y);
    }

    private static void SetCursorPosition(Vector2int position)
    {
        SetCursorPos(position.x, position.y);
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point pos);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    #endregion
}
