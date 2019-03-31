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
        private readonly string localFile;

        public LocalFileData([NotNull] string localFile)
        {
            this.localFile = localFile;
        }

        public T Data { get; set; }

        public bool HasChanges()
        {
            return File.GetLastWriteTime(localFile) > fileLastWrite;
        }

        /// <summary>
        ///
        /// </summary>
        public void Load()
        {
            Data = Deserialize();
            fileLastWrite = File.GetLastWriteTime(localFile);
        }

        public void Save()
        {
            var dir = System.IO.Path.GetDirectoryName(localFile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
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
            if (!File.Exists(localFile))
                throw new FileNotFoundException(localFile);
            return SerializerXml.Load<T>(localFile);
        }

        private void Serialize()
        {
            SerializerXml.Save(localFile, Data);
        }
    }
}
