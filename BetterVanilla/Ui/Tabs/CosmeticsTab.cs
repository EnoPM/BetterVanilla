using System.Collections;
using AmongUs.Data;
using BetterVanilla.Core;
using BetterVanilla.Ui.Base;
using EnoUnityLoader.Il2Cpp.Utils;
using UnityEngine;

namespace BetterVanilla.Ui.Tabs;

public sealed class CosmeticsTab : TabBase
{
    public OutfitItem outfit = null!;


    protected override void OnEnable()
    {
        base.OnEnable();
        this.StartCoroutine(CoStart());
    }

    private IEnumerator CoStart()
    {
        while (!HatManager.InstanceExists)
        {
            yield return new WaitForEndOfFrame();
        }

        while (DataManager.Player == null || DataManager.Player.Customization == null)
        {
            yield return new WaitForEndOfFrame();
        }
        
        var hatId = DataManager.Player.Customization.Hat;
        outfit.SetHat(hatId);
        
        var visorId = DataManager.Player.Customization.Visor;
        outfit.SetVisor(visorId);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            DebugHatParentPositioning();
        }
    }

    private void DebugHatParentPositioning()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
        {
            Ls.LogMessage("[HatDebug] No local player found");
            return;
        }

        var cosmetics = localPlayer.cosmetics;
        if (cosmetics == null)
        {
            Ls.LogMessage("[HatDebug] No cosmetics layer found");
            return;
        }

        // Get the hat parent
        var hatParent = cosmetics.hat;
        if (hatParent == null)
        {
            Ls.LogMessage("[HatDebug] No hat parent found");
            return;
        }

        // Log HatParent transform info
        Ls.LogMessage($"[HatDebug] === HatParent Transform ===");
        Ls.LogMessage($"[HatDebug] LocalPosition: {hatParent.transform.localPosition}");
        Ls.LogMessage($"[HatDebug] LocalScale: {hatParent.transform.localScale}");
        Ls.LogMessage($"[HatDebug] WorldPosition: {hatParent.transform.position}");

        // Log FrontLayer SpriteRenderer info
        if (hatParent.FrontLayer != null)
        {
            var frontSprite = hatParent.FrontLayer.sprite;
            Ls.LogMessage($"[HatDebug] === FrontLayer ===");
            Ls.LogMessage($"[HatDebug] FrontLayer.localPosition: {hatParent.FrontLayer.transform.localPosition}");
            if (frontSprite != null)
            {
                Ls.LogMessage($"[HatDebug] Sprite.name: {frontSprite.name}");
                Ls.LogMessage($"[HatDebug] Sprite.pivot: {frontSprite.pivot}");
                Ls.LogMessage($"[HatDebug] Sprite.rect: {frontSprite.rect}");
                Ls.LogMessage($"[HatDebug] Sprite.pixelsPerUnit: {frontSprite.pixelsPerUnit}");
                Ls.LogMessage($"[HatDebug] Sprite.bounds: {frontSprite.bounds}");

                // Calculate normalized pivot
                var normalizedPivot = new Vector2(
                    frontSprite.pivot.x / frontSprite.rect.width,
                    frontSprite.pivot.y / frontSprite.rect.height
                );
                Ls.LogMessage($"[HatDebug] NormalizedPivot: {normalizedPivot}");
            }
        }

        // Log BackLayer SpriteRenderer info
        if (hatParent.BackLayer != null)
        {
            var backSprite = hatParent.BackLayer.sprite;
            Ls.LogMessage($"[HatDebug] === BackLayer ===");
            Ls.LogMessage($"[HatDebug] BackLayer.localPosition: {hatParent.BackLayer.transform.localPosition}");
            if (backSprite != null)
            {
                Ls.LogMessage($"[HatDebug] BackSprite.name: {backSprite.name}");
                Ls.LogMessage($"[HatDebug] BackSprite.pivot: {backSprite.pivot}");
            }
        }

        // Log parent body sprite info for reference
        if (hatParent.Parent != null)
        {
            Ls.LogMessage($"[HatDebug] === Parent (Body) ===");
            Ls.LogMessage($"[HatDebug] Parent.localPosition: {hatParent.Parent.transform.localPosition}");
            Ls.LogMessage($"[HatDebug] Parent.bounds: {hatParent.Parent.bounds}");
            if (hatParent.Parent.sprite != null)
            {
                Ls.LogMessage($"[HatDebug] ParentSprite.pivot: {hatParent.Parent.sprite.pivot}");
                Ls.LogMessage($"[HatDebug] ParentSprite.pixelsPerUnit: {hatParent.Parent.sprite.pixelsPerUnit}");
            }
        }

        // Calculate offset from body center to hat
        if (hatParent.Parent != null)
        {
            var bodyBounds = hatParent.Parent.bounds;
            var hatLocalPos = hatParent.transform.localPosition;

            // Offset relative to body top
            var offsetFromBodyTop = hatLocalPos.y - (bodyBounds.size.y / 2f);
            Ls.LogMessage($"[HatDebug] === Calculated Offsets ===");
            Ls.LogMessage($"[HatDebug] Body bounds size: {bodyBounds.size}");
            Ls.LogMessage($"[HatDebug] Hat Y offset from body center: {hatLocalPos.y}");
            Ls.LogMessage($"[HatDebug] Hat Y offset from body top: {offsetFromBodyTop}");
        }
    }

    protected override void SetupTranslation()
    {

    }
}