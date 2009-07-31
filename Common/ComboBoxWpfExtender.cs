using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;

namespace Common
{
    class EnumComboBoxWpf : ComboBox
    {
        private Type enumType;
        public bool SetEnum(Type enumType)
        {
            this.enumType = enumType;
            if (enumType.BaseType != typeof(Enum)) return false;
            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as
                           DescriptionAttribute;

                var item = (attr != null) ? attr.Description : field.Name;
                if (item != "None") this.Items.Add(item);
            }
            return true;
        }

        public object SelectedEnumItem
        {
            get
            {
                string selItem = SelectedItem.ToString();
                if (enumType == null) return selItem;
                var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var field in fields)
                {
                    var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as
                               DescriptionAttribute;

                    string item = (attr != null) ? attr.Description : field.Name;
                    if (item == selItem)
                    {
                        selItem = field.Name;
                    }
                }
                return Enum.Parse(enumType, selItem);
            }
            set
            {
                if (enumType == null) return;
                var field = enumType.GetField(value.ToString());
                var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as
                              DescriptionAttribute;
                SelectedItem = (attr != null) ? attr.Description : field.Name;
            }
        }
    }

}
