using System;
using System.Collections.ObjectModel;
using SCHIZO.Helpers;

namespace SCHIZO.Tweaks.Doom;

// this is literally just a replay subject
// reactive extensions would make this 1000x better
internal class DoomClientManager(DoomEngine player) : ObservableCollection<IDoomClient>
{
    public void OnInit() => Broadcast(client => client.OnDoomInit());
    public void OnDrawFrame() => Broadcast(client => client.OnDoomFrame());
    public void OnTick() => Broadcast(client => client.OnDoomTick());
    public void OnWindowTitleChanged(string title) => Broadcast(client => client.OnWindowTitleChanged(title));
    public void OnExit(int exitCode) => Broadcast(client => client.OnDoomExit(exitCode));

    private void Broadcast(Action<IDoomClient> action)
    {
        RemoveDeadClients();
        if (Count == 0) return;
        player.RunOnUnityThread(() =>
        {
            // no foreach because collection might change
            for (int i = 0; i < Count; i++)
                action(this[i]);
        });
    }

    protected override void InsertItem(int index, IDoomClient item)
    {
        if (!item.Exists())
            throw new ArgumentNullException(nameof(item));
        if (Contains(item)) return;
        base.InsertItem(index, item);
        item.OnConnected();
        if (player.IsStarted)
            item.OnDoomInit();
    }
    protected override void RemoveItem(int index)
    {
        IDoomClient item = this[index];
        base.RemoveItem(index);

        if (item.Exists())
        {
            item.OnDoomExit(0);
            item.OnDisconnected();
        }
    }

    private void RemoveDeadClients()
    {
        for (int i = 0; i < Count; i++)
        {
            if (!this[i].Exists())
                RemoveAt(i--);
        }
    }
    protected override void ClearItems()
    {
        player.RunOnUnityThread(() =>
        {
            foreach (IDoomClient client in this)
            {
                if (client.Exists())
                    client.OnDisconnected();
            }
            base.ClearItems();
        });
    }
}
