namespace SelectByParameters.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Internal;
    using UI;

    public class SelectModel
    {
        private readonly ObservableCollection<GroupVm> _groups;
        private Document doc;
        private GroupVm groupCommon;

        public SelectModel(ObservableCollection<GroupVm> groups)
        {
            _groups = groups;
        }

        public List<SelectedItem> FromSelected(ObjectId[] ids)
        {
            doc = AcadHelper.Doc;
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

        private List<SelectedItem> Select(IEnumerable<ObjectId> ids)
        {
            groupCommon = _groups[0];
            var selIds = ids.GetObjects<Entity>().Where(CommonFilter).Select(s => s.Id).ToArray();
            if (!selIds.Any())
            {
                "Не найдено соответствий".WriteLine();
                return null;
            }

            doc.Editor.SetImpliedSelection(selIds);
            Utils.SetFocusToDwgView();
            return selIds.Select(GetSelItem).ToList();
        }

        private SelectedItem GetSelItem(ObjectId objectId)
        {
            return new SelectedItem
            {
                Name = objectId.ObjectClass.Name,
                Id = objectId
            };
        }

        private bool CommonFilter(Entity entity)
        {
            return true;
        }
    }
}
