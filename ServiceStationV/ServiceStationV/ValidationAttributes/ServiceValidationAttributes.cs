using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceStationV.ValidationAttributes
{
    class ServiceValidationAttributes
    {
        public class ServiceNameAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult("Название услуги не может быть пустым!");
                }

                string name = value.ToString();
                if (name.Length > 30)
                {
                    return new ValidationResult("Название услуги не должно превышать 30 символов!");
                }

                if (!Regex.IsMatch(name, @"^[а-яА-ЯёЁ0-9\s\.,!?-]+$"))
                {
                    return new ValidationResult("Название услуги должно содержать только русские буквы!");
                }

                return ValidationResult.Success;
            }
        }

        public class SmallDescriptionAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult("Краткое описание не может быть пустым!");
                }

                string description = value.ToString();
                if (description.Length > 50)
                {
                    return new ValidationResult("Краткое описание не должно превышать 50 символов!");
                }

                if (!Regex.IsMatch(description, @"^[а-яА-ЯёЁ0-9\s\.,!?-]+$"))
                {
                    return new ValidationResult("Краткое описание должно содержать только русские буквы!");
                }

                return ValidationResult.Success;
            }
        }

        public class LargeDescriptionAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult("Подробное описание не может быть пустым!");
                }

                string description = value.ToString();

                if (!Regex.IsMatch(description, @"^[а-яА-ЯёЁ0-9\s\.,!?\-\r\n]+$"))
                {
                    return new ValidationResult("Подробное описание должно содержать только русские буквы!");
                }

                return ValidationResult.Success;
            }
        }

        public class ServiceNameENAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    string name = value.ToString();
                    if (name.Length > 30)
                    {
                        return new ValidationResult("Английское название услуги не должно превышать 30 символов!");
                    }


                    if (!Regex.IsMatch(name, @"^[a-zA-Z0-9\s\.,!?-]+$"))
                    {
                        return new ValidationResult("Английское название должно содержать только латинские буквы!");
                    }
                }
                else return new ValidationResult("Английское название услуги не может быть пустым!");



                return ValidationResult.Success;
            }
        }

        public class SmallDescriptionENAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    string description = value.ToString();
                    if (description.Length > 50)
                    {
                        return new ValidationResult("Английское краткое описание не должно превышать 50 символов!");
                    }


                    if (!Regex.IsMatch(description, @"^[a-zA-Z0-9\s\.,!?-]+$"))
                    {
                        return new ValidationResult("Английское краткое описание должно содержать только латинские буквы!");
                    }
                }
                else return new ValidationResult("Английское описание услуги не может быть пустым!");

                return ValidationResult.Success;
            }
        }

        public class LargeDescriptionENAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    string description = value.ToString();


                    if (!Regex.IsMatch(description, @"^[a-zA-Z0-9\s\.,!?\-\r\n]+$"))
                    {
                        return new ValidationResult("Английское подробное описание должно содержать только латинские буквы!");
                    }
                }
                else return new ValidationResult("Английское описание услуги не может быть пустым!");


                return ValidationResult.Success;
            }
            public class PriceAttribute : ValidationAttribute
            {
                protected override ValidationResult IsValid(object value, ValidationContext validationContext)
                {
                    if (value == null)
                    {
                        return new ValidationResult("Цена не может быть пустой!");
                    }

                    decimal price = (decimal)value;
                    if (price < 0)
                    {
                        return new ValidationResult("Цена не может быть отрицательной!");
                    }

                    return ValidationResult.Success;
                }
            }
            
        }
    }


    }

