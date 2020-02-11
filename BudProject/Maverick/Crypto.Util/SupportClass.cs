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

namespace Maverick.Crypto.Util
{
	public interface IThreadRunnable
	{
		void Run();
	}

	public class SupportClass
	{

		static char[] hexDigits = {
									  '0', '1', '2', '3', '4', '5', '6', '7',
									  '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
 
	
		public static string ToHexString(byte[] bytes) 
		{
			char[] chars = new char[bytes.Length * 2];
			for (int i = 0; i < bytes.Length; i++) 
			{
				int b = bytes[i];
				chars[i * 2] = hexDigits[b >> 4];
				chars[i * 2 + 1] = hexDigits[b & 0xF];
			}
			return new string(chars);
		}

		public static String ToHexString(int b)
		{
			return ToHexString(new byte[]{ (byte)b});
		}

		public static String GetHomeDir()	
		{		
			try
			{
				return Environment.GetEnvironmentVariable("USERPROFILE");	
			} 
			catch
			{
				try
				{
					return Environment.GetEnvironmentVariable("HOME");	
				}
				catch
				{
					return "";
				}
			}
		}

		public static String GetUsername() 
		{
			try
			{
				return Environment.GetEnvironmentVariable("USERNAME");	
			}
			catch
			{
				try
				{
					return Environment.GetEnvironmentVariable("USER");	
				}
				catch
				{

					return "";
				}
			}
		}

		public static System.Object PutElement(System.Collections.Hashtable hashTable, System.Object key, System.Object newValue)
		{
			System.Object element = hashTable[key];
			hashTable[key] = newValue;
			return element;
		}

		/*******************************/
		/// <summary>
		/// Removes the element with the specified key from a Hashtable instance.
		/// </summary>
		/// <param name="hashtable">The Hashtable instance</param>
		/// <param name="key">The key of the element to remove</param>
		/// <returns>The element removed</returns>  
		public static System.Object HashtableRemove(System.Collections.Hashtable hashtable, System.Object key)
		{
			System.Object element = hashtable[key];
			hashtable.Remove(key);
			return element;
		}

		/*******************************/
		/// <summary>
		/// Converts an array of sbytes to an array of bytes
		/// </summary>
		/// <param name="sbyteArray">The array of sbytes to be converted</param>
		/// <returns>The new array of bytes</returns>
		public static byte[] ToByteArray(sbyte[] sbyteArray)
		{
			byte[] byteArray = new byte[sbyteArray.Length];
			for(int index=0; index < sbyteArray.Length; index++)
				byteArray[index] = (byte) sbyteArray[index];
			return byteArray;
		}

		/// <summary>
		/// Converts a string to an array of bytes
		/// </summary>
		/// <param name="sourceString">The string to be converted</param>
		/// <returns>The new array of bytes</returns>
		public static byte[] ToByteArray(string sourceString)
		{
			
			byte[] byteArray = new byte[sourceString.Length];
			for (int index=0; index < sourceString.Length; index++)
				byteArray[index] = (byte) sourceString[index];
			return System.Text.Encoding.UTF8.GetBytes(sourceString); //byteArray;
		}

		/*******************************/
		public static sbyte[] ToSByteArray(byte[] byteArray)
		{
			sbyte[] sbyteArray = new sbyte[byteArray.Length];
			for(int index=0; index < byteArray.Length; index++)
				sbyteArray[index] = (sbyte) byteArray[index];
			return sbyteArray;
		}

		/*******************************/
		/// <summary>Reads a number of characters from the current source Stream and writes the data to the target array at the specified index.</summary>
		/// <param name="sourceStream">The source Stream to read from</param>
		/// <param name="target">Contains the array of characteres read from the source Stream.</param>
		/// <param name="start">The starting index of the target array.</param>
		/// <param name="count">The maximum number of characters to read from the source Stream.</param>
		/// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source Stream.</returns>
		public static System.Int32 ReadInput(System.IO.Stream sourceStream, ref sbyte[] target, int start, int count)
		{
			byte[] receiver = new byte[target.Length];
			int bytesRead   = sourceStream.Read(receiver, start, count);
			
			for(int i = start; i < start + bytesRead; i++)
				target[i] = (sbyte)receiver[i];
			
			return bytesRead;
		}

		/// <summary>Reads a number of characters from the current source TextReader and writes the data to the target array at the specified index.</summary>
		/// <param name="sourceTextReader">The source TextReader to read from</param>
		/// <param name="target">Contains the array of characteres read from the source TextReader.</param>
		/// <param name="start">The starting index of the target array.</param>
		/// <param name="count">The maximum number of characters to read from the source TextReader.</param>
		/// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source TextReader.</returns>
		public static System.Int32 ReadInput(System.IO.TextReader sourceTextReader, ref sbyte[] target, int start, int count)
		{
			char[] charArray = new char[target.Length];
			int bytesRead = sourceTextReader.Read(charArray, start, count);

			for(int index=start; index<start+bytesRead; index++)
				target[index] = (sbyte)charArray[index];

			return bytesRead;
		}

		/*******************************/
		/// <summary>
		/// Creates an instance of a received Type
		/// </summary>
		/// <param name="classType">The Type of the new class instance to return</param>
		/// <returns>An Object containing the new instance</returns>
		public static System.Object CreateNewInstance(System.Type classType)
		{
			System.Reflection.ConstructorInfo[] constructors = classType.GetConstructors();

			if (constructors.Length == 0)
				return null;

			System.Reflection.ParameterInfo[] firstConstructor = constructors[0].GetParameters();
			int countParams = firstConstructor.Length;

			System.Type[] constructor = new System.Type[countParams];
			for( int i = 0; i < countParams; i++)
				constructor[i] = firstConstructor[i].ParameterType;

			return classType.GetConstructor(constructor).Invoke(new System.Object[]{});
		}

		/*******************************/
		/// <summary>
		/// Converts an array of sbytes to an array of chars
		/// </summary>
		/// <param name="sByteArray">The array of sbytes to convert</param>
		/// <returns>The new array of chars</returns>
		public static char[] ToCharArray(sbyte[] sByteArray) 
		{
			char[] charArray = new char[sByteArray.Length];	   
			sByteArray.CopyTo(charArray, 0);
			return charArray;
		}

		/// <summary>
		/// Converts an array of bytes to an array of chars
		/// </summary>
		/// <param name="byteArray">The array of bytes to convert</param>
		/// <returns>The new array of chars</returns>
		public static char[] ToCharArray(byte[] byteArray) 
		{
			char[] charArray = new char[byteArray.Length];	   
			byteArray.CopyTo(charArray, 0);
			return charArray;
		}

		/*******************************/
		public class ThreadClass:IThreadRunnable
		{
			private System.Threading.Thread threadField;

			public ThreadClass()
			{
				threadField = new System.Threading.Thread(new System.Threading.ThreadStart(Run));
			}

			public ThreadClass(System.Threading.ThreadStart p1)
			{
				threadField = new System.Threading.Thread(p1);
			}

			public virtual void Run()
			{
			}

			public virtual void Start()
			{
				threadField.Start();
			}

			public System.Threading.Thread Instance
			{
				get
				{
					return threadField;
				}
				set
				{
					threadField	= value;
				}
			}

			public System.String Name
			{
				get
				{
					return threadField.Name;
				}
				set
				{
					if (threadField.Name == null)
						threadField.Name = value; 
				}
			}

			public System.Threading.ThreadPriority Priority
			{
				get
				{
					return threadField.Priority;
				}
				set
				{
					threadField.Priority = value;
				}
			}

			public bool IsAlive
			{
				get
				{
					return threadField.IsAlive;
				}
			}

			public bool IsBackground
			{
				get
				{
					return threadField.IsBackground;
				} 
				set
				{
					threadField.IsBackground = value;
				}
			}

			public void Join()
			{
				threadField.Join();
			}

			public void Join(long p1)
			{
				lock(this)
				{
					threadField.Join(new System.TimeSpan(p1 * 10000));
				}
			}

			public void Join(long p1, int p2)
			{
				lock(this)
				{
					threadField.Join(new System.TimeSpan(p1 * 10000 + p2 * 100));
				}
			}

			//public void Resume()
			//{
			//	threadField.Resume();
			//}

			public void Abort()
			{
				threadField.Abort();
			}

			public void Abort(System.Object stateInfo)
			{
				lock(this)
				{
					threadField.Abort(stateInfo);
				}
			}

			//public void Suspend()
			//{
			//	threadField.Suspend();
			//}

			public override System.String ToString()
			{
				return "Thread[" + Name + "," + Priority.ToString() + "," + "" + "]";
			}

			public static ThreadClass Current()
			{
				ThreadClass CurrentThread = new ThreadClass();
				CurrentThread.Instance = System.Threading.Thread.CurrentThread;
				return CurrentThread;
			}
		}

		/*******************************/
		public static int URShift(int number, int bits)
		{
			if ( number >= 0)
				return number >> bits;
			else
				return (number >> bits) + (2 << ~bits);
		}

		public static int URShift(int number, long bits)
		{
			return URShift(number, (int)bits);
		}

		public static long URShift(long number, int bits)
		{
			if ( number >= 0)
				return number >> bits;
			else
				return (number >> bits) + (2L << ~bits);
		}

		public static long URShift(long number, long bits)
		{
			return URShift(number, (int)bits);
		}

		/*******************************/
		/// <summary>
		/// Removes the first occurrence of an specific object from an ArrayList instance.
		/// </summary>
		/// <param name="arrayList">The ArrayList instance</param>
		/// <param name="element">The element to remove</param>
		/// <returns>True if item is found in the ArrayList; otherwise, false</returns>  
		public static System.Boolean VectorRemoveElement(System.Collections.ArrayList arrayList, System.Object element)
		{
			System.Boolean containsItem = arrayList.Contains(element);
			arrayList.Remove(element);
			return containsItem;
		}

		/*******************************/
		/// <summary>
		/// This method is used as a dummy method to simulate VJ++ behavior
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static long Identity(long literal)
		{
			return literal;
		}

		/// <summary>
		/// This method is used as a dummy method to simulate VJ++ behavior
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static ulong Identity(ulong literal)
		{
			return literal;
		}

		/// <summary>
		/// This method is used as a dummy method to simulate VJ++ behavior
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static float Identity(float literal)
		{
			return literal;
		}

		/// <summary>
		/// This method is used as a dummy method to simulate VJ++ behavior
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static double Identity(double literal)
		{
			return literal;
		}

		/*******************************/
		/// <summary>
		/// Adds an element to the top end of a Stack instance.
		/// </summary>
		/// <param name="stack">The Stack instance</param>
		/// <param name="element">The element to add</param>
		/// <returns>The element added</returns>  
		public static System.Object StackPush(System.Collections.Stack stack, System.Object element)
		{
			stack.Push(element);
			return element;
		}

		/*******************************/
		public class DateTimeFormatManager
		{
			static public DateTimeFormatHashTable manager = new DateTimeFormatHashTable();

			public class DateTimeFormatHashTable :System.Collections.Hashtable 
			{
				public void SetDateFormatPattern(System.Globalization.DateTimeFormatInfo format, System.String newPattern)
				{
					if (this[format] != null)
						((DateTimeFormatProperties) this[format]).DateFormatPattern = newPattern;
					else
					{
						DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
						tempProps.DateFormatPattern  = newPattern;
						Add(format, tempProps);
					}
				}

				public string GetDateFormatPattern(System.Globalization.DateTimeFormatInfo format)
				{
					if (this[format] == null)
						return "d-MMM-yy";
					else
						return ((DateTimeFormatProperties) this[format]).DateFormatPattern;
				}
			
				public void SetTimeFormatPattern(System.Globalization.DateTimeFormatInfo format, System.String newPattern)
				{
					if (this[format] != null)
						((DateTimeFormatProperties) this[format]).TimeFormatPattern = newPattern;
					else
					{
						DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
						tempProps.TimeFormatPattern  = newPattern;
						Add(format, tempProps);
					}
				}

				public string GetTimeFormatPattern(System.Globalization.DateTimeFormatInfo format)
				{
					if (this[format] == null)
						return "h:mm:ss tt";
					else
						return ((DateTimeFormatProperties) this[format]).TimeFormatPattern;
				}

				class DateTimeFormatProperties
				{
					public string DateFormatPattern = "d-MMM-yy";
					public string TimeFormatPattern = "h:mm:ss tt";
				}
			}	
		}

		/*******************************/
		public static string FormatDateTime(System.Globalization.DateTimeFormatInfo format, System.DateTime date)
		{
			string timePattern = DateTimeFormatManager.manager.GetTimeFormatPattern(format);
			string datePattern = DateTimeFormatManager.manager.GetDateFormatPattern(format);
			return date.ToString(datePattern + " " + timePattern, format);            
		}

		/*******************************/
		public static System.Object Deserialize(System.IO.BinaryReader binaryReader)
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			return formatter.Deserialize(binaryReader.BaseStream);
		}

		/*******************************/
		public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
		{
			stream.Write(throwable.StackTrace);
			stream.Flush();
		}

		/*******************************/
		/// <summary>
		/// Writes an object to the specified Stream
		/// </summary>
		/// <param name="stream">The target Stream</param>
		/// <param name="objectToSend">The object to be sent</param>
		public static void Serialize(System.IO.Stream stream, System.Object objectToSend)
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			formatter.Serialize(stream, objectToSend);
		}

		/// <summary>
		/// Writes an object to the specified BinaryWriter
		/// </summary>
		/// <param name="stream">The target BinaryWriter</param>
		/// <param name="objectToSend">The object to be sent</param>
		public static void Serialize(System.IO.BinaryWriter binaryWriter, System.Object objectToSend)
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			formatter.Serialize(binaryWriter.BaseStream, objectToSend);
		}

		/*******************************/
		public static long FileLength(System.IO.FileInfo file)
		{
			if (System.IO.Directory.Exists(file.FullName))
				return 0;
			else 
				return file.Length;
		}

		/*******************************/
		public class Tokenizer
		{
			private System.Collections.ArrayList elements;
			private string source;
			//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
			private string delimiters = " \t\n\r";		
            private bool includeDelim = false;

			public Tokenizer(string source)
			{			
				this.elements = new System.Collections.ArrayList();
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
			}

			public Tokenizer(string source, string delimiters)
			{
				this.elements = new System.Collections.ArrayList();
				this.delimiters = delimiters;
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
			}

			public Tokenizer(string source, string delimiters, bool includeDelim)
			{
				this.elements = new System.Collections.ArrayList();
				this.delimiters = delimiters;
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
				this.includeDelim = includeDelim;
			}

			public int Count
			{
				get
				{
					return (this.elements.Count);
				}
			}

			public bool HasMoreTokens()
			{
				return (this.elements.Count > 0);			
			}

			public string NextToken()
			{			
				string result;
				if (source == "") throw new System.Exception();
				else
				{
					this.elements = new System.Collections.ArrayList();
					this.elements.AddRange(this.source.Split(delimiters.ToCharArray()));
					RemoveEmptyStrings();		
					result = (string) this.elements[0];
					this.elements.RemoveAt(0);				
					this.source = this.source.Remove(this.source.IndexOf(result),result.Length);
					this.source = this.source.TrimStart(this.delimiters.ToCharArray());
					return result;					
				}			
			}

			public string NextToken(string delimiters)
			{
				this.delimiters = delimiters;
				return NextToken();
			}

			private void RemoveEmptyStrings()
			{
				//VJ++ does not treat empty strings as tokens
				for (int index=0; index < this.elements.Count; index++)
					if ((string)this.elements[index]== "")
					{
						this.elements.RemoveAt(index);
						index--;
					}
			}
		}

		/*******************************/
		public class RandomAccessFileSupport
		{
			public static System.IO.FileStream CreateRandomAccessFile(string fileName, string mode) 
			{
				System.IO.FileStream newFile = null;

				if (mode.CompareTo("rw") == 0)
					newFile =  new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite); 
				else if (mode.CompareTo("r") == 0 )
					newFile =  new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read); 
				else
					throw new System.ArgumentException();

				return newFile;
			}

			public static System.IO.FileStream CreateRandomAccessFile(System.IO.FileInfo fileName, string mode)
			{
				return CreateRandomAccessFile(fileName.FullName, mode);
			}

			public static void WriteBytes(string data,System.IO.FileStream fileStream)
			{
				int index = 0;
				int length = data.Length;

				while(index < length)
					fileStream.WriteByte((byte)data[index++]);	
			}

			public static void WriteChars(string data,System.IO.FileStream fileStream)
			{
				WriteBytes(data, fileStream);	
			}

			public static void WriteRandomFile(sbyte[] sByteArray,System.IO.FileStream fileStream)
			{
				byte[] byteArray = ToByteArray(sByteArray);
				fileStream.Write(byteArray, 0, byteArray.Length);
			}
		}

		/*******************************/
		public static bool FileCanWrite(System.IO.FileInfo file)
		{
			return (System.IO.File.GetAttributes(file.FullName) & System.IO.FileAttributes.ReadOnly) != System.IO.FileAttributes.ReadOnly;
		}


		public static DateTime ConvertFromEpochTime(long milliseconds)
		{
			/**
			 * Java time is the number of milliseconds since the standard base time 
			 * known as "the epoch", namely January 1, 1970, 00:00:00 GMT.
			 * 
			 * C# Time values are measured in 100-nanosecond units called ticks, 
			 * and a particular date is the number of ticks since 12:00 midnight, 
			 * January 1, 1 A.D. (C.E.) in the GregorianCalendar calendar
			 * */
			return new DateTime((milliseconds * 10000) + 621355968000000000L).ToLocalTime();
		}

		public static long ConvertToEpochTime(DateTime dt)
		{
			return (dt.ToUniversalTime().Ticks - 621355968000000000L) / 10000;


		}

	}
}
