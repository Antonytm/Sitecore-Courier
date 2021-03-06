﻿using Sitecore.Update;
using Sitecore.Update.Engine;
using System;
using Sitecore.Courier.Rainbow;
using System.Collections.Generic;
using Sitecore.Update.Interfaces;

namespace Sitecore.Courier.Runner
{
    /// <summary>
    /// Defines the program class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine("Source: {0}", options.Source);
                Console.WriteLine("Target: {0}", options.Target);
                Console.WriteLine("Output: {0}", options.Output);
                Console.WriteLine("Collision behavior: {0}", options.CollisionBehavior);
                Console.WriteLine("Use Rainbow: {0}", options.UseRainbow);
                Console.WriteLine("Include Files: {0}", options.IncludeFiles);
                Console.WriteLine("Configuration: {0}", options.Configuration);
                Console.WriteLine("Path to project file: {0}", options.ScProjFilePath);

                SanitizeOptions(options);

                if (ExclusionHandler.HasValidExclusions(options.Configuration, options.ScProjFilePath))
                {
                    var exclusions = ExclusionHandler.GetExcludedItems(options.ScProjFilePath, options.Configuration);

                    ExclusionHandler.RemoveExcludedItems(options.Source, exclusions);
                    ExclusionHandler.RemoveExcludedItems(options.Target, exclusions);
                }

                RainbowSerializationProvider.Enabled = options.UseRainbow;
                RainbowSerializationProvider.IncludeFiles = options.IncludeFiles;
                
                List<ICommand> commands = null;
                if (options.UseNewDiffGenerator)
                {
                    commands = NewDiffGenerator.GetDiffCommands(options.Source, options.Target, options.CollisionBehavior);
                }
                else
                {
                    commands = DiffGenerator.GetDiffCommands(options.Source, options.Target, options.CollisionBehavior);
                }
                
                var diff = new DiffInfo(
                    commands,
                    "Sitecore Courier Package",
                    string.Empty,
                    string.Format("Diff between serialization folders '{0}' and '{1}'.", options.Source, options.Target));

                PackageGenerator.GeneratePackage(diff, string.Empty, options.Output);
            }
            else
            {
                Console.WriteLine(options.GetUsage());
            }
        }

        private static void SanitizeOptions(Options options)
        {
          if (options.Source != null)
          {
            options.Source = options.Source.Replace("'", string.Empty);
          }

          if (options.Target != null)
          {
            options.Target = options.Target.Replace("'", string.Empty);
          }

          if (options.Output != null)
          {
            options.Output = options.Output.Replace("'", string.Empty);
          }
        }
    }
}
