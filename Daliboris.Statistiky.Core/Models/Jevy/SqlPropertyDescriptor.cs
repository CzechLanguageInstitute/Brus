using System;
using System.Data.SqlTypes;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Diagnostics;

namespace Daliboris.Statistiky
{
	public class SqlPropertyDescriptor : PropertyDescriptor
	{
		/// <summary>
		/// Only way to create the PropertyDescriptor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="sqlType"></param>
		/// <returns></returns>
		public static SqlPropertyDescriptor GetProperty(string name, Type sqlType)
		{
			// we need to use the attributes of the base type
			Type baseType = sqlType.GetProperty("Value").PropertyType;
			ArrayList attribs = new ArrayList(TypeDescriptor.GetAttributes(baseType));

			Attribute[] attrs = (Attribute[])attribs.ToArray(typeof(Attribute));

			return new SqlPropertyDescriptor(name,attrs,sqlType,baseType);
		}

		private Type SqlType;
		private Type BaseType;

		// Unused
		protected SqlPropertyDescriptor(MemberDescriptor descr) : base(descr){}
		protected SqlPropertyDescriptor(MemberDescriptor descr,Attribute[] attrs) : base(descr,attrs){}
		protected SqlPropertyDescriptor(string name,Attribute[] attrs) : base(name,attrs){}

		// use this
		protected SqlPropertyDescriptor( string name,Attribute[] attrs, Type sqlType, Type baseType ) : base(name,attrs)
		{
			SqlType = sqlType;
			BaseType = baseType;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override bool CanResetValue(object component) 
		{ 
			return false; 
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="component"></param>
		public override void ResetValue(object component)
		{
			throw new NotSupportedException();
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return BaseType; }
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return BaseType; }
		}

		public override void SetValue(object component,object value)
		{
			try
			{
				PropertyInfo pi = component.GetType().GetProperty(this.Name);
				Object o;
				if ( value == DBNull.Value )
				{
					o = component.GetType().GetField("Null", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField).GetValue(component);
						
				}
				else
				{
					o = pi.PropertyType.GetConstructor(new Type[]{BaseType}).Invoke(new Object[]{value});
				}
				pi.SetValue(component,o, null);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public override object GetValue(object component)
		{
			try
			{
				object Property = component.GetType().GetProperty(this.Name).GetValue(component,null);

				// handle nulls
				if ( (bool)Property.GetType().GetProperty("IsNull").GetValue(Property,null) ) return DBNull.Value;

				object Value = Property.GetType().GetProperty("Value").GetValue(Property,null);
				return Value;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}
	}
}
