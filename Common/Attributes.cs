using System;


namespace Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SetByUserAttribute : Attribute
    {
        public string Caption  { get; set; }
        public string Category { get; set; }
        public SetByUserAttribute()
        {
            Category = "Остальные свойства";
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class UserTypeAttribute : Attribute
    {}

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NumericAttribute : UserTypeAttribute
    {
        public int MinValue  { get; set; }
        public int MaxValue  { get; set; }
        public int Increment { get; set; }
        public int Default   { get; set; }
        public NumericAttribute()
        {
            MaxValue = Int32.MaxValue;
            Increment = 1;
            Default = 1000;
        }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TextAttribute : UserTypeAttribute
    {
        public int MaxLength  { get; set; }
        public string Default { get; set; }
        public int LineCount  { get; set; }
        public TextAttribute()
        {
            MaxLength = 100;
            Default = "";
            LineCount = 1;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EnumAttribute : UserTypeAttribute
    {
        public Type EnumType  { get; set; }
        public object Default { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public sealed class BooleanAttribute : UserTypeAttribute
    {
        public bool Default { get; set; }
    }
}
