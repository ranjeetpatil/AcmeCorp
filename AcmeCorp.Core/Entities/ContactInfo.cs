﻿namespace AcmeCorp.Core.Entities
{

    public class ContactInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Customer Customer { get; set; }
    }
}
