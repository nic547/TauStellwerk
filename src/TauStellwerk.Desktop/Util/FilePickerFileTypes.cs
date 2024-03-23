// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Platform.Storage;

namespace TauStellwerk.Desktop.Util;

public class FilePickerFileTypes
{
    /// <summary>
    /// Gets a FilePickerFileType similar to <see cref="Avalonia.Platform.Storage.FilePickerFileTypes.ImageAll"/> but with .webp, .avif.
    /// </summary>
    public static FilePickerFileType ImageAllExtended { get; } = new FilePickerFileType("All images")
    {
        Patterns = ["*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.webp", "*.avif"],
        AppleUniformTypeIdentifiers = ["public.image"],
        MimeTypes = ["image/*"],
    };
}
