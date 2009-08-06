﻿using Common;

namespace Two_B7_78_Experiment
{
    public class TSettings : TwoDevicesSettings
    {
        [Text(LineCount = 3, MaxLength = 1000)]
        [SetByUser(Caption = "Команды инициализации", Category = "B7 78/1")]
        public string InitCommands { get; set; }

        [Text(MaxLength = 4, Default = "4557")]
        [SetByUser(Caption = "Id устройства", Category = "B7 78/1")]
        public string DeviceId { get; set; }

    }
}
