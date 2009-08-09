using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Common
{
    /// <summary>
    /// Статический класс, обеспечивающий запись ошибок в EventLog и отображение в интерфейсе
    /// </summary>
    static public class ErrorLogProvider
    {
        static EventLog el = TryCreateEventLog();

        private static EventLog TryCreateEventLog()
        {
            try
            {
                return new EventLog("Application") { Source = "Measurement Studio" };
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show("Запустите инсталлятор:\n" + @"C:\Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil AppInstaller.dll", "Нет доступа к логам системы", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            return null;
        }

        public static void ShowInformationMessage(string message)
        {
            MessageBox.Show(message, "Информация.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);          
        }

        /// <summary>
        /// Записать в Event Log сообщение об ошибке с дублированием в консоли и/или MessageBox'ом
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        /// <param name="showMessageBox">Показывать ли сообщение об ошибке Messagebox'ом</param>
        /// <param name="writeToConsole">Писать ли информацию об ошибку в консоль</param>
        public static void WriteToEventLog(string errorMessage, bool showMessageBox, bool writeToConsole)
        {
            try
            {
                if (el != null) el.WriteEntry(errorMessage, EventLogEntryType.Error);
            }
            finally
            {
                if (showMessageBox)
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (writeToConsole)
                    Console.WriteLine(errorMessage);
            }
        }

        /// <summary>
        /// Записать в Event Log сообщение об ошибке с дублированием в консоли
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        public static void WriteToEventLogAndConsole(string errorMessage) { WriteToEventLog(errorMessage, false, true); }

        /// <summary>
        /// Записать в Event Log сообщение об ошибке с дублированием MessageBox'ом
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        public static void WriteToEventLogAndShow(string errorMessage) { WriteToEventLog(errorMessage, true, false); }

        /// <summary>
        /// Записать в Event Log сообщение об ошибке с дублированием в консоли и MessageBox'ом
        /// </summary>
        /// /// <param name="errorMessage">Сообщение об ошибке</param>
        public static void WriteToEventLogAndConsoleAndShow(string errorMessage) { WriteToEventLog(errorMessage, true, true); }

        /// <summary>
        /// Записать в Event Log сообщение об ошибке
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        public static void WriteToEventLog(string errorMessage)
        { WriteToEventLog(errorMessage, false, false); }
    }
}
