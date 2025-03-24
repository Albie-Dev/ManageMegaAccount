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
                var dropDownModel = new DropdownItemModel
                {
                    Name = I18NHelper.GetString(key: $"Enum_{enumTypeName}_{enumName}_Entry"),
                    Value = value,
                    IsSelected = value.Equals(valueSelected)
                };
                dropDownItemModels.Add(dropDownModel);
            }

            return dropDownItemModels;
        }
    }
}