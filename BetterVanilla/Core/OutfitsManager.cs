using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmongUs.Data;
using AmongUs.Data.Player;
using BetterVanilla.Extensions;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Core;

public static class OutfitsManager
{
    public static readonly List<Outfit> AllOutfits = [];

    static OutfitsManager()
    {
        if (!File.Exists(ModPaths.SavedOutfitsFile2))
        {
            return;
        }
        using var file = File.OpenRead(ModPaths.SavedOutfitsFile2);
        using var reader = new BinaryReader(file);
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var outfit = new Outfit(reader);
            AllOutfits.Add(outfit);
        }
    }

    public static bool CurrentOutfitIsSaved => AllOutfits.Any(x => x.IsEquipped);

    public static void Delete(Outfit outfit)
    {
        AllOutfits.Remove(outfit);
        Save();
    }

    public static Outfit? SaveEquipped()
    {
        if (CurrentOutfitIsSaved) return null;
        var player = PlayerControl.LocalPlayer;
        if (player == null || player.Data == null || player.Data.DefaultOutfit == null) return null;
        var outfit = new Outfit(player.Data.DefaultOutfit);
        if (AllOutfits.Any(x => x.IsSame(outfit))) return null;
        AllOutfits.Add(outfit);
        Save();
        return outfit;
    }

    private static void Save()
    {
        using var file = File.Create(ModPaths.SavedOutfitsFile2);
        using var writer = new BinaryWriter(file);
        writer.Write(AllOutfits.Count);
        foreach (var outfit in AllOutfits)
        {
            outfit.Write(writer);
        }
    }
    
    public class Outfit
    {
        public int ColorId { get; }
        public Color VisorColor { get; }
        public string HatId { get; }
        public string SkinId { get; }
        public string VisorId { get; }
        public string PetId { get; }
        public string NameplateId { get; }

        public Outfit(NetworkedPlayerInfo.PlayerOutfit outfit)
        {
            ColorId = outfit.ColorId;
            VisorColor = SponsorOptions.Default.VisorColor.Value;
            HatId = outfit.HatId;
            SkinId = outfit.SkinId;
            VisorId = outfit.VisorId;
            PetId = outfit.PetId;
            NameplateId = outfit.NamePlateId;
        }

        public Outfit(BinaryReader reader)
        {
            ColorId = reader.ReadInt32();
            VisorColor = reader.ReadColor();
            HatId = reader.ReadString();
            SkinId = reader.ReadString();
            VisorId = reader.ReadString();
            PetId = reader.ReadString();
            NameplateId = reader.ReadString();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(ColorId);
            writer.Write(VisorColor);
            writer.Write(HatId);
            writer.Write(SkinId);
            writer.Write(VisorId);
            writer.Write(PetId);
            writer.Write(NameplateId);
        }

        public bool IsSame(Outfit outfit)
        {
            return ColorId == outfit.ColorId
                   && Mathf.Approximately(VisorColor.r, outfit.VisorColor.r)
                   && Mathf.Approximately(VisorColor.g, outfit.VisorColor.g)
                   && Mathf.Approximately(VisorColor.b, outfit.VisorColor.b)
                   && Mathf.Approximately(VisorColor.a, outfit.VisorColor.a)
                   && HatId == outfit.HatId
                   && SkinId == outfit.SkinId
                   && VisorId == outfit.VisorId
                   && PetId == outfit.PetId
                   && NameplateId == outfit.NameplateId;
        }

        public bool IsEquipped => DataManager.Player?.Customization == null || IsSame(DataManager.Player.Customization);

        private bool IsSame(PlayerCustomizationData outfit)
        {
            var visorColor = SponsorOptions.Default.VisorColor.Value;
            return ColorId == outfit.Color
                   && Mathf.Approximately(visorColor.r, VisorColor.r)
                   && Mathf.Approximately(visorColor.g, VisorColor.g)
                   && Mathf.Approximately(visorColor.b, VisorColor.b)
                   && Mathf.Approximately(visorColor.a, VisorColor.a)
                   && HatId == outfit.Hat
                   && SkinId == outfit.Skin
                   && VisorId == outfit.Visor
                   && PetId == outfit.Pet
                   && NameplateId == outfit.NamePlate;
        }
    }
}