using System;

namespace TrillBot.WebSub
{
    internal sealed class ContentTypeInfo : IEquatable<ContentTypeInfo>
    {
        public ContentTypeInfo(Type contentType)
        {
            ContentType = contentType;
        }

        public Type ContentType { get; }

        public bool Equals(ContentTypeInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ContentType, other.ContentType);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ContentTypeInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ContentType != null ? ContentType.GetHashCode() : 0;
        }

        public static bool operator ==(ContentTypeInfo left, ContentTypeInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ContentTypeInfo left, ContentTypeInfo right)
        {
            return !Equals(left, right);
        }
    }
}