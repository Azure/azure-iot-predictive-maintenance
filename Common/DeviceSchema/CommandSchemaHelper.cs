﻿namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.DeviceSchema
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Helpers;
    using Models.Commands;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Helper class to encapsulate interactions with the command schema.
    /// 
    /// Elsewhere in the app we try to always deal with this flexible schema as dynamic,
    /// but here we take a dependency on Json.Net to populate the objects behind the schema.
    /// </summary>
    public static class CommandSchemaHelper
    {
        /// <summary>
        /// Retrieve from a device the commands that it can perform
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static dynamic GetSupportedCommands(dynamic device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }

            dynamic commands = device.Commands;

            if (commands == null)
            {
                commands = new JArray();
                device.Commands = commands;
            }

            return commands;
        }

        /// <summary>
        /// Build up a new dynamic command object based on the provided name.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static dynamic CreateNewCommand(string command)
        {
            JObject result = new JObject();

            result.Add(DeviceCommandConstants.NAME, command);
            result.Add(DeviceCommandConstants.PARAMETERS, null);

            return result;
        }

        /// <summary>
        /// Retrieve the command parameters with their default type and return them as a list
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static List<dynamic> GetCommandParametersAsList(dynamic command)
        {
            object obj;
            IEnumerable parameters;
            List<dynamic> result;

            result = new List<dynamic>();

            obj =
                ReflectionHelper.GetNamedPropertyValue(
                    (object)command,
                    "Parameters",
                    true,
                    false);

            if ((parameters = obj as IEnumerable) != null)
            {
                foreach (object parameter in parameters)
                {
                    result.Add(parameter);
                }
            }

            return result;
        }

        /// <summary>
        /// Add a parameter value definition to an existing command. This is not the use of a parameter for a command being sent to the
        /// device, but the definition of a parameter with its names and the data type that it is meant to accept
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public static void DefineNewParameterOnCommand(dynamic command, string name, string type)
        {
            dynamic foundParams = command.Parameters;
            if (foundParams == null || foundParams.GetType() != typeof(JArray))
            {
                foundParams = new JArray();
                command.Parameters = foundParams;
            }

            JObject newParam = new JObject();
            newParam.Add("Name", name);
            newParam.Add("Type", type);
            foundParams.Add(newParam);
        }

        /// <summary>
        /// Looks through the supported commands to see if the requested command is supported by the device
        /// </summary>
        /// <param name="device">Device to check</param>
        /// <param name="commandName">Name of commmand to check to see if the device supports</param>
        /// <returns>True if device can perform command, false if it cannot</returns>
        public static bool CanDevicePerformCommand(dynamic device, string commandName)
        {
            IEnumerable commands;
            object obj;

            if (device == null)
            {
                return false;
            }

            obj =
                ReflectionHelper.GetNamedPropertyValue(
                    (object)device,
                    "Commands",
                    true,
                    false);

            if ((commands = obj as IEnumerable) == null)
            {
                return false;
            }

            foreach (object command in commands)
            {
                if (command == null)
                {
                    continue;
                }

                obj =
                    ReflectionHelper.GetNamedPropertyValue(
                        command,
                        DeviceCommandConstants.NAME,
                        true,
                        false);

                if ((obj != null) &&
                    (obj.ToString() == commandName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method will add the provided command to the provided device. If the underlying infrastructure needs
        /// to be built it will be handled.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="command"></param>
        public static void AddCommandToDevice(dynamic device, dynamic command)
        {
            dynamic commands = GetSupportedCommands(device);
            commands.Add(command);
        }
    }
}