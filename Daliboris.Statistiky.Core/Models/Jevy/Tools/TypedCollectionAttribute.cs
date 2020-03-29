using System;

namespace Daliboris.Statistiky
{
    [AttributeUsage(AttributeTargets.All)]
    public class TypedCollectionAttribute : Attribute
    {
        private Type UnderlyingType;

        public TypedCollectionAttribute(Type underlyingType)
        {
            UnderlyingType = underlyingType;
        }

        public Type CollectionType
        {
            get { return UnderlyingType; }
        }
    }
}