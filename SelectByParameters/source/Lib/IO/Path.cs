namespace SelectByParameters.Lib.IO
{
    using System;
    using JetBrains.Annotations;

    public static class Path
    {
        [NotNull]
        public static string GetUserDataFile([NotNull] string plugin, [NotNull] string name)
        {
            return System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AcadHelper.AppName,
                plugin,
                name);
        }

        public static LocalFileData<T> GetFileData<T>([NotNull] string plugin, [NotNull] string name)
            where T : class, new()
        {
            return new LocalFileData<T>(GetUserDataFile(plugin, name + ".xml"));
        }
    }
}
