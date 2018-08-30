﻿using System;

namespace MBINCompiler
{
    using static CommandLineOptions;

    internal class Console
    {

        public static void Write( object obj ) => System.Console.Write( obj );
        public static void WriteLine( object obj ) => System.Console.WriteLine( obj );
        public static void WriteLine() => System.Console.WriteLine();

        public static ConsoleKeyInfo ReadKey() => System.Console.ReadKey();
        public static string ReadLine() => System.Console.ReadLine();

        /// <summary>
        /// Display the help info and wait for a key press.
        /// </summary>
        /// <returns>Always returns 0 (exit code = success)</returns>
        public static int ShowHelp( ErrorCode code = ErrorCode.Success )
        {
            ShowHelpInfo();
            WaitForKeypress();
            return (int) code;
        }

        /// <summary>
        /// Display an error message and optionally wait for a key press.
        /// </summary>
        /// <param name="msg">The error message to display.</param>
        /// <param name="showHelp">If true, also calls <see cref="ShowHelp()"/>.</param>
        /// <param name="exitCode">The value to return, unless <paramref name="showHelp"/> is true.</param>
        /// <returns>Returns 1 if <paramref name="showHelp"/> is true, otherwise <paramref name="exitCode"/>.</returns>
        /// <summary>
        public static int ShowError( string msg, ErrorCode code = ErrorCode.Unknown, bool wait = true )
        {
            libMBIN.Logger.WriteLine( $"\nERROR: {msg}\n" );
            WaitForKeypress( wait );
            return (int) code;
        }

        /// <summary>
        /// Display a warning message.
        /// </summary>
        /// <param name="msg">The warning message to display.</param>
        public static void ShowWarning( string msg ) => libMBIN.Logger.WriteLine( $"WARNING: {msg}" );

        /// <summary>
        /// Display a command line parsing error message.
        /// <see cref="ShowError(String, Boolean, Int32)"/>
        /// </summary>
        /// <param name="msg">The error message to display.</param>
        /// <returns>ErrorCode.CommandLine</returns>
        public static int ShowCommandLineError( string msg )
        {
            ShowError( msg, wait: false );
            return ShowHelp( ErrorCode.CommandLine );
        }

        public static int ShowInvalidCommandLineArg( string arg ) => ShowCommandLineError( $"Invalid command line argument: {arg}" );
        public static int ShowInvalidCommandLineArg( CommandLineParser options ) => ShowInvalidCommandLineArg( options.Args[0] );

        /// <summary>
        /// Show the version string.
        /// </summary>
        /// <returns>Always returns 0 (exit code = success)</returns>
        public static int ShowVersion( bool quiet = false )
        {
            WriteLine( Version.GetVersionString( quiet ) );
            return 0;
        }

        /// <summary>
        /// Display "Press any key" and wait for keypress.
        /// </summary>
        public static void WaitForKeypress( bool wait = true )
        {
            if ( Quiet || !wait ) return;
            WriteLine( "\nPress any key to continue . . ." );
            ReadKey();
        }

        public static string WrapLine( string txt = null, int padleft = 0, int width = 0 ) {
            if (txt is null) { return ""; }
            width = ( width > 0 ) ? width : System.Console.BufferWidth;

            // split into lines
            string[] lines = txt.Split( new char[] { '\n' }, StringSplitOptions.None );

            // split each line into words and rebuild wrapped lines with proper indent
            txt = "";
            string padding = new string( ' ', padleft );
            string currentLine = padding;
            foreach(var line in lines ) {
                string[] words = line.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
                foreach (var word in words) {
                    if ((currentLine.Length + 1 + word.Length) >= width) { // flush
                        txt += currentLine + ( !currentLine.EndsWith( "\n" ) ? "\n" : "" );
                        currentLine = padding;
                    }
                    currentLine += ( currentLine.Length > padleft ) ? " " : "";
                    currentLine += word;
                }
                txt += currentLine + ( !currentLine.EndsWith( "\n" ) ? "\n" : "" );
                currentLine = padding;
            }

            return txt;
        }

    }
}
