using System;

namespace Common
{
    internal interface ISettingsFormBuilder
    {
        void Build();
        event EventHandler SettingsCreated;
    }
}