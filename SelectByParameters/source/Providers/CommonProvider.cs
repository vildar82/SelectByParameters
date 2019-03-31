namespace SelectByParameters.Providers
{
    using Lib.IO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.DatabaseServices;
    using Data;
    using JetBrains.Annotations;
    using Properties;

    public class CommonProvider : IGroupProvider
    {
        [NotNull] private readonly Dictionary<Type, List<Entity>> typeEnts;
        private Dictionary<Type, Dictionary<ObjectId, bool>> lays;
        private LocalFileData<DataCommon> data;

        public CommonProvider(Dictionary<Type, List<Entity>> typeEnts)
        {
            this.typeEnts = typeEnts;
            data = Path.GetFileData<DataCommon>("SelectByParameters", "CommonProvider");
            data.TryLoad(() => new DataCommon());
            Data = data.Data;
        }

        public DataCommon Data { get; set; }

        public string Name { get; set; } = Resources.CommonGroupName;

        public void Filter(Dictionary<Type, List<Entity>> allItems, Action<Entity> blockEntity)
        {
            InitProps();
            foreach (var typeItems in allItems)
            {
                var defEnts = GetDefEnts(typeItems.Key);
                if (defEnts == null)
                {
                    typeItems.Value.ForEach(blockEntity);
                    continue;
                }

                if (Data.Layer && lays.TryGetValue(typeItems.Key, out var lay))
                {
                    foreach (var entity in typeItems.Value.Where(e => !lay.ContainsKey(e.LayerId)))
                    {
                        blockEntity(entity);
                    }
                }
            }
        }

        public void OnClosed()
        {
            data.TrySave();
        }

        private void InitProps()
        {
            if (Data.Layer) lays = typeEnts.ToDictionary(k => k.Key, v => v.Value.GroupBy(g => g.LayerId).ToDictionary(k => k.Key, w => true));
        }

        private List<Entity> GetDefEnts([NotNull] Type type)
        {
            typeEnts.TryGetValue(type, out var ents);
            return ents;
        }
    }
}
