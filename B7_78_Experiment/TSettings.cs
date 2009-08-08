using Common;

namespace B7_78_Experiment
{
    [System.CLSCompliant(true)]
    public class TSettings : AbstractSettings
    {
        [Text(LineCount = 3, MaxLength=1000)]
        [SetByUser(Caption = "Команды инициализации")]
        public string InitCommands { get; set; }

        [Text(MaxLength=4, Default = "4557")]
        [SetByUser(Caption = "Id устройства")]
        public string DeviceId { get; set; }
    }
}
