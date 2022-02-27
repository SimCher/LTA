using System;

namespace LTA.Mobile.Domain.Models.BaseModels
{
    public class BaseModel : BindableModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
    }
}