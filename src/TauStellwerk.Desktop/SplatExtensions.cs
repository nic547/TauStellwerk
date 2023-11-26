// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Splat;

namespace TauStellwerk.Desktop;

public static class SplatExtensions
{
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver dependencyResolver) where T : class
    {
        return dependencyResolver.GetService<T>() ?? throw new InvalidOperationException($"Failed to locate an instance of {typeof(T)}");
    }
}
