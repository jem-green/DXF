﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using DXFLibrary;
using Microsoft.Win32;

namespace DXFConsole
{
    class App
    {
        #region Fields
        private readonly ILogger _logger;
        DXFDocument _dxf;
        #endregion
        #region Constructor
        public App(ILogger<App> logger)
        {
            if (logger != null)
            {
                _logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }
        }
        #endregion
        #region Methods
        public void Run(String[] args)
        {
            // Read in specific configuration

            _logger.LogDebug("In Run()");

            int pos = 0;
            Parameter HPGL2Path = new Parameter("");
            Parameter HPGL2Name = new Parameter("dxf.xml");

            // Required for the plot

            Parameter filename = new Parameter("");
            Parameter filePath = new Parameter("");
            Parameter outName = new Parameter("");

            HPGL2Path.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            pos = HPGL2Path.Value.ToString().LastIndexOf('\\');
            HPGL2Path.Value = HPGL2Path.Value.ToString().Substring(0, pos);
            HPGL2Path.Source = Parameter.SourceType.App;

            filePath.Value = Environment.CurrentDirectory;
            filePath.Source = Parameter.SourceType.App;

            try
            {
                RegistryKey key = Registry.LocalMachine;
                key = key.OpenSubKey("software\\green\\dxf\\");
                if (key != null)
                {
                    if (key.GetValue("path", "").ToString().Length > 0)
                    {
                        HPGL2Path.Value = (string)key.GetValue("path", HPGL2Path);
                        HPGL2Path.Source = Parameter.SourceType.Registry;
                        _logger.LogDebug("Use registry value Path=" + HPGL2Path);
                    }

                    if (key.GetValue("name", "").ToString().Length > 0)
                    {
                        HPGL2Name.Value = (string)key.GetValue("name", HPGL2Name);
                        HPGL2Name.Source = Parameter.SourceType.Registry;
                        _logger.LogDebug("Use registry value Name=" + HPGL2Name);
                    }
                }
            }
            catch (NullReferenceException)
            {
                _logger.LogError("Registry error use default values; Name=" + HPGL2Name.Value + " Path=" + HPGL2Path.Value);
            }
            catch (Exception e)
            {
                _logger.LogDebug(e.ToString());
            }

            // Check if the config file has been paased in and overwrite the registry

            int items = args.Length;
            for (int item = 0; item < items; item++)
            {
                {
                    switch (args[item])
                    {

                        case "/N":
                        case "--name":
                            {
                                HPGL2Name.Value = args[item + 1];
                                HPGL2Name.Value = HPGL2Name.Value.ToString().TrimStart('"');
                                HPGL2Name.Value = HPGL2Name.Value.ToString().TrimEnd('"');
                                HPGL2Name.Source = Parameter.SourceType.Command;
                                _logger.LogDebug("Use command value Name=" + HPGL2Name);
                                break;
                            }
                        case "/P":
                        case "--path":
                            {
                                HPGL2Path.Value = args[item + 1];
                                HPGL2Path.Value = HPGL2Path.Value.ToString().TrimStart('"');
                                HPGL2Path.Value = HPGL2Path.Value.ToString().TrimEnd('"');
                                HPGL2Path.Source = Parameter.SourceType.Command;
                                _logger.LogDebug("Use command value Path=" + HPGL2Path);
                                break;
                            }
                    }
                }
            }
            _logger.LogInformation("Use HPGL2Name=" + HPGL2Name.Value + " HPGL2Path=" + HPGL2Path.Value);

            // Read in configuration

            _dxf = new DXFDocument();

            //Serialise serialise = new Serialise(HPGL2Name.Value, HPGL2Path.Value, _logger);
            //_dxf = serialise.FromXML();
            //if (_dxf != null)
            //{

            //}

            // Read in the plot specific parameters

            string filenamePath = "";
            string extension = "";
            items = args.Length;
            if (items == 1)
            {
                int index = 0;
                filenamePath = args[index].Trim('"');
                pos = filenamePath.LastIndexOf('.');
                if (pos > 0)
                {
                    extension = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filenamePath = filenamePath.Substring(0, pos);
                }

                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    filePath.Value = filenamePath.Substring(0, pos);
                    filePath.Source = Parameter.SourceType.Command;
                    filename.Value = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filename.Source = Parameter.SourceType.Command;
                }
                else
                {
                    filename.Value = filenamePath;
                    filename.Source = Parameter.SourceType.Command;
                }
                _logger.LogInformation("Use filename=" + filename.Value);
                _logger.LogInformation("Use filePath=" + filePath.Value);
            }
            else
            {
                for (int item = 0; item < items; item++)
                {
                    {
                        switch (args[item])
                        {
                            case "/f":
                            case "--filename":
                                {
                                    filename.Value = args[item + 1];
                                    filename.Value = filename.Value.ToString().TrimStart('"');
                                    filename.Value = filename.Value.ToString().TrimEnd('"');
                                    filename.Source = Parameter.SourceType.Command;
                                    pos = filename.Value.ToString().LastIndexOf('.');
                                    if (pos > 0)
                                    {
                                        extension = filename.Value.ToString().Substring(pos + 1, filename.Value.ToString().Length - pos - 1);
                                        filename.Value = filename.Value.ToString().Substring(0, pos);
                                    }
                                    _logger.LogDebug("Use command value Filename=" + filename);
                                    break;
                                }
                            case "/O":
                            case "--output":
                                {
                                    outName.Value = args[item + 1];
                                    outName.Value = outName.Value.ToString().TrimStart('"');
                                    outName.Value = outName.Value.ToString().TrimEnd('"');
                                    outName.Source = Parameter.SourceType.Command;
                                    _logger.LogDebug("Use command value Output=" + outName);
                                    break;
                                }
                            case "/p":
                            case "--filepath":
                                {
                                    filePath.Value = args[item + 1];
                                    filePath.Value = filePath.Value.ToString().TrimStart('"');
                                    filePath.Value = filePath.Value.ToString().TrimEnd('"');
                                    filePath.Source = Parameter.SourceType.Command;
                                    _logger.LogDebug("Use command value Filename=" + filePath);
                                    break;
                                }
                        }
                    }
                }
                _logger.LogInformation("Use Filename=" + filename.Value);
                _logger.LogInformation("Use Filepath=" + filePath.Value);
            }

            if (outName.Value.ToString().Length == 0)
            {
                outName.Value = filename.Value;
            }

            // Read the plot data and create the CNC file

            _dxf.Load(filePath.Value.ToString(), filename.Value.ToString());

            _logger.LogDebug("Out Run()");
        }

        #endregion
    }
}
