using System;
namespace core.api.ViewModels
{
    public class ContactInfo
    {
        public string Address { get; set; }
        public string State { get; set; }
        public Int32 ZipCode { get; set; }

        public ContactInfo()
        {
        }
    }
}
