﻿using TOW.Scripts.KludgeBox.VFS.Base;

namespace TOW.Scripts.KludgeBox.VFS;

public static class FsExtensions
{
    public static string GetRealPath(this IProxyFileSystem fileSystem, string path)
    {
        return PathHelper.Combine(fileSystem.RealPath, PathHelper.RemoveRoot(path));
    }
}