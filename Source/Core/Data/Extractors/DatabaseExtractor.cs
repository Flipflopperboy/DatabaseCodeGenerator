﻿using System.Data.SqlClient;
using SqlFramework.Data.Models;
using Microsoft.SqlServer.Management.Common;
using Smo = Microsoft.SqlServer.Management.Smo;



namespace SqlFramework.Data.Extractors
{

	public sealed class DatabaseExtractor : IDatabaseExtractor
	{

		public DatabaseExtractor(IConnectionStringProvider connectionStringProvider)
		{
			this.connectionStringProvider = connectionStringProvider;
		}



		public DatabaseModel Extract(Configuration.DatabaseConfiguration configuration)
		{
			ConnectionDetails connectionDetails = GetConnectionDetails();

			var serverConnection = new ServerConnection(connectionDetails.DataSource);
			var server = new Smo.Server(serverConnection);

			try
			{
				server.ConnectionContext.Connect();

				Smo.Database database = server.Databases[connectionDetails.Database];

				var model = new DatabaseModel();
				if (configuration.UserDefinedTableTypes != null)
				{
					var userDefinedTypesExtractor = new UserDefinedTableTypeExtractor(database.UserDefinedTableTypes);
					model.UserDefinedTableTypes = userDefinedTypesExtractor.Extract(configuration);
				}
				if (configuration.StoredProcedures != null)
				{
					model.StoredProcedures = new StoredProcedureExtractor().Extract(this.connectionStringProvider, configuration, database.StoredProcedures);
				}

				return model;
			}
			finally
			{
				if (server.ConnectionContext.IsOpen)
				{
					server.ConnectionContext.Disconnect();
				}
			}
		}


		private ConnectionDetails GetConnectionDetails()
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(this.connectionStringProvider.ConnectionString))
				{
					connection.Open();
					return new ConnectionDetails()
					{
						DataSource = connection.DataSource,
						Database = connection.Database
					};
				}
			}
			catch (SqlException sqlException)
			{
				throw new InvalidArgumentException("Could not connect to database using connection string '" + this.connectionStringProvider.ConnectionString + "'.", sqlException);
			}
		}



		private readonly IConnectionStringProvider connectionStringProvider;



		private sealed class ConnectionDetails
		{

			public string DataSource { get; set; }
			public string Database { get; set; }

		}

	}

}