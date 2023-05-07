
using System;

namespace IntroSE.Kanban.Backend.Utilities
{

    /// <summary>
    /// This class represents a <b>case-insensitive</b> <see cref="string"/>.<br/><br/>
    /// Supports the Length method and allows implicit conversion between<br/>
    /// <see cref="CIString"/> and <see cref="string"/> and vice-versa.<br/><br/>
    /// Supports all the operators that can be used on a normal <see cref="string"/><br/>
    /// e.g: == , != , &gt; , &lt; , &gt;= , &lt;= <br/><br/>
    /// <b>Implements:</b> <see cref="IEquatable{T}"/>, <see cref="IComparable"/>, <see cref="IComparable{T}"/>, <see cref="ICloneable"/>
    /// <br/><br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Yuval Roth</c>
    /// <br/>
    /// ===================
    /// </summary>
    public sealed class CIString : IEquatable<CIString>,IComparable,IComparable<CIString>,ICloneable
    {
        private readonly string value;

        /// <summary>
        /// Gets the <see cref="string"/> held inside the CIString object
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public string Value => value;

        /// <summary>
        /// Gets the numbers of characters in the current CIString object
        /// </summary>
        /// <returns>
        /// The number of characters in the current CIString
        /// </returns>
        public int Length => value.Length;

        /// <summary>
        /// Explicitly builds a new CIString object
        /// </summary>
        /// <param name="Value"></param>
        public CIString(string Value)
        {
            value = Value;
        }


        //===========================================================
        //                      Interface Methods
        //===========================================================

        public bool Equals(CIString s)
        {
            if (s == null) return false;
            return value.ToLower().Equals(s.value.ToLower());
        }
        
        int IComparable.CompareTo(object obj)
        {
            if (obj is CIString s)
            {
                return value.ToLower().CompareTo(s.value.ToLower());
            }
            else throw new ArgumentException("Argument is not a CIString");
        }      
        public override string ToString()
        {
            return value;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return Equals((CIString)obj);
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }   
            if (obj is string @str)  return Equals(@str);
            return false;
        }
        public override int GetHashCode()
        {            
            return value.ToLower().GetHashCode();
        }
        public object Clone()
        {
            return new CIString(new string(value));
        }
        int IComparable<CIString>.CompareTo(CIString other)
        {
            return value.ToLower().CompareTo(other.value.ToLower());
        }

        //===========================================================
        //                      Private Methods
        //===========================================================

        private int CompareTo(CIString other)
        {
            return value.ToLower().CompareTo(other.value.ToLower());
        }

        private bool Equals(string s)
        {
            if (s == null) return false;
            return value.ToLower().Equals(s.ToLower());
        }

        //===========================================================
        //                      Operators
        //===========================================================

        public static bool operator ==(CIString left, CIString right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(CIString left, CIString right)
        {
            return !(left == right);
        }     
        public static bool operator <(CIString left, CIString right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(CIString left, CIString right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(CIString left, CIString right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }
        public static bool operator >=(CIString left, CIString right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
        public static implicit operator CIString(string v)
        {
            return new CIString(v);
        }
        public static implicit operator string(CIString v)
        {
            return v.value;
        }
    }
}
