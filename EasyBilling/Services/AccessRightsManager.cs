﻿using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.Services
{
    public class AccessRightsManager //: IDisposable
    {
        private readonly BillingDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public AccessRightsManager(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            _userManager = sp.GetRequiredService<UserManager<IdentityUser>>();
            _dbContext = sp.GetRequiredService<BillingDbContext>();
        }

        /// <summary>
        /// Получить массив всех прав
        /// </summary>
        /// <returns></returns>
        public async Task<AccessRight[]> GetAllAsync()
        {
            return await _dbContext.AccessRights
                .Include(ar => ar.Role).AsNoTracking().ToArrayAsync();
        }

        private async Task<IdentityUser> GetAccountAsync([NotNull] string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                return await _userManager.FindByNameAsync(userName);
            }
            else throw new ArgumentNullException();
        }

        private async Task<IList<string>> GetRolesAsync([NotNull] IdentityUser acc)
            => await _userManager.GetRolesAsync(acc);
        private async Task<IList<string>> GetRolesAsync([NotNull] string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                IdentityUser acc = await GetAccountAsync(userName);
                if (acc != null)
                {
                    var roles = await GetRolesAsync(acc);
                    return roles;
                }
                else return null;
            }
            throw new ArgumentNullException();
        }

        public async Task<AccessRight[]> GetRights([NotNull] string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var roles = await GetRolesAsync(userName);

                if (roles != null && roles.Count > 0)
                {
                    return await _dbContext.AccessRights
                        .Include(ar => ar.Role)
                        .Include(ar => ar.Controller)
                        .Where(ar => ar.Role.Name.Equals(roles[0]))
                        .AsNoTracking()
                        .ToArrayAsync();
                }
                return null;
            }
            else throw new ArgumentNullException();
        }

        public async Task<AccessRight> GetRights([NotNull] string userName, [NotNull] string controllerName)
        {
            if (!string.IsNullOrWhiteSpace(userName) &&
                !string.IsNullOrWhiteSpace(controllerName))
            {
                var roles = await GetRolesAsync(userName);

                if (roles != null && roles.Count > 0)
                {
                    var rights = await _dbContext.AccessRights
                        .Include(ar => ar.Role)
                        .Include(ar => ar.Controller)
                        .Where(ar => ar.Role.Name.Equals(roles[0]) &&
                            ar.Controller.Name.Equals(controllerName))
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    return rights;
                }
                else return null;
            }
            else throw new ArgumentNullException();
        }

        public async Task<Dictionary<string,string>> GetMenuItemsByRole([NotNull] string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var roles = await GetRolesAsync(userName);
                if (roles != null && roles.Count > 0)
                {
                    //Выбераем контроллеры в соотвествии с правами для пользователя
                    var cntrls = _dbContext.AccessRights
                        .Include(ar => ar.Role)
                        .Include(ar => ar.Controller)
                        .Where(ar => ar.Role.Name.Equals(roles[0]) && ar.IsAvailable)
                        .Select(ar => ar.Controller)
                        .AsNoTracking()
                        .ToArray();

                    //С помощью рефлексии получаем локализированное имя контроллера
                    var menuItems = new Dictionary<string, string>();
                    foreach (var item in cntrls)
                    {
                        try
                        {
                            string cntrlName = $"EasyBilling.Controllers.{item.Name}Controller";
                            var type = Type.GetType(cntrlName);

                            if (type.GetCustomAttribute(typeof(NoShowToMenuAttribute)) != null)
                            { continue; }

                            //var dnAtt = type.GetCustomAttribute(
                            //    typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                            // if (dnAtt != null)
                            menuItems.Add(item.Name, item.LocalizedName);
                        }
                        catch (Exception)
                        { }
                    }
                    
                    return menuItems;
                }
                else return null;
            }
            else throw new ArgumentNullException();
        }
    }
}
