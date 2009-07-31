using System;
using System.Collections.Generic;

namespace MeasurementStudio
{
    /// <summary>
    /// Исключение, которое может выбрасывать функция Load класса Settings
    /// </summary>
    public class LoadSettingsException: Exception
    {
        /// <summary>
        /// Загружено ли что-нибудь из файла
        /// </summary>
        
        public bool IsSomethingLoaded {get; set;}

        /// <summary>
        /// Существует ли файл
        /// </summary>
        public bool IsFileExists {get; set;}

        /// <summary>
        /// Список возникших ошибок
        /// </summary>
        public List<string> Errors {get; set;}

        public LoadSettingsException()
        {
            IsSomethingLoaded = false;
            IsFileExists = true;
            Errors = new List<string>();
        }
            

    }
}
