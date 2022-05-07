using System;
using System.IO;
using System.Linq;

namespace Logicality.Bullseye;

/// <summary>
/// Set of helper functions typically used in bullseye build programs.
/// </summary>
public static class BullseyeUtils
{
    /// <summary>
    /// Deletes all files (except .gitignore) and subdirectories from specified path.
    /// </summary>
    /// <param name="path">The path whose files and subdirectories will be deleted</param>
    public static void CleanDirectory(string path)
    {
        var filesToDelete = Directory
            .GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith(".gitignore"));
        foreach (var file in filesToDelete)
        {
            Console.WriteLine($"Deleting file {file}");
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        var directoriesToDelete = Directory.GetDirectories(path);
        foreach (var directory in directoriesToDelete)
        {
            Console.WriteLine($"Deleting directory {directory}");
            Directory.Delete(directory, true);
        }
    }
}