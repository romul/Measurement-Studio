using Common;

namespace Two_B7_78_Experiment
{
    [System.CLSCompliant(true)]
    public class TSettings : TwoDevicesSettings
    {
        [Text(LineCount = 3, MaxLength = 1000)]
        [SetByUser(Caption = "Команды инициализации", Category = "1-ое устройство")]
        public string FirstInitCommands { get; set; }

        [Text(MaxLength = 4, Default = "4557")]
        [SetByUser(Caption = "Id устройства", Category = "1-ое устройство")]
        public string FirstDeviceId { get; set; }

        [Text(LineCount = 3, MaxLength = 1000)]
        [SetByUser(Caption = "Команды инициализации", Category = "2-ое устройство")]
        public string SecondInitCommands { get; set; }

        [Text(MaxLength = 4, Default = "4557")]
        [SetByUser(Caption = "Id устройства", Category = "2-ое устройство")]
        public string SecondDeviceId { get; set; }

    }
}
