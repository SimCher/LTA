using System;

namespace LTA.Mobile.Client.Models.BaseModels
{
    public class BaseModel : BindableModel
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
    }
}