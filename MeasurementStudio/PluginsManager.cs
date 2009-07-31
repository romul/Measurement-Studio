using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common;

namespace MeasurementStudio
{
    static class PluginsManager
    {
        public static IList<AbstractExperiment> ExperimentModes { get; private set; }

        static PluginsManager()
        {
            ExperimentModes = new List<AbstractExperiment>();
        }

        /// <summary>
        /// Загрузка правил из папки со сборками
        /// </summary>
        /// <param name="path">Абсолютный путь к папке со сборками</param>
        public static void LoadRulesFromDlls(string path)
        {
            var di = new DirectoryInfo(path);
            ExperimentModes.Clear();
            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {               
                // Загружаем сборку
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFile(fi.FullName);
                }
                catch (BadImageFormatException)
                {
                    string message = "Файл " + fi.FullName + " не является корректным dll файлом";
                    ErrorLogProvider.WriteToEventLog(message);
                    continue;
                }


                //Пытаемся получить сам объект из сборки.
                try
                {
                    AbstractExperiment experiment = GetExperimentModeFromAssembly(assembly);
                    ExperimentModes.Add(experiment);
                }
                catch (Exception)
                {
                    string message = "В файле " + fi.FullName + " не найден необходимый класс Experiment или этот класс не унаследован от AbstractExperiment\n";
                    ErrorLogProvider.WriteToEventLog(message, true, false);
                    continue;
                }
            }
        }

        /// <summary>
        /// Получить объект правила из сборки
        /// </summary>
        /// <param name="a">Сборка</param>
        /// <returns></returns>
        private static AbstractExperiment GetExperimentModeFromAssembly(Assembly a)
        {
            string className = a.GetName().Name + ".Experiment";
            object o = a.CreateInstance(className);
            var experiment = o as AbstractExperiment;

            if (experiment == null)
                throw new Exception();

            if (Properties.Settings.Default.PluginCaption == experiment.ToString())
            {
                AppSettings.Mode = experiment;
                AppSettings.Mode.LoadSettings(Properties.Settings.Default.SettingsFileName);
            }

            return experiment;
        }



    }
}
