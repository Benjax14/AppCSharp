using AppService.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Book { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Enterprise> Enterprise { get; set; }

        private string ConnectionString;

        public AppDbContext(){

        }

        public AppDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (ConnectionString != null)
                optionsBuilder.UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 32)));
            else
                optionsBuilder.UseMySql("server=localhost;port=3306;database=booksapp;uid=papayon;password=contraseña123", new MySqlServerVersion(new Version(8, 0, 32)));
        }   

    }
}
