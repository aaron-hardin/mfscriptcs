using System;
using System.Collections.Generic;
using System.Text;
using Mono.Terminal;
using ScriptCs.Contracts;

namespace ScriptCs.Embedded
{
	public class BufferedConsole : IConsole
	{
		private readonly LineEditor _editor;
		private readonly Queue<string> _stringQueue;

		public BufferedConsole()
		{
			this._stringQueue = new Queue<string>();
			this._editor = new LineEditor( "scriptcs" );
		}

		public void Write( string value )
		{
			this._stringQueue.Enqueue( value );
		}

		public void WriteLine()
		{
			this._stringQueue.Enqueue( Environment.NewLine );
		}

		public void WriteLine( string value )
		{
			this._stringQueue.Enqueue( value + Environment.NewLine );
		}

		public string ReadLine( string prompt )
		{
			// TODO
			WriteLine("Attempted to read, but it is not supported");
			return this._editor.Edit( prompt, "" );
		}

		public void Clear()
		{
			// TODO
			//Console.Clear();
		}

		public void Exit()
		{
			ResetColor();
			Environment.Exit( 0 );
		}

		public void ResetColor()
		{
			// TODO
			//Console.ResetColor();
		}

		public ConsoleColor ForegroundColor
		{
			// TODO
			//get { return Console.ForegroundColor; }
			//set { Console.ForegroundColor = value; }
			get { return ConsoleColor.Black; }
			set { }
		}

		public int Width
		{
			// TODO: returning hard-coded value since Console.BufferWidth throws exception if you redirect, need to find an actual value for this, perhaps the width of the textarea in the html
			//get { return Console.BufferWidth; }
			get { return 80; }
		}

		public string GetMessages()
		{
			const int maxSize = 10;
			var builder = new StringBuilder();
			for( int i = 0; i < maxSize && this._stringQueue.Count > 0; i++ )
			{
				builder.Append( this._stringQueue.Dequeue() );
			}
			return builder.ToString();
		}
	}
}
