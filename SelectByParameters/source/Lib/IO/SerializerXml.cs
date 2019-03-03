namespace SelectByParameters.Lib.IO
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using JetBrains.Annotations;

    [PublicAPI]
    public class SerializerXml
    {
        private readonly string _settingsFile;

        public SerializerXml(string settingsFile)
        {
            _settingsFile = settingsFile;
        }

        /// <summary>
        /// Считывание объекта из файла
        /// </summary>
        /// <typeparam name="T">Тип считываемого объекта></typeparam>
        /// <param name="file">Файл xml</param>
        /// <returns>Объект T или null</returns>
        public static T Load<T>(string file)
            where T : class, new()
        {
            var ser = new SerializerXml(file);
            var res = ser.DeserializeXmlFile<T>();
            return res;
        }

        public static T Load<T>(string file, [NotNull] params Type[] types)
            where T : class, new()
        {
            var ser = new SerializerXml(file);
            var res = ser.DeserializeXmlFile<T>(types);
            return res;
        }

        /// <summary>
        /// Сохранение объекта в файл.
        /// При ошибке записывается лог.
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="file">Файл</param>
        /// <param name="obj">Объект</param>
        public static void Save<T>(string file, [NotNull] T obj)
        {
            var ser = new SerializerXml(file);
            ser.SerializeList(obj);
        }

        public static void Save<T>(string file, [NotNull] T obj, [NotNull] params Type[] types)
        {
            var ser = new SerializerXml(file);
            ser.SerializeList(obj, types);
        }

        public T DeserializeXmlFile<T>()
        {
            var ser = new XmlSerializer(typeof(T));
            using (var reader = XmlReader.Create(_settingsFile))
            {
                return (T)ser.Deserialize(reader);
            }
        }

        public T DeserializeXmlFile<T>([NotNull] params Type[] types)
        {
            var ser = new XmlSerializer(typeof(T), types);
            using (var reader = XmlReader.Create(_settingsFile))
            {
                return (T)ser.Deserialize(reader);
            }
        }

        public void SerializeList<T>([NotNull] T settings)
        {
            using (var fs = new FileStream(_settingsFile, FileMode.Create, FileAccess.Write))
            {
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(fs, settings);
            }
        }

        public void SerializeList<T>([NotNull] T settings, [NotNull] params Type[] types)
        {
            using (var fs = new FileStream(_settingsFile, FileMode.Create, FileAccess.Write))
            {
                var ser = new XmlSerializer(typeof(T), types);
                ser.Serialize(fs, settings);
            }
        }
    }
}
