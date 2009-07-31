using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Common
{


    internal sealed class SettingsFormBuilder : ISettingsFormBuilder
    {
        private readonly AbstractSettings settings;
        private Form sForm;        
        private Panel panel;
        private Control holder;
        private GroupBox currGroupBox;

        public SettingsFormBuilder(AbstractSettings settings)
        {
            this.settings = settings;
        }

        public void Build()
        {
            sForm = new Form
            {
                Size = new Size(400, 500),
                Text = "Настройки",
                FormBorderStyle = FormBorderStyle.FixedToolWindow
            };
            var mainLayout = new TableLayoutPanel
            {
                ColumnCount = 3,
                Dock = DockStyle.Fill,
                Name = "mainLayout",
                RowCount = 3,
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 2F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 96F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 2F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));

            sForm.Controls.Add(mainLayout);
            panel = new FlowLayoutPanel { Dock = DockStyle.Fill };
            holder = panel;
            mainLayout.Controls.Add(panel, 1, 1);

            //settings.ForEachSetByUserField(FillForm);
            settings.ForEachSetByUserFieldsGroup(CreatePropertiesGroupBox, FillGroup);
            var bSave = new Button
                            {
                                Text = "Сохранить", 
                                Margin = new Padding(110, 15, 0, 0)
                            };
            bSave.Click += bSave_Click;
            var bCancel = new Button
            {
                Text = "Отмена",
                Margin = new Padding(10, 15, 0, 0)
            };
            bCancel.Click += ((s, e) => sForm.Close());
            CreateLabelForProperty("");
            panel.Controls.Add(bSave);
            panel.Controls.Add(bCancel);
            sForm.ShowDialog();
        }

        private void CreatePropertiesGroupBox(string groupCaption)
        {
            currGroupBox = new GroupBox
            {
                AutoSize = false,
                Size = new Size(365, 150),                
                Text = groupCaption
            };
            var groupPanel = new FlowLayoutPanel { Dock = DockStyle.Fill };
            currGroupBox.Controls.Add(groupPanel);
            panel.Controls.Add(currGroupBox);
            holder = groupPanel;
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

        private void FillGroup(PropertyInfo property, String propertyCaption)
        {
            CreateLabelForProperty(propertyCaption);
            CreateControlIfPropertyIsNumeric(property);
            CreateControlIfPropertyIsTextual(property);
            CreateControlIfPropertyIsEnum(property);
        }


        private void CreateLabelForProperty(string propertyCaption)
        {
            var label = new Label
            {
                AutoSize = false,
                Size = new Size(165, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = propertyCaption
            };
            holder.Controls.Add(label);
        }

        private void CreateControlIfPropertyIsEnum(PropertyInfo property)
        {
            var enumAttribute =
                Attribute.GetCustomAttribute(property, typeof(EnumAttribute)) as
                EnumAttribute;

            if (enumAttribute != null)
            {
                var value = Convert.ChangeType(settings.GetPropertyValue<object>(property), enumAttribute.EnumType);
                var comboField = new EnumComboBox
                {
                    Name = property.Name,
                    Size = new Size(180, 20),
                };
                comboField.SetEnum(enumAttribute.EnumType);
                comboField.SelectedEnumItem = (value.ToString() != "None") ? value : enumAttribute.Default;

                holder.Controls.Add(comboField);
            }
        }

        private void CreateControlIfPropertyIsNumeric(PropertyInfo property)
        {
            var numericAttribute =
                Attribute.GetCustomAttribute(property, typeof(NumericAttribute)) as
                NumericAttribute;
            if (numericAttribute != null)
            {
                var value = settings.GetPropertyValue<int?>(property);
                var numericField = new NumericUpDown
                {
                    Name = property.Name,
                    Minimum = numericAttribute.MinValue,
                    Maximum = numericAttribute.MaxValue,
                    Increment = numericAttribute.Increment,
                    Value = (value ?? numericAttribute.Default),
                    ThousandsSeparator = true,
                    Size = new Size(180, 20),
                };
                holder.Controls.Add(numericField);
            }
        }

        private void CreateControlIfPropertyIsTextual(PropertyInfo property)
        {
            var textAttribute =
                Attribute.GetCustomAttribute(property, typeof(TextAttribute)) as
                TextAttribute;
            if (textAttribute != null)
            {
                var value = settings.GetPropertyValue<string>(property);
                var textField = new TextBox
                {
                    Name = property.Name,
                    Text = (value ?? textAttribute.Default),
                    MaxLength = textAttribute.MaxLength,
                    Size = new Size(180, 20),
                };
                holder.Controls.Add(textField);
            }
        }


        private void bSave_Click(object sender, EventArgs e)
        {
            panel.FindControls<NumericUpDown>()
                .ForEach(c => settings.SetProperty(c.Name, (int)c.Value));

            panel.FindControls<TextBox>()
                .ForEach(c => settings.SetProperty(c.Name, c.Text));

            panel.FindControls<EnumComboBox>()
                .ForEach(c => settings.SetProperty(c.Name, c.SelectedEnumItem));

            OnSettingsCreated();
            sForm.Close();
        }
    }


}
