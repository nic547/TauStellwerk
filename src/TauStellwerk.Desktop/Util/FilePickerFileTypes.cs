// <copyright file="FilePickerFileTypes.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia.Platform.Storage;

namespace TauStellwerk.Desktop.Util;
public class FilePickerFileTypes
{
    /// <summary>
    /// Gets a FilePickerFileType similar to <see cref="Avalonia.Platform.Storage.FilePickerFileTypes.ImageAll"/> but with .webp, .avif.
    /// </summary>
    public static FilePickerFileType ImageAllExtended { get; } = new FilePickerFileType("All images")
    {
        Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.webp", "*.avif" },
        AppleUniformTypeIdentifiers = new[] { "public.image" },
        MimeTypes = new[] { "image/*" },
    };
}
