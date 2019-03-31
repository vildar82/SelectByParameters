namespace SelectByParameters.Model
{
    using Providers;
    using JetBrains.Annotations;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.AutoCAD.Runtime;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Internal;
    using Lib.Block;
    using UI;

    public class SelectModel
    {
        private static RXObject blRefClass = RXObject.GetClass(typeof(BlockReference));
        private readonly ObservableCollection<IGroupProvider> groups;
        private readonly DictBlockName dictBlName;
        private Dictionary<ObjectId, string> dictBlRefNames = new Dictionary<ObjectId, string>();
        private Document doc;

        public SelectModel(ObservableCollection<IGroupProvider> groups, DictBlockName dictBlName)
        {
            this.groups = groups;
            this.dictBlName = dictBlName;
        }

        public List<SelectedItem> FromSelected(ObjectId[] ids)
        {
            doc = AcadHelper.Doc;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                var selIds = Select(ids);
                t.Commit();
                return selIds;
            }
        }

        public List<SelectedItem> SelectAll()
        {
            doc = AcadHelper.Doc;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                var selIds = Select(doc.Database.CurrentSpaceId.GetObjectT<BlockTableRecord>().Cast<ObjectId>());
                t.Commit();
                return selIds;
            }
        }

        private List<SelectedItem> Select([NotNull] IEnumerable<ObjectId> ids)
        {
            if (!ids.Any()) return new List<SelectedItem>();
            var selIds = new HashSet<ObjectId>();
            foreach (var id in ids) selIds.Add(id);
            var allItems = ids.GetObjects<Entity>().GroupBy(ContextMenu.GetType).ToDictionary(k => k.Key, v => v.ToList());
            foreach (var @group in groups) group.Filter(allItems, e => selIds.Remove(e.Id));
            if (!selIds.Any())
            {
                "Не найдено соответствий".WriteLine();
                return null;
            }

            var selItems = selIds.Select(s => new SelectedItem
            {
                Name = GetEntityName(s),
                Id = s
            }).ToList();
            doc.Editor.SetImpliedSelection(selItems.Select(s => s.Id).ToArray());
            Utils.SetFocusToDwgView();
            return selItems;
        }

        private string GetEntityName(ObjectId entId)
        {
            return entId.ObjectClass == blRefClass ? GetBlRefName(entId) : entId.ObjectClass.DxfName;
        }

        private string GetBlRefName(ObjectId blRefId)
        {
            if (!dictBlRefNames.TryGetValue(blRefId, out var name))
            {
                using (var blRef = blRefId.Open(OpenMode.ForRead, false, true) as BlockReference)
                {
                    name = dictBlName.GetName(blRef);
                    dictBlRefNames.Add(blRefId, name);
                }
            }

            return name;
        }
    }
}
