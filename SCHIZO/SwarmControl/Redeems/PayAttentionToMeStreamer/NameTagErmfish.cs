using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;
using SCHIZO.Twitch;
using SwarmControl.Models.Game.Messages;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.PayAttentionToMeStreamer;

#nullable enable
[Redeem(
    Name = "redeem_nametag",
    DisplayName = "Your Ermfish",
    Description = "Put your name on an ermfish!"
)]
internal class NameTagErmfish : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];
    private TechType _ermfishTechType;
    private static GameObject? _ermfishPrefab;

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Error("Requires a loaded game");

        if (_ermfishTechType is default(TechType))
        {
            try
            {
                _ermfishTechType = (TechType) Enum.Parse(typeof(TechType), "ermfish");
            }
            catch (ArgumentException)
            {
                return CommonResults.Error("Internal error - no ermfish techtype");
            }
        }

        GameObject ermfish = PhysicsHelpers.ObjectsInRange(Player.main.transform.position, 100)
            .OfTechType(_ermfishTechType)
            .SelectComponentInParent<PrefabIdentifier>() // sphere cast hits the collider which is on the model
            .Select(tag => tag.gameObject)
            .FirstOrDefault(erm =>
            {
                NameTag tag = erm.GetComponentInChildren<NameTag>(true);
                return tag && !tag.isActiveAndEnabled;
            });

        RemoteInput? input = ctx.Input as RemoteInput;
        TwitchUser? user = input?.Model.User;
        if (user is not { DisplayName: { } dispName })
            return CommonResults.Error("Could not get username");

        if (ermfish)
        {
            CoroutineHost.StartCoroutine(SetNameTag(ermfish, user));
            return CommonResults.OK();
        }
        else
        {
            CoroutineHost.StartCoroutine(SpawnCoro(user));
            return "We could not find a free ermfish so we will spawn one for you when it's safe to spawn stuff.";
        }
    }

    private IEnumerator SpawnCoro(TwitchUser? user)
    {
        if (!_ermfishPrefab)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(CraftData.GetClassIdForTechType(_ermfishTechType));
            yield return request;
            if (request.TryGetPrefab(out GameObject prefab))
                _ermfishPrefab = prefab;
            else
                yield break;
        }
        yield return new WaitUntil(() => SwarmControlManager.Instance.CanSpawn);

        GameObject ermfish = Utils.CreatePrefab(_ermfishPrefab);
        yield return null;
        yield return SetNameTag(ermfish, user);
    }

    private IEnumerator SetNameTag(GameObject ermfish, TwitchUser? user)
    {
        if (user is not { DisplayName: { } dispName }) yield break;

        NameTag tag = ermfish.GetComponentInChildren<NameTag>(true);
        if (!tag || tag.isActiveAndEnabled) yield break;

        tag.textMesh.text = dispName;
        tag.gameObject.SetActive(true);

        Task<Color> task = TwitchIntegration.GetUserChatColor(user.Id);
        yield return task.PoorMansAwait();
        tag.textMesh.color = task.Result;
    }
}
