using System;
using System.Collections.Generic;
using System.IO.Ports;
using Caliburn.Micro;
using kaleidoscope_companion.Helpers;
using kaleidoscope_companion.Models;
using kaleidoscope_companion.Services;
using kaleidoscope_companion.ViewModels;

namespace kaleidoscope_companion
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
            _container.RegisterPerRequest(typeof(LogErrorHandler), nameof(LogErrorHandler), typeof(LogErrorHandler));
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
            _container.RegisterPerRequest(typeof(SettingsService), nameof(SettingsService), typeof(SettingsService));


            // UI
            _container.RegisterPerRequest(typeof(ShellViewModel), nameof(ShellViewModel), typeof(ShellViewModel));
            _container.RegisterPerRequest(typeof(HomeViewModel), nameof(HomeViewModel), typeof(HomeViewModel));
            _container.RegisterPerRequest(typeof(ConfigureViewModel), nameof(ConfigureViewModel),
                typeof(ConfigureViewModel));
            _container.RegisterPerRequest(typeof(SettingsViewModel), nameof(SettingsViewModel),
                typeof(SettingsViewModel));
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