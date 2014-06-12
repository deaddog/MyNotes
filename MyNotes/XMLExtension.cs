using System;
using System.Xml.Linq;

namespace MyNotes
{
    public static class XMLExtension
    {
        public delegate bool TryParse<T>(string input, out T value);

        public static T Attribute<T>(this XElement element, XName attributename, T defaultvalue, TryParse<T> reader)
        {
            T value = defaultvalue;
            var attribute = element.Attribute(attributename);
            if (attribute != null && !reader(attribute.Value, out value))
                value = defaultvalue;
            return value;
        }
        public static T Element<T>(this XElement element, XName elementname, T defaultvalue, TryParse<T> reader)
        {
            T value = defaultvalue;
            var child = element.Element(elementname);
            if (child != null && !reader(child.Value, out value))
                value = defaultvalue;
            return value;
        }

        public static bool HasAttribute(this XElement element, XName attributename)
        {
            return element.Attribute(attributename) != null;
        }
        public static bool HasElement(this XElement element, XName elementname)
        {
            return element.Element(elementname) != null;
        }

        public static XElement Element(this XElement element, XName name, bool create)
        {
            XElement child = element.Element(name);
            if(child == null && create)
            {
                child = new XElement(name);
                element.Add(child);
            }
            return child;
        }

        public static T Attribute<T>(this XElement element, XName attributename, T defaultvalue)
        {
            object ret = null;

            if (typeof(T) == typeof(int))
                ret = Attribute(element, attributename, (int)(object)defaultvalue, int.TryParse);
            else if (typeof(T) == typeof(double))
                ret = Attribute(element, attributename, (double)(object)defaultvalue, double.TryParse);
            else if (typeof(T) == typeof(string))
                ret = Attribute(element, attributename, (string)(object)defaultvalue, stringTryParse);
            else if (typeof(T) == typeof(bool))
                ret = Attribute(element, attributename, (bool)(object)defaultvalue, bool.TryParse);
            else
                throw new ArgumentException("Unsupported type.");

            return (T)ret;
        }
        public static T Element<T>(this XElement element, XName elementname, T defaultvalue)
        {
            object ret = null;

            if (typeof(T) == typeof(int))
                ret = Element(element, elementname, (int)(object)defaultvalue, int.TryParse);
            else if (typeof(T) == typeof(double))
                ret = Element(element, elementname, (double)(object)defaultvalue, double.TryParse);
            else if (typeof(T) == typeof(string))
                ret = Element(element, elementname, (string)(object)defaultvalue, stringTryParse);
            else if (typeof(T) == typeof(bool))
                ret = Element(element, elementname, (bool)(object)defaultvalue, bool.TryParse);
            else
                throw new ArgumentException("Unsupported type.");

            return (T)ret;
        }

        private static bool stringTryParse(string input, out string output)
        {
            output = input;
            return true;
        }
    }
}
