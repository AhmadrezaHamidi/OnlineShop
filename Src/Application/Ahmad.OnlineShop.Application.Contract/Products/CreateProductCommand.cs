using AhmadBase.Application;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Contract.Products;


public class CreateProductCommand : ICommand<long>
{
    [JsonIgnore]
    public long Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public long CategoryId { get; set; }
    
    [JsonIgnore]
    public long InventoryId { get; set; }           
}