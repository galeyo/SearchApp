﻿using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using FileHelpers;
namespace Persistence
{
    public static class Seed
    {
        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        DisplayName = "Bob",
                        UserName = "bob",
                        Email = "bob@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Tom",
                        UserName = "tom",
                        Email = "tom@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Alice",
                        UserName = "alice",
                        Email = "alice@test.com"
                    }
                };
                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "password");
                }

                await context.SaveChangesAsync();
            }
            if (!context.Aircraft.Any())
            {
                var engine = new FileHelperEngine<Mapping.Aircraft>();
                var aircrafts = engine.ReadFile("Aircraft.csv");

                foreach (var aircraft in aircrafts)
                {
                    context.Aircraft.Add(new Domain.Aircraft
                    {
                        AircraftName = aircraft.AircraftName,
                        Description = aircraft.Description,
                        YearInService = aircraft.YearInService,
                        Image = aircraft.Image,
                        Country = aircraft.Country
                    });
                }
                await context.SaveChangesAsync();
            }
            if (!context.Category.Any())
            {
                var engine = new FileHelperEngine<Mapping.Category>();
                var categories = engine.ReadFile("Category.csv");

                foreach (var category in categories)
                {
                    context.Category.Add(new Domain.Category
                    {
                        CategoryName = category.CategoryName
                    });

                }
                await context.SaveChangesAsync();
            }
            if (!context.Type.Any())
            {
                var engine = new FileHelperEngine<Mapping.Type>();
                var types = engine.ReadFile("Type.csv");

                foreach (var type in types)
                {
                    context.Type.Add(new Domain.Type
                    {
                        TypeName = type.TypeName
                    });
                }
                await context.SaveChangesAsync();
            }
            if (!context.AircraftCategory.Any())
            {
                var engine = new FileHelperEngine<Mapping.AircraftCategory>();
                var aircraftCategories = engine.ReadFile("AircraftCategory.csv");

                foreach (var aircraftCategory in aircraftCategories)
                {
                    context.AircraftCategory.Add(new Domain.AircraftCategory
                    {
                        AircraftId = aircraftCategory.AircraftId,
                        CategoryId = aircraftCategory.CategoryId
                    });
                }
                await context.SaveChangesAsync();
            }

            if (!context.AircraftType.Any())
            {
                var engine = new FileHelperEngine<Mapping.AircraftType>();
                var aircraftTypes = engine.ReadFile("AircraftType.csv");

                foreach (var aircraftType in aircraftTypes)
                {
                    context.AircraftType.Add(new Domain.AircraftType
                    {
                        AircraftId = aircraftType.AircraftId,
                        TypeId = aircraftType.TypeId
                    });
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
