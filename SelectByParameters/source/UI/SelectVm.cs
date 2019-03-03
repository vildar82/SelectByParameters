using System.ComponentModel;

namespace SelectByParameters.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Lib.Mvvm;
    using Model;

    public class SelectVm : BaseViewModel
    {
        private readonly SelectModel selModel;

        public SelectVm(List<GroupVm> groups)
        {
            Groups = new ObservableCollection<GroupVm>(groups);
            FromSelected = new RelayCommand(FromSelectedExec);
            SelectAll = new RelayCommand(SelectAllExec);
            selModel = new SelectModel(Groups);
            Show = new RelayCommand(ShowExec);
            PropertyChanged += PropChanges;
        }

        public ObservableCollection<GroupVm> Groups { get; set; }

        public List<SelectedItem> SelIds { get; set; }

        public RelayCommand FromSelected { get; set; }

        public RelayCommand SelectAll { get; set; }

        public RelayCommand Show { get; set; }

        public SelectedItem SelId { get; set; }

        private void FromSelectedExec(object obj)
        {
            var doc = AcadHelper.Doc;
            using (HideWindow())
            using (doc.LockDocument())
            {
                doc.Editor.SetImpliedSelection(new ObjectId[] { });
                var selRes = doc.Editor.GetSelection();
                if (selRes.Status == PromptStatus.OK)
                    SelIds = selModel.FromSelected(selRes.Value.GetObjectIds());
            }
        }

        private void SelectAllExec(object obj)
        {
            SelIds = selModel.SelectAll();
        }

        private void ShowExec(object obj)
        {
            if (obj is SelectedItem selectedItem)
            {
                selectedItem.Id.ShowEnt(false);
            }
        }

        private void PropChanges(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelId))
            {
                ShowExec(SelId);
            }
        }
    }
}
