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
    }
}