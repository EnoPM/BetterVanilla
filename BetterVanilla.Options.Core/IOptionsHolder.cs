using System.Collections.Generic;
using System.IO;

namespace BetterVanilla.Options.Core;

public interface IOptionsHolder
{
    IEnumerable<OptionBase> GetAllOptions();
    void Save(BinaryWriter writer);
    void Load(BinaryReader reader);
    void ResetAll();
}
