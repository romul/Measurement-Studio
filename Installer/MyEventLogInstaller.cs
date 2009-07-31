using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Configuration.Install;


namespace AppInstaller
{
    [RunInstaller(true)]
    public class MyEventLogInstaller : Installer
    {
        private EventLogInstaller myEventLogInstaller;

        public MyEventLogInstaller()
        {
            //Create Instance of EventLogInstaller
            myEventLogInstaller = new EventLogInstaller();

            // Set the Source of Event Log, to be created.
            myEventLogInstaller.Source = "Measurement Studio";

            // Set the Log that source is created in
            myEventLogInstaller.Log = "Application";

            // Add myEventLogInstaller to the Installers Collection.
            Installers.Add(myEventLogInstaller);
        }

        public static void Main()
        {
        }

    }
}
