using System;
using System.Collections.Generic;
using System.IO.Ports;
using Caliburn.Micro;
using kio_windows_integration.Helpers;
using kio_windows_integration.Models;
using kio_windows_integration.Services;
using kio_windows_integration.ViewModels;

namespace kio_windows_integration
{
    public partial class AppBootstrapper
    {
        private SimpleContainer _container = new SimpleContainer();
        
        private static SerialPort keyboardSerialPort = new SerialPort();
        private static ISet<ApplicationLayerMapping> appLayerMappings = new HashSet<ApplicationLayerMapping>();

        protected override void Configure()
        {
            _container.Instance(_container);

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();

            // App wide
            _container.RegisterHandler(typeof(SerialPort), "keyboardSerialPort",
                container => keyboardSerialPort);
            _container.RegisterHandler(typeof(ISet<ApplicationLayerMapping>), "appLayerMappings",
                container => appLayerMappings);
            _container.Singleton<WinApi, WinApi>();
            _container.Singleton<LayerSwitcher, LayerSwitcher>();
            _container.RegisterPerRequest(typeof(ErrorMangementHelper), nameof(ErrorMangementHelper),
                typeof(ErrorMangementHelper));
            _container.RegisterPerRequest(typeof(PersistenceService), nameof(PersistenceService),
                typeof(PersistenceService));

            // UI
            _container.RegisterPerRequest(typeof(ShellViewModel), nameof(ShellViewModel), typeof(ShellViewModel));
            _container.RegisterPerRequest(typeof(HomeViewModel), nameof(HomeViewModel), typeof(HomeViewModel));
            _container.RegisterPerRequest(typeof(ConfigureViewModel), nameof(ConfigureViewModel),
                typeof(ConfigureViewModel));
            _container.RegisterPerRequest(typeof(DebugViewModel), nameof(DebugViewModel), typeof(DebugViewModel));
            _container.RegisterPerRequest(typeof(SerialMonViewModel), nameof(SerialMonViewModel),
                typeof(SerialMonViewModel));
        }
        
        
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}