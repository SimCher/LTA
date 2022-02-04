﻿namespace LTA.API.Domain.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Topic>? Topics { get; set; }
}