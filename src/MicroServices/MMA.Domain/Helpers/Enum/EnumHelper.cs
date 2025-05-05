using System.Reflection;

namespace MMA.Domain
{
    public static class EnumHelper
    {
        public static List<DropdownItemModel> ToDropdownModels(this Enum enumType, object? valueSelected = null)
        {
            var dropDownItemModels = new List<DropdownItemModel>();

            var enumValues = Enum.GetValues(enumType.GetType());
            var enumTypeName = enumType.GetType().Name;

            foreach (var value in enumValues)
            {
                var enumName = Enum.GetName(enumType.GetType(), value) ?? string.Empty;
                var nameEnum = I18NHelper.GetString(key: $"Enum_{enumTypeName}_{enumName}_Entry");
                var dropDownModel = new DropdownItemModel
                {
                    Name = !string.IsNullOrEmpty(nameEnum) ? nameEnum : (value.ToString() ?? string.Empty),
                    Value = value,
                    IsSelected = value.Equals(valueSelected)
                };
                dropDownItemModels.Add(dropDownModel);
            }

            return dropDownItemModels;
        }


        public static List<CComboboxModel> ToComboboxModels(this Enum enumType, object? valueSelected = null)
        {
            var comboboxModels = new List<CComboboxModel>();

            var enumValues = Enum.GetValues(enumType.GetType());
            var enumTypeName = enumType.GetType().Name;

            foreach (var value in enumValues)
            {
                var enumName = Enum.GetName(enumType.GetType(), value) ?? string.Empty;
                var nameEnum = I18NHelper.GetString(key: $"Enum_{enumTypeName}_{enumName}_Entry");
                var dropDownModel = new CComboboxModel
                {
                    Name = !string.IsNullOrEmpty(nameEnum) ? nameEnum : (value.ToString() ?? string.Empty),
                    Value = value,
                    Checked = value.Equals(valueSelected)
                };
                comboboxModels.Add(dropDownModel);
            }

            return comboboxModels;
        }


        public static Dictionary<Type, Dictionary<string, object>> GetTranslations(Dictionary<string, Type> values)
        {
            var translations = new Dictionary<Type, Dictionary<string, object>>();
            foreach (var entry in values)
            {
                var enumType = entry.Value;
                var enumValues = Enum.GetValues(enumType);
                var enumDict = new Dictionary<string, object>();
                foreach (Enum enumValue in enumValues)
                {
                    string translation = GetEnumDescription(enumValue);
                    enumDict[enumValue.ToString()] = translation;
                }
                translations[enumType] = enumDict;
            }
            return translations;
        }

        public static string GetEnumDescription(Enum enumValue)
        {
            Type type = enumValue.GetType();
            MemberInfo[] memInfo = type.GetMember(enumValue.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(I18NDescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    var description = ((I18NDescriptionAttribute)attrs[0]).Description;
                    return I18NHelper.GetString(description);
                }
            }
            return enumValue.ToString();
        }
    }
}