using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Mvc.FilterBindingSyntax;
using Hangfire;

using PaD.DataContexts;
using PaD.CustomFilters;

namespace PaD.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;

            // This is so hangfire will use Ninject dependency resolver
            GlobalConfiguration.Configuration.UseNinjectActivator(_kernel);
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
    }
}