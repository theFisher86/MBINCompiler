﻿//#define DEBUG_LOG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using libMBIN;

namespace MBINCompiler
{

    using static CommandLineOptions;
    using static Convert;

    public enum ErrorCode {
        Success =  0,
        Unknown,
        CommandLine,
        FileNotFound,
        FileExists,
        FileInvalid,
    }

    internal class Program
    {

        public static int Main( string[] args )
        {

            var logStream = new FileStream( $"{Utils.GetExecutableName()}.log", FileMode.Create );
            Logger.AddStream( logStream );
            Debug.Listeners.Add( new TextWriterTraceListener( logStream ) );

#if DEBUG_LOG // log to console
            Debug.Listeners.Add( new ConsoleTraceListener() );
#endif

            var options = new CommandLineParser( args );
            options.AddOptions( null,      OPTIONS_GENERAL );
            options.AddOptions( "help",    OPTIONS_HELP );
            options.AddOptions( "version", OPTIONS_VERSION );
            options.AddOptions( "convert", OPTIONS_CONVERT );

            // save the error state
            bool invalidArguments = !options.Parse( "convert" );

            // get the Quiet option first, before we emit anything
            Quiet = options.GetOptionSwitch( "quiet" );
            if ( !Quiet ) Logger.AddStream( System.Console.OpenStandardOutput() );

            // now we can emit an error if we need to
            if ( invalidArguments ) return Console.ShowInvalidCommandLineArg( options );

            try {

                switch ( options.Verb ) {
                    case "help": return Console.ShowHelp();
                    case "version": return HandleVersionMode( options );
                    default: return HandleConvertMode( options );
                }

            } catch ( Exception e ) {
                e = e.GetBaseException();
                return Console.ShowError( $"{e.Message}\n\nStacktrace:\n\n{e.StackTrace}" );
            }

        }

        private static int HandleVersionMode( CommandLineParser options )
        {
            var files = options.GetFileParams();
            if ( files.Count == 0 ) return Console.ShowVersion( Quiet );
            if ( files.Count >  1 ) return Console.ShowInvalidCommandLineArg( files[1] );

            var fIn = new FileStream( files[0], FileMode.Open, FileAccess.Read );
            var mbin = new MBINFile( fIn );
            if ( !mbin.Load() || !mbin.Header.IsValid ) {
                return Console.ShowCommandLineError( "Invalid file type.\n" +
                                             "Only MBIN files can be versioned.\n" +
                                            $"\"{files[0]}\"" );
            }

            Console.WriteLine( Version.GetMBINVersion( mbin, Quiet ) );
            return (int) ErrorCode.Success;
        }

        private static int HandleConvertMode( CommandLineParser options )
        {
            var paths = options.GetFileParams();

            var code = GetOverwriteOption( options, out var overwrite );
            if ( code != (int) ErrorCode.Success ) return code;

            var force = options.GetOptionSwitch( "force" );

            var inputDir = paths[0];
            var outputDir = options.GetOptionArg( "output-dir" )?.value;
            if ( outputDir != null ) {
                if ( paths.Count > 1 ) return Console.ShowInvalidCommandLineArg( paths[1] );
                outputDir = Path.GetFullPath( outputDir );
                if ( File.Exists( inputDir ) ) inputDir = Path.GetDirectoryName( inputDir );
            }

            var optFormatI = options.GetOptionArg( "input-format" );
            var optFormatO = options.GetOptionArg( "output-format" );

            var optIncludes = options.GetOptionArg( "include" );
            var optExcludes = options.GetOptionArg( "exclude" );

            var formatI = optFormatI?.value.ToUpper();
            var formatO = optFormatO?.value.ToUpper();

            // handle --input-format and --output-format options
            // and set the default include filters accordingly
            var defaultInclude = "*";
            bool autoFormat = ( formatI == null ) && ( formatO == null );
            if ( autoFormat ) {
                defaultInclude = "*.MBIN;*.MBIN.PC;*.EXML";
            } else {
                code = SetFormatOptions( formatI, formatO );
                if ( code != (int) ErrorCode.Success ) return code;
                defaultInclude = ( InputFormat == FormatType.MBIN ) ? "*.MBIN;*.MBIN.PC" : "*.EXML";
            }

            IncludeFilters = new List<string>( ( optIncludes?.value ?? defaultInclude ).Split( ';' ) );
            ExcludeFilters = new List<string>( ( optExcludes?.value ?? @"LANGUAGE\*;*.GEOMETRY.*" ).Split( ';' ) );
            // if not auto-detecting then OutputFormat can be excluded
            if ( !autoFormat ) ExcludeFilters.Add( $"*.{OutputFormat}" );

            // generate a filtered file listing of the combined paths
            code = GetFileList( paths, out var fileList );
            if ( code != (int) ErrorCode.Success ) return code;

            if ( autoFormat ) {
                code = AutoDetectFormat( fileList );
                if ( code != (int) ErrorCode.Success ) return code;
            }

            Debug.WriteLine( $"--input-format={InputFormat} --output-format={OutputFormat}" );

            return (int) ConvertFileList( inputDir, outputDir, fileList, force );
        }

        private static int GetOverwriteOption( CommandLineParser options, out OverwriteMode mode )
        {
            mode = OverwriteMode.Prompt;

            var optO = options.GetOptionArg( "overwrite" );
            var optK = options.GetOptionArg( "keep" );
            bool isO = ( optO != null );
            bool isK = ( optK != null );
            if ( isO && isK ) return Console.ShowCommandLineError( $"The {optO.name} and {optK.name} options cannot be used together." );

            mode = isO ? OverwriteMode.Always : isK ? OverwriteMode.Never : mode;
            return (int) ErrorCode.Success;
        }

        private static int SetFormatOptions( string formatI, string formatO )
        {
            if ( formatI != null ) {
                InputFormat = ( formatI == "MBIN" ) ? FormatType.MBIN : InputFormat;
                InputFormat = ( formatI == "EXML" ) ? FormatType.EXML : InputFormat;
                if ( InputFormat == FormatType.Unknown ) {
                    return Console.ShowCommandLineError( $"Invalid format specified: {formatI}" );
                }
            }

            if ( formatO != null ) {
                OutputFormat = ( formatO == "MBIN" ) ? FormatType.MBIN : OutputFormat;
                OutputFormat = ( formatO == "EXML" ) ? FormatType.EXML : OutputFormat;
                if ( OutputFormat == FormatType.Unknown ) {
                    return Console.ShowCommandLineError( $"Invalid format specified: {formatI}" );
                }
            }

            if ( formatI == null ) InputFormat = ( OutputFormat == FormatType.MBIN ) ? FormatType.EXML : FormatType.MBIN;
            if ( formatO == null ) OutputFormat = ( InputFormat == FormatType.MBIN ) ? FormatType.EXML : FormatType.MBIN;

            if ( InputFormat == OutputFormat ) {
                return Console.ShowCommandLineError( "--input-format and --output-format cannot be the same type!" );
            }

            return (int) ErrorCode.Success;
        }

        private static int GetFileList( List<string> paths, out List<string> fileList )
        {
            fileList = new List<string>();
            foreach ( var path in paths ) {
                if ( File.Exists( path ) ) {
                    fileList.Add( path );
                } else if ( Directory.Exists( path ) ) {
                    fileList.AddRange( GetFilteredFiles( path ) );
                } else {
                    return Console.ShowCommandLineError( $"Invalid path.\n\"{path}\"" );
                }
            }

            return (int) ErrorCode.Success;
        }

        private static List<string> GetFilteredFiles( string path )
        {
            var files = new List<string>();

            var includeFiles = new List<string>();
            var excludeFiles = new List<string>();
            foreach ( var filter in IncludeFilters ) {
                includeFiles.AddRange( GetDirectoryFiles( path, filter ) );
            }
            foreach ( var filter in ExcludeFilters ) {
                excludeFiles.AddRange( GetDirectoryFiles( path, filter ) );
            }

            // add the filtered files to fileList
            foreach ( var file in includeFiles ) {
                if ( !excludeFiles.Contains( file ) ) files.Add( file );
            }

            return files;
        }

        private static string[] GetDirectoryFiles( string path, string filter )
        {
            try {
                return Directory.GetFiles( path, filter, SearchOption.AllDirectories );
            } catch ( DirectoryNotFoundException ) { }
            return new string[] { };
        }

        private static int AutoDetectFormat( List<string> fileList )
        {
            // detect what types of file formats are found
            bool foundMBIN = false;
            bool foundEXML = false;
            foreach ( var file in fileList ) {
                if ( Path.HasExtension( file ) ) {
                    var ext = Path.GetExtension( file ).ToUpper();
                    foundMBIN |= ( ext == ".MBIN" ) || ( ext == ".PC" );
                    foundEXML |= ( ext == ".EXML" );
                }
            }

            if ( foundMBIN && foundEXML ) {
                Console.WriteLine( "Both MBIN and EXML file types were detected!\n" +
                    "Unable to automatically determine the --input-format type." );
                InputFormat = Utils.PromptInputFormat();
            } else if ( foundMBIN ) {
                Logger.WriteLine( "Auto-Detected --input-format=MBIN" );
                InputFormat = FormatType.MBIN;
            } else if ( foundEXML ) {
                Logger.WriteLine( "Auto-Detected --input-format=EXML" );
                InputFormat = FormatType.EXML;
            } else {
                return Console.ShowError( "No valid files found!" );
            }

            OutputFormat = ( InputFormat == FormatType.MBIN ) ? FormatType.EXML : FormatType.MBIN;

            return (int) ErrorCode.Success;
        }

    }
}
