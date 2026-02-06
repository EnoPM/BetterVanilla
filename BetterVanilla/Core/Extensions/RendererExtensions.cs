using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class RendererExtensions
{
    extension(Renderer renderer)
    {
        public void SetVisorColor(Color color)
        {
            if (renderer.material.GetColor(PlayerMaterial.VisorColor) == color) return;
            renderer.material.SetColor(PlayerMaterial.VisorColor, color);
        }
    }
}