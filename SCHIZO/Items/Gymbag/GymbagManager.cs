using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UWE;

namespace SCHIZO.Items.Gymbag;

partial class GymbagManager
{
    public static GymbagManager Instance { get; private set; }

    public uGUI_ItemsContainer InventoryUGUI { get; set; }
    public InventoryItem CurrentOpenedRootGymbag { get; set; }
    public bool OpeningGymbag { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OnOpen(InventoryItem item)
    {
        Vector2Int cursorPosition = GetCursorPosition();

        PDA pda = Player.main.GetPDA();

        OpeningGymbag = true;
        pda.Close();
        pda.isInUse = false;

        StorageContainer container = item.item.gameObject.GetComponentInChildren<PickupableStorage>().storageContainer;
        container.Open(container.transform);

        if (PlayerInventoryContains(item))
        {
            if (CurrentOpenedRootGymbag != null)
            {
                CurrentOpenedRootGymbag.isEnabled = true;
                SetChroma(CurrentOpenedRootGymbag, 1f);
            }

            item.isEnabled = false;
            SetChroma(item, 0);
            CurrentOpenedRootGymbag = item;
        }

        CoroutineHost.StartCoroutine(ResetCursor(cursorPosition));
        OpeningGymbag = false;
    }

    public uGUI_ItemIcon GetItemIcon(InventoryItem item)
    {
        return InventoryUGUI.items.GetOrDefault(item, null);
    }

    public void SetChroma(InventoryItem item, float chroma)
    {
        uGUI_ItemIcon icon = GetItemIcon(item);
        if (icon) icon.SetChroma(chroma);
    }

    private static bool PlayerInventoryContains(InventoryItem item)
    {
        IList<InventoryItem> matchingItems = Inventory.main.container.GetItems(item.item.GetTechType());
        return matchingItems?.Contains(item) == true;
    }

    #region Mouse Position

    private static IEnumerator ResetCursor(Vector2Int position)
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

    private static Vector2Int GetCursorPosition()
    {
        GetCursorPos(out Point point);
        return new Vector2Int(point.X, point.Y);
    }

    private static void SetCursorPosition(Vector2Int position)
    {
        SetCursorPos(position.x, position.y);
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point pos);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    #endregion
}
