﻿using GTFS.CLI.Switches.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFS.CLI
{
    class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            Itinero.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };
            GTFS.Logging.Logger.LogAction = (o, level, message, parameter) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };

            // parse switches first.
            var switches = SwitchParser.ParseSwitches(args);

            // convert commands into data processors.
            if (switches == null)
            {
                throw new Exception("Please specifiy a valid data processing command!");
            }

            var collapsedSwitches = new List<ProcessorBase>();
            for (int idx = 0; idx < switches.Length; idx++)
            {
                var processor = switches[idx].CreateProcessor();
                processor.Collapse(collapsedSwitches);
            }

            if (collapsedSwitches.Count > 1)
            { // there is more than one command left.
                throw new Exception("Command list could not be interpreted. Make sure you have the correct source/filter/target combinations.");
            }

            if (collapsedSwitches[0].CanExecute)
            { // execute the last remaining fully collapsed command.
                var ticks = DateTime.Now.Ticks;
                collapsedSwitches[0].Execute();
                Itinero.Logging.Logger.Log("Program", Itinero.Logging.TraceEventType.Information, "Processing finished, took {0}.",
                    (new TimeSpan(DateTime.Now.Ticks - ticks)).ToString());
            }

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}