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
 * 
 * This file has been derived from the BouncyCastle Java Crypto API
 * 
 * Copyright (c) 2000 The Legion Of The Bouncy Castle (http://www.bouncycastle.org)
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do 
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *****************************************************************************/
using System;
using System.IO;
using System.Text;
using Maverick.Crypto.Math;

namespace Maverick.Crypto.IO
{
	/// <summary>
	/// Summary description for Buffer.
	/// </summary>
	public class ByteBuffer
	{
		protected internal byte[] buf;
		protected internal int pos;
		protected internal int used;
		protected internal int mark;
		protected internal int maximumSize = -1;
		protected internal int incrementBlockSize = 4096;

		internal Encoding encoding = System.Text.Encoding.UTF8;

		public ByteBuffer(int size)
		{
			if(size <= 0)
				throw new IOException("Buffer must have a size > 0");
			buf = new byte[size];
			Reset();

		}

		public ByteBuffer() : this(4096)
		{
			
		}

		public ByteBuffer(ByteBuffer buffer)
		{
			this.buf = buffer.buf;
			this.used = buffer.used;
			this.pos = buffer.pos;
		}

		public ByteBuffer(byte[] buf)
		{
			this.buf = buf;
			this.used = buf.Length;
			this.pos = 0;
		}

		public System.Text.Encoding Encoding
		{
			get 
			{

				return encoding;
			}

			set
			{
				this.encoding = value;
			}

		}

		public byte[] Array
		{
			get
			{
				return buf;
			}
		}

		internal void CheckArrayLimits(int bytesToAdd)
		{
			if(buf.Length - used < bytesToAdd)
			{
				if(maximumSize > -1)
				{
					// Check here for boundry limits
				}
				bytesToAdd  = System.Math.Max(bytesToAdd, incrementBlockSize);
				byte[] tmp = new byte[buf.Length + bytesToAdd];
				System.Array.Copy(buf, 0, tmp, 0, used);
				buf = tmp;
			}
		}

		public int Position
		{ 
			get
			{
				return pos;
			}
		}

		public int Length
		{
			get
			{
				return used;
			}
		}

		public int Limit
		{
			get
			{
				return buf.Length;
			}
		}

		public int Available
		{
			get
			{
				return Length - pos;
			}
		}

		public void Skip(int num)
		{
			UpdatePosition(num);
		}

		public void MoveToPosition(int pos)
		{
			this.pos = pos;
		}

		public void MoveToEnd()
		{
			pos = used;
		}

		public void MoveToMark() 
		{
			pos = mark;
		}

		public void Mark()
		{
			mark = pos;
		}

		internal void UpdatePosition(int num)
		{	
			if(pos==used)
			{
				pos = used += num;
			}
			else
			{
				

				int add = (pos+num) - used;
				
				pos+=num;

				if(add > 0)
					used += add;
			}

		}

		public void Reset() 
		{
			pos = 0;
			used = 0;
		}

		public void WriteBool(bool b)
		{
			CheckArrayLimits(1);

			buf[pos] = (byte) (b ? 1 : 0);

			UpdatePosition(1);
		}

		public void WriteUINT32(uint v)
		{
			CheckArrayLimits(4);

			int current = pos;

			buf[current++] = (byte)(v >> 24);
			buf[current++] = (byte)(v >> 16);
			buf[current++] = (byte)(v >> 8);
			buf[current++] = (byte) v;
			
			UpdatePosition(4);

		}

		public void WriteInt(int v) 
		{
			WriteUINT32(v);
		}

		public void WriteMPINT(BigInteger b) 
		{
			short bytes = (short) ((b.bitLength() + 7) / 8);
			byte[] raw = b.toByteArray();
			WriteShort( (short) b.bitLength());
			if (raw[0] == 0) 
			{
				WriteBytes(raw, 1, bytes);
			}
			else 
			{
				WriteBytes(raw, 0, bytes);
			}
		}


		public short ReadShort() 
		{
			int ch1 = ReadByte();
			int ch2 = ReadByte();

			return (short) ((ch1 << 8) + (ch2 << 0));
		}

		public void WriteShort(short s) 
		{
			Write( s >> 8);
			Write( s >> 0);
		}

		public void WriteUINT32(int v)
		{
			CheckArrayLimits(4);

			int current = pos;

			buf[current++] = (byte)(v >> 24);
			buf[current++] = (byte)(v >> 16);
			buf[current++] = (byte)(v >> 8);
			buf[current++] = (byte) v;
			
			UpdatePosition(4);

		}



		public void WriteUINT64(long v)
		{
			WriteUINT64((ulong)v);
		}

		public void WriteUINT64(ulong v)
		{
			CheckArrayLimits(8);

			int current = pos;

			buf[current++] = (byte)(v >> 56);
			buf[current++] = (byte)(v >> 48);
			buf[current++] = (byte)(v >> 40);
			buf[current++] = (byte)(v >> 32);
			buf[current++] = (byte)(v >> 24);
			buf[current++] = (byte)(v >> 16);
			buf[current++] = (byte)(v >> 8);
			buf[current++] = (byte) v;
			
			UpdatePosition(8);

		}

		public void WriteByte(int b)
		{
			WriteByte((byte)b);
		}

		public void WriteByte(byte v)
		{
			CheckArrayLimits(1);

			int current = pos;

			buf[current++] = v;
			
			UpdatePosition(1);
		}

		public void Write(int b)
		{
			WriteByte((byte)b);
		}

		public void WriteBytes(byte[] b)
		{
			WriteBytes(b, 0, b.Length);
		}

		public void WriteBytes(byte[] b, int offset, int length)
		{
			CheckArrayLimits(length);

			System.Array.Copy(b, offset, buf, pos, length);
			
			UpdatePosition(length);
		}

		public void WriteString(String v)
		{
			WriteString(v, encoding);
		}

		public void WriteString(String v, Encoding encoding)
		{
			byte[] array = encoding.GetBytes(v);
			
			WriteUINT32(array.Length);
			WriteBytes(array, 0, array.Length);

		}

		public void WriteBinaryString(byte[] b, int offset, int length)
		{
			WriteUINT32(length);
			WriteBytes(b, offset, length);
		}

		public void WriteBinaryString(byte[] b)
		{
			WriteBinaryString(b, 0, b.Length);
		}

		public void WriteBigInteger(BigInteger b)
		{
			byte[] tmp = b.toByteArray();
			if(tmp[0] >= 0x80) 
			{
				WriteUINT32(tmp.Length+1);
				WriteByte((byte)0);
			}
			else
			{
				WriteUINT32(tmp.Length);
			}

			WriteBytes(tmp);
		}

		public uint ReadInt() 
		{
			return ReadUINT32();
		}



		public uint ReadUINT32() 
		{
			return (uint)( (buf[pos++] << 24) 
				| (buf[pos++] << 16)
				| (buf[pos++] << 8)
				| (buf[pos++]));
		}

		public static uint ReadInt(byte[] buf, int start)
		{
			if(start > buf.Length - 4)
				throw new ArgumentOutOfRangeException("Buffer too short for Integer read operation");

			return (uint)( (buf[start] << 24) 
				| (buf[start+1] << 16)
				| (buf[start+2] << 8)
				| (buf[start+3]));
		}

		public ulong ReadUINT64()
		{
			ulong u = ReadUINT64(buf, pos);
			UpdatePosition(8);
			return u;

		}

		public long ReadLong()
		{
			byte[] tmp = new byte[8];
			ReadBytes(tmp);
			return new BigInteger(tmp).longValue();
		}

		public static uint ReadUINT32(byte[] buf, int off)
		{
			return (uint)( (buf[off++] << 24) 
				| (buf[off++] << 16)
				| (buf[off++] << 8)
				| (buf[off]));
		}

		public static ulong ReadUINT64(byte[] buf, int off)
		{

			byte[] tmp = new byte[8];
			System.Array.Copy(buf, off, tmp, 0, 8);
			return (ulong)new BigInteger(tmp).longValue();
		}

		public byte ReadByte() 
		{
			return buf[pos++];
		}

		public bool ReadBool()
		{
			return buf[pos++] == 1;
		}

		public int Read()
		{
			return ReadByte();
		}

		public int ReadBytes(byte[] b)
		{
			return ReadBytes(b, 0, b.Length);
		}

		public int ReadBytes(byte[] b, int off, int length)
		{
			int count = System.Math.Min(Available, length);
			System.Array.Copy(buf, pos, b, off, count);
			pos+= count;
			return count;
		}

		public String ReadString()
		{
			return ReadString(encoding);
		}

		public String ReadString(Encoding encoding)
		{
			int count = (int)ReadUINT32();
			byte[] tmp = new byte[count];
			ReadBytes(tmp,0, count);
			return ConvertBytesToString(tmp, 0, tmp.Length, encoding);
		}

		public String ConvertBytesToString(byte[] b, int off, int len, Encoding localEncoding)
		{
			char[] array = new char[localEncoding.GetCharCount(b, off, len)];
			localEncoding.GetChars(b, off, len, array, 0);
			return new String(array);
		}
		
		public String ConvertBytesToString(byte[] b, int off, int len)
		{
			char[] array = new char[encoding.GetCharCount(b, off, len)];
			encoding.GetChars(b, off, len, array, 0);
			return new String(array);
		}

		public static String ConvertBytesToAsciiString(byte[] b, int off, int len)
		{
			Encoding encoding = Encoding.ASCII;
			char[] array = new char[encoding.GetCharCount(b, off, len)];
			encoding.GetChars(b, off, len, array, 0);
			return new String(array);

		}

		public static byte[] ConvertStringToBytes(String str, Encoding encoding)
		{
			char[] tmp = str.ToCharArray();
			byte[] array = new byte[encoding.GetByteCount(tmp, 0, tmp.Length)];
			encoding.GetBytes(tmp, 0, tmp.Length, array, 0);
			return array;
		}


		public static byte[] ConvertAsciiStringToBytes(String str)
		{
			Encoding encoding = Encoding.ASCII;
			char[] tmp = str.ToCharArray();
			byte[] array = new byte[encoding.GetByteCount(tmp, 0, tmp.Length)];
			encoding.GetBytes(tmp, 0, tmp.Length, array, 0);
			return array;

		}

		public byte[] ReadBinaryString()
		{
			int count = (int) ReadUINT32();
			byte[] tmp = new byte[count];
			ReadBytes(tmp);
			return tmp;

		}

		public BigInteger ReadBigInteger()
		{	
			int count = (int)ReadUINT32();
			byte[] tmp = new byte[count];
			ReadBytes(tmp,0,count);
			return new BigInteger(tmp);

		}




		public BigInteger ReadMPINT()
		{
			short bits = ReadShort();

			byte[] raw = new byte[(bits + 7) / 8 + 1];

			raw[0] = 0;
			ReadBytes(raw, 1, raw.Length - 1);

			return new BigInteger(raw);
		}

		public BigInteger ReadMPINT32()
		{
			uint bits = ReadInt();

			byte[] raw = new byte[((bits+7)/8) + 1];
			raw[0] = 0;
			ReadBytes(raw, 1, raw.Length - 1);

			return new BigInteger(raw);
		
		}

		public byte[] ToByteArray()
		{
			byte[] tmp = new byte[used];
			System.Array.Copy(buf, 0, tmp, 0, used);
			return tmp;

		}


		public void WriteToStream(Stream output)
		{
			output.Write(buf, 0, used);
			output.Flush();
		}

		public void ReadFromStream(Stream input, int numBytes)
		{

			CheckArrayLimits(numBytes);

			int n = 0;
			while (n < numBytes) 
			{
				int count = input.Read(buf, pos, numBytes - n);
                if (count <= 0 || count > numBytes)
                {
                    throw new EndOfStreamException("Failed to read required number of bytes from stream count="+count+" numBytes="+numBytes);
                }
				pos+=count;
				used+=count;
				n += count;
			}
		}


	}
}
