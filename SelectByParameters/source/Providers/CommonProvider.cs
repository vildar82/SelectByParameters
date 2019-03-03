using SelectByParameters.UI.Values;

namespace SelectByParameters.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.DatabaseServices;
    using Data;
    using Properties;
    using UI;

    public class CommonProvider
    {
        private readonly List<Entity> ents;
        private readonly Database db;
        private Options options;
        private GroupVm groupVm;

        public CommonProvider(List<Entity> ents, Database db, Options options)
        {
            this.ents = ents;
            this.db = db;
            this.options = options;
        }

        public GroupVm GetGroup()
        {
            groupVm = new GroupVm { Group = new Data.Group { Name = Resources.CommonGroupName } };
            AddParameter(GetLayer());
            return groupVm;
        }

        private ParameterVm GetLayer()
        {
            var layersVm = new StringListVm(db.GetLayers());
            var paramLayVm = new ParameterVm
            {
                Parameter = new Parameter { Name = Resources.PropLayerName },
                Control = new StrinListControl(layersVm)
            };
            var ent = ents[0];
            layersVm.Value = ents.All(e => e.LayerId == ent.LayerId) ? ent.Layer : layersVm.Values[0];
            return paramLayVm;
        }

        private void AddParameter(ParameterVm parameterVm)
        {
            groupVm.Group.Parameters.Add(parameterVm.Parameter);
            groupVm.Parameters.Add(parameterVm);
        }
    }
}
