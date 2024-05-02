using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

partial class SeatruckDoomPlayer : HandTarget
{
    private Transform pictureFrame;
    private Transform screen;
    private Transform handTrigger;
    private GenericHandTarget _oldHandTarget;
    private GenericHandTarget ourHandTarget;
    private DoomPlayerConnection connection;
    private void Start()
    {
        // seatruck modules have no discriminator, they're just prefabs
        pictureFrame = transform.parent.Find("PictureFrame");
        if (!pictureFrame)
        {
            Destroy(this);
            return;
        }
        pictureFrame.GetComponent<PictureFrame>().enabled = false;

        handTrigger = pictureFrame.Find("Trigger");
        _oldHandTarget = handTrigger.GetComponent<GenericHandTarget>();
        _oldHandTarget.enabled = false;

        screen = pictureFrame.transform.Find("Screen");
        screen.GetComponent<MeshRenderer>().enabled = true;

        connection = screen.gameObject.AddComponent<DoomPlayerConnection>();
        connection.Connected += PlayerConnected;
        connection.Disconnected += PlayerDisconnected;
        connection.enabled = false;

        ourHandTarget = handTrigger.gameObject.AddComponent<GenericHandTarget>();
        ourHandTarget.onHandHover = new();
        ourHandTarget.onHandHover.AddListener(OnHandHover);
        ourHandTarget.onHandClick = new();
        ourHandTarget.onHandClick.AddListener(OnHandClick);
    }

    private void OnHandHover(HandTargetEventData eventData)
    {
        if (connection.enabled) return;

        HandReticle.main.SetText(HandReticle.TextType.Hand, $"Play {DoomEngine.Instance.WindowTitle ?? "DOOM"}", false, GameInput.Button.LeftHand);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "", false);
        HandReticle.main.SetIcon(HandReticle.IconType.Interact);
    }

    private void OnHandClick(HandTargetEventData eventData)
    {
        if (connection.enabled) return;

        connection.enabled = true;
    }

    private void PlayerConnected()
    {
        handTrigger.gameObject.SetActive(false);
    }
    private void PlayerDisconnected()
    {
        handTrigger.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (connection.enabled && Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.LeftControl))
        {
            connection.enabled = false;
        }
    }
}
