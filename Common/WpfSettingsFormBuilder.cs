using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Common.Wpf;
using Microsoft.Samples.KMoore.WPFSamples.NumericUpDownControl;

namespace Common
{
    public delegate Control PropertyToControlHandler(PropertyInfo property, UserTypeAttribute attr);

    class WpfSettingsFormBuilder : ISettingsFormBuilder
    {
        private readonly AbstractSettings settings;
        private IAddChild container;
        private Panel mainStackPanel;
        private Window sWindow;
        private Dictionary<Type, PropertyToControlHandler> propertyToControlHandlers;

        public WpfSettingsFormBuilder(AbstractSettings settings)
        {
            this.settings = settings;
            propertyToControlHandlers =
                new Dictionary<Type, PropertyToControlHandler>
                    {
                        {typeof (BooleanAttribute), CreateControlIfPropertyIsBoolean},
                        {typeof (EnumAttribute), CreateControlIfPropertyIsEnum},
                        {typeof (TextAttribute), CreateControlIfPropertyIsTextual},
                        {typeof (NumericAttribute), CreateControlIfPropertyIsNumeric}
                    };
        }


        public void Build()
        {
            sWindow = new Window {Width = 350, Height = 500, Title = "Настройки"};
            var scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            var stackPanel = new StackPanel {Margin = new Thickness(5)};
            container = scrollViewer;
            container.AddChild(stackPanel);
            mainStackPanel = stackPanel;
            settings.ForEachSetByUserFieldsGroup(CreatePropertiesExpander, FillGroup);
            var bSave = new Button {Content = "Сохранить"};
            bSave.Click += bSave_Click;
            (mainStackPanel as IAddChild).AddChild(bSave);
            container = sWindow;
            container.AddChild(scrollViewer);
            sWindow.ShowDialog();
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            mainStackPanel.FindControls<NumericUpDown>()
                .ForEach(c => settings.SetProperty(c.Name, (int)c.Value));

            mainStackPanel.FindControls<TextBox>()
                .ForEach(c => settings.SetProperty(c.Name, c.Text));

            mainStackPanel.FindControls<EnumComboBoxWpf>()
                .ForEach(c => settings.SetProperty(c.Name, c.SelectedEnumItem));

            mainStackPanel.FindControls<CheckBox>()
                .ForEach(c => settings.SetProperty(c.Name, c.IsChecked));

            OnSettingsCreated();
            sWindow.Close();
        }

        public event EventHandler SettingsCreated;
        private void OnSettingsCreated()
        {
            EventHandler localHandler = SettingsCreated;
            if (localHandler != null)
            {
                localHandler(this, new EventArgs());
            }
        }

        private void CreatePropertiesExpander(string groupCaption)
        {
            var expander = new Expander();
            expander.Header = groupCaption;
            (mainStackPanel as IAddChild).AddChild(expander);
            container = expander;
            var stackPanel = new StackPanel();
            container.AddChild(stackPanel);
            container = stackPanel;
        }


        private void FillGroup(PropertyInfo property, string propertyCaption)
        {
            var attrs = Attribute.GetCustomAttributes(property, typeof(UserTypeAttribute));
            if (attrs.IsNullOrEmpty()) return;

            var attr = attrs[0] as UserTypeAttribute;
            if (attr != null)
            {
                var dockPanel = new DockPanel {Margin = new Thickness(3)};
                var parentPanel = dockPanel as IAddChild;
                parentPanel.AddChild(
                    new Label {Content = propertyCaption, Width = 170}
                    );

                var c = propertyToControlHandlers[attr.GetType()].Invoke(property, attr);
                if (c != null) parentPanel.AddChild(c);

                container.AddChild(dockPanel);
            }
        }


        private Control CreateControlIfPropertyIsBoolean(PropertyInfo property, UserTypeAttribute attr)
        {
            var booleanAttribute = attr as BooleanAttribute;
            if (booleanAttribute != null)
            {
                var value = settings.GetPropertyValue<bool>(property);
                var checkBox = new CheckBox {IsChecked = value, Name = property.Name};
                return checkBox;
            }
            return null;
        }

        private Control CreateControlIfPropertyIsNumeric(PropertyInfo property, UserTypeAttribute attr)
        {
            var numericAttribute = attr as NumericAttribute;
            if (numericAttribute != null)
            {
                var value = settings.GetPropertyValue<int>(property);                
                var numericField = new NumericUpDown
                                       {
                                           Name = property.Name,
                                           Minimum = numericAttribute.MinValue,
                                           Maximum = numericAttribute.MaxValue,
                                           Change = numericAttribute.Increment,
                                           Value = value
                                       };
                return numericField;
            }
            return null;
        }

        private Control CreateControlIfPropertyIsTextual(PropertyInfo property, UserTypeAttribute attr)
        {
            var textAttribute = attr as TextAttribute;
            if (textAttribute != null)
            {
                var value = settings.GetPropertyValue<string>(property);
                var textField = new TextBox
                                    {
                                        Name = property.Name,
                                        Text = (value ?? ""),
                                        MaxLength = textAttribute.MaxLength,
                                        
                                    };
                if (textAttribute.LineCount > 1)
                {
                    textField.MinLines = textAttribute.LineCount;
                    textField.TextWrapping = TextWrapping.Wrap;
                    textField.AcceptsReturn = true;
                    textField.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                return textField;
            }
            return null;
        }

        private Control CreateControlIfPropertyIsEnum(PropertyInfo property, UserTypeAttribute attr)
        {
            var enumAttribute = attr as EnumAttribute;
            if (enumAttribute != null)
            {
                var value = Convert.ChangeType(settings.GetPropertyValue<object>(property), enumAttribute.EnumType);
                var comboField = new EnumComboBoxWpf
                                     {
                                         Name = property.Name,
                                     };
                comboField.SetEnum(enumAttribute.EnumType);
                if (value!=null) comboField.SelectedEnumItem = value;

                return comboField;
            }
            return null;
        }

    }
}
