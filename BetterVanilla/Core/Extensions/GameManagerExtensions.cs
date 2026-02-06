using System.Collections.Generic;
using BetterVanilla.Options;

namespace BetterVanilla.Core.Extensions;

public static class GameManagerExtensions
{
    extension(GameManager gameManager)
    {
        public List<RulesCategory> GetAllCategories()
        {
            var results = new List<RulesCategory>
            {
                HostOptions.Default.MenuCategory
            };

            foreach (var category in gameManager.GameSettingsList.AllCategories)
            {
                results.Add(category);
            }

            return results;
        }
    }
}