// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Reflection;

// namespace NewsCollector.Core.Helpers
// {
//     public class DataExample 
//     {
//         public Tuple<bool, List<string>> ValidateDespatch(IrsaliyeZarfGonderYapisalGirdi IrsaliyeYapisalGirdi)
//         {
//             var invalidFieldErrorMessageList = new List<string>();

//             var validator = new DataAnnotationValidator();
//             var validationResults = new List<ValidationResult>();

//             var isValidate = validator.TryValidateObjectRecursive(IrsaliyeYapisalGirdi.Belgeler, validationResults);

//             if (!isValidate)
//             {
//                 foreach (var item in validationResults)
//                 {
//                     invalidFieldErrorMessageList.Add(item.ErrorMessage);
//                 }
//             }

//             return Tuple.Create(isValidate, invalidFieldErrorMessageList);
//         }
        
//         public bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results, IDictionary<object, object> validationContextItems = null)
//         {
//             return TryValidateObjectRecursive(obj, results, new HashSet<object>(), validationContextItems);
//         }

//         private bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results, ISet<object> validatedObjects, IDictionary<object, object> validationContextItems = null)
//         {
//             if (validatedObjects.Contains(obj))
//             {
//                 return true;
//             }

//             validatedObjects.Add(obj);
//             bool result = TryValidateObject(obj, results, validationContextItems);

//             var properties = obj.GetType().GetProperties().Where(prop => prop.CanRead
//                 && !prop.GetCustomAttributes(typeof(SkipRecursiveValidation), false).Any()
//                 && prop.GetIndexParameters().Length == 0).ToList();

//             foreach (var property in properties)
//             {
//                 if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType) continue;

//                 var value = obj.GetPropertyValue(property.Name);

//                 if (value == null) continue;

//                 var asEnumerable = value as IEnumerable;
//                 if (asEnumerable != null)
//                 {
//                     foreach (var enumObj in asEnumerable)
//                     {
//                         if (enumObj != null)
//                         {
//                             var nestedResults = new List<ValidationResult>();
//                             if (!TryValidateObjectRecursive(enumObj, nestedResults, validatedObjects, validationContextItems))
//                             {
//                                 result = false;
//                                 foreach (var validationResult in nestedResults)
//                                 {
//                                     PropertyInfo property1 = property;
//                                     results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
//                                 }
//                             };
//                         }
//                     }
//                 }
//                 else
//                 {
//                     var nestedResults = new List<ValidationResult>();
//                     if (!TryValidateObjectRecursive(value, nestedResults, validatedObjects, validationContextItems))
//                     {
//                         result = false;
//                         foreach (var validationResult in nestedResults)
//                         {
//                             PropertyInfo property1 = property;
//                             results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
//                         }
//                     };
//                 }
//             }

//             return result;
//         }
//     }
    
// }