using System;


namespace Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SetByUserAttribute : Attribute
    {
        public string Caption;
        public string Category = "Остальные свойства";
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class UserTypeAttribute : Attribute
    {}

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NumericAttribute : UserTypeAttribute
    {
        public int MinValue;
        public int MaxValue = Int32.MaxValue;
        public int Increment = 1;
        public int Default = 1000;
    }


    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TextAttribute : UserTypeAttribute
    {
        public int MaxLength = 100;
        public string Default = "";
        public int LineCount = 1;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EnumAttribute : UserTypeAttribute
    {
        public Type EnumType;
        public object Default;
    }


    [AttributeUsage(AttributeTargets.Property)]
    public sealed class BooleanAttribute : UserTypeAttribute
    {
        public bool Default;
    }
}
