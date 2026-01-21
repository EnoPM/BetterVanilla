using System;
using System.Reflection;
using Cpp2IL.Core.Extensions;

namespace BetterVanilla.Extensions;

public static class AssemblyExtensions
{
    extension(Assembly assembly)
    {
        public byte[] GetResourceBytes(string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                return stream.ReadBytes();
            }
            throw new InvalidOperationException($"Unable to find embedded resource: {resourceName} in assembly {assembly.FullName}");
        }
    }
}