using Microsoft.Extensions.DependencyInjection;
using NPoco;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Adapter.Common
{
    public static class DbConnectionManagerExtensions
    {
        public static IServiceCollection AddDbConnectionManager(this IServiceCollection services)
        {
            // Sql connection tüm request scope boyunca tek kalmalı
            services.AddScoped<IDbConnectionManager, DbConnectionManager>();

            //Npoco database nesnesinin de request scope boyunca tek olması gerekiyor
            //services.AddScoped<IDatabase, Database>(x =>
            //{
            //    //var connectionContainer = x.GetRequiredService<IDbConnectionManager>();

            //    //connectionContainer.OpenConnection();
            //    //var db = new Database(connectionContainer.Connection, DatabaseType.SqlServer2012);

            //    var connstr = $@"Data Source = 213.155.108.99,1433; Initial Catalog = QorchDev; User Id = emse-dev; Password = Emse2019!; ";
            //    var db = new Database(connstr, DatabaseType.SqlServer2012, System.Data.SqlClient.SqlClientFactory.Instance);
            //    //db.SetTransaction(connectionContainer.Transaction);
            //    return db;
            //});

            return services;
        }
    }
}
