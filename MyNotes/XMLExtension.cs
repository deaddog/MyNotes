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
    }
}
