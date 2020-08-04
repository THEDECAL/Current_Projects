﻿using EasyBilling.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace EasyBilling.Models.Pocos
{
    public class AccessRight
    {
        public int Id { get; set; }
        [Required]
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }
        [Required]
        public string ControllerName { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        public string ComponentsJson { get; private set; }
        [NotMapped]
        public ObservableCollection<PageComponent> Components { get; private set; }
        public AccessRight()
        {
            Components = new ObservableCollection<PageComponent>();

            NotifyCollectionChangedEventHandler converter = (sender, e) =>
            {
                try
                {
                    ComponentsJson =
                        JsonConvert.SerializeObject(Components);
                }
                catch (JsonSerializationException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            };

            if (Components.Count == 0 &&
                !String.IsNullOrEmpty(ComponentsJson))
            {
                converter(null, null);
            }

            Components.CollectionChanged += converter;
        }
    }

}