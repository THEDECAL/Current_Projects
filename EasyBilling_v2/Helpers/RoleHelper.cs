﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Models;
using EasyBilling.Models.Entities;
using System.Diagnostics.CodeAnalysis;

namespace EasyBilling.Helpers
{
    [Flags]
    public enum RoleName
    {
        admin,
        @operator,
        casher,
        client
    };
    static public class RoleHelper
    {
        /// <summary>
        /// Получение словаря ролей где key = Role,
        /// value[0] = локализированное имя
        /// value[1] = Контроллер по умолчанию
        /// </summary>
        /// <returns></returns>
        static async public Task<Dictionary<string, string[]>> GetRolesAsync() =>
            await Task.Run(() => new Dictionary<string, string[]>()
            {
                { RoleName.admin.ToString(), new string[]{ "Администратор", "UsersController" } },
                { RoleName.@operator.ToString(), new string[]{ "Оператор", "UsersController" } },
                { RoleName.casher.ToString(), new string[]{ "Кассир", "CassaController" } },
                { RoleName.client.ToString(), new string[]{ "Клиент", "ClientController" } }
            });
    }
}