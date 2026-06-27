global using Xunit;

global using Ahmad.OnlineShop.Domain.BackOffice.Aggregates;
global using Ahmad.OnlineShop.Domain.BackOffice.Args;
global using Ahmad.OnlineShop.Domain.BackOffice.Enums;
global using Ahmad.OnlineShop.Domain.BackOffice.Exceptions;
global using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
global using Ahmad.OnlineShop.Domain.Bnpl.Args;
global using Ahmad.OnlineShop.Domain.Bnpl.Enums;
global using Ahmad.OnlineShop.Domain.Bnpl.Exceptions;
global using Ahmad.OnlineShop.Domain.Order.Args;
global using Ahmad.OnlineShop.Domain.Order.Enums;
global using Ahmad.OnlineShop.Domain.Order.Exceptions;
global using Ahmad.OnlineShop.Domain.Products.Args;
global using Ahmad.OnlineShop.Domain.Products.Enums;
global using Ahmad.OnlineShop.Domain.Products.Exceptions;
global using Ahmad.OnlineShop.Domain.Repositories;
global using BackOffice.Domain.Repositories;

global using Ahmad.OnlineShop.Application.Commands;
global using Ahmad.OnlineShop.Application.Contract.Order.Commands;
global using BackOffice.Application.Commands;

// Aliases برای جلوگیری از conflict با namespace
global using OrderAgg  = Ahmad.OnlineShop.Domain.Order.Aggregates.Order;
global using ProductAgg = Ahmad.OnlineShop.Domain.Products.Product;
global using CategoryAgg = Ahmad.OnlineShop.Domain.Products.Category;
