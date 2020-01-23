using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProjectModul.Exteensions
{
    public static class ToExpandObject
    {
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                expando.Add(property.Name, property.GetValue(value));
            }

            return expando as ExpandoObject;
        }
    }
}

//Класс ExpandoObject, добавляемый в.NET 4, позволяет произвольно устанавливать свойства объекта во время выполнения.
//Есть ли в этом какие-то преимущества по сравнению с использованием Dictionary<string, object> или даже Hashtable? 
//Насколько я могу судить, это не что иное, как хеш-таблица, к которой вы можете получить доступ с немного более лаконичным синтаксисом. 
//Например, почему это так?
//Какие реальные преимущества можно получить, используя ExpandoObject вместо использования произвольного типа словаря, 
//если не очевидно, что вы используете тип, который будет определен во время выполнения.
//expand - расширять