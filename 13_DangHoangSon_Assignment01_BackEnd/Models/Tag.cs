using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _13_DangHoangSon_Assignment01_BackEnd.Models;

public partial class Tag
{
    [Key]
    public int TagId { get; set; }

    public string? TagName { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
