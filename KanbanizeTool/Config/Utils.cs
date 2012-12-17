using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using KanbanizeTool.Properties;

namespace KanbanizeTool.Config
{
    public static class Utils
    {
        public static IEnumerable<PropertyInfo> GetUserSettingsAsEnumerable()
        {
            PropertyInfo[] properties = typeof(Properties.Settings).GetProperties();
            var userProperties = properties.Where(property => Attribute.GetCustomAttribute(property, typeof(UserScopedSettingAttribute)) as UserScopedSettingAttribute != null);
            return userProperties;
        }

        internal static void CheckSettings()
        {
            bool needSave = false;
            PropertyInfo[] properties = typeof(Properties.Settings).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                UserScopedSettingAttribute attribute =
                    Attribute.GetCustomAttribute(property, typeof(UserScopedSettingAttribute)) as UserScopedSettingAttribute;
                if (attribute != null && property.PropertyType == typeof(string)) // This property has a StoredDataValueAttribute
                {
                    var propertyValue = property.GetValue(Properties.Settings.Default, null);
                    if (propertyValue is string && string.IsNullOrWhiteSpace((string)propertyValue))
                    {
                        needSave = true;
                        string requestMessage = string.Format("Empty configuration parameter. Please specify {0}:", property.Name);
                        var propertyNewValue = GetValueFromUser(requestMessage);
                        property.SetValue(Properties.Settings.Default, propertyNewValue, null);

                        if (string.IsNullOrWhiteSpace(propertyNewValue))
                            throw new ArgumentException("Empty configuration parameter", "Properties.Settings.Default." + property.Name);
                    }
                }
            }

            var boardIdProperty = typeof (Properties.Settings).GetProperty("KanbanizeBoardId");
            object boardIdValue = null;
            try
            {
                boardIdValue = boardIdProperty.GetValue(Properties.Settings.Default, null);
            }
            catch{}
            if (boardIdValue == null)
            {
                needSave = true;
                string requestMessage = string.Format("Empty configuration parameter. Please specify {0}:", boardIdProperty.Name);
                var propertyNewValue = GetValueFromUser(requestMessage);
                boardIdProperty.SetValue(Properties.Settings.Default, Convert.ToInt32(propertyNewValue), null);

                if (string.IsNullOrWhiteSpace(propertyNewValue))
                    throw new ArgumentException("Empty configuration parameter", "Properties.Settings.Default." + boardIdProperty.Name);
            }

            if (needSave)
            {
                Properties.Settings.Default.Save();
            }
        }

        /*internal static void FillSettings()
        {
            
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.KanbanizeApiKey))
            {
                string requestMessage = "Please paste KanbanizeApiKey:";
                Properties.Settings.Default.KanbanizeApiKey = GetValueFromUser(requestMessage);
                needSave = true;
            }

            
            if (needSave)
            {
                Properties.Settings.Default.Save();
            }
        }*/

        private static string GetValueFromUser(string requestMessage)
        {
            Console.WriteLine(requestMessage);
            return Console.ReadLine().Trim();
        }
    }
}
