/**
 * Copyright (c) 2003-2013 SSHTOOLS LIMITED. All Rights Reserved.
 *
 * This file contains Original Code and/or Modifications of Original Code and
 * its use is subject to the terms of the GNU Public License v3.0. You may not use
 * this file except in compliance with the license terms.
 *
 * You should have received a copy of the GNU Public License v3.0 along with this
 * software; see the file LICENSE.html.  If not, write to or contact:
 *
 * SSHTOOLS, PO BOX 9700, Langar, Nottinghamshire. NG13 9WE
 *
 * Email:     support@sshtools.com
 * 
 * WWW:       http://www.sshtools.com
 *****************************************************************************/
using System;
using Maverick.Crypto.Util;

namespace Maverick.SSH
{
	
	/// <summary>
	/// This namespace contains a set of interfaces that will enable you to integrate SSH client
	/// functionality into your .NET applications. The interfaces are designed to be SSH protocol
	/// independent so that your applications can support both versions of the SSH protocol.
	/// </summary>
	internal class NamespaceDoc
	{
	}


	/// <summary>
	/// An abstract utility class used to store the available transport components 
	/// and provide delimited listing as required in the key exchange initialization 
	/// process.
	/// </summary>
	public abstract class AbstractComponentFactory
	{

		/// <summary>
		/// A hashtable containing the supported components. 
		/// </summary>
		protected internal System.Collections.Hashtable supported;
		internal System.Type type;
		
		private void  InitBlock()
		{
			supported = new System.Collections.Hashtable();
		}
		
		/// <summary>
		/// Create a component factory with the base type supplied.
		/// </summary>
		/// <param name="type"></param>
		public AbstractComponentFactory(System.Type type)
		{
			InitBlock();
			this.type = type;
		}
		/// <summary>
		/// Determine whether the factory supports a given component type. 
		/// </summary>
		/// <param name="name">The name given to this component when added to the factory.</param>
		/// <returns></returns>
		public virtual bool Contains(System.String name)
		{
			return supported.ContainsKey(name);
		}
		
		/// <summary>
		/// List the types of components supported by this factory. Returns the list as a comma 
		/// delimited string with the preferred value as the first entry in the list. If the 
		/// preferred value is "" then the list is returned un-ordered. 
		/// </summary>
		/// <param name="preferred">The name of the preferred component.</param>
		/// <returns></returns>
		public virtual System.String List(System.String preferred)
		{
			return CreateDelimitedList(preferred, supported.Keys.GetEnumerator());
		}
		
		/// <summary>
		/// Add a new component type to the factory. The name of the component IS NOT verified to 
		/// allow component implementations to be overriden. 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="cls"></param>
		public virtual void  Add(System.String name, System.Type cls)
		{
			SupportClass.PutElement(supported, name, cls);
		}
		
		/// <summary>
		/// Get a new instance of a supported component.
		/// </summary>
		/// <param name="name">The name of the instance required.</param>
		/// <returns></returns>
		/// <exception cref="Maverick.SSH.SSHException">Thrown if the object name is not supported</exception>
		public virtual System.Object GetInstance(System.String name)
		{
			if (supported.ContainsKey(name))
			{
				try
				{
					return CreateInstance(name, (System.Type) supported[name]);
				}
				catch (System.Exception t)
				{
					throw new SSHException(t.Message, SSHException.INTERNAL_ERROR);
				}
			}
			else
			{
				throw new SSHException(name + " is not supported",
					SSHException.UNSUPPORTED_ALGORITHM);
			}
		}
		
		/// <summary>
		/// Overide this method to create an instance of the component. 
		/// </summary>
		/// <param name="name">The name of the instance being created</param>
		/// <param name="cls">The type of the instance being created</param>
		/// <returns></returns>
		protected internal abstract System.Object CreateInstance(System.String name, System.Type cls);
		
		private System.String CreateDelimitedList(System.String preferred, System.Collections.IEnumerator names)
		{
			System.String list = preferred;
			System.String i;
			
			while (names.MoveNext())
			{
				i = (System.String) names.Current;
				if (!i.Equals(preferred))
				{
					list += (list.Length == 0?"":",") + i;
				}
			}
			return list;
		}
		
		/// <summary>
		/// Remove a supported component.
		/// </summary>
		/// <param name="name">The name of the component to be removed.</param>
		public virtual void  Remove(System.String name)
		{
			SupportClass.HashtableRemove(supported, name);
		}
	}
}
