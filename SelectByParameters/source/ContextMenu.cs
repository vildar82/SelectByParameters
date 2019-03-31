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
    using Lib.Block;
    using Properties;
    using Providers;
    using UI;
    using Exception = System.Exception;

    public static class ContextMenu
    {
        private static DictBlockName _dictBlName;
        private static Dictionary<Type, IGroupProvider> _providers = new Dictionary<Type, IGroupProvider>();
        private static SelectView selView;

        public static void Start()
        {
            var cm = new ContextMenuExtension();
            var menu = new MenuItem(Resources.ContextMenu);
            menu.Click += Click;
            cm.MenuItems.Add(menu);
            Application.AddObjectContextMenuExtension(RXObject.GetClass(typeof(Entity)), cm);
        }

        private static void Click(object sender, EventArgs e)
        {
            try
            {
                var doc = AcadHelper.Doc;
                var sel = doc.Editor.SelectImplied();
                if (sel.Status != PromptStatus.OK) return;
                List<IGroupProvider> groups;
                _dictBlName = new DictBlockName();
                using (var t = doc.TransactionManager.StartTransaction())
                {
                    var ents = sel.Value.GetObjectIds().GetObjects<Entity>()
                        .GroupBy(GetType).ToDictionary(k => k.Key, v => v.ToList());
                    groups = ents.Select(s => GetTypeProvider(s.Key))
                        .Where(w => w != null).OrderBy(o => o.Name).ToList();
                    groups.Insert(0, new CommonProvider(ents));
                    t.Commit();
                }

                var selVm = new SelectVm(groups, _dictBlName);
                selView?.Close();
                selView = new SelectView(selVm);
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowModelessWindow(selView);
            }
            catch (Exception ex)
            {
                ex.Message.WriteLine();
            }
            finally
            {
                _dictBlName = null;
            }
        }

        public static Type GetType(Entity ent)
        {
            return ent.GetType();
        }

        private static IGroupProvider GetTypeProvider(Type type)
        {
            _providers.TryGetValue(type, out var provider);
            return provider;
        }
    }
}
