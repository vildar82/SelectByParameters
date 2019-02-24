using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;

namespace SelectByParameters
{
    using System;
    using Autodesk.AutoCAD.ApplicationServices;
    using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

    public static class AcadHelper
    {
        public static Document Doc => DocumentManager.MdiActiveDocument ?? throw new InvalidOperationException();

        public static bool IsValidEx(this ObjectId id)
        {
            return id.IsValid && !id.IsNull && !id.IsErased;
        }

        [NotNull]
        public static IEnumerable<T> GetObjects<T>([NotNull] this IEnumerable ids, OpenMode mode = OpenMode.ForRead)
            where T : DBObject
        {
            return ids
                .Cast<ObjectId>()
                .Select(id => id.GetObject<T>(mode))
                .Where(res => res != null);
        }

        [CanBeNull]
        public static T GetObject<T>(this ObjectId id, OpenMode mode)
            where T : DBObject
        {
            if (!id.IsValidEx())
                return null;
            return id.GetObject(mode, false, true) as T;
        }
    }
}
