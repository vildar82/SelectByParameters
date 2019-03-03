namespace SelectByParameters.Lib.IO
{
    using System;
    using System.IO;
    using JetBrains.Annotations;

    /// <summary>
    /// Данные хранимые в файле json локально
    /// </summary>
    [PublicAPI]
    public class LocalFileData<T>
        where T : class, new()
    {
        private DateTime fileLastWrite;
        public readonly string LocalFile;

        public LocalFileData([NotNull] string localFile)
        {
            LocalFile = localFile;
        }

        public T Data { get; set; }

        public bool HasChanges()
        {
            return File.GetLastWriteTime(LocalFile) > fileLastWrite;
        }

        /// <summary>
        ///
        /// </summary>
        public void Load()
        {
            Data = Deserialize();
            fileLastWrite = File.GetLastWriteTime(LocalFile);
        }

        public void Save()
        {
            Serialize();
        }

        public void TryLoad(Func<T> onError)
        {
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                Data = onError();
                if (!(ex is FileNotFoundException))
                {
                    ex.Message.WriteLine();
                }
            }
        }

        public void TryLoad()
        {
            try
            {
                Load();
            }
            catch (FileNotFoundException)
            {
                // Файл не найден
            }
            catch (Exception ex)
            {
                ex.Message.WriteLine();
            }
        }

        public void TrySave()
        {
            try
            {
                Save();
            }
            catch (Exception ex)
            {
                ex.Message.WriteLine();
            }
        }

        private T Deserialize()
        {
            if (!File.Exists(LocalFile))
                throw new FileNotFoundException(LocalFile);
            return SerializerXml.Load<T>(LocalFile);
        }

        private void Serialize()
        {
            SerializerXml.Save(LocalFile, Data);
        }
    }
}
