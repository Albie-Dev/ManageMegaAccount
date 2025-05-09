using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MMA.Domain
{
    public static class ModelStateValidExtension
    {
        public static CustomModelState ModelStateValidate(this object model)
        {
            var customModelState = new CustomModelState();
            if (model == null)
            {
                customModelState.AddError(field: string.Empty, 
                    errorMessage: "Dữ liệu gửi về không hợp lệ. Vui lòng kiểm tra lại.",
                    errorScope: CErrorScope.FormSummary);
                return customModelState;
            }
            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                var validationAttributes = property.GetCustomAttributes<ValidationAttribute>();

                foreach (var attribute in validationAttributes)
                {
                    if (attribute == null)
                    {
                        customModelState.AddError(property.Name, "Unknown error");
                    }
                    else
                    {
                        var errorMessage = attribute.FormatErrorMessage(property.Name);
                        if (!attribute.IsValid(value))
                        {
                            customModelState.AddError(property.Name, errorMessage);
                        }
                    }
                }
            }
            return customModelState;
        }


        public class CustomModelState
        {
            private readonly List<ErrorDetailDto> _errors = new List<ErrorDetailDto>();

            public void AddError(string field, string errorMessage, CErrorScope errorScope = CErrorScope.Field)
            {
                field = string.IsNullOrEmpty(field) ? string.Empty : $"{field}_Error";
                _errors.Add(new ErrorDetailDto
                {
                    ErrorScope = errorScope,
                    Field = field,
                    Error = errorMessage
                });
            }

            public bool IsValid()
            {
                return !_errors.Any();
            }

            public List<ErrorDetailDto> GetErrors()
            {
                return _errors;
            }
        }
    }
}