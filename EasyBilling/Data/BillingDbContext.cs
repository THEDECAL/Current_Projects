﻿using System;
using System.Collections.Generic;
using System.Text;
using EasyBilling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyBilling.Data
{
    public class BillingDbContext : IdentityDbContext<IdentityAccount>
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CashOutlay> CashOutlays { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }

        public BillingDbContext(DbContextOptions<BillingDbContext> options)
            : base(options)
        {
            /*Database.EnsureDeleted();*/
            /*Database.EnsureCreated();*/
        }

/*        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Инициализация ролей в БД
            modelBuilder.Entity<IdentityRole>().HasData(RolesHelper.GetRoles());

            base.OnModelCreating(modelBuilder);
        }*/
    }
}