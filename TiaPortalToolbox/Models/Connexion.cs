using CommunityToolkit.Mvvm.ComponentModel;

using TiaPortalOpenness.Models.ProjectTree.Plc.Blocks.DataBlocks;

namespace TiaPortalToolbox.Models
{
    public class Connexion(TiaPortalOpenness.Models.ProjectTree.Connexion connexion) : ObservableObject
    {
        public readonly TiaPortalOpenness.Models.ProjectTree.Connexion connexion = connexion;
        private bool selected;
        private string? alarmClassDefaut;

        public string Name => connexion.Name;
        public string HmiName => connexion.HmiName;
        public string PlcName => connexion.PlcName;
        public IEnumerable<string>? AlarmClasses => connexion.AlarmClasses;

        public IEnumerable<DataBlock>? Blocks => connexion.Blocks;
        
        public string? AlarmClassDefault 
        { 
            get => alarmClassDefaut; 
            set => SetProperty(ref alarmClassDefaut, value);
        }

        public bool Selected
        {
            get => selected;
            set => SetProperty(ref selected, value);
        }
    }
}
