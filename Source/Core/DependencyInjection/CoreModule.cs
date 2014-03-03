﻿using Autofac;
using SqlFramework.Configuration;
using SqlFramework.Data.Extractors;
using SqlFramework.Data.Extractors.SqlServer2012;
using SqlFramework.IO;

namespace SqlFramework.DependencyInjection
{
    public sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConsoleTextWriter>().As<ITextWriter>().SingleInstance();
            builder.RegisterType<ConfigurationReader>().As<IConfigurationReader>().SingleInstance();
            builder.RegisterType<DatabaseExtractor>().As<IDatabaseExtractor>().SingleInstance();
            builder.RegisterType<FileStorageProvider>().As<IStorageProvider>().SingleInstance();
            builder.RegisterType<DatabaseWriter>().As<IDatabaseWriter>().SingleInstance();
        }
    }
}