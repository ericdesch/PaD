using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PaD.Infrastructure
{
    public static class ControllerNames
    {
        private const string CONTROLLER_NAMES = "ApplicationState_ControllerNames";

        public static List<string> Get()
        {
            List<string> controllerNames = new List<string>();

            // If the list is already in the Application object, get it.
            // Otherwise build the list and store it in the Application object for future calls.
            if (HttpContext.Current.Application[CONTROLLER_NAMES] != null)
            {
                controllerNames = (List<string>)HttpContext.Current.Application[CONTROLLER_NAMES];
            }
            else
            {
                // For each type that is a subtype of Controller, add it to the list.
                GetSubClasses<Controller>().ForEach(type =>
                {
                    // Skip ControllerBase
                    if (type.Name != "ControllerBase")
                    {
                        // Trim 'Controller' from the end of each name and convert to lowercase.
                        controllerNames.Add(type.Name.Replace("Controller", "").ToLowerInvariant());
                    }
                });

                HttpContext.Current.Application[CONTROLLER_NAMES] = controllerNames;
            }

            return controllerNames;
        }

        // Returns a list of classes in the assembly of type <T>.
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

    }
}