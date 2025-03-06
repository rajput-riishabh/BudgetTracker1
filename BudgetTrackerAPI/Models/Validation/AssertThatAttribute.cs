using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BudgetTrackerAPI.Models.Validation
{
    public class AssertThatAttribute : ValidationAttribute
    {
        public string PropertyName { get; } // Name of the boolean property to assert

        public AssertThatAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(PropertyName);
            if (property == null)
            {
                return new ValidationResult($"Unknown property: {PropertyName}");
            }

            object propertyValue = property.GetValue(validationContext.ObjectInstance, null);

            if (propertyValue is bool assertion && assertion) // Assuming property is boolean and should be true
            {
                return ValidationResult.Success; // Validation passes if the boolean property is true
            }
            else
            {
                return new ValidationResult(ErrorMessageString); // Validation fails if boolean property is false
            }
        }
    }
}