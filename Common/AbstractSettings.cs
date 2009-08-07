using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Common
{
    public delegate void PropertyHandler(PropertyInfo property, String propertyCaption);
    public delegate void PropertiesGroupHandler(string groupCaption);

    public abstract class AbstractSettings
    {
        #region Свойства, задаваемые пользователем
        
        [Text][SetByUser(Caption = "Заголовок графика")]
        public string ChartCaption { get; set; }

        [Text][SetByUser(Caption = "Подпись по оси Х")]
        public string XAxisLegend { get; set; }  

        [Text][SetByUser(Caption = "Легенда №1", Category = "1-ая зависимость")]
        public virtual string Series1Legend
        {
            get { return series1Legend; }
            set { series1Legend = value; }
        }
        protected string series1Legend = "Series1";

       
        [Enum(EnumType = typeof(Transform.Functions))]
        [SetByUser(Caption = "y = f(y)", Category = "1-ая зависимость")]
        public virtual Transform.Functions Y1Transform 
        {
            get { return y1Transform; }
            set { y1Transform = value; } 
        }
        protected Transform.Functions y1Transform = Transform.Functions.Nothing;

        
        [Numeric(MinValue = 125, MaxValue = 10000, Increment = 125)]
        [SetByUser(Caption = "Периодичность замеров [мс]")]
        public virtual int MeasurementPeriod 
        {
            get { return measurementPeriod; }
            set { measurementPeriod = value; } 
        }
        protected int measurementPeriod = 500;


        [Boolean][SetByUser(Caption = "Автопрокрутка")]
        public virtual bool AutoScroll
        {
            get { return autoScroll; }
            set { autoScroll = value; } 
        }
        protected bool autoScroll = true;


        [Boolean][SetByUser(Caption = "Показывать легенду")]
        public virtual bool LegendVisible
        {
            get { return legendVisible; }
            set { legendVisible = value; }
        }
        protected bool legendVisible = true;

        [Numeric(MinValue = 10, MaxValue = 1000, Increment = 5)]
        [SetByUser(Caption = "Кол-во точек на кадр")]
        public virtual int DotsPerFrame
        {
            get { return dotsPerFrame; }
            set { dotsPerFrame = value; }
        }
        protected int dotsPerFrame = 50;


        /// <summary>
        /// Режим сохранения снятых показаний
        /// </summary>
        [SetByUser(Caption = "Режим сохранения")]
        [Enum(EnumType = typeof(SaveMode))]
        public virtual SaveMode SaveMode
        {
            get { return saveMode; }
            set { saveMode = value; }
        }
        protected SaveMode saveMode = SaveMode.SaveInRuntime;

        #endregion
        

        private string fileForSavingName;

        #region Public Methods
        
        /// <summary>
        /// Устанавливает значение заданного свойства
        /// </summary>
        /// <param name="name">имя свойства</param>
        /// <param name="value">значение</param>
        public void SetProperty(string name, object value)
        {
            var prop = this.GetType().GetProperty(name);
            if (prop != null)
            {
                prop.SetValue(this, value, null);
            }  
        }

        /// <summary>
        /// Возвращает значение заданного свойства
        /// </summary>
        /// <param name="property">свойство</param>
        public T GetPropertyValue<T>(PropertyInfo property)
        {
            T res;
            try
            {
                res = (T) property.GetValue(this, null);
            }
            catch
            {
                res = default(T);
            }
            return res;
        }

        /// <summary>
        /// Возвращает удобочитаемое строковое представление всех настроек
        /// </summary>
        public override string ToString()
        {
            var descr = new StringBuilder();
            ForEachSetByUserFieldsGroup(
                groupName => descr.AppendLine(groupName + ":"),
                (property, caption) =>
                    {
                        descr.Append("\t");
                        descr.Append(caption);
                        descr.Append(" = ");
                        descr.Append(property.GetValue(this, null));
                        descr.AppendLine();
                    });
            return descr.ToString();
        }

        /// <summary>
        /// Путь к настройкам данного типа
        /// </summary>
        public string SettingsPath 
        {
            get { return Application.StartupPath + @"\Settings\" + GetType().Namespace; }
        }

        /// <summary>
        /// Создаёт форму для редактирования настроек
        /// </summary>
        /// <param name="targetFileName">полный путь к файлу для сохранения настроек</param>
        public void CreateForm(string targetFileName)
        {
            fileForSavingName = targetFileName;
            var formBuilder = new WpfSettingsFormBuilder(this);
            formBuilder.SettingsCreated += formBuilder_SettingsCreated;
            formBuilder.Build();            
        }

        /// <summary>
        /// Обработчик для кнопки "Сохранить" формы редактирования настроек 
        /// </summary>
        void formBuilder_SettingsCreated(object sender, EventArgs e)
        {
            this.SaveToFile();
            ErrorLogProvider.ShowInformationMessage(this.ToString());
        }

        #endregion

        private SortedDictionary<string, List<PropertyInfo>> propertiesByGroup;
        private Dictionary<PropertyInfo, string> propertiesCaptions;

        /// <summary>
        /// Осуществляет мемоизацию свойств, устанавливаемых пользователем, по группам.
        /// </summary>
        /// <param name="handler">вызывается для каждого свойства во время мемоизации</param>
        protected void ForEachSetByUserField(PropertyHandler handler)
        {
            if (propertiesByGroup == null)
            {
                propertiesByGroup = new SortedDictionary<string, List<PropertyInfo>>();
                propertiesCaptions = new Dictionary<PropertyInfo, string>();
                var properties = GetType().GetProperties();
                foreach (var prop in properties)
                {
                    var sbuAttr =
                        Attribute.GetCustomAttribute(prop, typeof (SetByUserAttribute)) as
                        SetByUserAttribute;
                    if (sbuAttr != null)
                    {
                        if (!propertiesByGroup.ContainsKey(sbuAttr.Category))
                        {
                            propertiesByGroup.Add(sbuAttr.Category, new List<PropertyInfo>());
                        }                        
                        propertiesByGroup[sbuAttr.Category].Add(prop);
                        propertiesCaptions.Add(prop, String.IsNullOrEmpty(sbuAttr.Caption) ? prop.Name : sbuAttr.Caption);
                        if (handler!=null)
                        {
                            handler.Invoke(prop, String.IsNullOrEmpty(sbuAttr.Caption) ? prop.Name : sbuAttr.Caption);
                        }
                    }
                }
            }                                    
        }

        /// <summary>
        /// Перебирает все группы свойств, устанавливаемых пользователем,
        /// и все свойства в каждой группе 
        /// </summary>
        /// <param name="groupHandler">обработчик для групп</param>
        /// <param name="propHandler">обработчик для свойств</param>
        public void ForEachSetByUserFieldsGroup(PropertiesGroupHandler groupHandler, PropertyHandler propHandler)
        {
            ForEachSetByUserField(null);
            foreach(var group in propertiesByGroup.Keys)
            {
                if (groupHandler != null) groupHandler.Invoke(group);
                if (propHandler != null)
                {
                    foreach (var prop in propertiesByGroup[group])
                    {
                        propHandler.Invoke(prop, propertiesCaptions[prop]);
                    }
                }
            }
        }

        /// <summary>
        /// С помощью SaveFileDialog выбирает куда сохранить настройки
        /// </summary>
        internal void SaveToFile()
        {
            if (fileForSavingName != null)
            {
                if (File.Exists(fileForSavingName)) File.Delete(fileForSavingName);
                SaveToXml(fileForSavingName);
            }
            else
            {
                if (!Directory.Exists(SettingsPath)) Directory.CreateDirectory(SettingsPath);
                if (Directory.Exists(SettingsPath))
                {
                    var sfd = new SaveFileDialog
                                  {
                                      DefaultExt = ".xml",
                                      InitialDirectory = SettingsPath,
                                      Filter = "Настройки|*.xml"
                                  };
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        SaveToXml(sfd.FileName);
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException(
                        "Директория, выбранная для сохранения не найдена и не может быть создана.");
                }
            }
        }

        /// <summary>
        /// Осуществляет сохранение настроек в заданный файл в формате XML
        /// </summary>
        /// <param name="filePath">полный путь к целевому файлу</param>
        private void SaveToXml(string filePath)
        {
            var xmlSettings = new XElement("Settings",
                                           new XAttribute("type", GetType()));
            ForEachSetByUserFieldsGroup(null,
               (property, caption) =>
               {
                   var node = new XElement(property.Name, property.GetValue(this, null));
                   xmlSettings.Add(node);
               });
            xmlSettings.Save(filePath);
        }
    }
}
