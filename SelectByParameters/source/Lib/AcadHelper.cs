using Autodesk.AutoCAD.Internal;
using SelectByParameters.Properties;

namespace SelectByParameters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Geometry;
    using JetBrains.Annotations;
    using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

    public static class AcadHelper
    {
        public const string AppName = "Vildar";

        [NotNull]
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
        public static T GetObject<T>(this ObjectId id, OpenMode mode = OpenMode.ForRead)
            where T : DBObject
        {
            if (!id.IsValidEx())
                return null;
            return id.GetObject(mode, false, true) as T;
        }

        [NotNull]
        public static T GetObjectT<T>(this ObjectId id, OpenMode mode = OpenMode.ForRead)
            where T : DBObject
        {
            if (!id.IsValidEx()) throw new InvalidOperationException();
            return (T) id.GetObject(mode, false, true);
        }

        /// <summary>
        /// Сообщение в ком.строку. автокада
        /// </summary>
        public static void WriteLine(this string msg)
        {
            try
            {
                Doc.Editor.WriteMessage($"\n{Resources.ContextMenu}: {msg}");
            }
            catch
            {
                // Может не быть открытого чертежа и командной строки.
            }
        }

        public static List<string> GetLayers(this Database db)
        {
            return db.LayerTableId.GetObject<LayerTable>().GetObjects<LayerTableRecord>().Select(s => s.Name)
                .OrderBy(o => o).ToList();
        }

        public static void ShowEnt(this ObjectId id, bool flick = true, int num = 2, int delay1 = 100, int delay2 = 100)
        {
            var doc = DocumentManager.MdiActiveDocument;
            if (doc == null || !id.IsValidEx())
                return;
            using (doc.LockDocument())
            using (var t = id.Database.TransactionManager.StartTransaction())
            {
                if (id.GetObject(OpenMode.ForRead) is Entity ent)
                {
                    try
                    {
                        doc.Editor.Zoom(ent.GeometricExtents.Offset());
                        if (flick) id.FlickObjectHighlight(num, delay1, delay2);
                        doc.Editor.SetImpliedSelection(new[] { id });
                    }
                    catch
                    {
                        //
                    }
                }

                t.Commit();
            }

            Utils.SetFocusToDwgView();
        }

        public static void Zoom([CanBeNull] this Editor ed, Extents3d ext)
        {
            if (ed == null)
                return;
            using (var view = ed.GetCurrentView())
            {
                ext.TransformBy(view.WorldToEye());
                view.Width = ext.MaxPoint.X - ext.MinPoint.X;
                view.Height = ext.MaxPoint.Y - ext.MinPoint.Y;
                view.CenterPoint = new Point2d(
                    (ext.MaxPoint.X + ext.MinPoint.X) / 2.0,
                    (ext.MaxPoint.Y + ext.MinPoint.Y) / 2.0);
                ed.SetCurrentView(view);
            }
        }

        public static void ZoomExtents([CanBeNull] this Editor ed)
        {
            if (ed == null)
                return;
            var db = ed.Document.Database;
            var ext = (short)Application.GetSystemVariable("cvport") == 1
                ? new Extents3d(db.Pextmin, db.Pextmax)
                : new Extents3d(db.Extmin, db.Extmax);
            ed.Zoom(ext);
        }

        public static Matrix3d EyeToWorld([NotNull] this ViewTableRecord view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            return
                Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) *
                Matrix3d.Displacement(view.Target - Point3d.Origin) *
                Matrix3d.PlaneToWorld(view.ViewDirection);
        }

        public static Matrix3d WorldToEye([NotNull] this ViewTableRecord view)
        {
            return view.EyeToWorld().Inverse();
        }

        public static Extents3d Offset(this Extents3d ext, double percent = 50)
        {
            var dX = ext.GetLength() * (percent / 100) * 0.5;
            var dY = ext.GetHeight() * (percent / 100) * 0.5;
            return new Extents3d(
                new Point3d(ext.MinPoint.X - dX, ext.MinPoint.Y - dY, 0),
                new Point3d(ext.MaxPoint.X + dX, ext.MaxPoint.Y + dY, 0)
            );
        }

        /// <summary>
        /// Расстояние по Y
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static double GetHeight(this Extents3d ext)
        {
            return ext.MaxPoint.Y - ext.MinPoint.Y;
        }

        /// <summary>
        /// Расстояние по X
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static double GetLength(this Extents3d ext)
        {
            return ext.MaxPoint.X - ext.MinPoint.X;
        }

        /// <summary>
        /// Функция производит "мигание" объектом при помощи Highlight/Unhighlight
        /// </summary>
        /// <param name="id">ObjectId для примитива</param>
        /// <param name="num">Количество "миганий"</param>
        /// <param name="delay1">Длительность "подсвеченного" состояния</param>
        /// <param name="delay2">Длительность "неподсвеченного" состояния</param>
        public static void FlickObjectHighlight(this ObjectId id, int num, int delay1, int delay2)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            for (var i = 0; i < num; i++)
            {
                // Highlight entity
                using (doc.LockDocument())
                using (var tr = doc.TransactionManager.StartTransaction())
                {
                    var en = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    var ids = new ObjectId[1];
                    ids[0] = id;
                    var index = new SubentityId(SubentityType.Null, IntPtr.Zero);
                    var path = new FullSubentityPath(ids, index);
                    en.Highlight(path, true);
                    tr.Commit();
                }

                doc.Editor.UpdateScreen();

                // Wait for delay1 msecs
                Thread.Sleep(delay1);

                // Unhighlight entity
                using (doc.LockDocument())
                using (var tr = doc.TransactionManager.StartTransaction())
                {
                    var en = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    var ids = new ObjectId[1];
                    ids[0] = id;
                    var index = new SubentityId(SubentityType.Null, IntPtr.Zero);
                    var path = new FullSubentityPath(ids, index);
                    en.Unhighlight(path, true);
                    tr.Commit();
                }

                doc.Editor.UpdateScreen();

                // Wait for delay2 msecs
                Thread.Sleep(delay2);
            }
        }

        /// <summary>
        /// Функция производит "мигание" подобъектом при помощи Highlight/Unhighlight
        /// </summary>
        /// <param name="idsPath">Цепочка вложенности объектов. BlockReference->Subentity</param>
        /// <param name="num">Количество "миганий"</param>
        /// <param name="delay1">Длительность "подсвеченного" состояния</param>
        /// <param name="delay2">Длительность "неподсвеченного" состояния</param>
        public static void FlickSubentityHighlight(ObjectId[] idsPath, int num, int delay1, int delay2)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            for (var i = 0; i < num; i++)
            {
                // Highlight entity
                using (doc.LockDocument())
                using (var tr = doc.TransactionManager.StartTransaction())
                {
                    var subId = new SubentityId(SubentityType.Null, IntPtr.Zero);
                    var path = new FullSubentityPath(idsPath, subId);
                    var ent = (Entity)idsPath[0].GetObject(OpenMode.ForRead);
                    ent.Highlight(path, true);
                    tr.Commit();
                }

                doc.Editor.UpdateScreen();

                // Wait for delay1 msecs
                Thread.Sleep(delay1);

                // Unhighlight entity
                using (doc.LockDocument())
                using (var tr = doc.TransactionManager.StartTransaction())
                {
                    var subId = new SubentityId(SubentityType.Null, IntPtr.Zero);
                    var path = new FullSubentityPath(idsPath, subId);
                    var ent = (Entity)idsPath[0].GetObject(OpenMode.ForRead);
                    ent.Unhighlight(path, true);
                    tr.Commit();
                }

                doc.Editor.UpdateScreen();

                // Wait for delay2 msecs
                Thread.Sleep(delay2);
            }
        }

        /// <summary>
        /// Имя блока в том числе динамического.
        /// Без условия открытой транзакции.
        /// br.DynamicBlockTableRecord.Open(OpenMode.ForRead)
        /// </summary>
        public static string GetEffectiveName([NotNull] this BlockReference br)
        {
            using (var btrDyn = (BlockTableRecord)br.DynamicBlockTableRecord.Open(OpenMode.ForRead))
            {
                return btrDyn.Name;
            }
        }
    }
}
