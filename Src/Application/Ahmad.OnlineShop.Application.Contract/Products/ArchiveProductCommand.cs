using AhmadBase.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Application.Contract.Products;
public sealed record ArchiveProductCommand(long Id) : ICommand<long>;
