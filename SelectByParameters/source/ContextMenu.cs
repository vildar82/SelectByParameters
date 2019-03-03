namespace SelectByParameters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Runtime;
    using Autodesk.AutoCAD.Windows;
    using Data;
    using Providers;
    using UI;

    public static class ContextMenu
    {
        public static void Start()
        {
            var cm = new ContextMenuExtension();
            var menu = new MenuItem(Properties.Resources.ContextMenu);
            menu.Click += Click;
            cm.MenuItems.Add(menu);
            Application.AddObjectContextMenuExtension(RXObject.GetClass(typeof(Entity)), cm);
        }

        private static void Click(object sender, EventArgs e)
        {
            try
            {
                var doc = AcadHelper.Doc;
                var db = doc.Database;
                var sel = doc.Editor.SelectImplied();
                if (sel.Status != PromptStatus.OK) return;
                List<GroupVm> groups;
                using (var t = doc.TransactionManager.StartTransaction())
                {
                    var ents = sel.Value.GetObjectIds().GetObjects<Entity>().ToList();
                    var commonGroup = new CommonProvider(ents, db, new Options()).GetGroup();
                    groups = ents.GroupBy(g => g.GetType())
                        .Select(GetTypeParams).Where(w => w != null).OrderBy(o => o.Group.Name).ToList();
                    groups.Insert(0, commonGroup);
                    t.Commit();
                }

                var selVm = new SelectVm(groups);
                var selView = new SelectView(selVm);
                Application.ShowModelessWindow(selView);
            }
            catch (System.Exception ex)
            {
                ex.Message.WriteLine();
            }
        }

        private static GroupVm GetTypeParams(IGrouping<Type, Entity> entities)
        {
            return null;
        }
    }
}
